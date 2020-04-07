#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "session.h"
#include "proto_man.h"
#include "service.h"
#include "service_man.h"

#define MAX_SERVICE 512  //0~511

static service* g_service_set[MAX_SERVICE];


void service_man::init()
{
	memset(g_service_set, 0, sizeof(g_service_set));
}

bool service_man::register_service(int stype, service* s)
{
	if (stype < 0 || stype >= MAX_SERVICE)
	{
		return false;
	}

	if (g_service_set[stype] != NULL)
	{
		return false;
	}
	g_service_set[stype] = s;
	return true;
}

bool service_man::on_recv_cmd_msg(session* s, cmd_msg* msg)
{
	if (g_service_set[msg->stype] == NULL)
	{
		return false;
	}

	return g_service_set[msg->stype]->on_session_recv_cmd(s, msg);
}

void service_man::on_session_disconnect(session* s)
{
	for (int i = 0; i < MAX_SERVICE; i++)
	{
		if (g_service_set[i] == NULL)
		{
			continue;
		}
		g_service_set[i]->on_session_disconnect(s);

	}

}
