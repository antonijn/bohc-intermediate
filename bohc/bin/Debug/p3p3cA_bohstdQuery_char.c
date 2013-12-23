#include "p3p3cA_bohstdQuery_char.h"



const struct vtable_p3p3cA_bohstdQuery_char instance_vtable_p3p3cA_bohstdQuery_char = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3cA_bohstdQuery_char(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3cA_bohstdQuery_char * new_p3p3cA_bohstdQuery_char_673bcabf(struct p3p3i10_bohstdICollection_char * p_base)
{
	struct p3p3cA_bohstdQuery_char * result = GC_malloc(sizeof(struct p3p3cA_bohstdQuery_char));
	result->vtable = &instance_vtable_p3p3cA_bohstdQuery_char;
	p3p3cA_bohstdQuery_char_m_static_0();
	p3p3cA_bohstdQuery_char_fi(result);
	p3p3cA_bohstdQuery_char_m_this_673bcabf(result, p_base);
	return result;
}

void p3p3cA_bohstdQuery_char_fi(struct p3p3cA_bohstdQuery_char * const self)
{
	self->f_base = NULL;
}

void p3p3cA_bohstdQuery_char_m_this_673bcabf(struct p3p3cA_bohstdQuery_char * const self, struct p3p3i10_bohstdICollection_char * p_base)
{
	(self->f_base = p_base);
}
struct p3p3iE_bohstdIIterator_char * p3p3cA_bohstdQuery_char_m_iterator_35cf4c(struct p3p3cA_bohstdQuery_char * const self)
{
	struct p3p3i10_bohstdICollection_char * temp32;
	return (temp32 = self->f_base)->m_iterator_35cf4c(temp32->object);
}
struct p3p3cA_bohstdQuery_char * p3p3cA_bohstdQuery_char_m_where_5a618770(struct p3p3cA_bohstdQuery_char * const self, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3i10_bohstdICollection_char * temp33;
	return new_p3p3c12_bohstdWhereIterator_char_17589fbb((temp33 = self->f_base)->m_iterator_35cf4c(temp33->object), p_condition);
}
struct p3p3cA_bohstdQuery_char * p3p3cA_bohstdQuery_char_m_query_35cf4c(struct p3p3cA_bohstdQuery_char * const self)
{
	return self;
}
void p3p3cA_bohstdQuery_char_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	{
	}
}
