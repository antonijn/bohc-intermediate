/* Copyright (c) 2013 Antonie Blom
 * The antonijn open-source license, draft 1, short form.
 * This source file is licensed under the antonijn open-source license, a
 * full version of which is included with the project.
 * Please refer to the long version for a list of rights and restrictions
 * pertaining to source file use and modification. */

#ifndef BOH_INTERNAL_H
#define BOH_INTERNAL_H

#include <setjmp.h>
#include <wchar.h>
#include <string.h>
#if __STDC_VERSION__ >= 201112L
#include <uchar.h>
#endif

#include "p3p3c9_bohstdException.h"
#include "p3p3c6_bohstdString.h"

typedef _Bool boh_bool;
typedef uint8_t boh_byte;
typedef int16_t boh_short;
typedef int32_t boh_int;
typedef int64_t boh_long;
typedef unsigned char boh_char;
typedef float boh_float;
typedef double boh_double;

extern struct c_boh_p_lang_p_Exception * exception;
extern struct c_boh_p_lang_p_Type * exception_type;
extern jmp_buf exception_buf;

void boh_throw_ex(struct c_boh_p_lang_p_Exception * ex);
struct c_boh_p_lang_p_String * boh_create_string(const boh_char * const str, size_t len);

struct c_boh_p_lang_p_String * boh_create_string_empty(size_t len);

const char * boh_get_cstr(struct c_boh_p_lang_p_String * const str);
const wchar_t * boh_get_wcstr(struct c_boh_p_lang_p_String * const str);
struct c_boh_p_lang_p_String * boh_get_str_from_cstr(const char * const str);
struct c_boh_p_lang_p_String * boh_get_str_from_wcstr(const wchar_t * const str);

#define boh_force_cast(x) (x)

#define boh_deref_ptr(x, y) (x[y])
#define boh_set_deref(x, y, z) (x[y]=z)

typedef struct c_boh_p_lang_p_Object * boh_object;
typedef struct c_boh_p_lang_p_String * boh_string;
typedef struct c_boh_p_lang_p_Type * boh_type;

#define BOHCALL

#endif
