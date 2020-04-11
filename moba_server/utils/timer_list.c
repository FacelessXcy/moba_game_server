#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "uv.h"
#include "timer_list.h"

#define my_malloc malloc
#define my_free free
struct timer
{
	uv_timer_t uv_timer;//libuv timer handle
	void(*on_timer_cb)(void* udata);
	void* udata;
	int repeat_count;//-1则一直循环
};

//创建一个timer
static struct timer* alloc_timer(void(*on_timer)(void* udata),
	void* udata,
	int repeat_count)
{
	struct timer* t = my_malloc(sizeof(struct timer));
	memset(t, 0, sizeof(struct timer));
	t->on_timer_cb = on_timer;
	t->repeat_count = repeat_count;
	t->udata = udata;
	uv_timer_init(uv_default_loop(), &(t->uv_timer));
	return t;
}

static void free_timer(struct timer* t)
{
	printf("free timer\n");
	my_free(t);
}

static void on_uv_timer(uv_timer_t* handle)
{
	struct timer* t = handle->data;
	if (t->repeat_count < 0)//不断地触发
	{
		t->on_timer_cb(t->udata);
	}
	else
	{
		t->repeat_count--;
		t->on_timer_cb(t->udata);
		if (t->repeat_count == 0)//timer结束
		{
			uv_timer_stop(&t->uv_timer);//停止uv_timer
			free_timer(t);
		}
	}
}

struct timer* schedule_repeat(void(*on_timer)(void* udata),//开启一个计时器timer
	void* udata,
	int after_sec,
	int repeat_count,
	int repeat_msec)
{
	struct timer* t = alloc_timer(on_timer, udata, repeat_count);
	//启动libuv timer
	t->uv_timer.data = t;
	uv_timer_start(&t->uv_timer, on_uv_timer, after_sec, repeat_msec);
	return t;
}

struct timer* schedule_once(void(*on_timer)(void* udata),
	void* udata,
	int after_sec)
{
	return	schedule_repeat(on_timer, udata, after_sec, 1,after_sec);
}

void cancel_timer(struct timer* t)
{
	if (t->repeat_count == 0)//全部触发完成
	{
		return;
	}
	uv_timer_stop(&t->uv_timer);
	free_timer(t);
}

void* get_timer_udata(struct timer* t)
{
	return t->udata;
}
