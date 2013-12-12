#pragma once

struct p3p3iD_bohstdIIterator_int;

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
#include "p3p3c17_bohstdBox_my_pack_EnumExample.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i16_bohstdIIndexedCollection_int.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"

extern struct p3p3iD_bohstdIIterator_int * new_p3p3iD_bohstdIIterator_int(struct p3p3c6_bohstdObject * object, _Bool (*m_next_d5aca7eb)(struct p3p3c6_bohstdObject * const self), _Bool (*m_previous_d5aca7eb)(struct p3p3c6_bohstdObject * const self), void (*m_moveLast_d5aca7eb)(struct p3p3c6_bohstdObject * const self), void (*m_reset_d5aca7eb)(struct p3p3c6_bohstdObject * const self), int32_t (*m_current_d5aca7eb)(struct p3p3c6_bohstdObject * const self));

struct p3p3iD_bohstdIIterator_int
{
	struct p3p3c6_bohstdObject * object;
	_Bool (*m_next_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	_Bool (*m_previous_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	void (*m_moveLast_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	void (*m_reset_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	int32_t (*m_current_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
};

