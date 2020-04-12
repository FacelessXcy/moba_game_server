#ifndef __SERVICE_MAN_H__
#define __SERVICE_MAN_H__

class session;
class service;
struct cmd_msg;
struct raw_cmd;

class service_man {
public:
	static void init();
	static bool register_service(int stype, service* s);
	static bool on_recv_raw_cmd(session* s, struct raw_cmd* raw);
	static void on_session_disconnect(session* s);
};


#endif // !__SERVICE_MAN_H__