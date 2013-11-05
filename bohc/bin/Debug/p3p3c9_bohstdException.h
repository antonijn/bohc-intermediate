#pragma once

struct p3p3c9_bohstdException;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"

extern struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdException(void);

extern struct p3p3c9_bohstdException * new_p3p3c9_bohstdException(struct p3p3c6_bohstdString * p_description);
extern struct p3p3c9_bohstdException * new_p3p3c9_bohstdException(void);

extern struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_what_d5aca7eb(struct p3p3c9_bohstdException * const self);
extern struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_getDescription_d5aca7eb(struct p3p3c9_bohstdException * const self);
extern void p3p3c9_bohstdException_m_this_125bf9a2(struct p3p3c9_bohstdException * const self, struct p3p3c6_bohstdString * p_description);
extern void p3p3c9_bohstdException_m_this_d5aca7eb(struct p3p3c9_bohstdException * const self);


struct vtable_p3p3c9_bohstdException
{
	_Bool (*m_equals_5289cddf)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_d5aca7eb)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_what_d5aca7eb)(struct p3p3c9_bohstdException * const self);
	struct p3p3c6_bohstdString * (*m_getDescription_d5aca7eb)(struct p3p3c9_bohstdException * const self);
};

extern const struct vtable_p3p3c9_bohstdException instance_vtable_p3p3c9_bohstdException;

struct p3p3c9_bohstdException
{
	const struct vtable_p3p3c9_bohstdException * vtable;
	struct p3p3c6_bohstdString * f_description;
};

