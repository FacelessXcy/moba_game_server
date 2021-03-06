#ifndef __NETBUS_H__
#define __NETBUS_H__

class session;
class netbus
{
private:
	void* udp_handler;
public:
	static netbus* instance();
	netbus();

public:
	void init();
	void tcp_listen(int port);//提供启动tcp_server接口
	void udp_listen(int port);//提供启动udp server接口;
	void ws_listen(int port);//提供启动ws server接口;
	void run();
	void tcp_connect(const char* server_ip,int port, 
							void (*on_connected)(int err, session* s, void* udata),
							void* udata);
	void udp_send_to(char* ip, int port, unsigned char* body, int len);
};
#endif // !__NETBUS_H__
