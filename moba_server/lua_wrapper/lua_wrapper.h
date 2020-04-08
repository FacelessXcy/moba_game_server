#ifndef __LUA_WRAPPER_H__
#define  __LUA_WRAPPER_H__
#include "lua.hpp"

class lua_wrapper
{
public:
	static void init();
	static void exit();

	//执行第一个lua文件
	static bool exe_lua_file(const char* lua_file);

	//注册函数到lua
	static void reg_func2lua(const char* name,
									int (*c_func) (lua_State* L));
};
#endif // !__LUA_WRAPPER_H_