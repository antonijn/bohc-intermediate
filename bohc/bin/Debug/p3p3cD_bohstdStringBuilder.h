#pragma once

struct p3p3cD_bohstdStringBuilder;

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
#include "p3p3i16_bohstdIIndexedCollection_int.h"
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

extern struct p3p3c4_bohstdType * typeof_p3p3cD_bohstdStringBuilder(void);

extern struct p3p3cD_bohstdStringBuilder * new_p3p3cD_bohstdStringBuilder_adeaa357(int32_t p_capacity);
extern struct p3p3cD_bohstdStringBuilder * new_p3p3cD_bohstdStringBuilder_35cf4c(void);

extern void p3p3cD_bohstdStringBuilder_fi(struct p3p3cD_bohstdStringBuilder * const self);

extern void p3p3cD_bohstdStringBuilder_m_this_adeaa357(struct p3p3cD_bohstdStringBuilder * const self, int32_t p_capacity);
extern void p3p3cD_bohstdStringBuilder_m_this_35cf4c(struct p3p3cD_bohstdStringBuilder * const self);
extern void p3p3cD_bohstdStringBuilder_m_append_f13b0af3(struct p3p3cD_bohstdStringBuilder * const self, struct p3p3c6_bohstdString * p_str);
extern void p3p3cD_bohstdStringBuilder_m_append_111bcd8d(struct p3p3cD_bohstdStringBuilder * const self, unsigned char p_ch);
extern void p3p3cD_bohstdStringBuilder_m_toString_35cf4c(struct p3p3cD_bohstdStringBuilder * const self);
extern void p3p3cD_bohstdStringBuilder_m_static_0(void);


struct vtable_p3p3cD_bohstdStringBuilder
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3cD_bohstdStringBuilder instance_vtable_p3p3cD_bohstdStringBuilder;

struct p3p3cD_bohstdStringBuilder
{
	const struct vtable_p3p3cD_bohstdStringBuilder * vtable;
	struct p3p3c9_bohstdList_char * f_characters;
};

