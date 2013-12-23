#include "p3p3c14_bohstdQuery_boh_std_String.h"



const struct vtable_p3p3c14_bohstdQuery_boh_std_String instance_vtable_p3p3c14_bohstdQuery_boh_std_String = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c14_bohstdQuery_boh_std_String(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c14_bohstdQuery_boh_std_String * new_p3p3c14_bohstdQuery_boh_std_String_f67e4109(struct p3p3i1A_bohstdICollection_boh_std_String * p_base)
{
	struct p3p3c14_bohstdQuery_boh_std_String * result = GC_malloc(sizeof(struct p3p3c14_bohstdQuery_boh_std_String));
	result->vtable = &instance_vtable_p3p3c14_bohstdQuery_boh_std_String;
	p3p3c14_bohstdQuery_boh_std_String_m_static_0();
	p3p3c14_bohstdQuery_boh_std_String_fi(result);
	p3p3c14_bohstdQuery_boh_std_String_m_this_f67e4109(result, p_base);
	return result;
}

void p3p3c14_bohstdQuery_boh_std_String_fi(struct p3p3c14_bohstdQuery_boh_std_String * const self)
{
	self->f_base = NULL;
}

void p3p3c14_bohstdQuery_boh_std_String_m_this_f67e4109(struct p3p3c14_bohstdQuery_boh_std_String * const self, struct p3p3i1A_bohstdICollection_boh_std_String * p_base)
{
	(self->f_base = p_base);
}
struct p3p3i18_bohstdIIterator_boh_std_String * p3p3c14_bohstdQuery_boh_std_String_m_iterator_35cf4c(struct p3p3c14_bohstdQuery_boh_std_String * const self)
{
	struct p3p3i1A_bohstdICollection_boh_std_String * temp30;
	return (temp30 = self->f_base)->m_iterator_35cf4c(temp30->object);
}
struct p3p3c14_bohstdQuery_boh_std_String * p3p3c14_bohstdQuery_boh_std_String_m_where_5a618770(struct p3p3c14_bohstdQuery_boh_std_String * const self, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3i1A_bohstdICollection_boh_std_String * temp31;
	return new_p3p3c1C_bohstdWhereIterator_boh_std_String_e84002c6((temp31 = self->f_base)->m_iterator_35cf4c(temp31->object), p_condition);
}
struct p3p3c14_bohstdQuery_boh_std_String * p3p3c14_bohstdQuery_boh_std_String_m_query_35cf4c(struct p3p3c14_bohstdQuery_boh_std_String * const self)
{
	return self;
}
void p3p3c14_bohstdQuery_boh_std_String_m_static_0(void)
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
