#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "../utils/timestamp.h"
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
#include "utils_export_to_lua.h"

#include "../utils/small_alloc.h"
#define my_malloc small_alloc
#define my_free small_free


static int lua_timestamp(lua_State* toLua_S)
{
	unsigned long ts = timestamp();
	lua_pushinteger(toLua_S, ts);

	return 1;
}

static int lua_timestamp_today(lua_State* toLua_S)
{
	unsigned long ts = timestamp_today();
	lua_pushinteger(toLua_S, ts);

	return 1;
}

static int lua_timestamp_yesterday(lua_State* toLua_S)
{
	unsigned long ts = timestamp_yesterday();
	lua_pushinteger(toLua_S, ts);

	return 1;
}


int register_utils_export(lua_State* toLua_S)
{
	lua_getglobal(toLua_S, "_G");//获取全局变量的_G的值,并将其放入栈顶
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "Utils", 0);
		tolua_beginmodule(toLua_S, "Utils");

		tolua_function(toLua_S, "timestamp", lua_timestamp);
		tolua_function(toLua_S, "timestamp_today", lua_timestamp_today);
		tolua_function(toLua_S, "timestamp_yesterday", lua_timestamp_yesterday);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//从栈中弹出1个元素

	return 0;
}
