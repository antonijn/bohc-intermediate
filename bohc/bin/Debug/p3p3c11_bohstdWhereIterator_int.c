#include "p3p3c11_bohstdWhereIterator_int.h"



const struct vtable_p3p3c11_bohstdWhereIterator_int instance_vtable_p3p3c11_bohstdWhereIterator_int = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c11_bohstdWhereIterator_int(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c11_bohstdWhereIterator_int * new_p3p3c11_bohstdWhereIterator_int_1c6adf31(struct p3p3iD_bohstdIIterator_int * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3c11_bohstdWhereIterator_int * result = GC_malloc(sizeof(struct p3p3c11_bohstdWhereIterator_int));
	result->vtable = &instance_vtable_p3p3c11_bohstdWhereIterator_int;
	p3p3c11_bohstdWhereIterator_int_m_static_0();
	p3p3c11_bohstdWhereIterator_int_fi(result);
	p3p3c11_bohstdWhereIterator_int_m_this_1c6adf31(result, p_base, p_condition);
	return result;
}

void p3p3c11_bohstdWhereIterator_int_fi(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	self->f_base = NULL;
	self->f_condition = BOH_FP_NULL;
}

void p3p3c11_bohstdWhereIterator_int_m_this_1c6adf31(struct p3p3c11_bohstdWhereIterator_int * const self, struct p3p3iD_bohstdIIterator_int * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	(self->f_base = p_base);
	(self->f_condition = p_condition);
}
_Bool p3p3c11_bohstdWhereIterator_int_m_next_35cf4c(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	struct p3p3iD_bohstdIIterator_int * temp43;
	return (temp43 = self->f_base)->m_next_35cf4c(temp43->object);
}
_Bool p3p3c11_bohstdWhereIterator_int_m_previous_35cf4c(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	struct p3p3iD_bohstdIIterator_int * temp44;
	return (temp44 = self->f_base)->m_previous_35cf4c(temp44->object);
}
void p3p3c11_bohstdWhereIterator_int_m_moveLast_35cf4c(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	struct p3p3iD_bohstdIIterator_int * temp45;
	return (temp45 = self->f_base)->m_moveLast_35cf4c(temp45->object);
}
void p3p3c11_bohstdWhereIterator_int_m_reset_35cf4c(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	struct p3p3iD_bohstdIIterator_int * temp46;
	return (temp46 = self->f_base)->m_reset_35cf4c(temp46->object);
}
int32_t p3p3c11_bohstdWhereIterator_int_m_current_35cf4c(struct p3p3c11_bohstdWhereIterator_int * const self)
{
	struct p3p3iD_bohstdIIterator_int * temp47;
	int32_t l_curr = (temp47 = self->f_base)->m_current_35cf4c(temp47->object);
	struct f1E_p07_booleanp3p3c6_bohstdString temp48;
	temp48 = self->f_condition;
	while ((!temp48.function(temp48.context, (struct p3p3c6_bohstdString *)(l_curr))))
	{
		p3p3c11_bohstdWhereIterator_int_m_next_35cf4c(self);
		struct p3p3iD_bohstdIIterator_int * temp49;
		(l_curr = (temp49 = self->f_base)->m_current_35cf4c(temp49->object));
	}
	return l_curr;
}
void p3p3c11_bohstdWhereIterator_int_m_static_0(void)
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
