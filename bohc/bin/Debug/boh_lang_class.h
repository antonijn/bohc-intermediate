#ifndef BOH_LANG_CLASS_H
#define BOH_LANG_CLASS_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
#include <uchar.h>
#include <longjmp.h>
#include "boh_lang_exception.h"
#include "boh_lang_object.h"
#include "boh_lang_type.h"

struct c_boh_p_lang_p_Class;

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void);

extern struct c_boh_p_lang_p_Class * new_c_boh_p_lang_p_Class(void);

extern bool c_boh_p_lang_p_Class_m_isSubTypeOf(struct c_boh_p_lang_p_Class * const self, struct c_boh_p_lang_p_Type * p_other);
extern struct c_boh_p_lang_p_Class * c_boh_p_lang_p_Class_m_getBase(struct c_boh_p_lang_p_Class * const self);
extern void c_boh_p_lang_p_Class_m_this(struct c_boh_p_lang_p_Class * const self);


struct vtable_c_boh_p_lang_p_Class
{
	bool (*isSubTypeOf)(struct c_boh_p_lang_p_Type * const self, struct c_boh_p_lang_p_Type * p_type);
};

extern const struct vtable_c_boh_p_lang_p_Class instance_vtable_c_boh_p_lang_p_Class;

struct c_boh_p_lang_p_Class
{
	const struct vtable_c_boh_p_lang_p_Class * vtable;
	int32_t f_antonijn;
	struct c_boh_p_lang_p_Class * f_base;
};

#endif
