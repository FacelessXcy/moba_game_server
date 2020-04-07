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

static void on_logger_timer(void* udata)
{
	log_debug("hello World");
}

int main()
{
	proto_man::init(PROTO_BUF);
	init_pf_cmd_map();
	logger::init((char*)"logger/",(char*)"netbus_log",true);

	schedule(on_logger_timer, NULL, 3000, -1);

	netbus::instance()->init();
	netbus::instance()->start_tcp_server(6080);
	netbus::instance()->start_ws_server(8001);
	
	netbus::instance()->run();

}