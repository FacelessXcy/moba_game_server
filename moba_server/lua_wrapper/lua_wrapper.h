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
	static lua_State* lua_state();
	//注册函数到lua
	static void reg_func2lua(const char* name,
									int (*c_func) (lua_State* L));
	//当调用的时候，先把函数调用的参数入栈
	static int execute_script_handler(int nHandler,int numArgs);
	
	//不用这个handle的时候，
	//根据handleid从全局对象里面移除:
	static void remove_script_handler(int nHandler);

};
#endif // !__LUA_WRAPPER_H_