#ifndef BOH_LANG_TYPE_H
#define BOH_LANG_TYPE_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <uchar.h>
#include <longjmp.h>
#include "boh_lang_exception.h"
#include "boh_lang_object.h"
#include "boh_lang_class.h"

struct c_boh_p_lang_p_Type;

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void);




struct vtable_c_boh_p_lang_p_Type
{
	bool (*isSubTypeOf)(struct c_boh_p_lang_p_Type * const self, struct c_boh_p_lang_p_Type * p_type);
};

extern const struct vtable_c_boh_p_lang_p_Type instance_vtable_c_boh_p_lang_p_Type;

struct c_boh_p_lang_p_Type
{
	const struct vtable_c_boh_p_lang_p_Type * vtable;
	int32_t f_antonijn;
};

#endif
