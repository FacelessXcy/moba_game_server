#ifndef __SERVICE_H__
#define __SERVICE_H__
class session;

class service {
public:
	virtual bool on_session_recv_cmd(session* s, struct cmd_msg* msg);
	virtual void on_session_disconnect(session* s);
};

#endif // !__SERVICE_H__
