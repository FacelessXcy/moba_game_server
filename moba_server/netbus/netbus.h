#ifndef __NETBUS_H__
#define __NETBUS_H__

class netbus
{
public:
	static netbus* instance();

public:
	void init();
	void start_tcp_server(int port);//�ṩ����tcp_server�ӿ�
	void start_ws_server(int port);//�ṩ����ws server�ӿ�;
	void run();
};
#endif // !__NETBUS_H__
