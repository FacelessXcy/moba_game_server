#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "lua_wrapper.h"
#include "../database/redis_wrapper.h"
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus
#include "tolua_fix.h"
#include "redis_export_to_lua.h"
//context是数据库连接的句柄
//udata携带脚本中的回调函数
static void on_redis_open_cb(const char* err, void* context, void* udata)
{
	if (err)
	{
		tolua_pushstring(lua_wrapper::lua_state(), err);
		lua_pushnil(lua_wrapper::lua_state());
	}
	else
	{
		lua_pushnil(lua_wrapper::lua_state());
		tolua_pushuserdata(lua_wrapper::lua_state(), context);
	}
	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_redis_connect(lua_State* toLua_S)
{
	char* ip = (char*)tolua_tostring(toLua_S, 1, NULL);
	if (ip == NULL)
	{
		goto lua_failed;
	}
	int port = (int)tolua_tonumber(toLua_S, 2, 0);


	int handler = toluafix_ref_function(toLua_S, 3, 0);//获取lua脚本中的函数
	redis_wrapper::connect(ip, port, on_redis_open_cb, (void*)handler);


lua_failed:
	return 0;
}

static int lua_redis_close(lua_State* toLua_S)
{
	void* context = tolua_touserdata(toLua_S, 1, 0);
	if (context)
	{
		redis_wrapper::close_redis(context);
	}
	return 0;
}

static void push_result_to_lua(redisReply* result)
{
	switch (result->type)
	{
	case REDIS_REPLY_STRING:
	case REDIS_REPLY_STATUS:
		lua_pushstring(lua_wrapper::lua_state(), result->str);
		break;
	case REDIS_REPLY_INTEGER:
		lua_pushinteger(lua_wrapper::lua_state(),result->integer);
		break;
	case REDIS_REPLY_NIL:
		lua_pushnil(lua_wrapper::lua_state());
		break;
	case REDIS_REPLY_ARRAY:
		lua_newtable(lua_wrapper::lua_state());
		int index = 1;
		for (int i = 0; i < result->elements; i++)
		{
			push_result_to_lua(result->element[i]);
			lua_rawseti(lua_wrapper::lua_state(), -2, index);
			++index;
		}
		break;
	}
}

//udata用来携带lua脚本中的回调函数
static void on_lua_query_cb(const char* err, redisReply* result,
	void* udata)
{
	//function(err,context)
	if (err)
	{
		lua_pushstring(lua_wrapper::lua_state(), err);
		lua_pushnil(lua_wrapper::lua_state());
	}
	else
	{
		lua_pushnil(lua_wrapper::lua_state());
		if (result)//把查询的结果push到lua栈
		{
			push_result_to_lua(result);
		}
		else
		{
			lua_pushnil(lua_wrapper::lua_state());
		}
	}
	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_redis_query(lua_State* toLua_S)
{
	void* context = tolua_touserdata(toLua_S, 1, 0);
	if (!context)
	{
		goto lua_failed;
	}
	char* cmd = (char*)tolua_tostring(toLua_S, 2, NULL);
	if (cmd == NULL)
	{
		goto lua_failed;
	}
	int handler = toluafix_ref_function(toLua_S, 3, 0);//获取lua脚本中的函数
	if (handler == 0)
	{
		goto lua_failed;
	}
	redis_wrapper::query(context, cmd, on_lua_query_cb, (void*)handler);

lua_failed:
	return 0;
}

int register_redis_export(lua_State* toLua_S)
{
	lua_getglobal(toLua_S, "_G");//获取全局变量的_G的值,并将其放入栈顶
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "Redis", 0);
		tolua_beginmodule(toLua_S, "Redis");

		tolua_function(toLua_S, "connect", lua_redis_connect);
		tolua_function(toLua_S, "close_redis", lua_redis_close);
		tolua_function(toLua_S, "query", lua_redis_query);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//从栈中弹出1个元素
	return 0;
}