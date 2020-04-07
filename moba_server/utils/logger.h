#ifndef __LOGGER_H__
#define __LOGGER_H__

enum {
	DEBUG = 0,
	WARNING,
	ERROR,
};

// ## __VA_ARGS__�������� ��������"..."
#define log_debug(msg, ...) logger::log(__FILE__, __LINE__, DEBUG, msg, ## __VA_ARGS__);
#define log_warning(msg, ...) logger::log(__FILE__, __LINE__, WARNING, msg, ## __VA_ARGS__);
#define log_error(msg, ...) logger::log(__FILE__, __LINE__, ERROR, msg, ## __VA_ARGS__);

class logger {
public:
	//path  �ļ���·��
	//prefix ǰ׺
	//std_output �Ƿ����������̨
	static void init(char* path, char* prefix, bool std_output = false);

	//file_name �ļ���(����������־���ĸ��ļ������)
	//line_num �к�(����������־����һ�������)
	//level ��־�ȼ�  DEBUG  WARNING  ERROR
	static void log(const char* file_name, 
	                int line_num, 
	                int level, const char* msg, ...);
};


#endif

