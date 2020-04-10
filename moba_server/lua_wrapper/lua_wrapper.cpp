#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../utils/logger.h"
#include "tolua_fix.h"
#include "lua_wrapper.h"
#include "mysql_export_to_lua.h"
#include "redis_export_to_lua.h"
#include "service_export_to_lua.h"
#include "session_export_to_lua.h"
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
	toluafix_open(g_lua_State);

	//注册模块
	register_mysql_export(g_lua_State);
	register_redis_export(g_lua_State);
	register_service_export(g_lua_State);
	register_session_export(g_lua_State);

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

lua_State* lua_wrapper::lua_state()
{
	return g_lua_State;
}

void lua_wrapper::reg_func2lua(const char* name, 
											int(*c_func)(lua_State* L))
{
	//将函数push入栈
	lua_pushcfunction(g_lua_State, c_func);
	//把栈中的该函数设为全局
	lua_setglobal(g_lua_State, name);

}
static bool pushFunctionByHandler(int nHandler)
{
	//将函数从全局表push到栈中
	toluafix_get_function_by_refid(g_lua_State, nHandler);     /* L: ... func */
	if (!lua_isfunction(g_lua_State, -1))
	{
		log_error("[LUA ERROR]  未注册该函数句柄：%d", nHandler);
		lua_pop(g_lua_State, 1);
		return false;
	}
	return true;
}

static int
executeFunction(int numArgs)
{
	int functionIndex = -(numArgs + 1);
	if (!lua_isfunction(g_lua_State, functionIndex))
	{
		log_error("value at stack [%d] is not function", functionIndex);
		lua_pop(g_lua_State, numArgs + 1); // 将栈中函数与参数删除
		return 0;
	}

	int traceback = 0;
	lua_getglobal(g_lua_State, "__G__TRACKBACK__"); /* L: ... func arg1 arg2 ... G */
	if (!lua_isfunction(g_lua_State, -1))
	{
		lua_pop(g_lua_State, 1);      /* L: ... func arg1 arg2 ... */
	}
	else
	{
		lua_insert(g_lua_State, functionIndex - 1);   /* L: ... G func arg1 arg2 ... */
		traceback = functionIndex - 1;
	}

	int error = 0;
	error = lua_pcall(g_lua_State, numArgs, 1, traceback);  /* L: ... [G] ret */
	if (error)
	{
		if (traceback == 0)
		{
			log_error("[LUA ERROR] %s", lua_tostring(g_lua_State, -1));   /* L: ... error */
			lua_pop(g_lua_State, 1); //从栈中删除错误信息
		}
		else  /* L: ... G error */
		{
			lua_pop(g_lua_State, 2); // 从栈中删除 __G__TRACKBACK__ 和错误信息
		}
		return 0;
	}

	//获取返回值
	int ret = 0;
	if (lua_isnumber(g_lua_State, -1))
	{
		ret = (int)lua_tointeger(g_lua_State, -1);
	}
	else if (lua_isboolean(g_lua_State, -1))
	{
		ret = (int)lua_toboolean(g_lua_State, -1);
	}
	//删除栈中的返回值
	lua_pop(g_lua_State, 1);  /* L: ... [G] */

	if (traceback)
	{
		lua_pop(g_lua_State, 1); // 从栈中删除 __G__TRACKBACK__       /* L: ... */
	}
	return ret;
}

int lua_wrapper::execute_script_handler(int nHandler, int numArgs)
{
	int ret = 0;
	//将函数压栈
	if (pushFunctionByHandler(nHandler))   /* L: ... arg1 arg2 ... func */
	{
		if (numArgs > 0)
		{
			lua_insert(g_lua_State, -(numArgs + 1));/* L: ... func arg1 arg2 ... */
		}
		//执行函数
		ret = executeFunction(numArgs);
	}
	lua_settop(g_lua_State, 0);//设置栈为0
	return ret;
}

void lua_wrapper::remove_script_handler(int nHandler)
{
	toluafix_remove_function_by_refid(g_lua_State, nHandler);
}
