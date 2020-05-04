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
	unsigned char* cache_mem;//�ڴ�صĿ�ʼ��ַ,������������ص��ڴ�ռ�
	int capacity;//����Ԫ������
	struct node* free_list;
	int elem_size;//ÿ��Ԫ�صĴ�С
};

//�����ڴ��
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
		//ͷ�� ��������
		walk->next = allocer->free_list;
		allocer->free_list = walk;
	}
	return allocer;
}

//�����ڴ��
void destroy_cache_allocer(struct cache_allocer* allocer)
{
	if (allocer->cache_mem != NULL)
	{
		free(allocer->cache_mem);
	}
	free(allocer);
}

//���ڴ���л�ȡ�ڴ�
void* cache_alloc(struct cache_allocer* allocer, int elem_size)
{
	if (allocer->elem_size < elem_size)
	{
		return malloc(elem_size);
	}

	if (allocer->free_list != NULL)
	{
		void* now = allocer->free_list;//��ͷ��ȡ�ռ�
		allocer->free_list = allocer->free_list->next;
		return now;
	}

	return malloc(elem_size);
}

//�����ڴ���ڴ��
void cache_free(struct cache_allocer* allocer, void* mem)
{
	if (((unsigned char*)mem) >= allocer->cache_mem && 
		((unsigned char*)mem) < allocer->cache_mem + allocer->capacity * allocer->elem_size)//�ж��ڴ��Ƿ������ڴ����
	{
		struct node* node = mem;//ͷ�巨
		node->next = allocer->free_list;
		allocer->free_list = node;
		return;
	}
	free(mem);
}