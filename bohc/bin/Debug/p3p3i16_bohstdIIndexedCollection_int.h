#pragma once

struct p3p3i16_bohstdIIndexedCollection_int;

#include "boh_internal.h"
#include "function_types.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3cD_bohstdStringBuilder.h"
#include "p3p3p7c7_bohstdinteropVoidPtr.h"
#include "p3p3p7c7_bohstdinteropInterop.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3cA_bohstdArray_char.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i10_bohstdICollection_char.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i17_bohstdIIndexedCollection_char.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_char.h"
#include "p3p3iD_bohstdIIterator_int.h"
#include "p3p3c9_bohstdList_char.h"
#include "p3p3c8_bohstdList_int.h"
#include "p3p3p7c8_bohstdinteropPtr_char.h"
#include "p3p3c14_bohstdQuery_boh_std_String.h"
#include "p3p3cA_bohstdQuery_char.h"
#include "p3p3c9_bohstdQuery_int.h"
#include "p3p3c1C_bohstdWhereIterator_boh_std_String.h"
#include "p3p3c11_bohstdWhereIterator_int.h"
#include "p3p3c12_bohstdWhereIterator_char.h"

extern struct p3p3i16_bohstdIIndexedCollection_int * new_p3p3i16_bohstdIIndexedCollection_int(struct p3p3c6_bohstdObject * object, struct p3p3iD_bohstdIIterator_int * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c9_bohstdQuery_int * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_get_adeaa357)(struct p3p3c6_bohstdObject * const self, int32_t p_i));

struct p3p3i16_bohstdIIndexedCollection_int
{
	struct p3p3c6_bohstdObject * object;
	struct p3p3iD_bohstdIIterator_int * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c9_bohstdQuery_int * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self);
	int32_t (*m_size_35cf4c)(struct p3p3c6_bohstdObject * const self);
	int32_t (*m_get_adeaa357)(struct p3p3c6_bohstdObject * const self, int32_t p_i);
};

