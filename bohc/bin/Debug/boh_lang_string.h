#pragma once

struct c_boh_p_lang_p_String;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
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
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_boh_lang_string.h"
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_String(void);

extern struct c_boh_p_lang_p_String * new_c_boh_p_lang_p_String(void);

extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_toString_3526476(struct c_boh_p_lang_p_String * const self);
extern int32_t c_boh_p_lang_p_String_m_size_3526476(struct c_boh_p_lang_p_String * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_substring_3255497772(struct c_boh_p_lang_p_String * const self, int32_t p_first, int32_t p_len);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_substring_2607005255(struct c_boh_p_lang_p_String * const self, int32_t p_first);
extern int32_t c_boh_p_lang_p_String_m_indexOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch);
extern int32_t c_boh_p_lang_p_String_m_lastIndexOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch);
extern _Bool c_boh_p_lang_p_String_m_stringAtPos_247466877(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str, int32_t p_pos);
extern int32_t c_boh_p_lang_p_String_m_indexOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str);
extern int32_t c_boh_p_lang_p_String_m_lastIndexOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str);
extern struct c_boh_p_lang_p_Array_int * c_boh_p_lang_p_String_m_indicesOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch);
extern struct c_boh_p_lang_p_Array_int * c_boh_p_lang_p_String_m_indicesOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str);
extern struct c_boh_p_lang_p_Array_boh_lang_String * c_boh_p_lang_p_String_m_split_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trim_3526476(struct c_boh_p_lang_p_String * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trimStart_3526476(struct c_boh_p_lang_p_String * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trimEnd_3526476(struct c_boh_p_lang_p_String * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_op_add_4275178606(struct c_boh_p_lang_p_String * p_left, struct c_boh_p_lang_p_Object * p_robj);
extern void c_boh_p_lang_p_String_m_this_3526476(struct c_boh_p_lang_p_String * const self);


struct vtable_c_boh_p_lang_p_String
{
	struct c_boh_p_lang_p_String * (*m_toString_3526476)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3526476)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3526476)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_2378881924)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_String instance_vtable_c_boh_p_lang_p_String;

struct c_boh_p_lang_p_String
{
	const struct vtable_c_boh_p_lang_p_String * vtable;
	int32_t f_length;
	struct c_boh_p_lang_p_Character f_first;
};

