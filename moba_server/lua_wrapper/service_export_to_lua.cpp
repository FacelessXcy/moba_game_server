#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "lua_wrapper.h"
#include "../netbus/service.h"
#include "../netbus/session.h"
#include "../netbus/proto_man.h"
#include "../netbus/service_man.h"
#include "../utils/logger.h"
#include "google/protobuf/message.h"
using namespace google::protobuf;
#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus
#include "tolua++.h"
#ifdef __cplusplus
}
#endif // __cplusplus
#include "service_export_to_lua.h"

#define SERVICE_FUNCTION_MAPPING "service_function_mapping"


static void init_service_function_map(lua_State* toLua_S)
{
	lua_pushstring(toLua_S, SERVICE_FUNCTION_MAPPING);
	lua_newtable(toLua_S);
	lua_rawset(toLua_S, LUA_REGISTRYINDEX);
}
static unsigned int s_function_ref_id = 0;
static unsigned int save_service_function(lua_State* L, int lo, int def)
{
	// function at lo
	if (!lua_isfunction(L, lo)) return 0;

	s_function_ref_id++;

	lua_pushstring(L, SERVICE_FUNCTION_MAPPING);
	lua_rawget(L, LUA_REGISTRYINDEX);                           /* stack: fun ... refid_fun */
	lua_pushinteger(L, s_function_ref_id);                      /* stack: fun ... refid_fun refid */
	lua_pushvalue(L, lo);                                       /* stack: fun ... refid_fun refid fun */

	lua_rawset(L, -3);                  /* refid_fun[refid] = fun, stack: fun ... refid_ptr */
	lua_pop(L, 1);                                              /* stack: fun ... */

	return s_function_ref_id;

	// lua_pushvalue(L, lo);                                           /* stack: ... func */
	// return luaL_ref(L, LUA_REGISTRYINDEX);
}

static void get_service_function(lua_State* L, int refid)
{
	lua_pushstring(L, SERVICE_FUNCTION_MAPPING);
	lua_rawget(L, LUA_REGISTRYINDEX);                           /* stack: ... refid_fun */
	lua_pushinteger(L, refid);                                  /* stack: ... refid_fun refid */
	lua_rawget(L, -2);                                          /* stack: ... refid_fun fun */
	lua_remove(L, -2);                                          /* stack: ... fun */
}


static bool push_service_function(int nHandler)
{
	//将函数从全局表push到栈中
	get_service_function(lua_wrapper::lua_state(), nHandler);     /* L: ... func */
	if (!lua_isfunction(lua_wrapper::lua_state(), -1))
	{
		log_error("[LUA ERROR]  未注册该函数句柄：%d", nHandler);
		lua_pop(lua_wrapper::lua_state(), 1);
		return false;
	}
	return true;
}

static int exe_function(int numArgs)
{
	int functionIndex = -(numArgs + 1);
	if (!lua_isfunction(lua_wrapper::lua_state(), functionIndex))
	{
		log_error("value at stack [%d] is not function", functionIndex);
		lua_pop(lua_wrapper::lua_state(), numArgs + 1); // 将栈中函数与参数删除
		return 0;
	}

	int traceback = 0;
	lua_getglobal(lua_wrapper::lua_state(), "__G__TRACKBACK__"); /* L: ... func arg1 arg2 ... G */
	if (!lua_isfunction(lua_wrapper::lua_state(), -1))
	{
		lua_pop(lua_wrapper::lua_state(), 1);      /* L: ... func arg1 arg2 ... */
	}
	else
	{
		lua_insert(lua_wrapper::lua_state(), functionIndex - 1);   /* L: ... G func arg1 arg2 ... */
		traceback = functionIndex - 1;
	}

	int error = 0;
	error = lua_pcall(lua_wrapper::lua_state(), numArgs, 1, traceback);  /* L: ... [G] ret */
	if (error)
	{
		if (traceback == 0)
		{
			log_error("[LUA ERROR] %s", lua_tostring(lua_wrapper::lua_state(), -1));   /* L: ... error */
			lua_pop(lua_wrapper::lua_state(), 1); //从栈中删除错误信息
		}
		else  /* L: ... G error */
		{
			lua_pop(lua_wrapper::lua_state(), 2); // 从栈中删除 __G__TRACKBACK__ 和错误信息
		}
		return 0;
	}

	//获取返回值
	int ret = 0;
	if (lua_isnumber(lua_wrapper::lua_state(), -1))
	{
		ret = (int)lua_tointeger(lua_wrapper::lua_state(), -1);
	}
	else if (lua_isboolean(lua_wrapper::lua_state(), -1))
	{
		ret = (int)lua_toboolean(lua_wrapper::lua_state(), -1);
	}
	//删除栈中的返回值
	lua_pop(lua_wrapper::lua_state(), 1);  /* L: ... [G] */

	if (traceback)
	{
		lua_pop(lua_wrapper::lua_state(), 1); // 从栈中删除 __G__TRACKBACK__       /* L: ... */
	}
	return ret;
}

static int execute_service_function(int nHandler, int numArgs)
{
	int ret = 0;
	//将函数压栈
	if (push_service_function(nHandler))   /* L: ... arg1 arg2 ... func */
	{
		if (numArgs > 0)
		{
			lua_insert(lua_wrapper::lua_state(), -(numArgs + 1));/* L: ... func arg1 arg2 ... */
		}
		//执行函数
		ret = exe_function(numArgs);
	}
	lua_settop(lua_wrapper::lua_state(), 0);//设置栈为0
	return ret;
}

class lua_service :public service
{
public:
	unsigned int lua_recv_cmd_handler;
	unsigned int lua_disconnect_handler;
public:
	virtual bool on_session_recv_cmd(session* s, struct cmd_msg* msg);
	virtual void on_session_disconnect(session* s);
};

static void push_proto_message_tolua(const Message* message)
{
	lua_State* state = lua_wrapper::lua_state();
	if (!message) {
		// printf("PushProtobuf2LuaTable failed, message is NULL");
		return;
	}
	const Reflection* reflection = message->GetReflection();

	// 顶层table
	lua_newtable(state);

	const Descriptor* descriptor = message->GetDescriptor();
	for (int32_t index = 0; index < descriptor->field_count(); ++index) {
		const FieldDescriptor* fd = descriptor->field(index);
		const std::string& name = fd->lowercase_name();

		// key
		lua_pushstring(state, name.c_str());

		bool bReapeted = fd->is_repeated();

		if (bReapeted) {
			// repeated这层的table
			lua_newtable(state);
			int size = reflection->FieldSize(*message, fd);
			for (int i = 0; i < size; ++i) {
				char str[32] = { 0 };
				switch (fd->cpp_type()) {
				case FieldDescriptor::CPPTYPE_DOUBLE:
					lua_pushnumber(state, reflection->GetRepeatedDouble(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_FLOAT:
					lua_pushnumber(state, (double)reflection->GetRepeatedFloat(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_INT64:
					sprintf(str, "%lld", (long long)reflection->GetRepeatedInt64(*message, fd, i));
					lua_pushstring(state, str);
					break;
				case FieldDescriptor::CPPTYPE_UINT64:
					sprintf(str, "%llu", (unsigned long long)reflection->GetRepeatedUInt64(*message, fd, i));
					lua_pushstring(state, str);
					break;
				case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
					lua_pushinteger(state, reflection->GetRepeatedEnum(*message, fd, i)->number());
					break;
				case FieldDescriptor::CPPTYPE_INT32:
					lua_pushinteger(state, reflection->GetRepeatedInt32(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_UINT32:
					lua_pushinteger(state, reflection->GetRepeatedUInt32(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_STRING:
				{
					std::string value = reflection->GetRepeatedString(*message, fd, i);
					lua_pushlstring(state, value.c_str(), value.size());
				}
				break;
				case FieldDescriptor::CPPTYPE_BOOL:
					lua_pushboolean(state, reflection->GetRepeatedBool(*message, fd, i));
					break;
				case FieldDescriptor::CPPTYPE_MESSAGE:
					push_proto_message_tolua(&(reflection->GetRepeatedMessage(*message, fd, i)));
					break;
				default:
					break;
				}
				lua_rawseti(state, -2, i + 1); // lua's index start at 1
			}
		}
		else {
			char str[32] = { 0 };
			switch (fd->cpp_type()) {

			case FieldDescriptor::CPPTYPE_DOUBLE:
				lua_pushnumber(state, reflection->GetDouble(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_FLOAT:
				lua_pushnumber(state, (double)reflection->GetFloat(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_INT64:
				sprintf(str, "%lld", (long long)reflection->GetInt64(*message, fd));
				lua_pushstring(state, str);
				break;
			case FieldDescriptor::CPPTYPE_UINT64:
				sprintf(str, "%llu", (unsigned long long)reflection->GetUInt64(*message, fd));
				lua_pushstring(state, str);
				break;
			case FieldDescriptor::CPPTYPE_ENUM: // 与int32一样处理
				lua_pushinteger(state, (int)reflection->GetEnum(*message, fd)->number());
				break;
			case FieldDescriptor::CPPTYPE_INT32:
				lua_pushinteger(state, reflection->GetInt32(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_UINT32:
				lua_pushinteger(state, reflection->GetUInt32(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_STRING:
			{
				std::string value = reflection->GetString(*message, fd);
				lua_pushlstring(state, value.c_str(), value.size());
			}
			break;
			case FieldDescriptor::CPPTYPE_BOOL:
				lua_pushboolean(state, reflection->GetBool(*message, fd));
				break;
			case FieldDescriptor::CPPTYPE_MESSAGE:
				push_proto_message_tolua(&(reflection->GetMessage(*message, fd)));
				break;
			default:
				break;
			}
		}
		lua_rawset(state, -3);
	}
}

//protbuf: 把message对象(key，value)转换成lua table
//json: 直接把json string 传给 lua
//{1:stype 2:ctype 3:utag 4:body: table or jsonStr}
bool lua_service::on_session_recv_cmd(session* s, cmd_msg* msg)
{
	//call lua func
	tolua_pushuserdata(lua_wrapper::lua_state(), (void*)s);
	int index = 1;

	lua_newtable(lua_wrapper::lua_state());
	lua_pushinteger(lua_wrapper::lua_state(), msg->stype);
	lua_rawseti(lua_wrapper::lua_state(), -2, index);
	++index;

	lua_pushinteger(lua_wrapper::lua_state(), msg->ctype);
	lua_rawseti(lua_wrapper::lua_state(), -2, index);
	++index;

	lua_pushinteger(lua_wrapper::lua_state(), msg->utag);
	lua_rawseti(lua_wrapper::lua_state(), -2, index);
	++index;
	if (msg->body == NULL)
	{
		lua_pushnil(lua_wrapper::lua_state());
	}
	else
	{
		if (proto_man::proto_type() == PROTO_JSON)
		{
			lua_pushstring(lua_wrapper::lua_state(), (char*)msg->body);
		}
		else
		{//proto_buf
			push_proto_message_tolua((const Message*)msg->body);
		}
		lua_rawseti(lua_wrapper::lua_state(), -2, index);
		++index;
	}
	execute_service_function(this->lua_recv_cmd_handler, 2);
	return true;
}
void lua_service::on_session_disconnect(session* s)
{
	tolua_pushuserdata(lua_wrapper::lua_state(), (void*)s);
	//call lua func
	execute_service_function(this->lua_disconnect_handler, 1);

}


static int lua_register_service(lua_State* toLua_S)
{
	int stype = (int)tolua_tonumber(toLua_S, 1, 0);
	bool ret = false;
	//table
	if (!lua_istable(toLua_S, 2))
	{//第二个参数不是一个table
		goto lua_failed;
	}
	
	unsigned int lua_recv_cmd_handler;
	unsigned int lua_disconnect_handler;

	//从table中取值
	lua_getfield(toLua_S, 2, "on_session_recv_cmd");
	lua_getfield(toLua_S, 2, "on_session_disconnect");
	//stack 3  on_session_recv_cmd,4  on_session_disconnect
	lua_recv_cmd_handler=save_service_function(toLua_S, 3, 0);
	lua_disconnect_handler =save_service_function(toLua_S, 4, 0);
	if (lua_recv_cmd_handler == 0 || lua_disconnect_handler == 0)
	{//没有这两个函数
		goto lua_failed;
	}

	lua_service* s = new lua_service();
	s->lua_recv_cmd_handler = lua_recv_cmd_handler;
	s->lua_disconnect_handler = lua_disconnect_handler;
	ret = service_man::register_service(stype, s);
lua_failed :
	lua_pushboolean(toLua_S, ret ? 1 : 0);
	return 1;
}

int register_service_export(lua_State* toLua_S)
{
	init_service_function_map(toLua_S);
	lua_getglobal(toLua_S, "_G");//获取全局变量的_G的值,并将其放入栈顶
	if (lua_istable(toLua_S, -1)) {
		tolua_open(toLua_S);
		tolua_module(toLua_S, "service", 0);
		tolua_beginmodule(toLua_S, "service");

		tolua_function(toLua_S, "register", lua_register_service);

		tolua_endmodule(toLua_S);
	}
	lua_pop(toLua_S, 1);//从栈中弹出1个元素

	return 0;
}

