#pragma once

struct p3p3c6_bohstdString;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdString(void);

extern struct p3p3c6_bohstdString * new_p3p3c6_bohstdString(void);

extern void p3p3c6_bohstdString_m_this_d5aca7eb(struct p3p3c6_bohstdString * const self);


struct vtable_p3p3c6_bohstdString
{
	_Bool (*m_equals_5289cddf)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c6_bohstdString instance_vtable_p3p3c6_bohstdString;

struct p3p3c6_bohstdString
{
	const struct vtable_p3p3c6_bohstdString * vtable;
	struct p3p3c6_bohstdString * f_str;
	int32_t f_length;
	char16_t f_first;
};

