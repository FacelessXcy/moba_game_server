#ifndef __MY_TIMER_LIST_H__
#define __MY_TIMER_LIST_H__

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

struct timer;
//on_timer 回调函数
//udata用户的自定义数据结构
//on_timer执行的udata就是传入的udata
//after_sec 第一次运行间隔时间（毫秒）
//repeat_count  重复多少次  repeat_count==-1则一直执行
//repeat_msec 第一次运行后，每次运行间隔时间（毫秒）
//返回timer的句柄
struct timer* schedule_repeat(void(*on_timer)(void* udata),
									void* udata, 
									int after_sec,
									int repeat_count,
									int repeat_msec);

//取消timer句柄
void cancel_timer(struct timer* t);


struct timer* schedule_once(void(*on_timer)(void* udata),											void* udata, 												int after_sec);void* get_timer_udata(struct timer* t);
#ifdef __cplusplus
}
#endif // __cplusplus

#endif // !__TIMER_LIST_H__