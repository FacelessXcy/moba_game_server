#ifndef __NETBUS_H__
#define __NETBUS_H__

class netbus
{
public:
	static netbus* instance();

public:
	void init();
	void tcp_listen(int port);//�ṩ����tcp_server�ӿ�
	void udp_listen(int port);//�ṩ����udp server�ӿ�;
	void ws_listen(int port);//�ṩ����ws server�ӿ�;
	void run();
};
#endif // !__NETBUS_H__
