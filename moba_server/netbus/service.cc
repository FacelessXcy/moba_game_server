#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "session.h"
#include "proto_man.h"
#include "service.h"

//����ֵΪfalse,��ر�socket
bool service::on_session_recv_cmd(session* s, cmd_msg* msg)
{
	return false;
}

void service::on_session_disconnect(session* s)
{

}
