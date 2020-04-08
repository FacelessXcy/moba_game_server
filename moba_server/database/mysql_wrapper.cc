
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "uv.h"

#ifdef _WIN64
#endif // WIN64
#include <winsock.h>
#pragma comment(lib,"ws2_32.lib")
#pragma comment(lib,"libmysql.lib")


#include "mysql.h"


#include "mysql_wrapper.h"


#define my_malloc malloc
#define my_free free

struct connect_req
{
	char* ip;
	int port;
	char* db_name;
	char* uname;
	char* upwd;
	void(*open_cb)(const char* err, void* context);

	char* error;
	void* context;//mysql_context
};
struct mysql_context
{
	void* pConn;//mysql对象
	uv_mutex_t lock;//锁

	int is_closed;
};


 //工作队列的用处：
//1.把复杂的算法放到工作队列
//2.I/O放在工作队列：如获取数据库结果

//在线程池中的另外一个线程里调用,不在主线程;
static void connect_work(uv_work_t* req)
{
	struct connect_req* r = (struct connect_req*)req->data;
	//创建一个mysql的连接
	MYSQL* pConn = mysql_init(NULL);
	//将连接，连接到对应的数据库
	if (mysql_real_connect(pConn, r->ip, r->uname, r->upwd,
		r->db_name, r->port, NULL, 0))
	{//连接成功
		struct mysql_context* c = (struct mysql_context*)my_malloc(sizeof(struct mysql_context));
		memset(c, 0, sizeof(struct mysql_context));
		c->pConn = pConn;
		uv_mutex_init(&c->lock);
		r->context = c;
		r->error = NULL;
	}
	else
	{//连接失败
		r->context = NULL;
		r->error = strdup(mysql_error(pConn));
	}
}

//当工作队列执行完上面的任务后，通知主线程;
//主线程调用这个函数
static void on_connect_complete(uv_work_t* req, int status)
{
	struct connect_req* r = (struct connect_req*)req->data;
	r->open_cb(r->error, r->context);
	//释放资源
	if (r->ip)
		free(r->ip);
	if (r->db_name)
		free(r->db_name);
	if (r->uname)
		free(r->uname);
	if (r->upwd)
		free(r->upwd);
	if (r->error)
		free(r->error);
	my_free(r);
	my_free(req);

}

void mysql_wrapper::connect(char* ip, int port, 
	char* db_name, char* uname, char* pwd,
	void(*open_cb)(const char* err, void* context))
{
	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	struct connect_req* r = (struct connect_req*)my_malloc(sizeof(struct connect_req));
	memset(r, 0, sizeof(struct connect_req));
	r->ip = strdup(ip);
	r->port = port;
	r->db_name = strdup(db_name);
	r->uname = strdup(uname);
	r->upwd = strdup(pwd);
	r->open_cb = open_cb;

	w->data = (void*)r;
	uv_queue_work(uv_default_loop(), w, connect_work, on_connect_complete);
}

static void close_work(uv_work_t* req)
{
	struct mysql_context* r = (struct mysql_context*)req->data;
	uv_mutex_lock(&r->lock);
	MYSQL* pConn = (MYSQL*)r->pConn;
	mysql_close(pConn);
	uv_mutex_unlock(&r->lock);
}

static void on_close_complete(uv_work_t* req, int status)
{
	struct mysql_context* r = (struct mysql_context*)req->data;
	printf("数据库连接已关闭！\n");
	my_free(r);
	my_free(req);
}

void mysql_wrapper::close(void* context)
{
	struct mysql_context* c = (struct mysql_context*)context;
	if (c->is_closed)//数据库连接已关闭
	{
		return;
	}

	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));
	w->data = context;

	c->is_closed = 1;
	uv_queue_work(uv_default_loop(), w, close_work, on_close_complete);
}


struct query_req
{
	void* context;//sql连接
	char* sql;//sql语句
	void(*query_cb)(const char* err, std::vector<std::vector<std::string>>* result);

	char* error;//错误提示
	std::vector<std::vector<std::string>>* result;//查询结果
};

static void query_work(uv_work_t* req)
{
	query_req* r = (query_req*)req->data;
	struct mysql_context* my_conn = (struct mysql_context*)r->context;

	//线程锁
	uv_mutex_lock(&my_conn->lock);

	MYSQL* pConn = (MYSQL*)my_conn->pConn;


	int ret = mysql_query(pConn, r->sql);
	if (ret != 0)
	{
		r->error = strdup(mysql_error(pConn));
		r->result = NULL;
		//释放锁
		uv_mutex_unlock(&my_conn->lock);
		return;
	}
	r->error = NULL;
	MYSQL_RES* result = mysql_store_result(pConn);
	if (!result)
	{
		r->result = NULL;
		return;
	}
	MYSQL_ROW row;

	r->result = new std::vector<std::vector<std::string>>;
	int num = mysql_num_fields(result);//属性个数
	std::vector<std::string> empty;

	std::vector<std::vector<std::string>>::iterator end_elem;
	while (row = mysql_fetch_row(result))
	{
		r->result->push_back(empty);//插入一行空数据
		end_elem = r->result->end()-1;
		for (int i = 0; i < num; i++)
		{
			end_elem->push_back(row[i]);//填充一行数据
		}
	}
	mysql_free_result(result);
	uv_mutex_unlock(&my_conn->lock);
}

static void on_query_complete(uv_work_t* req, int status)
{
	query_req* r = (query_req*)req->data;
	r->query_cb(r->error, r->result);
	if (r->sql)
		free(r->sql);
	if (r->result)
		delete r->result;
	if (r->error)
		free(r->error);

	my_free(r);
	my_free(req);
}

void mysql_wrapper::query(void* context, 
										char* sql, 
										void(*query_cb)
	(const char* err, std::vector<std::vector<std::string>>* result))
{
	struct mysql_context* c = (struct mysql_context*)context;
	if (c->is_closed)//数据库连接已关闭
	{
		return;
	}

	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	query_req* r = (query_req*)my_malloc(sizeof(query_req));
	memset(r, 0, sizeof(query_req));
	r->context = context;
	r->sql = strdup(sql);
	r->query_cb = query_cb;
	w->data = (void*)r;
	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);
}
