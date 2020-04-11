#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "../utils/timer_list.h"

#include "lua_wrapper.h"

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus
#include "tolua_fix.h"
#include "scheduler_export_to_lua.h"

#define my_malloc malloc
#define my_free free

struct timer_repeat
{
	int handler;
	int repeat_count;
};

static void on_lua_repeat_timer(void* udata)
{
	timer_repeat* tr = (timer_repeat*)udata;
	lua_wrapper::execute_script_handler(tr->handler,0);
	if (tr->repeat_count == -1)
	{
		return;
	}
	tr->repeat_count--;
	if (tr->repeat_count<=0)
	{
		lua_wrapper::remove_script_handler(tr->handler);
		my_free(tr);
	}
}

static int lua_schedule_repeat(lua_State* toLua_S)
{
	int handler = toluafix_ref_function(toLua_S, 1, 0);
	if (handler==0)
	{
		goto lua_failed;
	}
	int after_sec = lua_tointeger(toLua_S, 2,0);
	if (after_sec <= 0)
	{
		goto lua_failed;
	}
	int repeat_count = lua_tointeger(toLua_S, 3, 0);
	if (repeat_count == 0)
	{
		goto lua_failed;
	}
	if (repeat_count<0)//-1 forever
	{
		repeat_count = -1;
	}
	int repeat_msec = lua_tointeger(toLua_S, 4, 0);
	if (repeat_msec <= 0)
	{
		repeat_msec = after_sec;
	}
	timer_repeat* tr = (timer_repeat*)my_malloc(sizeof(timer_repeat));
	tr->handler = handler;
	tr->repeat_count = repeat_count;
	timer* t = schedule_repeat(on_lua_repeat_timer, tr, after_sec, repeat_count, repeat_msec);
	tolua_pushuserdata(toLua_S, t);
	return 1;
lua_failed:
	if (handler != 0)
	{
		lua_wrapper::remove_script_handler(handler);
	}
	lua_pushnil(toLua_S);
	return 1;
}
static int lua_schedule_once(lua_State* toLua_S)
{

	int handler = toluafix_ref_function(toLua_S, 1, 0);
	if (handler == 0)
	{
		goto lua_failed;
	}
	int after_sec = lua_tointeger(toLua_S, 2, 0);
	if (after_sec <= 0)
	{
		goto lua_failed;
	}
	timer_repeat* tr = (timer_repeat*)my_malloc(sizeof(timer_repeat));
	tr->handler = handler;
	tr->repeat_count = 1;
	timer* t = schedule_once(on_lua_repeat_timer, tr, after_sec);
	tolua_pushuserdata(toLua_S, t);
	return 1;
lua_failed:
	if (handler != 0)
	{
		lua_wrapper::remove_script_handler(handler);
	}
	lua_pushnil(toLua_S);
	return 1;
}
static int lua_schedule_cancel(lua_State* toLua_S)
{
	if (!lua_isuserdata(toLua_S, 1))
	{
		goto lua_failed;
	}

	timer* t = (timer*)lua_touserdata(toLua_S, 1);
	timer_repeat* tr = (timer_repeat*)get_timer_udata(t);
	lua_wrapper::remove_script_handler(tr->handler);
	my_free(tr);

	cancel_timer(t);
lua_failed:
	return 0;
}

int register_scheduler_export(lua_State* toLua_S)
{
	lua_getglobal(toLua_S, "_G");//获取全局变量的_G的值,并将其放入栈顶
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "scheduler", 0);
		tolua_beginmodule(toLua_S, "scheduler");

		tolua_function(toLua_S, "schedule", lua_schedule_repeat);
		tolua_function(toLua_S, "once", lua_schedule_once);
		tolua_function(toLua_S, "cancel", lua_schedule_cancel);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//从栈中弹出1个元素

	return 0;
}
