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

extern struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_70fcd6e5(int32_t p_size);

extern void p3p3c6_bohstdString_fi(struct p3p3c6_bohstdString * const self);

extern _Bool p3p3c6_bohstdString_m_isNullOrEmpty_5bf6fcab(struct p3p3c6_bohstdString * p_str);
extern _Bool p3p3c6_bohstdString_m_equals_5289cddf(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdObject * p_other);
extern unsigned char p3p3c6_bohstdString_m_get_70fcd6e5(struct p3p3c6_bohstdString * const self, int32_t p_i);
extern void p3p3c6_bohstdString_m_static_2d2816fe(void);

extern struct p3p3c6_bohstdString * p3p3c6_bohstdString_sf_empty;

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
	int32_t f_offset;
	int32_t f_length;
	boh_char* f_chars;
};

