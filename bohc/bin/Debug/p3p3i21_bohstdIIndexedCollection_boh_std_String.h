#pragma once

struct p3p3i21_bohstdIIndexedCollection_boh_std_String;

#include "boh_internal.h"
#include "function_types.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"

extern struct p3p3i21_bohstdIIndexedCollection_boh_std_String * new_p3p3i21_bohstdIIndexedCollection_boh_std_String(struct p3p3c6_bohstdObject * object, struct p3p3i18_bohstdIIterator_boh_std_String * (*m_iterator_d5aca7eb)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_d5aca7eb)(struct p3p3c6_bohstdObject * const self), struct p3p3c6_bohstdString * (*m_get_70fcd6e5)(struct p3p3c6_bohstdObject * const self, int32_t p_i));

struct p3p3i21_bohstdIIndexedCollection_boh_std_String
{
	struct p3p3c6_bohstdObject * object;
	struct p3p3i18_bohstdIIterator_boh_std_String * (*m_iterator_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	int32_t (*m_size_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_get_70fcd6e5)(struct p3p3c6_bohstdObject * const self, int32_t p_i);
};

