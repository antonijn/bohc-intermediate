#pragma once

struct c_boh_p_lang_p_IndexedEnumerator_int;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "boh_lang_string.h"
#include "boh_lang_exception.h"
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
#include "boh_lang_indexedenumerator_boh_lang_string.h"
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_IndexedEnumerator_int(void);

extern struct c_boh_p_lang_p_IndexedEnumerator_int * new_c_boh_p_lang_p_IndexedEnumerator_int(struct c_boh_p_lang_p_IIndexedCollection_int * p_collection);

extern void c_boh_p_lang_p_IndexedEnumerator_int_m_this_3422026619(struct c_boh_p_lang_p_IndexedEnumerator_int * const self, struct c_boh_p_lang_p_IIndexedCollection_int * p_collection);
extern int32_t c_boh_p_lang_p_IndexedEnumerator_int_m_current_3526476(struct c_boh_p_lang_p_IndexedEnumerator_int * const self);


struct vtable_c_boh_p_lang_p_IndexedEnumerator_int
{
	struct c_boh_p_lang_p_String * (*m_toString_3526476)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3526476)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3526476)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_2378881924)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_IndexedEnumerator_int instance_vtable_c_boh_p_lang_p_IndexedEnumerator_int;

struct c_boh_p_lang_p_IndexedEnumerator_int
{
	const struct vtable_c_boh_p_lang_p_IndexedEnumerator_int * vtable;
	struct c_boh_p_lang_p_IIndexedCollection_int * f_collection;
	int32_t f_current;
};

