#ifndef BOH_LANG_TYPE_H
#define BOH_LANG_TYPE_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <uchar.h>
#include <longjmp.h>
#include "boh_lang_exception.h"
#include "boh_lang_object.h"

struct c_boh_p_lang_p_Type;

extern struct c_boh_p_lang_p_Type * new_c_boh_p_lang_p_Type(void);

extern struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Type_m_getType(struct c_boh_p_lang_p_Type * const self);
extern void c_boh_p_lang_p_Type_m_this(struct c_boh_p_lang_p_Type * const self);


struct vtable_c_boh_p_lang_p_Type
{
	struct c_boh_p_lang_p_Type * (*getType)(struct c_boh_p_lang_p_Object * const self);
};

extern const struct vtable_c_boh_p_lang_p_Type instance_vtable_c_boh_p_lang_p_Type;

struct c_boh_p_lang_p_Type
{
	const struct vtable_c_boh_p_lang_p_Type * vtable;
};

#endif
