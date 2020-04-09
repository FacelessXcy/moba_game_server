#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "lua_wrapper.h"
#include "../database/mysql_wrapper.h"
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus
#include "tolua_fix.h"
#include "mysql_export_to_lua.h"

//context�����ݿ����ӵľ��
static void on_lua_open_cb(const char* err, void* context, void* udata)
{
	if (err)
	{
		tolua_pushstring(lua_wrapper::lua_state(), err);
		lua_pushnil(lua_wrapper::lua_state());
	}
	else
	{
		lua_pushnil(lua_wrapper::lua_state());
		tolua_pushuserdata(lua_wrapper::lua_state(),context);
	}
	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_mysql_connect(lua_State* toLua_S)
{
	char* ip = (char*)tolua_tostring(toLua_S, 1, NULL);
	if (ip == NULL)
	{
		goto lua_failed;
	}
	int port = (int)tolua_tonumber(toLua_S, 2, 0);

	char* db_name = (char*)tolua_tostring(toLua_S, 3, NULL);
	if (db_name == NULL)
	{
		goto lua_failed;
	}

	char* uname = (char*)tolua_tostring(toLua_S, 4, NULL);
	if (uname == NULL)
	{
		goto lua_failed;
	}

	char* upwd = (char*)tolua_tostring(toLua_S, 5, NULL);
	if (upwd == NULL)
	{
		goto lua_failed;
	}

	int handler = toluafix_ref_function(toLua_S, 6, 0);//��ȡlua�ű��еĺ���
	mysql_wrapper::connect(ip, port, db_name, uname, upwd, on_lua_open_cb, (void*)handler);


lua_failed:
	return 0;
}
static int lua_mysql_close(lua_State* toLua_S)
{
	void* context = tolua_touserdata(toLua_S, 1, 0);
	if (context)
	{
		mysql_wrapper::close(context);
	}
	return 0;
}

//row ÿһ������
//numÿһ���ֶ�����
static void push_mysql_row(MYSQL_ROW row,int num)
{
	lua_newtable(lua_wrapper::lua_state());
	int index = 1;
	for (int i = 0; i < num; i++)
	{
		if (row[i]==NULL)
		{
			lua_pushnil(lua_wrapper::lua_state());
		}
		else
		{
			lua_pushstring(lua_wrapper::lua_state(), row[i]);
		}
		lua_rawseti(lua_wrapper::lua_state(), -2, index);
		++index;
	}
}

//udata����Я��lua�ű��еĻص�����
static void on_lua_query_cb(const char* err, MYSQL_RES* result, 
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
		if (result)//�Ѳ�ѯ�Ľ��pushΪһ����:{ {},{},{},... }
		{
			lua_newtable(lua_wrapper::lua_state());
			int index = 1;
			int num = mysql_num_fields(result);
			MYSQL_ROW row;
			while (row = mysql_fetch_row(result))
			{
				push_mysql_row(row, num);//��ÿһ������push��һ����
				lua_rawseti(lua_wrapper::lua_state(), -2, index);
				++index;
			}
		}
		else
		{
			lua_pushnil(lua_wrapper::lua_state());
		}
	}
	lua_wrapper::execute_script_handler((int)udata, 2);
	lua_wrapper::remove_script_handler((int)udata);
}

static int lua_mysql_query(lua_State* toLua_S)
{
	void* context = tolua_touserdata(toLua_S, 1, 0);
	if (!context)
	{
		goto lua_failed;
	}
	char* sql = (char*)tolua_tostring(toLua_S, 2, NULL);
	if (sql == NULL)
	{
		goto lua_failed;
	}
	int handler = toluafix_ref_function(toLua_S, 3, 0);//��ȡlua�ű��еĺ���
	if (handler == 0)
	{
		goto lua_failed;
	}
	mysql_wrapper::query(context, sql, on_lua_query_cb, (void*)handler);

lua_failed:
	return 0;
}

//����mysqlģ�鵽lua
int register_mysql_export(lua_State* toLua_S) {
	lua_getglobal(toLua_S, "_G");//��ȡȫ�ֱ�����_G��ֵ,���������ջ��
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "mysql_wrapper", 0);
		tolua_beginmodule(toLua_S, "mysql_wrapper");

		tolua_function(toLua_S, "connect", lua_mysql_connect);
		tolua_function(toLua_S, "close", lua_mysql_close);
		tolua_function(toLua_S, "query", lua_mysql_query);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//��ջ�е���1��Ԫ��
	return 0;
}