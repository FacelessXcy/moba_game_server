#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;

#include "../../netbus/proto_man.h"
#include "../../netbus/netbus.h"
#include "../../utils/logger.h"
#include "../../utils/timer_list.h"
#include "../../utils/timestamp.h"
#include "../../database/mysql_wrapper.h"
#include "../../database/redis_wrapper.h"
#include "../../lua_wrapper/lua_wrapper.h"

int main(int argc,char**  argv)
{
	netbus::instance()->init();
	//初始化lua虚拟机
	lua_wrapper::init();

	if (argc!=3)//调试状态
	{
		//启动第一个lua脚本
		std::string search_path = "../../apps/lua/scripts/";
		lua_wrapper::add_search_path(search_path);
		std::string lua_file = search_path + "logic_server/main.lua";
		lua_wrapper::do_file(lua_file);
		//end
	}
	else//发布状态
	{
		std::string search_path = argv[1];
		if (*(search_path.end()-1)!='/')
		{
			search_path += '/';
		}
		lua_wrapper::add_search_path(search_path);
		std::string lua_file = search_path+argv[2];
		lua_wrapper::do_file(lua_file);
	}


	netbus::instance()->run();
	lua_wrapper::exit();

	system("pause");
	return 0;
}