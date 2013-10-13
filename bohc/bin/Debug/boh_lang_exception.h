#ifndef BOH_LANG_EXCEPTION_H
#define BOH_LANG_EXCEPTION_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <uchar.h>
#include <longjmp.h>
#include "boh_lang_object.h"
#include "boh_lang_type.h"
#include "boh_lang_class.h"

struct c_boh_p_lang_p_Exception;

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void);

extern struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(void);

extern void c_boh_p_lang_p_Exception_m_this(struct c_boh_p_lang_p_Exception * const self);


struct vtable_c_boh_p_lang_p_Exception
{
};

extern const struct vtable_c_boh_p_lang_p_Exception instance_vtable_c_boh_p_lang_p_Exception;

struct c_boh_p_lang_p_Exception
{
	const struct vtable_c_boh_p_lang_p_Exception * vtable;
};

#endif
