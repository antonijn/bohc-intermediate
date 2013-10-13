#ifndef BOH_INTERNAL_H
#define BOH_INTERNAL_H

#include <setjmp.h>
#if __STDC_VERSION__ >= 201112L
#include <stdnoreturn.h>
#endif

#include <libstdboh/boh_lang_exception.h>

extern struct c_boh_p_lang_p_Exception * exception;
extern jmp_buf exception_buf;

#if __STDC_VERSION__ >= 201112L
void _Noreturn boh_throw_ex(struct c_boh_p_lang_p_Exception * ex);
#else
void boh_throw_ex(struct c_boh_p_lang_p_Exception * ex);
#endif

#endif
