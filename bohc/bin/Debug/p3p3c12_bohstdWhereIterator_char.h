#pragma once

struct p3p3c12_bohstdWhereIterator_char;

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

extern struct p3p3c4_bohstdType * typeof_p3p3c12_bohstdWhereIterator_char(void);

extern struct p3p3c12_bohstdWhereIterator_char * new_p3p3c12_bohstdWhereIterator_char_17589fbb(struct p3p3iE_bohstdIIterator_char * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition);

extern void p3p3c12_bohstdWhereIterator_char_fi(struct p3p3c12_bohstdWhereIterator_char * const self);

extern void p3p3c12_bohstdWhereIterator_char_m_this_17589fbb(struct p3p3c12_bohstdWhereIterator_char * const self, struct p3p3iE_bohstdIIterator_char * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition);
extern _Bool p3p3c12_bohstdWhereIterator_char_m_next_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self);
extern _Bool p3p3c12_bohstdWhereIterator_char_m_previous_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self);
extern void p3p3c12_bohstdWhereIterator_char_m_moveLast_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self);
extern void p3p3c12_bohstdWhereIterator_char_m_reset_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self);
extern unsigned char p3p3c12_bohstdWhereIterator_char_m_current_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self);
extern void p3p3c12_bohstdWhereIterator_char_m_static_0(void);


struct vtable_p3p3c12_bohstdWhereIterator_char
{
	_Bool (*m_equals_e9664e21)(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other);
	int64_t (*m_hash_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c4_bohstdType * (*m_getType_35cf4c)(struct p3p3c6_bohstdObject * const self);
	struct p3p3c6_bohstdString * (*m_toString_35cf4c)(struct p3p3c6_bohstdObject * const self);
};

extern const struct vtable_p3p3c12_bohstdWhereIterator_char instance_vtable_p3p3c12_bohstdWhereIterator_char;

struct p3p3c12_bohstdWhereIterator_char
{
	const struct vtable_p3p3c12_bohstdWhereIterator_char * vtable;
	struct p3p3iE_bohstdIIterator_char * f_base;
	struct f1E_p07_booleanp3p3c6_bohstdString f_condition;
};

