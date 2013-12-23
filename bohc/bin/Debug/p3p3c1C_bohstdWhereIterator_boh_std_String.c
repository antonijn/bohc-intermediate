#include "p3p3c1C_bohstdWhereIterator_boh_std_String.h"



const struct vtable_p3p3c1C_bohstdWhereIterator_boh_std_String instance_vtable_p3p3c1C_bohstdWhereIterator_boh_std_String = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c1C_bohstdWhereIterator_boh_std_String(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c1C_bohstdWhereIterator_boh_std_String * new_p3p3c1C_bohstdWhereIterator_boh_std_String_e84002c6(struct p3p3i18_bohstdIIterator_boh_std_String * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3c1C_bohstdWhereIterator_boh_std_String * result = GC_malloc(sizeof(struct p3p3c1C_bohstdWhereIterator_boh_std_String));
	result->vtable = &instance_vtable_p3p3c1C_bohstdWhereIterator_boh_std_String;
	p3p3c1C_bohstdWhereIterator_boh_std_String_m_static_0();
	p3p3c1C_bohstdWhereIterator_boh_std_String_fi(result);
	p3p3c1C_bohstdWhereIterator_boh_std_String_m_this_e84002c6(result, p_base, p_condition);
	return result;
}

void p3p3c1C_bohstdWhereIterator_boh_std_String_fi(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	self->f_base = NULL;
	self->f_condition = BOH_FP_NULL;
}

void p3p3c1C_bohstdWhereIterator_boh_std_String_m_this_e84002c6(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self, struct p3p3i18_bohstdIIterator_boh_std_String * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	(self->f_base = p_base);
	(self->f_condition = p_condition);
}
_Bool p3p3c1C_bohstdWhereIterator_boh_std_String_m_next_35cf4c(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	struct p3p3i18_bohstdIIterator_boh_std_String * temp36;
	return (temp36 = self->f_base)->m_next_35cf4c(temp36->object);
}
_Bool p3p3c1C_bohstdWhereIterator_boh_std_String_m_previous_35cf4c(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	struct p3p3i18_bohstdIIterator_boh_std_String * temp37;
	return (temp37 = self->f_base)->m_previous_35cf4c(temp37->object);
}
void p3p3c1C_bohstdWhereIterator_boh_std_String_m_moveLast_35cf4c(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	struct p3p3i18_bohstdIIterator_boh_std_String * temp38;
	return (temp38 = self->f_base)->m_moveLast_35cf4c(temp38->object);
}
void p3p3c1C_bohstdWhereIterator_boh_std_String_m_reset_35cf4c(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	struct p3p3i18_bohstdIIterator_boh_std_String * temp39;
	return (temp39 = self->f_base)->m_reset_35cf4c(temp39->object);
}
struct p3p3c6_bohstdString * p3p3c1C_bohstdWhereIterator_boh_std_String_m_current_35cf4c(struct p3p3c1C_bohstdWhereIterator_boh_std_String * const self)
{
	struct p3p3i18_bohstdIIterator_boh_std_String * temp40;
	struct p3p3c6_bohstdString * l_curr = (temp40 = self->f_base)->m_current_35cf4c(temp40->object);
	struct f1E_p07_booleanp3p3c6_bohstdString temp41;
	temp41 = self->f_condition;
	while ((!temp41.function(temp41.context, l_curr)))
	{
		p3p3c1C_bohstdWhereIterator_boh_std_String_m_next_35cf4c(self);
		struct p3p3i18_bohstdIIterator_boh_std_String * temp42;
		(l_curr = (temp42 = self->f_base)->m_current_35cf4c(temp42->object));
	}
	return l_curr;
}
void p3p3c1C_bohstdWhereIterator_boh_std_String_m_static_0(void)
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
