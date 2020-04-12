#ifndef __SMALL_ALLOC_H__
#define __SMALL_ALLOC_H__

#ifdef __cplusplus
extern "C"
{
#endif // __cplusplus

	void* small_alloc(int size);
	void small_free(void* mem);


#ifdef __cplusplus
}
#endif // __cplusplus

#endif // !__SMALL_ALLOC_H__
