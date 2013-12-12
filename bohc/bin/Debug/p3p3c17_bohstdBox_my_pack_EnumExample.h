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
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i16_bohstdIIndexedCollection_int.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
#include "p3p3iD_bohstdIIterator_int.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c17_bohstdBox_my_pack_EnumExample(void);

extern struct p3p3c17_bohstdBox_my_pack_EnumExample * new_p3p3c17_bohstdBox_my_pack_EnumExample_154530dc(enum p2p4eB_mypackEnumExample p_value);

extern void p3p3c17_bohstdBox_my_pack_EnumExample_fi(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self);

extern void p3p3c17_bohstdBox_my_pack_EnumExample_m_this_154530dc(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self, enum p2p4eB_mypackEnumExample p_value);
extern struct p3p3c6_bohstdString * p3p3c17_bohstdBox_my_pack_EnumExample_m_toString_d5aca7eb(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self);
extern void p3p3c17_bohstdBox_my_pack_EnumExample_m_static_2d2816fe(void);


struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample
{
	_Bool (*m_equals_5289cddf)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample instance_vtable_p3p3c17_bohstdBox_my_pack_EnumExample;

struct p3p3c17_bohstdBox_my_pack_EnumExample
{
	const struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample * vtable;
	enum p2p4eB_mypackEnumExample f_value;
};

