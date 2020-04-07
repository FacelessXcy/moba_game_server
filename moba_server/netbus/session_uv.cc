#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include <iostream>
#include <string>
using namespace std;

#include "uv.h"
#include "session.h"
#include "session_uv.h"
#include "proto_man.h"
#include "service_man.h"

#include "../utils/cache_alloc.h"
#include "ws_protocol.h"
#include "tp_protocol.h"

#define SESSION_CACHE_CAPACITY 6000
#define WQ_CACHE_CAPACITY 4096
#define WBUF_CACHE_CAPACITY 1024
#define CMD_CACHE_SIZE 1024

struct cache_allocer* session_allocer = NULL;
static cache_allocer* wr_allocer = NULL;
cache_allocer* wbuf_allocer = NULL;
void init_session_allocer()
{
	if (session_allocer == NULL)
	{
		session_allocer = create_cache_allocer(SESSION_CACHE_CAPACITY,sizeof(uv_session));
	}

	if (wr_allocer == NULL)
	{
		wr_allocer = create_cache_allocer(WQ_CACHE_CAPACITY, sizeof(uv_write_t));
	}

	if (wbuf_allocer == NULL)
	{
		wbuf_allocer = create_cache_allocer(WBUF_CACHE_CAPACITY, CMD_CACHE_SIZE);
	}
}

extern "C"
{
	static void after_write(uv_write_t* req, int status)
	{
		if (status == 0)
		{
			printf("send success!\n");
		}
		//free(req);
		cache_free(wr_allocer, req);
	}
	static void on_close(uv_handle_t* handle)
	{
		printf("session close\n");
		uv_session* s = (uv_session*)handle->data;
		uv_session::destory(s);
	}

	static void on_shutdown(uv_shutdown_t* req, int status)
	{
		uv_close((uv_handle_t*)req->handle, on_close);
	}
}

//创建一个session连接
uv_session* uv_session::create()
{
	//uv_session* uv_s = new uv_session();//temp
	uv_session* uv_s = (uv_session*)cache_alloc(session_allocer, sizeof(uv_session));
	uv_s->uv_session::uv_session();
	uv_s->init();
	return uv_s;
}

//销毁session连接
void uv_session::destory(uv_session* s)
{
	s->exit();
	//delete s;//temp
	s->uv_session::~uv_session();
	cache_free(session_allocer, s);
}
 
void uv_session::init()//create时调用
{
	memset(this->c_address, 0, sizeof(this->c_address));
	this->c_port = 0;
	this->recved = 0;
	this->is_shutdown = false;
	this->is_ws_shake = 0;
	this->long_pkg = NULL;
	this->long_pkg_size = 0;
}

void uv_session::exit()//destory时调用
{
	printf("exit\n");
}

//关闭连接
void uv_session::close()
{
	if (this->is_shutdown)
	{
		return;
	}
	//通知所有service，断线
	service_man::on_session_disconnect(this);

	this->is_shutdown = true;
	uv_shutdown_t* req = &this->shudown;
	memset(req, 0, sizeof(uv_shutdown_t));
	uv_shutdown(req, (uv_stream_t*)&this->tcp_handler, on_shutdown);
}

//发送数据
void uv_session::send_data(unsigned char* body, int len)
{
	//uv_write_t* w_req = (uv_write_t*)malloc(sizeof(uv_write_t));
	uv_write_t* w_req = (uv_write_t*)cache_alloc(wr_allocer, sizeof(uv_write_t));
	uv_buf_t w_buf;

	if (this->socket_type == WS_SOCKET)
	{
		if (this->is_ws_shake)//已握手
		{
			int ws_pkg_len;
			unsigned char* ws_pkg = ws_protocol::package_ws_send_data(body, len, &ws_pkg_len);
			w_buf = uv_buf_init((char*)ws_pkg, ws_pkg_len);
			uv_write(w_req, (uv_stream_t*)& this->tcp_handler, &w_buf, 1, after_write);
			ws_protocol::free_ws_send_pkg(ws_pkg);
		}
		else//未握手
		{
			w_buf = uv_buf_init((char*)body, len);
			uv_write(w_req, (uv_stream_t*)& this->tcp_handler, &w_buf, 1, after_write);
		}
	}
	else
	{//TCP
		/*w_buf = uv_buf_init((char*)body, len);
		uv_write(w_req, (uv_stream_t*)& this->tcp_handler, &w_buf, 1, after_write);*/
		int tp_pkg_len;
		unsigned char* tp_pkg = tp_protocol::package(body, len, &tp_pkg_len);
		w_buf = uv_buf_init((char*)tp_pkg, tp_pkg_len);
		uv_write(w_req, (uv_stream_t*)&this->tcp_handler, &w_buf, 1, after_write);
		tp_protocol::release_package(tp_pkg);
	}
	
}

//获取该session对象的端口及ip
const char* uv_session::get_address(int* port)
{
	*port = this->c_port;
	return (const char*)this->c_address;
}

void uv_session::send_msg(struct cmd_msg* msg)
{
	unsigned char* encode_pkg = NULL;
	int encode_len = 0;
	encode_pkg = proto_man::encode_msg_to_raw(msg, &encode_len);
	if (encode_pkg)
	{
		this->send_data(encode_pkg, encode_len);
		proto_man::msg_raw_free(encode_pkg);
	}
}
