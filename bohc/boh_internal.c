#include "boh_internal.h"

#include "all.h"

struct p4p3c9_aquastdException * exception;
struct p4p3c4_aquastdType * exception_type;
jmp_buf exception_buf;

void * boh_temp_storage;

noreturn void * boh_throw_null_ptr_ex(const char * const var) {
	boh_throw_ex(new_p4p3c10_aquastdNullPtrException_bc0caf9f(boh_create_string(var, strlen(var))));
	return NULL;
}

void noreturn boh_throw_ex(struct p4p3c9_aquastdException * ex) {
	exception = ex;
	exception_type = NULL;
	longjmp(exception_buf, 1);
}

struct p4p3c6_aquastdString * boh_create_string(const boh_char * const str, size_t len) {
	struct p4p3c6_aquastdString *result = boh_gc_alloc(sizeof(struct p4p3c6_aquastdString));
	result->vtable = &instance_vtable_p4p3c6_aquastdString;
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
