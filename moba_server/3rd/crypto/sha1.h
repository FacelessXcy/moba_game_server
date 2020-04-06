#ifndef __CRYPT_SHA1_
#define __CRYPT_SHA1_

#include <stdint.h>

#define SHA1_DIGEST_SIZE 20

	void
		crypt_sha1(uint8_t * buffer, int sz, uint8_t * output, int* e_sz);


#endif

