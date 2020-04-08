#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "lua_wrapper.h"

//lua虚拟机对象
lua_State* g_lua_State = NULL;

void lua_wrapper::init()
{
	g_lua_State = luaL_newstate();//创建lua虚拟机
	luaL_openlibs(g_lua_State);//开启lua所有基础库
}

void lua_wrapper::exit()
{
	if (g_lua_State != NULL)
	{
		lua_close(g_lua_State);
		g_lua_State = NULL;
	}

}

bool lua_wrapper::exe_lua_file(const char* lua_file)
{
	if (luaL_dofile(g_lua_State, lua_file))//执行失败
	{
		return false;
	}
	return true;
}
