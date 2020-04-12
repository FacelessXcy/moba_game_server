#ifndef __SESSION_H__
#define __SESSION_H__

class session
{
public:
	unsigned int as_client;
	unsigned int utag;
	session()
	{
		this->as_client = 0;
		this->utag = 0;
	}
public:
	virtual void close() = 0;
	//发送数据,在方法内进行tcp的打包(头部加上包长)
	virtual void send_data(unsigned char* body, int len) = 0;
	virtual const char* get_address(int* client_port) = 0;
	//发送对象，在方法内进行自定义协议的打包
	//(stype,ctype,utag,body)
	//之后交给send_data处理
	virtual void send_msg(struct cmd_msg* msg)=0;

	virtual void send_raw_msg(struct raw_cmd* raw) = 0;
};


#endif // !__SESSION_H__
