#include <HiredisWrapper.h>
#include <uv.h>
#include "redis_wrapper.h"
//#pragma comment(lib,"HiredisWrapper.lib")


#include "../utils/small_alloc.h"
#define my_malloc small_alloc
#define my_free small_free


static char* my_strdup(const char* src)
{
	int len = strlen(src) + 1;
	char* dst = (char*)my_malloc(len);
	strcpy(dst, src);
	return dst;
}

static void free_strdup(char* str)
{
	my_free(str);
}


struct connect_req
{
	char* ip;
	int port;

	void(*open_cb)(const char* err, void* context, void* udata);

	char* error;
	void* context;//redis_context
	void* udata;
};
struct redis_context
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
	connect_req* r = (struct connect_req*)req->data;
	timeval timeout = {5, 0 }; // 5 seconds
	redisContext* rc = RedisConnectWithTimeout((char*)r->ip, r->port, timeout);
	if (rc->err) {//连接出错
		printf("Connection error: %s\n", rc->errstr);
		r->error = my_strdup(rc->errstr);
		r->context = NULL;
		RedisFree(rc);
	}
	else
	{
		redis_context* c = (redis_context*)my_malloc(sizeof(struct redis_context));
		memset(c, 0, sizeof(redis_context));
		c->pConn = rc;
		uv_mutex_init(&c->lock);
		r->error = NULL;
		r->context = c;
	}
}

//当工作队列执行完上面的任务后，通知主线程;
//主线程调用这个函数
static void on_connect_complete(uv_work_t* req, int status)
{
	connect_req* r = (connect_req*)req->data;
	r->open_cb(r->error, r->context,r->udata);
	//释放资源
	if (r->ip)
		free_strdup(r->ip);

	if (r->error)
		free_strdup(r->error);
	my_free(r);
	my_free(req);

}

void redis_wrapper::connect(char* ip, int port,
							void(*open_cb)(const char* err, void* context, void* udata),
								void* udata)
{
	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	connect_req* r = (connect_req*)my_malloc(sizeof(connect_req));
	memset(r, 0, sizeof(connect_req));
	r->ip = my_strdup(ip);
	r->port = port;

	r->open_cb = open_cb;
	r->udata = udata;
	w->data = (void*)r;
	uv_queue_work(uv_default_loop(), w, connect_work, on_connect_complete);
}


static void close_work(uv_work_t* req)
{
	redis_context* r = (redis_context*)req->data;
	uv_mutex_lock(&r->lock);
	redisContext* c = (redisContext*)r->pConn;
	RedisFree(c);
	r->pConn = NULL;
	uv_mutex_unlock(&r->lock);
}

static void on_close_complete(uv_work_t* req, int status)
{
	redis_context* r = (redis_context*)req->data;
	printf("redis数据库连接已关闭！\n");
	my_free(r);
	my_free(req);
}

void redis_wrapper::close_redis(void* context)
{
	redis_context* c = (redis_context*)context;
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
	void* context;//redis连接
	char* cmd;//redis命令
	void(*query_cb)(const char* err, redisReply* result, void* udata);

	char* error;//错误提示
	redisReply* result;//查询结果
	void* udata;
};

static void query_work(uv_work_t* req)
{
	query_req* r = (query_req*)req->data;
	redis_context* my_conn = (redis_context*)r->context;
	redisContext* rc = (redisContext*)my_conn->pConn;
	//线程锁
	uv_mutex_lock(&my_conn->lock);
	redisReply* reply = (redisReply*)RedisCommand(rc, r->cmd, 15);
	if (reply->type == REDIS_REPLY_ERROR)
	{
		r->error = my_strdup(reply->str);
		r->result = NULL;
		FreeReplyObject(reply);
	}
	else
	{
		r->result = reply;
		r->error = NULL;
	}
	uv_mutex_unlock(&my_conn->lock);
}

static void on_query_complete(uv_work_t* req, int status)
{
	query_req* r = (query_req*)req->data;
	r->query_cb(r->error, r->result,r->udata);

	if (r->cmd)
		free_strdup(r->cmd);
	if (r->result)
		FreeReplyObject(r->result);
	//freeReplyObject(r->result);
	if (r->error)
		free_strdup(r->error);

	my_free(r);
	my_free(req);
}

void redis_wrapper::query(void* context,
										char* cmd, 
										void(*query_cb)(const char* err, redisReply* result, void* udata),
										void* udata)
{
	struct redis_context* c = (struct redis_context*)context;
	if (c->is_closed)//数据库连接已关闭
	{
		return;
	}

	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	query_req* r = (query_req*)my_malloc(sizeof(query_req));
	memset(r, 0, sizeof(query_req));
	r->context = context;
	r->cmd = my_strdup(cmd);
	r->query_cb = query_cb;
	r->udata = udata;
	w->data = (void*)r;
	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);
}
