#ifndef BOH_LANG_OBJECT_H
#define BOH_LANG_OBJECT_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <uchar.h>
#include <longjmp.h>
#include "boh_lang_exception.h"
#include "boh_lang_type.h"
#include "boh_lang_class.h"

struct c_boh_p_lang_p_Object;

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void);




struct vtable_c_boh_p_lang_p_Object
{
};

extern const struct vtable_c_boh_p_lang_p_Object instance_vtable_c_boh_p_lang_p_Object;

struct c_boh_p_lang_p_Object
{
	const struct vtable_c_boh_p_lang_p_Object * vtable;
};

#endif
