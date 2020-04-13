#ifndef __SERVICE_H__
#define __SERVICE_H__
class session;
struct cmd_msg;
struct raw_cmd;


class service {
public:
	bool using_raw_cmd;
	service();
public:
	virtual bool on_session_recv_raw_cmd(session* s, struct raw_cmd* raw);
	virtual bool on_session_recv_cmd(session* s, struct cmd_msg* msg);
	virtual void on_session_disconnect(session* s, int stype);
};

#endif // !__SERVICE_H__
