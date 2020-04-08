#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include "../utils/logger.h"
#include "lua_wrapper.h"

//lua���������
lua_State* g_lua_State = NULL;
//luaֻ�ܵ���int (*lua_CFunction) (lua_State *L)��ʽ��C����

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
		//��ȡ���log���ļ������к��Լ������Ϣ
		lua_getinfo(g_lua_State, "S", &info);
		lua_getinfo(g_lua_State, "n", &info);
		lua_getinfo(g_lua_State, "l", &info);

		if (info.source[0] == '@') {//info.source[1] �ļ��� |  info.currentline  �к�
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
	//��ջ���ò���
	const char* msg = luaL_checkstring(L,-1);
	if (msg)//��Ϊ�մ���ջ���Ĳ���Ϊstring
	{//file_name  line_num	����lua������Ϣ
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

//�Լ�����panic�������������ʱ��ֱ��ɱ�����������
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
	g_lua_State = luaL_newstate();//����lua�����
	lua_atpanic(g_lua_State, lua_panic);//�����Զ����panic�������������abort��ֱ����ֹ��
	luaL_openlibs(g_lua_State);//����lua���л�����
	//��������log����
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
	if (luaL_dofile(g_lua_State, lua_file))//ִ��ʧ��
	{
		lua_log_error(g_lua_State);
		return false;
	}
	return true;
}

void lua_wrapper::reg_func2lua(const char* name, 
											int(*c_func)(lua_State* L))
{
	//������push��ջ
	lua_pushcfunction(g_lua_State, c_func);
	//��ջ�еĸú�����Ϊȫ��
	lua_setglobal(g_lua_State, name);

}
