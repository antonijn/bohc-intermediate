#pragma once

struct c_boh_p_lang_p_Object;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "boh_lang_string.h"
#include "boh_lang_exception.h"
#include "boh_lang_type.h"
#include "boh_lang_package.h"
#include "boh_lang_character.h"
#include "boh_lang_array_int.h"
#include "boh_lang_array_boh_lang_string.h"
#include "boh_lang_icollection_int.h"
#include "boh_lang_icollection_boh_lang_string.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_boh_lang_string.h"
#include "boh_lang_iindexedcollection_int.h"
#include "boh_lang_iindexedcollection_boh_lang_string.h"
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_boh_lang_string.h"
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Object(void);

extern struct c_boh_p_lang_p_Object * new_c_boh_p_lang_p_Object(void);

extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Object_m_toString_3526476(struct c_boh_p_lang_p_Object * const self);
extern int64_t c_boh_p_lang_p_Object_m_hash_3526476(struct c_boh_p_lang_p_Object * const self);
extern struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Object_m_getType_3526476(struct c_boh_p_lang_p_Object * const self);
extern _Bool c_boh_p_lang_p_Object_m_equals_2378881924(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
extern _Bool c_boh_p_lang_p_Object_m_is_713218619(struct c_boh_p_lang_p_Object * p_o, struct c_boh_p_lang_p_Type * p_t);
extern _Bool c_boh_p_lang_p_Object_m_valEquals_2338730496(struct c_boh_p_lang_p_Object * p_l, struct c_boh_p_lang_p_Object * p_r);
extern void c_boh_p_lang_p_Object_m_this_3526476(struct c_boh_p_lang_p_Object * const self);


struct vtable_c_boh_p_lang_p_Object
{
	struct c_boh_p_lang_p_String * (*m_toString_3526476)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3526476)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3526476)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_2378881924)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_Object instance_vtable_c_boh_p_lang_p_Object;

struct c_boh_p_lang_p_Object
{
	const struct vtable_c_boh_p_lang_p_Object * vtable;
};

