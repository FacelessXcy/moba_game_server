#ifndef __LOGGER_H__
#define __LOGGER_H__

enum {
	DEBUG = 0,
	WARNING,
	ERROR,
};

// ## __VA_ARGS__用来接收 不定参数"..."
#define log_debug(msg, ...) logger::log(__FILE__, __LINE__, DEBUG, msg, ## __VA_ARGS__);
#define log_warning(msg, ...) logger::log(__FILE__, __LINE__, WARNING, msg, ## __VA_ARGS__);
#define log_error(msg, ...) logger::log(__FILE__, __LINE__, ERROR, msg, ## __VA_ARGS__);

class logger {
public:
	//path  文件夹路径
	//prefix 前缀
	//std_output 是否输出到控制台
	static void init(char* path, char* prefix, bool std_output = false);

	//file_name 文件名(表明该条日志是哪个文件输出的)
	//line_num 行号(表明该条日志是哪一行输出的)
	//level 日志等级  DEBUG  WARNING  ERROR
	static void log(const char* file_name, 
	                int line_num, 
	                int level, const char* msg, ...);
};


#endif

