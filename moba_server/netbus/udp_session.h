#ifndef __UDP_SESSION_H__
#define __UDP_SESSION_H__


class udp_session : session {
public:
	uv_udp_t* udp_handler;
	char c_address[32];
	int c_port;
	const struct sockaddr* addr;

public:
	virtual void close();
	virtual void send_data(unsigned char* body, int len);
	virtual const char* get_address(int* client_port);
	virtual void send_msg(struct cmd_msg* msg);
	virtual void send_raw_msg(struct raw_cmd* raw);
};




#endif
