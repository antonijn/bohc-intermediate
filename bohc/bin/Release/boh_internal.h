/* Copyright (c) 2013 Antonie Blom
 * The antonijn open-source license, draft 1, short form.
 * This source file is licensed under the antonijn open-source license, a
 * full version of which is included with the project.
 * Please refer to the long version for a list of rights and restrictions
 * pertaining to source file use and modification. */

#pragma once

#include <setjmp.h>
#include <wchar.h>
#include <string.h>
#include <uchar.h>
#include <stdint.h>
#include <stdnoreturn.h>

#ifdef __cplusplus
#define bohcall extern "C"
#else
#define bohcall
#endif

bohcall noreturn void * boh_throw_null_ptr_ex(const char * const var);

#define boh_check_null(X, T) (X == NULL ? *(T*)boh_throw_null_ptr_ex(#X) : X)

bohcall void * boh_gc_alloc(size_t size);
bohcall void * boh_gc_realloc(void * ptr, size_t size);
bohcall void boh_gc_collect(void);

struct p3p3c9_bohstdException;
struct p3p3c6_bohstdString;
struct p3p3c4_bohstdType;
struct p3p3c6_bohstdObject;

typedef _Bool boh_bool;
typedef uint8_t boh_byte;
typedef int16_t boh_short;
typedef int32_t boh_int;
typedef int64_t boh_long;
typedef unsigned char boh_char;
typedef float boh_float;
typedef double boh_double;

extern struct p3p3c9_bohstdException * exception;
extern struct p3p3c4_bohstdType * exception_type;
extern jmp_buf exception_buf;

bohcall noreturn void boh_throw_ex(struct p3p3c9_bohstdException * ex);
bohcall struct p3p3c6_bohstdString * boh_create_string(const boh_char * const str, size_t len);

bohcall struct p3p3c6_bohstdString * boh_create_string_empty(size_t len);

bohcall const char * boh_get_cstr(struct p3p3c6_bohstdString * const str);
bohcall const wchar_t * boh_get_wcstr(struct p3p3c6_bohstdString * const str);
bohcall struct p3p3c6_bohstdString * boh_get_str_from_cstr(const char * const str);
bohcall struct p3p3c6_bohstdString * boh_get_str_from_wcstr(const wchar_t * const str);

typedef struct p3p3c6_bohstdObject * bobject;
typedef struct p3p3c6_bohstdString * bstring;
typedef struct p3p3c4_bohstdType * btype;
