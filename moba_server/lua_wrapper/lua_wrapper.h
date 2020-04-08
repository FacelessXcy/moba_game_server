#ifndef __LUA_WRAPPER_H__
#define __LUA_WRAPPER_H__
#include "lua.hpp"

class lua_wrapper
{
public:
	static void init();
	static void exit();

	static bool exe_lua_file(const char* lua_file);
};

#endif // !__LUA_WRAPPER_H__
