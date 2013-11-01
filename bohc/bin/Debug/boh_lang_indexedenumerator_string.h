#pragma once

struct c_boh_p_lang_p_IndexedEnumerator_String;

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
#include "boh_lang_array_string.h"
#include "boh_lang_icollection_int.h"
#include "boh_lang_icollection_string.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_string.h"
#include "boh_lang_iindexedcollection_int.h"
#include "boh_lang_iindexedcollection_string.h"
#include "boh_lang_indexedenumerator_int.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_IndexedEnumerator_String(void);

extern struct c_boh_p_lang_p_IndexedEnumerator_String * new_c_boh_p_lang_p_IndexedEnumerator_String(struct c_boh_p_lang_p_IIndexedCollection_String * p_collection);

extern void c_boh_p_lang_p_IndexedEnumerator_String_m_this_4126921818(struct c_boh_p_lang_p_IndexedEnumerator_String * const self, struct c_boh_p_lang_p_IIndexedCollection_String * p_collection);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_IndexedEnumerator_String_m_current_3584862187(struct c_boh_p_lang_p_IndexedEnumerator_String * const self);


struct vtable_c_boh_p_lang_p_IndexedEnumerator_String
{
	struct c_boh_p_lang_p_String * (*m_toString_3584862187)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3584862187)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3584862187)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_4071216051)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_IndexedEnumerator_String instance_vtable_c_boh_p_lang_p_IndexedEnumerator_String;

struct c_boh_p_lang_p_IndexedEnumerator_String
{
	const struct vtable_c_boh_p_lang_p_IndexedEnumerator_String * vtable;
	struct c_boh_p_lang_p_IIndexedCollection_String * f_collection;
	int32_t f_current;
};

