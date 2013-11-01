#pragma once

struct c_boh_p_lang_p_Array_String;

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
#include "boh_lang_icollection_int.h"
#include "boh_lang_icollection_string.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_string.h"
#include "boh_lang_iindexedcollection_int.h"
#include "boh_lang_iindexedcollection_string.h"
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Array_String(void);

extern struct c_boh_p_lang_p_Array_String * new_c_boh_p_lang_p_Array_String(int32_t p_length);

extern void c_boh_p_lang_p_Array_String_m_this_2325378186(struct c_boh_p_lang_p_Array_String * const self, int32_t p_length);
extern int32_t c_boh_p_lang_p_Array_String_m_size_3584862187(struct c_boh_p_lang_p_Array_String * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Array_String_m_get_2325378186(struct c_boh_p_lang_p_Array_String * const self, int32_t p_i);
extern void c_boh_p_lang_p_Array_String_m_set_1796709708(struct c_boh_p_lang_p_Array_String * const self, int32_t p_i, struct c_boh_p_lang_p_String * p_value);
extern struct c_boh_p_lang_p_IIterator_String * c_boh_p_lang_p_Array_String_m_getIterator_3584862187(struct c_boh_p_lang_p_Array_String * const self);


struct vtable_c_boh_p_lang_p_Array_String
{
	struct c_boh_p_lang_p_String * (*m_toString_3584862187)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3584862187)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3584862187)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_4071216051)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_Array_String instance_vtable_c_boh_p_lang_p_Array_String;

struct c_boh_p_lang_p_Array_String
{
	const struct vtable_c_boh_p_lang_p_Array_String * vtable;
	int32_t f_length;
	String* f_items;
};

