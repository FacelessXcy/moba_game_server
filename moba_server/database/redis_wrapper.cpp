//#include <stdio.h>
//#include <string.h>
//#include <stdlib.h>
//
//#include <hiredis.h>
//
//#ifdef _WIN64
//#define NO_QFORKIMPL //��һ�б���Ӳ�������ʹ��
//#include <Win32_Interop/win32fixes.h>
//#pragma comment(lib,"hiredis.lib")
//#pragma comment(lib,"Win32_Interop.lib")
//#endif // _WIN64

#include <HiredisWrapper.h>
#pragma comment(lib,"HiredisWrapper.lib")
#include "uv.h"
#include "redis_wrapper.h"


#define my_malloc malloc
#define my_free free

struct connect_req
{
	char* ip;
	int port;

	void(*open_cb)(const char* err, void* context);

	char* error;
	void* context;//redis_context
};
struct redis_context
{
	void* pConn;//mysql����
	uv_mutex_t lock;//��

	int is_closed;
};


 //�������е��ô���
//1.�Ѹ��ӵ��㷨�ŵ���������
//2.I/O���ڹ������У����ȡ���ݿ���

//���̳߳��е�����һ���߳������,�������߳�;
static void connect_work(uv_work_t* req)
{
	connect_req* r = (struct connect_req*)req->data;
	timeval timeout = {5, 0 }; // 5 seconds
	redisContext* rc = RedisConnectWithTimeout((char*)r->ip, r->port, timeout);
	if (rc->err) {//���ӳ���
		printf("Connection error: %s\n", rc->errstr);
		r->error = strdup(rc->errstr);
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

//����������ִ��������������֪ͨ���߳�;
//���̵߳����������
static void on_connect_complete(uv_work_t* req, int status)
{
	connect_req* r = (connect_req*)req->data;
	r->open_cb(r->error, r->context);
	//�ͷ���Դ
	if (r->ip)
		free(r->ip);

	if (r->error)
		free(r->error);
	my_free(r);
	my_free(req);

}

void redis_wrapper::connect(char* ip, int port,
							void(*open_cb)(const char* err, void* context))
{
	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	connect_req* r = (connect_req*)my_malloc(sizeof(connect_req));
	memset(r, 0, sizeof(connect_req));
	r->ip = strdup(ip);
	r->port = port;

	r->open_cb = open_cb;

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
	printf("���ݿ������ѹرգ�\n");
	my_free(r);
	my_free(req);
}

void redis_wrapper::close_redis(void* context)
{
	redis_context* c = (redis_context*)context;
	if (c->is_closed)//���ݿ������ѹر�
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
	void* context;//redis����
	char* cmd;//redis����
	void(*query_cb)(const char* err, redisReply* result);

	char* error;//������ʾ
	redisReply* result;//��ѯ���
};

static void query_work(uv_work_t* req)
{
	query_req* r = (query_req*)req->data;
	redis_context* my_conn = (redis_context*)r->context;
	redisContext* rc = (redisContext*)my_conn->pConn;
	//�߳���
	uv_mutex_lock(&my_conn->lock);
	r->error = NULL;
	redisReply* reply = (redisReply*)RedisCommand(rc, r->cmd, 15);
	if (reply != NULL)
	{
		r->result = reply;
	}

	uv_mutex_unlock(&my_conn->lock);
}

static void on_query_complete(uv_work_t* req, int status)
{
	query_req* r = (query_req*)req->data;
	r->query_cb(r->error, r->result);

	if (r->cmd)
		free(r->cmd);
	if (r->result)
		FreeReplyObject(r->result);
	//freeReplyObject(r->result);
	if (r->error)
		free(r->error);

	my_free(r);
	my_free(req);
}

void redis_wrapper::query(void* context,
										char* cmd, 
										void(*query_cb)(const char* err, redisReply* result))
{
	struct redis_context* c = (struct redis_context*)context;
	if (c->is_closed)//���ݿ������ѹر�
	{
		return;
	}

	uv_work_t* w = (uv_work_t*)my_malloc(sizeof(uv_work_t));
	memset(w, 0, sizeof(uv_work_t));

	query_req* r = (query_req*)my_malloc(sizeof(query_req));
	memset(r, 0, sizeof(query_req));
	r->context = context;
	r->cmd = strdup(cmd);
	r->query_cb = query_cb;
	w->data = (void*)r;
	uv_queue_work(uv_default_loop(), w, query_work, on_query_complete);
}
