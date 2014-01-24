#include "boh_internal.h"

#include "all.h"

struct p3p3c9_bohstdException * exception;
struct p3p3c4_bohstdType * exception_type;
jmp_buf exception_buf;

void * boh_temp_storage;

noreturn void * boh_throw_null_ptr_ex(const char * const var) {
	boh_throw_ex(new_p3p3c10_bohstdNullPtrException_f13b0af3(boh_create_string(var, strlen(var))));
	return NULL;
}

void noreturn boh_throw_ex(struct p3p3c9_bohstdException * ex) {
	exception = ex;
	exception_type = NULL;
	longjmp(exception_buf, 1);
}

struct p3p3c6_bohstdString * boh_create_string(const boh_char * const str, size_t len) {
	struct p3p3c6_bohstdString *result = boh_gc_alloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	result->f_offset = 0;
	result->f_length = len;
	*(unsigned char **)&result->f_chars = boh_gc_alloc(len * sizeof(boh_char));
	for (int i = 0; i < len; ++i) {
		(*(unsigned char **)&result->f_chars)[i] = str[i];
	}
	return result;
}

void * boh_gc_alloc(size_t size) {
	return GC_malloc(size);
}

void * boh_gc_realloc(void * const ptr, size_t size) {
	return GC_realloc(ptr, size);
}
