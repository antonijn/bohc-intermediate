#include "p3p3c9_bohstdQuery_int.h"



const struct vtable_p3p3c9_bohstdQuery_int instance_vtable_p3p3c9_bohstdQuery_int = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdQuery_int(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c9_bohstdQuery_int * new_p3p3c9_bohstdQuery_int_e625b83f(struct p3p3iF_bohstdICollection_int * p_base)
{
	struct p3p3c9_bohstdQuery_int * result = GC_malloc(sizeof(struct p3p3c9_bohstdQuery_int));
	result->vtable = &instance_vtable_p3p3c9_bohstdQuery_int;
	p3p3c9_bohstdQuery_int_m_static_0();
	p3p3c9_bohstdQuery_int_fi(result);
	p3p3c9_bohstdQuery_int_m_this_e625b83f(result, p_base);
	return result;
}

void p3p3c9_bohstdQuery_int_fi(struct p3p3c9_bohstdQuery_int * const self)
{
	self->f_base = NULL;
}

void p3p3c9_bohstdQuery_int_m_this_e625b83f(struct p3p3c9_bohstdQuery_int * const self, struct p3p3iF_bohstdICollection_int * p_base)
{
	(self->f_base = p_base);
}
struct p3p3iD_bohstdIIterator_int * p3p3c9_bohstdQuery_int_m_iterator_35cf4c(struct p3p3c9_bohstdQuery_int * const self)
{
	struct p3p3iF_bohstdICollection_int * temp34;
	return (temp34 = self->f_base)->m_iterator_35cf4c(temp34->object);
}
struct p3p3c9_bohstdQuery_int * p3p3c9_bohstdQuery_int_m_where_5a618770(struct p3p3c9_bohstdQuery_int * const self, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3iF_bohstdICollection_int * temp35;
	return new_p3p3c11_bohstdWhereIterator_int_1c6adf31((temp35 = self->f_base)->m_iterator_35cf4c(temp35->object), p_condition);
}
struct p3p3c9_bohstdQuery_int * p3p3c9_bohstdQuery_int_m_query_35cf4c(struct p3p3c9_bohstdQuery_int * const self)
{
	return self;
}
void p3p3c9_bohstdQuery_int_m_static_0(void)
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
