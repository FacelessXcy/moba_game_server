#ifndef __SESSION_EXPORT_TO_LUA_H__
#define __SESSION_EXPORT_TO_LUA_H__


struct lua_State;

int register_session_export(lua_State* toLua_S);

#endif // !__SESSION_EXPORT_TO_LUA_H__
