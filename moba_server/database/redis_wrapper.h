#ifndef __REDIS_WRAPPER_H__
#define __REDIS_WRAPPER_H__
#include <hiredis.h>
//#include <uv.h>

struct redisReply;

class redis_wrapper 
{
public:
	static void connect(char* ip, int port,
		void (*open_cb)(const char* err, void* context,void* udata), void* udata=NULL);

	static void close_redis(void* context);

	//SQL”Ôæ‰÷¥––
	static void query(void* context,
								char* cmd,
		void(*query_cb)(const char* err, redisReply* result, void* udata),void* udata=NULL);
};


#endif
