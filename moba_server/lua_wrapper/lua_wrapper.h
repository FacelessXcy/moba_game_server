#ifndef __LUA_WRAPPER_H__
#define  __LUA_WRAPPER_H__
#include "lua.hpp"
#include <string>
class lua_wrapper
{
public:
	static void init();
	static void exit();

	//执行lua文件
	static bool do_file(std::string &lua_file);
	static lua_State* lua_state();
	//注册函数到lua
	static void reg_func2lua(const char* name,
									int (*c_func) (lua_State* L));
	//lua搜索路径
	static void add_search_path(std::string &path);

	//当调用的时候，先把函数调用的参数入栈
	//调用脚本函数
	static int execute_script_handler(int nHandler,int numArgs);
	
	//不用该handle时，
	//根据handleid从全局对象里面
	//移除脚本函数:
	static void remove_script_handler(int nHandler);

};
#endif // !__LUA_WRAPPER_H_