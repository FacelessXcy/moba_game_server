#ifndef __SESSION_H__
#define __SESSION_H__

class session
{
public:
	virtual void close() = 0;
	//��������,�ڷ����ڽ���tcp�Ĵ��(ͷ�����ϰ���)
	virtual void send_data(unsigned char* body, int len) = 0;
	virtual const char* get_address(int* client_port) = 0;
	//���Ͷ����ڷ����ڽ����Զ���Э��Ĵ��
	//(stype,ctype,utag,body)
	//֮�󽻸�send_data����
	virtual void send_msg(struct cmd_msg* msg)=0;
};


#endif // !__SESSION_H__
