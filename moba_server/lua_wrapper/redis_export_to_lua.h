#ifndef __REDIS_EXPORT_LUA_H__
#define __REDIS_EXPORT_LUA_H__

struct lua_State;

int register_redis_export(lua_State* toLua_S);
#endif // !__REDIS_EXPORT_LUA_H__
