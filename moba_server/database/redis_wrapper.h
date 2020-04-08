#ifndef __REDIS_WRAPPER_H__
#define __REDIS_WRAPPER_H__
#include <vector>
#include <string>

struct redisReply;

class redis_wrapper 
{
public:
	static void connect(char* ip, int port,
		void (*open_cb)(const char* err, void* context));

	static void close_redis(void* context);

	//SQL”Ôæ‰÷¥––
	static void query(void* context,
								char* cmd,
		void(*query_cb)(const char* err, redisReply* result));
};


#endif
