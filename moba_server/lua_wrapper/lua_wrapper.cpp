#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../utils/logger.h"
#include "lua_wrapper.h"

//lua虚拟机对象
lua_State* g_lua_State = NULL;
//lua只能调用int (*lua_CFunction) (lua_State *L)格式的C函数

static void print_error(const char* file_name, 
												int line_num,
									const char* msg)
{
	logger::log(file_name, line_num, ERROR, msg);
}
static void print_warning(const char* file_name,
	int line_num,
	const char* msg)
{
	logger::log(file_name, line_num, WARNING, msg);
}
static void print_debug(const char* file_name,
	int line_num,
	const char* msg)
{
	logger::log(file_name, line_num, DEBUG, msg);
}

static void
do_log_message(void(*log)(const char* file_name, int line_num, const char* msg),
				const char* msg) 
{
	lua_Debug info;
	int depth = 0;
	while (lua_getstack(g_lua_State, depth, &info)) {
		//读取输出log的文件名，行号以及输出信息
		lua_getinfo(g_lua_State, "S", &info);
		lua_getinfo(g_lua_State, "n", &info);
		lua_getinfo(g_lua_State, "l", &info);

		if (info.source[0] == '@') {//info.source[1] 文件名 |  info.currentline  行号
			log(&info.source[1], info.currentline, msg);
			return;
		}
		++depth;
	}
	if (depth == 0) {
		log("trunk", 0, msg);
	}
}



static int lua_log_debug(lua_State* L)
{
	//从栈顶拿参数
	const char* msg = luaL_checkstring(L,-1);
	if (msg)//不为空代表栈顶的参数为string
	{//file_name  line_num	访问lua调用信息
		do_log_message(print_debug, msg);
	}
	return 0;
}
static int lua_log_warning(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{
		do_log_message(print_warning, msg);
	}
	return 0;
}
static int lua_log_error(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{
		do_log_message(print_error, msg);
	}
	return 0;
}

//自己定义panic函数，避免出错时，直接杀掉虚拟机进程
static int lua_panic(lua_State* L)
{
	const char* msg = luaL_checkstring(L, -1);
	if (msg)
	{
		do_log_message(print_error, msg);
	}
	return 0;
}

void lua_wrapper::init()
{
	g_lua_State = luaL_newstate();//创建lua虚拟机
	lua_atpanic(g_lua_State, lua_panic);//加入自定义的panic函数，避免调用abort（直接终止）
	luaL_openlibs(g_lua_State);//开启lua所有基础库
	//导出三个log函数
	lua_wrapper::reg_func2lua("log_error", lua_log_error);
	lua_wrapper::reg_func2lua("log_debug", lua_log_debug);
	lua_wrapper::reg_func2lua("log_warning", lua_log_warning);

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
		lua_log_error(g_lua_State);
		return false;
	}
	return true;
}

void lua_wrapper::reg_func2lua(const char* name, 
											int(*c_func)(lua_State* L))
{
	//将函数push入栈
	lua_pushcfunction(g_lua_State, c_func);
	//把栈中的该函数设为全局
	lua_setglobal(g_lua_State, name);

}
