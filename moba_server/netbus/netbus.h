#ifndef __NETBUS_H__
#define __NETBUS_H__

class netbus
{
public:
	static netbus* instance();

public:
	void init();
	void start_tcp_server(int port);//提供启动tcp_server接口
	void start_ws_server(int port);//提供启动ws server接口;
	void run();
};
#endif // !__NETBUS_H__
