#ifndef __PROTO_MAN_H__
#define __PROTO_MAN_H__

#include <string>
#include <map>

#include "google/protobuf/message.h"

enum {
	PROTO_JSON = 0,
	PROTO_BUF = 1,
};
struct cmd_msg {
	int stype;//服务号
	int ctype;//命令号
	unsigned int utag;//用户标识
	void* body; // JSON str 或者是message;
};

//自定义协议管理
//对发送与接收的数据进行
//打包与解包
class proto_man {
public:
	static void init(int proto_type);
	static int proto_type();

	static void register_pb_cmd_map(std::map<int,std::string> &map);
	static const char* protobuf_cmd_name(int ctype);

	static bool decode_cmd_msg(unsigned char* cmd, int cmd_len, struct cmd_msg** out_msg);
	static void cmd_msg_free(struct cmd_msg* msg);

	static unsigned char* encode_msg_to_raw(const struct cmd_msg* msg, int* out_len);
	static void msg_raw_free(unsigned char* raw);

	static google::protobuf::Message* create_message(const char* type_name);
	static void release_message(google::protobuf::Message* m);
};
#endif