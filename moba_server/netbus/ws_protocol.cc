#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;

extern "C"
{
#include "../3rd/http_parser/http_parser.h"
#include "../3rd/crypto/base64_encoder.h"
#include "../3rd/crypto/sha1.h"
#include "../utils/cache_alloc.h"
}
//
#include "session.h"
#include "ws_protocol.h"
//#include "../3rd/crypto/base64_encoder.c"

extern cache_allocer* wbuf_allocer;

static const char* wb_migic = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
//base64(sha1(key+migic))
static const char* wb_accept = "HTTP/1.1 101 Switching Protocols\r\n" \
"Upgrade:websocket\r\n" \
"Connection: Upgrade\r\n" \
"Sec-WebSocket-Accept: %s\r\n" \
"WebSocket-Protocol:chat\r\n\r\n";

static char field_sec_key[512];
static char value_sec_key[512];
static int is_sec_key = 0;
static int has_sec_key = 0;
static int is_shake_ended = 0;

extern "C"
{
	int on_message_end(http_parser* p)
	{
		is_shake_ended = 1;
		return 0;
	}
	
}

//�ص�
static int on_ws_header_field(http_parser* p, const char* at, size_t length)
{
	if (strncmp(at, "Sec-WebSocket-Key", length) == 0)
	{
		is_sec_key = 1;
	}
	else
	{
		is_sec_key = 0;
	}
	return 0;
}
//�ص�
static int on_ws_header_value(http_parser* p, const char* at, size_t length)
{
	if (is_sec_key == 0)
	{
		return 0;
	}
	strncpy(value_sec_key, at, length);
	value_sec_key[length] = '\0';
	has_sec_key = 1;
	return 0;
}

//websocket����
bool ws_protocol::ws_shake_hand(session* s, char* body, int len)
{
	http_parser_settings settings;
	http_parser_settings_init(&settings);
	settings.on_header_field = on_ws_header_field;
	settings.on_header_value = on_ws_header_value;
	settings.on_message_complete = on_message_end;

	http_parser p;
	http_parser_init(&p, HTTP_REQUEST);
	is_sec_key = 0;
	has_sec_key = 0;
	is_shake_ended = 0;
	http_parser_execute(&p, &settings, body, len);

	if (has_sec_key&&is_shake_ended)//��������websocket�����Sec-WebSocket-Key
	{
		printf("Sec-WebSocket-Key��%s\n", value_sec_key);
		//key+migic
		static char key_migic[512];
		static char sha1_key_migic[SHA1_DIGEST_SIZE];
		static char send_client[512];
		int sha1_size;//������ܺ���ַ�������
		int base64_len;
		sprintf(key_migic, "%s%s", value_sec_key, wb_migic);
		//sha1����
		crypt_sha1((unsigned char*)key_migic, strlen(key_migic), (uint8_t*)sha1_key_migic, &sha1_size);
		//base64����
		char* base_buf = base64_encode((uint8_t*)sha1_key_migic, sha1_size, &base64_len);
		sprintf(send_client, wb_accept, base_buf);
		base64_encode_free(base_buf);

		//send_data(stream, (unsigned char*)send_client, strlen(send_client));
		s->send_data((unsigned char*)send_client, strlen(send_client));
		return true;
	}
	return false;
}

//��websocket��ͷ
bool ws_protocol::read_ws_header(unsigned char* recv_data, int recv_len, int* pkg_size, int* out_header_size)
{
	if (recv_data[0] != 0x81 && recv_data[0] != 0x82)//���������������˵��������Ч��http����
	{
		return false;
	}

	if (recv_len < 2)//������������Ϣ
	{
		return false;
	}

	unsigned int data_len = recv_data[1] & 0x7f;//�ڶ����ֽ�
	int head_size = 2;
	if (data_len == 126)//���������ֽڱ�ʾ���ݳ��ȣ�data[2],data[3]
	{
		head_size += 2;
		if (recv_len < head_size)//�޷������С
		{
			return false;
		}
		data_len = recv_data[3] | (recv_data[2] << 8);//data[2]��Ϊ��λ data[3]��Ϊ��λ
	}
	else if (data_len == 127)//����8���ֽڱ�ʾ���ݳ���, 2, 3, 4, 5| 6, 7, 8, 9
	{
		head_size += 8;
		if (recv_len < head_size)//�޷������С
		{
			return false;
		}
		unsigned int low = recv_data[5] | (recv_data[4] << 8) | (recv_data[3] << 16) | (recv_data[2] << 24);
		unsigned int high = recv_data[9] | (recv_data[8] << 8) | (recv_data[7] << 16) | (recv_data[6] << 24);
		data_len = low;
	}
	head_size += 4;//4���ֽڵ�mask
	*pkg_size = data_len + head_size;
	*out_header_size = head_size;

	return true;
}

//��Websocket��
void ws_protocol::parser_ws_recv_data(unsigned char* raw_data, unsigned char* mask, int raw_len)
{
	for (int i = 0; i < raw_len; i++)//���������������ݣ�
	{
		raw_data[i] = raw_data[i] ^ mask[i % 4];
	}
}

unsigned char* ws_protocol::package_ws_send_data(const unsigned char* raw_data, int len, int* ws_data_len)
{
	int head_size = 2;
	if (len > 125 && len < 65536)//���2���ֽڵİ������ֽ�[0,65535]
	{
		head_size += 2;
	}
	else if (len >= 65536)//���8���ֽڵİ������ֽ�
	{
		head_size += 8;
		return NULL;
	}
	//����ʹ���ڴ��
	//unsigned char* data_buf = (unsigned char*)malloc(head_size + len);
	unsigned char* data_buf = (unsigned char*)cache_alloc(wbuf_allocer, head_size + len);
	data_buf[0] = 0x82;//0x81 �ַ��� 0x82������
	if (len <= 125)
	{
		data_buf[1] = len;
	}
	else if (len > 125 && len < 65536)
	{
		data_buf[1] = 126;
		data_buf[2] = ((len & 0xff00) >> 8);
		data_buf[3] = (len & 0xff);
	}
	memcpy(data_buf + head_size, raw_data, len);
	*ws_data_len = (head_size + len);
	return data_buf;
}

void ws_protocol::free_ws_send_pkg(unsigned char* ws_pkg)
{
	//free(ws_pkg);
	cache_free(wbuf_allocer, ws_pkg);
}
