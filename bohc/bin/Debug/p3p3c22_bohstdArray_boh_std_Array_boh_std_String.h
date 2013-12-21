#pragma once

struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String;

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
#include "p3p3c17_bohstdBox_my_pack_EnumExample.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3cA_bohstdArray_char.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3cA_bohstdArray_long.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i10_bohstdICollection_char.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i28_bohstdICollection_boh_std_Array_boh_std_String.h"
#include "p3p3i10_bohstdICollection_long.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i17_bohstdIIndexedCollection_char.h"
#include "p3p3i16_bohstdIIndexedCollection_int.h"
#include "p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String.h"
#include "p3p3i17_bohstdIIndexedCollection_long.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_char.h"
#include "p3p3iD_bohstdIIterator_int.h"
#include "p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_long.h"
#include "p3p3c9_bohstdList_char.h"
#include "p3p3c8_bohstdList_int.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c22_bohstdArray_boh_std_Array_boh_std_String(void);

extern struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * new_p3p3c22_bohstdArray_boh_std_Array_boh_std_String_adeaa357(int32_t p_length);

extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_fi(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self);

extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_this_adeaa357(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_length);
extern int32_t p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_size_35cf4c(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self);
extern struct p3p3c14_bohstdArray_boh_std_String * p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_get_adeaa357(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_i);
extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_set_9c920c15(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_i, struct p3p3c14_bohstdArray_boh_std_String * p_value);
extern struct p3p3c14_bohstdArray_boh_std_String * p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_getFast_adeaa357(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_i);
extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_setFast_9c920c15(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_i, struct p3p3c14_bohstdArray_boh_std_String * p_value);
extern struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String * p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_iterator_35cf4c(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self);
extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_resize_adeaa357(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_newsize);
extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_move_10aba1b7(struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * const self, int32_t p_dest, int32_t p_src, int32_t p_size);
extern void p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_static_0(void);


struct vtable_p3p3c22_bohstdArray_boh_std_Array_boh_std_String
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c22_bohstdArray_boh_std_Array_boh_std_String instance_vtable_p3p3c22_bohstdArray_boh_std_Array_boh_std_String;

struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String
{
	const struct vtable_p3p3c22_bohstdArray_boh_std_Array_boh_std_String * vtable;
	int32_t f_length;
	struct p3p3c14_bohstdArray_boh_std_String ** f_items;
};

