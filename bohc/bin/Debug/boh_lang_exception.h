#pragma once

struct c_boh_p_lang_p_Exception;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "boh_lang_string.h"
#include "boh_lang_object.h"
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

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Exception(void);

extern struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(void);
extern struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(struct c_boh_p_lang_p_String * p_description);

extern void c_boh_p_lang_p_Exception_m_this_3526476(struct c_boh_p_lang_p_Exception * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_stackTrace_3526476(struct c_boh_p_lang_p_Exception * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_getDescription_3526476(struct c_boh_p_lang_p_Exception * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_what_3526476(struct c_boh_p_lang_p_Exception * const self);
extern void c_boh_p_lang_p_Exception_m_this_2510264406(struct c_boh_p_lang_p_Exception * const self, struct c_boh_p_lang_p_String * p_description);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_toString_3526476(struct c_boh_p_lang_p_Exception * const self);


struct vtable_c_boh_p_lang_p_Exception
{
	struct c_boh_p_lang_p_String * (*m_toString_3526476)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3526476)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3526476)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_2378881924)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
	struct c_boh_p_lang_p_String * (*m_what_3526476)(struct c_boh_p_lang_p_Exception * const self);
};

extern const struct vtable_c_boh_p_lang_p_Exception instance_vtable_c_boh_p_lang_p_Exception;

struct c_boh_p_lang_p_Exception
{
	const struct vtable_c_boh_p_lang_p_Exception * vtable;
	struct c_boh_p_lang_p_String * f_stackTrace;
	struct c_boh_p_lang_p_String * f_description;
};

