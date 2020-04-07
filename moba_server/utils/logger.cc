#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>
#include <fcntl.h>
#include <time.h>

#include <string>
using namespace std;

#include "logger.h"
#include "uv.h"


static string g_log_path;
static string g_prefix;
static uv_fs_t g_file_handle;
static uint32_t g_current_day;
static uint32_t g_last_second;
static char g_format_time[64] = { 0 };
static const char* g_log_level[] = { "DEBUG ", "WARNING ", "ERROR "};
static bool g_std_out = false;

static void open_file(tm* time_struct) 
{
	int result = 0;
	char fileName[128] = { 0 };

	if (g_file_handle.result != 0)//表明已经打开了一个log文件
	{
		uv_fs_close(uv_default_loop(), &g_file_handle, g_file_handle.result, NULL);
		uv_fs_req_cleanup(&g_file_handle);
		g_file_handle.result = 0;
	}

	sprintf(fileName, "%s%s.%4d%02d%02d.log", g_log_path.c_str(), g_prefix.c_str(), time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday);
	result = uv_fs_open(NULL, &g_file_handle, fileName, O_CREAT | O_RDWR | O_APPEND, S_IREAD | S_IWRITE, NULL);
	if (result < 0) 
	{
		fprintf(stderr, "open file failed! name=%s, reason=%s", fileName, uv_strerror(result));
	}
}

static void prepare_file() 
{
	time_t  now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (g_file_handle.result == 0) //表明没有打开日志文件
	{
		g_current_day = time_struct->tm_mday;
		open_file(time_struct);
	}
	else 
	{//表明已经打开了一个日志文件
		if (g_current_day != time_struct->tm_mday) 
		{//如果打开的日志文件不是当前日期的日志文件
			g_current_day = time_struct->tm_mday;
			open_file(time_struct);
		}
	}
}

static void format_time() 
{
	time_t  now = time(NULL);
	now += 8 * 60 * 60;
	tm* time_struct = gmtime(&now);
	if (now != g_last_second) //当前的时间不等于上一次打印log的时间
	{
		g_last_second = (uint32_t)now;
		memset(g_format_time, 0, sizeof(g_format_time));
		sprintf(g_format_time, "%4d%02d%02d %02d:%02d:%02d ",
			time_struct->tm_year + 1900, time_struct->tm_mon + 1, time_struct->tm_mday,
			time_struct->tm_hour, time_struct->tm_min, time_struct->tm_sec);
	}
}


void logger::init(char* path, char* prefix, bool std_output) 
{
	g_log_path = path;
	g_prefix = prefix;
	g_std_out = std_output;

	if (*(g_log_path.end() - 1) != '/') 
	{
		g_log_path += "/";
	}

	std::string tmp_path = g_log_path;
	int find = tmp_path.find("/");
	uv_fs_t req;
	int result;

	while (find != std::string::npos) 
	{
		result = uv_fs_mkdir(uv_default_loop(), &req, tmp_path.substr(0, find).c_str(), 0755, NULL);
		find = tmp_path.find("/", find + 1);
	}
	uv_fs_req_cleanup(&req);
}

void logger::log(const char* file_name,
									int line_num,
									int level, 
									const char* msg, ...) {
	prepare_file();
	format_time();
	static char msg_meta_info[1024] = { 0 };
	static char msg_content[1024 * 10] = { 0 };
	static char new_line = '\n';

	va_list args;
	va_start(args, msg);//获取不定参数
	//将msg和不定参数的信息输入到msg_content
	vsnprintf(msg_content, sizeof(msg_content), msg, args);
	va_end(args);

	sprintf(msg_meta_info, "%s:%u  ", file_name, line_num);
	uv_buf_t buf[6]; 
	//输出时间
	buf[0] = uv_buf_init(g_format_time, strlen(g_format_time));
	//日志等级
	buf[1] = uv_buf_init((char*)g_log_level[level], strlen(g_log_level[level]));
	//输出log的文件以及行数
	buf[2] = uv_buf_init(msg_meta_info, strlen(msg_meta_info));
	buf[3] = uv_buf_init(&new_line, 1);//newline
	//log的内容
	buf[4] = uv_buf_init(msg_content, strlen(msg_content));//fileandline
	buf[5] = uv_buf_init(&new_line, 1);//newline

	uv_fs_t writeReq;
	int result = uv_fs_write(NULL, &writeReq, g_file_handle.result, buf, sizeof(buf) / sizeof(buf[0]), -1, NULL);
	if (result < 0) {
		fprintf(stderr, "log failed %s%s%s%s", g_format_time, g_log_level[level], msg_meta_info, msg_content);
	}

	uv_fs_req_cleanup(&writeReq);

	//需要输出到控制台
	if (g_std_out) {
		printf("%s:%u\n[%s] %s\n", file_name, line_num, g_log_level[level], msg_content);
	}
}



