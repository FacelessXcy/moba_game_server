#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <string>
#include <map>
#include "google/protobuf/message.h"
#include "proto_man.h"
#include "../utils/cache_alloc.h"
#include "../utils/small_alloc.h"


extern cache_allocer* wbuf_allocer;
#define my_malloc small_alloc
#define my_free small_free

#define CMD_HEADER 8
static int g_proto_type = PROTO_BUF;

static std::map<int, std::string> g_pb_cmd_map;

void proto_man::init(int proto_type)
{
	g_proto_type = proto_type;
}

int proto_man::proto_type()
{
	return g_proto_type;
}

//ע����������ӳ��
void proto_man::register_protobuf_cmd_map(std::map<int, std::string>& map)
{
	std::map<int, std::string>::iterator it;
	for ( it = map.begin(); it != map.end(); it++)
	{
		g_pb_cmd_map[it->first] = it->second;
	}
}

const char* proto_man::protobuf_cmd_name(int ctype)
{
	return g_pb_cmd_map[ctype].c_str();
}

//�û�����һ��message���͵��ַ��������������͹������Ӧ�����ʵ��
//"Person" �ַ���-->Person��ʵ��-->���ص���һ������Message���ָ��
//create_message-->delete��ɾ�����msg�Ķ���ʵ��
google::protobuf::Message* proto_man::create_message(const char* type_name) {
	google::protobuf::Message* message = NULL;
	//���������ҵ�message����������
	const google::protobuf::Descriptor* descriptor =
		google::protobuf::DescriptorPool::generated_pool()->FindMessageTypeByName(type_name);

	if (descriptor) {
		//�����������󵽶��󹤳������ɶ�Ӧ��ģ�������
		//����ģ�帴�Ƴ���һ��
		const google::protobuf::Message* prototype =
			google::protobuf::MessageFactory::generated_factory()->GetPrototype(descriptor);
		if (prototype) {
			message = prototype->New();
		}
	}
	return message;
}

void proto_man::release_message(google::protobuf::Message* m)
{
	delete m;
}

//���룬�������ʽΪ
//[����� 2�ֽ� | ����� 2�ֽ� | �û���ʶ 4�ֽ�  | ] body
bool proto_man::decode_cmd_msg(unsigned char* cmd, int cmd_len, cmd_msg** out_msg)
{
	*out_msg = NULL;
	if (cmd_len < CMD_HEADER)//����Ҫ��8���ֽ�
	{
		return false;
	}
	struct cmd_msg* msg = (struct cmd_msg*)my_malloc(sizeof(struct cmd_msg));
	//memset(msg, 0, sizeof(struct cmd_msg*));
	msg->stype = cmd[0] | (cmd[1] << 8);
	msg->ctype = cmd[2] | (cmd[3] << 8);
	msg->utag = cmd[4] | (cmd[5] << 8) | (cmd[6] << 16) | (cmd[7] << 24);
	msg->body = NULL;

	*out_msg = msg;
	if (cmd_len == CMD_HEADER)
	{
		return	true;
	}
	//���Խ���body����

	if (g_proto_type == PROTO_JSON)
	{
		int json_len = cmd_len - CMD_HEADER;
		//char* json_str = (char*)malloc(json_len + 1);
		char* json_str = (char*)cache_alloc(wbuf_allocer, json_len + 1);
		memcpy(json_str, cmd + CMD_HEADER, json_len);
		json_str[json_len] = '\0';
		msg->body = (void*)json_str;
	}
	else//protobufЭ��
	{
		google::protobuf::Message* p_m = create_message(g_pb_cmd_map[msg->ctype].c_str());
		if (p_m == NULL)//δ�ҵ���Ӧ����
		{
			my_free(msg);
			*out_msg = NULL;
			return false;
		}
		if (!p_m->ParseFromArray(cmd + CMD_HEADER, cmd_len - CMD_HEADER))//����ʧ��
		{
			my_free(msg);
			*out_msg = NULL;
			release_message(p_m);
			return false;
		}
		msg->body = p_m;
	}
	return true;
}

void proto_man::cmd_msg_free(cmd_msg* msg)
{
	if (msg->body)
	{
		if (g_proto_type == PROTO_JSON)
		{
			//free(msg->body);
			cache_free(wbuf_allocer, msg->body);
			msg->body = NULL;
		}
		else
		{
			google::protobuf::Message* p_m = (google::protobuf::Message*)msg->body;
			delete p_m;//Ϊ�˵�����������
			msg->body = NULL;
		}
	}
	my_free(msg);
}

//ת��Ϊ�ֽ���
unsigned char* proto_man::encode_msg_to_raw(const cmd_msg* msg, int* out_len)
{
	int raw_len = 0;
	unsigned char* raw_data = NULL;
	
	*out_len = 0;
	if (g_proto_type == PROTO_JSON)
	{
		char* json_str = NULL;
		int len = 0;
		if (msg->body!=NULL)
		{
			json_str = (char*)msg->body;
			len = strlen(json_str) + 1;
		}
		//raw_data = (unsigned char*)malloc(CMD_HEADER + len);
		raw_data = (unsigned char*)cache_alloc(wbuf_allocer, CMD_HEADER + len);
		if (msg->body!=NULL)
		{
			memcpy(raw_data + CMD_HEADER, json_str, len - 1);
			raw_data[8 + len] = '\0';
		}
		*out_len = (CMD_HEADER + len);
	}
	else if (g_proto_type == PROTO_BUF )
	{//protobuf

		google::protobuf::Message* p_m=NULL;
		int pf_len = 0;
		if (msg->body!=NULL)
		{
			p_m= (google::protobuf::Message*)msg->body;
			pf_len = p_m->ByteSize();
		}
		//raw_data = (unsigned char*)malloc(CMD_HEADER + pf_len);
		raw_data = (unsigned char*)cache_alloc(wbuf_allocer, CMD_HEADER + pf_len);
		if (msg->body!=NULL)
		{
			if (!p_m->SerializePartialToArray(raw_data + CMD_HEADER, pf_len))
			{
				//free(raw_data);
				cache_free(wbuf_allocer, raw_data);
				return NULL;
			}
		}
		*out_len = (pf_len + CMD_HEADER);
	}
	else
	{
		return NULL;
	}
	//����ͷ��
	raw_data[0] = (msg->stype & 0xff);
	raw_data[1] = ((msg->stype & 0xff00) >> 8);
	raw_data[2] = (msg->ctype & 0xff);
	raw_data[3] = ((msg->ctype & 0xff00) >> 8);
	memcpy(raw_data + 4, &msg->utag, 4);

	return raw_data;
}

void proto_man::msg_raw_free(unsigned char* raw)
{
	cache_free(wbuf_allocer, raw);
}
