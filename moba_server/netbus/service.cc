#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "session.h"
#include "proto_man.h"
#include "service.h"

service::service()
{
	this->using_raw_cmd = false;
}

bool service::on_session_recv_raw_cmd(session* s, raw_cmd* raw)
{
	return false;
}

//返回值为false,则关闭socket
bool service::on_session_recv_cmd(session* s, cmd_msg* msg)
{
	return false;
}

void service::on_session_disconnect(session* s ,int stype)
{

}
