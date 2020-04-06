#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;

#include "../../netbus/netbus.h"

int main()
{
	netbus::instance()->init();
	netbus::instance()->start_tcp_server(6080);
	netbus::instance()->start_ws_server(8001);
	
	netbus::instance()->run();

}