#ifndef __MY_TIMER_LIST_H__
#define __MY_TIMER_LIST_H__

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

struct timer;
//on_timer �ص�����
//udata�û����Զ������ݽṹ
//on_timerִ�е�udata���Ǵ����udata
//after_sec ��һ�����м��ʱ�䣨���룩
//repeat_count  �ظ����ٴ�  repeat_count==-1��һֱִ��
//repeat_msec ��һ�����к�ÿ�����м��ʱ�䣨���룩
//����timer�ľ��
struct timer* schedule_repeat(void(*on_timer)(void* udata),
									void* udata, 
									int after_sec,
									int repeat_count,
									int repeat_msec);

//ȡ��timer���
void cancel_timer(struct timer* t);


struct timer* schedule_once(void(*on_timer)(void* udata),											void* udata, 												int after_sec);void* get_timer_udata(struct timer* t);
#ifdef __cplusplus
}
#endif // __cplusplus

#endif // !__TIMER_LIST_H__