#pragma once

struct p3p3c6_bohstdString;

#include "boh_internal.h"
#include "function_types.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4eB_mypackEnumExample.h"
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3cD_bohstdStringBuilder.h"
#include "p3p3p7c7_bohstdinteropVoidPtr.h"
#include "p3p3p7c7_bohstdinteropInterop.h"
#include "p3p3c17_bohstdBox_my_pack_EnumExample.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3cA_bohstdArray_char.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3c22_bohstdArray_boh_std_Array_boh_std_String.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i10_bohstdICollection_char.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i28_bohstdICollection_boh_std_Array_boh_std_String.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i17_bohstdIIndexedCollection_char.h"
#include "p3p3i16_bohstdIIndexedCollection_int.h"
#include "p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_char.h"
#include "p3p3iD_bohstdIIterator_int.h"
#include "p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String.h"
#include "p3p3c9_bohstdList_char.h"
#include "p3p3c8_bohstdList_int.h"
#include "p3p3p7c8_bohstdinteropPtr_char.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdString(void);

extern struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_51708853(struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length);
extern struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_adeaa357(int32_t p_length);

extern void p3p3c6_bohstdString_fi(struct p3p3c6_bohstdString * const self);

extern void p3p3c6_bohstdString_m_this_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_length);
extern _Bool p3p3c6_bohstdString_m_isNullOrEmpty_ef2d95bf(struct p3p3c6_bohstdString * p_str);
extern _Bool p3p3c6_bohstdString_m_equals_e9664e21(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdObject * p_other);
extern unsigned char p3p3c6_bohstdString_m_get_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_i);
extern struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_idx);
extern struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_dd8c3cec(struct p3p3c6_bohstdString * const self, int32_t p_idx, int32_t p_len);
extern int32_t p3p3c6_bohstdString_m_indexOf_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch);
extern int32_t p3p3c6_bohstdString_m_count_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch);
extern struct p3p3c14_bohstdArray_boh_std_String * p3p3c6_bohstdString_m_split_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch);
extern struct p3p3c6_bohstdString * p3p3c6_bohstdString_op_add_5264d1a0(struct p3p3c6_bohstdString * p_left, struct p3p3c6_bohstdString * p_right);
extern void p3p3c6_bohstdString_m_static_0(void);

extern struct p3p3c6_bohstdString * p3p3c6_bohstdString_sf_empty;

struct vtable_p3p3c6_bohstdString
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c6_bohstdString instance_vtable_p3p3c6_bohstdString;

struct p3p3c6_bohstdString
{
	const struct vtable_p3p3c6_bohstdString * vtable;
	int32_t f_offset;
	int32_t f_length;
	struct p3p3p7c8_bohstdinteropPtr_char f_chars;
};

