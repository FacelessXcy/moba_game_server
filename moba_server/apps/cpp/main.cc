#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;

#include "../../netbus/proto_man.h"
#include "../../netbus/netbus.h"
#include "proto/pf_cmd_map.h"
#include "../../utils/logger.h"
#include "../../utils/timer_list.h"
#include "../../utils/timestamp.h"
#include "../../database/mysql_wrapper.h"
#include "../../database/redis_wrapper.h"

static void on_logger_timer(void* udata)
{
	log_debug("hello World");
}

static void on_query_cb
(const char* err, MYSQL_RES* result,void* udata)
{
	if (err)
	{
		printf("%s\n", err);
		return;
	}
	printf("�ɹ���\n");
}

static void on_open_cb(const char* err, void* context, void* udata)
{
	if (err!=NULL)
	{
		printf("%s", err);
		return;
	}
	printf("���ݿ����ӳɹ�\n");
	/*mysql_wrapper::query(context,
		(char*)"update class_test set name = \"xcy1\" where id = 3;",
		on_query_cb);*/
	mysql_wrapper::query(context,(char*)"select * from class_test;",
		on_query_cb,udata);

	mysql_wrapper::close(context);
}

static void test_db()
{
	mysql_wrapper::connect((char*)"127.0.0.1",3306, (char*)"class_sql",
		(char*)"root", (char*)"xcy19990419",on_open_cb);
}

static void on_redis_query(const char* err, redisReply* result, void* udata)
{
	if (err)
	{
		printf("%s", err);
		return;
	}
	printf("ָ��ִ�гɹ�\n");
}

static void on_redis_open_cb(const char* err, void* context, void* udata)
{
	if (err)
	{
		printf("%s", err);
		return;
	}
	printf("redis���ݿ����ӳɹ�\n");

	redis_wrapper::query(context, "select 1", on_redis_query);

	redis_wrapper::close_redis(context);

}

static void test_redis()
{
	redis_wrapper::connect("127.0.0.1",6379, on_redis_open_cb);
}

int main()
{
	//test_db();
	//test_redis();
	proto_man::init(PROTO_BUF);
	init_pf_cmd_map();
	logger::init((char*)"logger/",(char*)"netbus_log",true);

	//schedule(on_logger_timer, NULL, 3000, -1);
	log_debug("%d", timestamp_today());
	netbus::instance()->init();
	netbus::instance()->tcp_listen(6080);
	netbus::instance()->ws_listen(8001);
	netbus::instance()->udp_listen(8002);

	//��ʼ��lua�����
	//lua_wrapper::init();
	//std::string file_name = "./main.lua";
	//lua_wrapper::do_file(file_name);


	netbus::instance()->run();
	//lua_wrapper::exit();
	return 0;
}