#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "../netbus/proto_man.h"
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
#include "proto_man_export_to_lua.h"

#define my_malloc malloc
#define my_free free

//local cmd_name_map={"Name2","Name1","Name3"...} Lua 从1开始
static int lua_register_protobuf_cmd_map(lua_State* toLua_S)
{
	std::map<int, std::string> cmd_map;
	int n = luaL_len(toLua_S, 1);//获取表的长度
	if (n<=0)
	{
		goto lua_failed;
	}
	for (int i = 1; i <= n; i++)
	{
		lua_pushnumber(toLua_S, i);
		lua_gettable(toLua_S, 1);
		const char* name = luaL_checkstring(toLua_S, -1);
		if (name!=NULL)
		{
			cmd_map[i] = name;
		}
		lua_pop(toLua_S, 1);
	}
	proto_man::register_protobuf_cmd_map(cmd_map);
lua_failed:
	return 0;
}
static int lua_proto_type(lua_State* toLua_S)
{
	lua_pushinteger(toLua_S, proto_man::proto_type());
lua_failed:
	return 1;
}
static int lua_proto_man_init(lua_State* toLua_S)
{
	int argc = lua_gettop(toLua_S);
	if (argc != 1)
	{
		goto lua_failed;
	}
	int proto_type = lua_tointeger(toLua_S, 1);
	if (proto_type != PROTO_JSON && proto_type !=PROTO_BUF)
	{
		goto lua_failed;
	}
	proto_man::init(proto_type);
lua_failed:
	return 0;
}

int register_proto_man_export(lua_State* toLua_S)
{
	lua_getglobal(toLua_S, "_G");//获取全局变量的_G的值,并将其放入栈顶
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "ProtoMan", 0);
		tolua_beginmodule(toLua_S, "ProtoMan");

		tolua_function(toLua_S, "init", lua_proto_man_init);
		tolua_function(toLua_S, "proto_type", lua_proto_type);
		tolua_function(toLua_S, "register_protobuf_cmd_map", lua_register_protobuf_cmd_map);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//从栈中弹出1个元素

	return 0;
}
