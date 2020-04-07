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
//after_sec ���м��ʱ�䣨���룩
//repeat  �ظ����ٴ�  repeat_count==-1��һֱִ��
//����timer�ľ��
struct timer* schedule(void(*on_timer)(void* udata),
									void* udata, 
									int after_sec,
									int repeat_count);

//ȡ��timer���
void cancel_timer(struct timer* t);


struct timer* schedule_once(void(*on_timer)(void* udata),											void* udata, 												int after_sec);
#ifdef __cplusplus
}
#endif // __cplusplus

#endif // !__TIMER_LIST_H__