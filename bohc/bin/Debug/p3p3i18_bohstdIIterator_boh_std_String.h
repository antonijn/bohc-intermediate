#pragma once

struct p3p3i18_bohstdIIterator_boh_std_String;

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
#include "p3p3iE_bohstdIIterator_char.h"
#include "p3p3iD_bohstdIIterator_int.h"
#include "p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_long.h"
#include "p3p3c9_bohstdList_char.h"
#include "p3p3c8_bohstdList_int.h"

extern struct p3p3i18_bohstdIIterator_boh_std_String * new_p3p3i18_bohstdIIterator_boh_std_String(struct p3p3c6_bohstdObject * object, _Bool (*m_next_35cf4c)(struct p3p3c6_bohstdObject * const self), _Bool (*m_previous_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_moveLast_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_reset_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c6_bohstdString * (*m_current_35cf4c)(struct p3p3c6_bohstdObject * const self));

struct p3p3i18_bohstdIIterator_boh_std_String
{
	struct p3p3c6_bohstdObject * object;
	_Bool (*m_next_35cf4c)(struct p3p3c6_bohstdObject * const self);
	_Bool (*m_previous_35cf4c)(struct p3p3c6_bohstdObject * const self);
	void (*m_moveLast_35cf4c)(struct p3p3c6_bohstdObject * const self);
	void (*m_reset_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_current_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

