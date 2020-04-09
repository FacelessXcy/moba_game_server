#ifndef _MYSQL_EXPORT_TO_LUA_H__
#define _MYSQL_EXPORT_TO_LUA_H__

struct lua_State;

int register_mysql_export(lua_State* toLua_S);

#endif // !_MY_SQL_EXPORT_TO_LUA_H__
