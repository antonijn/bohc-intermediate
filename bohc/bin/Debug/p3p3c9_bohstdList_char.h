#pragma once

struct p3p3c9_bohstdList_char;

#include "boh_internal.h"
#include "function_types.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4eB_mypackEnumExample.h"
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
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
#include "p3p3c8_bohstdList_int.h"
#include "p3p3p7c8_bohstdinteropPtr_char.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdList_char(void);

extern struct p3p3c9_bohstdList_char * new_p3p3c9_bohstdList_char_adeaa357(int32_t p_capacity);
extern struct p3p3c9_bohstdList_char * new_p3p3c9_bohstdList_char_35cf4c(void);

extern void p3p3c9_bohstdList_char_fi(struct p3p3c9_bohstdList_char * const self);

extern int32_t p3p3c9_bohstdList_char_m_size_35cf4c(struct p3p3c9_bohstdList_char * const self);
extern int32_t p3p3c9_bohstdList_char_m_capacity_35cf4c(struct p3p3c9_bohstdList_char * const self);
extern void p3p3c9_bohstdList_char_m_this_adeaa357(struct p3p3c9_bohstdList_char * const self, int32_t p_capacity);
extern void p3p3c9_bohstdList_char_m_this_35cf4c(struct p3p3c9_bohstdList_char * const self);
extern struct p3p3iE_bohstdIIterator_char * p3p3c9_bohstdList_char_m_iterator_35cf4c(struct p3p3c9_bohstdList_char * const self);
extern unsigned char p3p3c9_bohstdList_char_m_get_adeaa357(struct p3p3c9_bohstdList_char * const self, int32_t p_index);
extern void p3p3c9_bohstdList_char_m_set_d5ad6698(struct p3p3c9_bohstdList_char * const self, int32_t p_index, unsigned char p_value);
extern int32_t p3p3c9_bohstdList_char_m_indexOf_111bcd8d(struct p3p3c9_bohstdList_char * const self, unsigned char p_item);
extern void p3p3c9_bohstdList_char_m_add_111bcd8d(struct p3p3c9_bohstdList_char * const self, unsigned char p_item);
extern void p3p3c9_bohstdList_char_m_add_cdea1985(struct p3p3c9_bohstdList_char * const self, struct p3p3i17_bohstdIIndexedCollection_char * p_items);
extern void p3p3c9_bohstdList_char_m_insert_d5ad6698(struct p3p3c9_bohstdList_char * const self, int32_t p_i, unsigned char p_item);
extern void p3p3c9_bohstdList_char_m_remove_111bcd8d(struct p3p3c9_bohstdList_char * const self, unsigned char p_item);
extern void p3p3c9_bohstdList_char_m_removeWhen_a6fd6f77(struct p3p3c9_bohstdList_char * const self, struct f1A_p07_booleanp04_charp03_int p_when);
extern void p3p3c9_bohstdList_char_m_removeWhen_bc4fa1e(struct p3p3c9_bohstdList_char * const self, struct f13_p07_booleanp04_char p_when);
extern void p3p3c9_bohstdList_char_m_removeAll_111bcd8d(struct p3p3c9_bohstdList_char * const self, unsigned char p_item);
extern void p3p3c9_bohstdList_char_m_removeRange_dd8c3cec(struct p3p3c9_bohstdList_char * const self, int32_t p_start, int32_t p_amount);
extern void p3p3c9_bohstdList_char_m_removeAt_adeaa357(struct p3p3c9_bohstdList_char * const self, int32_t p_i);
extern void p3p3c9_bohstdList_char_m_static_0(void);

extern int32_t p3p3c9_bohstdList_char_sf_INIcharIAL_CAPACIcharY;

struct vtable_p3p3c9_bohstdList_char
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c9_bohstdList_char instance_vtable_p3p3c9_bohstdList_char;

struct p3p3c9_bohstdList_char
{
	const struct vtable_p3p3c9_bohstdList_char * vtable;
	struct p3p3cA_bohstdArray_char * f_array;
	int32_t f_length;
};

