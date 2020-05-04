#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "cache_alloc.h"

struct node
{
	struct node* next;

};

struct cache_allocer
{
	unsigned char* cache_mem;//内存池的开始地址,代表整个对象池的内存空间
	int capacity;//池中元素数量
	struct node* free_list;
	int elem_size;//每个元素的大小
};

//创建内存池
struct cache_allocer* create_cache_allocer(int capacity, int elem_size)
{
	int i;
	struct cache_allocer *allocer = malloc(sizeof(struct cache_allocer));
	memset(allocer, 0, sizeof(struct cache_allocer));
	elem_size = (elem_size < sizeof(struct node)) ? sizeof(struct node) : elem_size;

	allocer->capacity = capacity;
	allocer->elem_size = elem_size;
	allocer->cache_mem = malloc(capacity * elem_size);
	memset(allocer->cache_mem, 0, capacity * elem_size);

	allocer->free_list = NULL;
	for (i = 0; i < capacity; i++)
	{
		struct node* walk = (struct node*)(allocer->cache_mem + elem_size * i);
		//头插 插入链表
		walk->next = allocer->free_list;
		allocer->free_list = walk;
	}
	return allocer;
}

//销毁内存池
void destroy_cache_allocer(struct cache_allocer* allocer)
{
	if (allocer->cache_mem != NULL)
	{
		free(allocer->cache_mem);
	}
	free(allocer);
}

//从内存池中获取内存
void* cache_alloc(struct cache_allocer* allocer, int elem_size)
{
	if (allocer->elem_size < elem_size)
	{
		return malloc(elem_size);
	}

	if (allocer->free_list != NULL)
	{
		void* now = allocer->free_list;//从头部取空间
		allocer->free_list = allocer->free_list->next;
		return now;
	}

	return malloc(elem_size);
}

//返回内存给内存池
void cache_free(struct cache_allocer* allocer, void* mem)
{
	if (((unsigned char*)mem) >= allocer->cache_mem && 
		((unsigned char*)mem) < allocer->cache_mem + allocer->capacity * allocer->elem_size)//判断内存是否属于内存池中
	{
		struct node* node = mem;//头插法
		node->next = allocer->free_list;
		allocer->free_list = node;
		return;
	}
	free(mem);
}