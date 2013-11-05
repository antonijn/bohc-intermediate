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

#include "boh_lang_exception.h"
#include "boh_lang_string.h"

extern struct c_boh_p_lang_p_Exception * exception;
extern struct c_boh_p_lang_p_Type * exception_type;
extern jmp_buf exception_buf;

#if __STDC_VERSION__ >= 201112L
void _Noreturn boh_throw_ex(struct c_boh_p_lang_p_Exception * const ex);
struct c_boh_p_lang_p_String * boh_create_string(const char16_t * const str, size_t len);
#else
void boh_throw_ex(struct c_boh_p_lang_p_Exception * ex);
struct c_boh_p_lang_p_String * boh_create_string(const wchar_t * const str, size_t len);
#endif

struct c_boh_p_lang_p_String * boh_create_string_empty(size_t len);

const char * boh_get_cstr(struct c_boh_p_lang_p_String * const str);
const wchar_t * boh_get_wcstr(struct c_boh_p_lang_p_String * const str);
struct c_boh_p_lang_p_String * boh_get_str_from_cstr(const char * const str);
struct c_boh_p_lang_p_String * boh_get_str_from_wcstr(const wchar_t * const str);

#define boh_force_cast(x) (x)
#define boh_r_eq(x, y) ((x) == (y))

#define boh_str_get_ch(str, offs) (&str->f_first)[offs]
#define boh_str_set_ch(str, offs, ch) ((&str->f_first)[offs] = ch)

#define boh_deref_ptr(x, y) (x[y])
#define boh_set_deref(x, y, z) (x[y]=z)

typedef struct c_boh_p_lang_p_Object * boh_object;
typedef struct c_boh_p_lang_p_String * boh_string;
typedef struct c_boh_p_lang_p_Type * boh_type;

typedef _Bool boh_bool;
typedef uint8_t boh_byte;
typedef int16_t boh_short;
typedef int32_t boh_int;
typedef int64_t boh_long;
#if __STDC_VERSION__ >= 201112L
typedef char16_t boh_char;
#else
typedef wchar_t boh_char;
#endif
typedef float boh_float;
typedef double boh_double;

#define BOHCALL

#endif
