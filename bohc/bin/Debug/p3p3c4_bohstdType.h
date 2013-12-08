#pragma once

struct p3p3c4_bohstdType;

#include "boh_internal.h"
#include "function_types.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c4_bohstdType(void);

extern struct p3p3c4_bohstdType * new_p3p3c4_bohstdType_125bf9a2(struct p3p3c6_bohstdString * p_name);

extern void p3p3c4_bohstdType_fi(struct p3p3c4_bohstdType * const self);

extern void p3p3c4_bohstdType_m_this_125bf9a2(struct p3p3c4_bohstdType * const self, struct p3p3c6_bohstdString * p_name);
extern struct p3p3c6_bohstdString * p3p3c4_bohstdType_m_getName_d5aca7eb(struct p3p3c4_bohstdType * const self);
extern _Bool p3p3c4_bohstdType_m_isSubTypeOf_46dba1cc(struct p3p3c4_bohstdType * const self, struct p3p3c4_bohstdType * p_t);
extern void p3p3c4_bohstdType_m_static_2d2816fe(void);


struct vtable_p3p3c4_bohstdType
{
	_Bool (*m_equals_5289cddf)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_getName_d5aca7eb)(struct p3p3c4_bohstdType * const self);
	_Bool (*m_isSubTypeOf_46dba1cc)(struct p3p3c4_bohstdType * const self, struct p3p3c4_bohstdType * p_t);
};

extern const struct vtable_p3p3c4_bohstdType instance_vtable_p3p3c4_bohstdType;

struct p3p3c4_bohstdType
{
	const struct vtable_p3p3c4_bohstdType * vtable;
	struct p3p3c6_bohstdString * f_name;
	struct p3p3c4_bohstdType * f_base;
};

