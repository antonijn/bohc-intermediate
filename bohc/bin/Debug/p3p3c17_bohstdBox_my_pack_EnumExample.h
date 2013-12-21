#pragma once

struct p3p3c17_bohstdBox_my_pack_EnumExample;

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
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3cA_bohstdArray_char.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3c22_bohstdArray_boh_std_Array_boh_std_String.h"
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

extern struct p3p3c4_bohstdType * typeof_p3p3c17_bohstdBox_my_pack_EnumExample(void);

extern struct p3p3c17_bohstdBox_my_pack_EnumExample * new_p3p3c17_bohstdBox_my_pack_EnumExample_3b9b8234(enum p2p4eB_mypackEnumExample p_value);

extern void p3p3c17_bohstdBox_my_pack_EnumExample_fi(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self);

extern void p3p3c17_bohstdBox_my_pack_EnumExample_m_this_3b9b8234(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self, enum p2p4eB_mypackEnumExample p_value);
extern struct p3p3c6_bohstdString * p3p3c17_bohstdBox_my_pack_EnumExample_m_toString_35cf4c(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self);
extern void p3p3c17_bohstdBox_my_pack_EnumExample_m_static_0(void);


struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample instance_vtable_p3p3c17_bohstdBox_my_pack_EnumExample;

struct p3p3c17_bohstdBox_my_pack_EnumExample
{
	const struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample * vtable;
	enum p2p4eB_mypackEnumExample f_value;
};

