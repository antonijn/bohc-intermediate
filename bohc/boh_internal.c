#include "boh_internal.h"

#include "p3p3c6_bohstdString.h"
#include <gc/gc.h>

struct p3p3c9_bohstdException * exception;
struct p3p3c4_bohstdType * exception_type;
jmp_buf exception_buf;

void boh_throw_ex(struct p3p3c9_bohstdException * ex)
{
	exception = ex;
	exception_type = NULL;
	longjmp(exception_buf, 1);
}

struct p3p3c6_bohstdString * boh_create_string(const boh_char * const str, size_t len)
{
	struct p3p3c6_bohstdString *result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	result->f_offset = 0;
	result->f_length = len;
	*(unsigned char **)&result->f_chars = GC_malloc(len * sizeof(boh_char));
	for (int i = 0; i < len; ++i)
	{
		(*(unsigned char **)&result->f_chars)[i] = str[i];
	}
	return result;
}

