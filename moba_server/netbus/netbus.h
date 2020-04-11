#ifndef __NETBUS_H__
#define __NETBUS_H__

class netbus
{
public:
	static netbus* instance();

public:
	void init();
	void tcp_listen(int port);//提供启动tcp_server接口
	void udp_listen(int port);//提供启动udp server接口;
	void ws_listen(int port);//提供启动ws server接口;
	void run();
};
#endif // !__NETBUS_H__
