#include <stdio.h>
#include <string.h>
#include <stdlib.h>


#include "tp_protocol.h"
extern "C"
{
#include "../utils/cache_alloc.h"
}
extern cache_allocer* wbuf_allocer;
//��ȡͷ������ȡ���ĳ���(�������)
bool tp_protocol::read_header(unsigned char* data, int data_len, int* pkg_size, int* out_header_size)
{
	if (data_len < 2)
	{
		return false;
	}
	*pkg_size = (data[0] | (data[1] << 8));//���Ĵ�С
	*out_header_size = 2;
	return true;
}

//���
unsigned char* tp_protocol::package(const unsigned char* raw_data, int len, int* pkg_len)
{
	int head_size = 2;
	*pkg_len = (head_size + len);
	//����ʹ���ڴ��
	//unsigned char* data_buf = (unsigned char*)malloc(*pkg_len);
	unsigned char* data_buf = (unsigned char*)cache_alloc(wbuf_allocer, (*pkg_len));
	data_buf[0] = (unsigned char)(*pkg_len) & 0xff;//��λ����ǰ��
	data_buf[1] = (unsigned char)(((*pkg_len) & 0xff00)>>8);//��λ���ں���

	memcpy(data_buf + head_size, raw_data, len);
	return data_buf;
}

void tp_protocol::release_package(unsigned char* tp_pkg)
{
	//����ʹ���ڴ��
	cache_free(wbuf_allocer,tp_pkg);
}
