#pragma once

struct p2p4c9_mypackMainClass;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"

extern struct p3p3c4_bohstdType * typeof_p2p4c9_mypackMainClass(void);

extern struct p2p4c9_mypackMainClass * new_p2p4c9_mypackMainClass_d5aca7eb(void);

extern void p2p4c9_mypackMainClass_fi(struct p2p4c9_mypackMainClass * const self);

extern void p2p4c9_mypackMainClass_m_main_2d2816fe(void);
extern void p2p4c9_mypackMainClass_m_this_d5aca7eb(struct p2p4c9_mypackMainClass * const self);
extern void p2p4c9_mypackMainClass_m_static_2d2816fe(void);


struct vtable_p2p4c9_mypackMainClass
{
	_Bool (*m_equals_5289cddf)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p2p4c9_mypackMainClass instance_vtable_p2p4c9_mypackMainClass;

struct p2p4c9_mypackMainClass
{
	const struct vtable_p2p4c9_mypackMainClass * vtable;
};

