#ifndef __LUA_WRAPPER_H__
#define  __LUA_WRAPPER_H__
#include "lua.hpp"
#include <string>
class lua_wrapper
{
public:
	static void init();
	static void exit();

	//ִ��lua�ļ�
	static bool do_file(std::string &lua_file);
	static lua_State* lua_state();
	//ע�ắ����lua
	static void reg_func2lua(const char* name,
									int (*c_func) (lua_State* L));
	//lua����·��
	static void add_search_path(std::string &path);

	//�����õ�ʱ���ȰѺ������õĲ�����ջ
	//���ýű�����
	static int execute_script_handler(int nHandler,int numArgs);
	
	//���ø�handleʱ��
	//����handleid��ȫ�ֶ�������
	//�Ƴ��ű�����:
	static void remove_script_handler(int nHandler);

};
#endif // !__LUA_WRAPPER_H_