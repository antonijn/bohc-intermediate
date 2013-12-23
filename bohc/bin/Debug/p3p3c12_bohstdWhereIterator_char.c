#include "p3p3c12_bohstdWhereIterator_char.h"



const struct vtable_p3p3c12_bohstdWhereIterator_char instance_vtable_p3p3c12_bohstdWhereIterator_char = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c12_bohstdWhereIterator_char(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c12_bohstdWhereIterator_char * new_p3p3c12_bohstdWhereIterator_char_17589fbb(struct p3p3iE_bohstdIIterator_char * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	struct p3p3c12_bohstdWhereIterator_char * result = GC_malloc(sizeof(struct p3p3c12_bohstdWhereIterator_char));
	result->vtable = &instance_vtable_p3p3c12_bohstdWhereIterator_char;
	p3p3c12_bohstdWhereIterator_char_m_static_0();
	p3p3c12_bohstdWhereIterator_char_fi(result);
	p3p3c12_bohstdWhereIterator_char_m_this_17589fbb(result, p_base, p_condition);
	return result;
}

void p3p3c12_bohstdWhereIterator_char_fi(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	self->f_base = NULL;
	self->f_condition = BOH_FP_NULL;
}

void p3p3c12_bohstdWhereIterator_char_m_this_17589fbb(struct p3p3c12_bohstdWhereIterator_char * const self, struct p3p3iE_bohstdIIterator_char * p_base, struct f1E_p07_booleanp3p3c6_bohstdString p_condition)
{
	(self->f_base = p_base);
	(self->f_condition = p_condition);
}
_Bool p3p3c12_bohstdWhereIterator_char_m_next_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	struct p3p3iE_bohstdIIterator_char * temp50;
	return (temp50 = self->f_base)->m_next_35cf4c(temp50->object);
}
_Bool p3p3c12_bohstdWhereIterator_char_m_previous_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	struct p3p3iE_bohstdIIterator_char * temp51;
	return (temp51 = self->f_base)->m_previous_35cf4c(temp51->object);
}
void p3p3c12_bohstdWhereIterator_char_m_moveLast_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	struct p3p3iE_bohstdIIterator_char * temp52;
	return (temp52 = self->f_base)->m_moveLast_35cf4c(temp52->object);
}
void p3p3c12_bohstdWhereIterator_char_m_reset_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	struct p3p3iE_bohstdIIterator_char * temp53;
	return (temp53 = self->f_base)->m_reset_35cf4c(temp53->object);
}
unsigned char p3p3c12_bohstdWhereIterator_char_m_current_35cf4c(struct p3p3c12_bohstdWhereIterator_char * const self)
{
	struct p3p3iE_bohstdIIterator_char * temp54;
	unsigned char l_curr = (temp54 = self->f_base)->m_current_35cf4c(temp54->object);
	struct f1E_p07_booleanp3p3c6_bohstdString temp55;
	temp55 = self->f_condition;
	while ((!temp55.function(temp55.context, (struct p3p3c6_bohstdString *)(l_curr))))
	{
		p3p3c12_bohstdWhereIterator_char_m_next_35cf4c(self);
		struct p3p3iE_bohstdIIterator_char * temp56;
		(l_curr = (temp56 = self->f_base)->m_current_35cf4c(temp56->object));
	}
	return l_curr;
}
void p3p3c12_bohstdWhereIterator_char_m_static_0(void)
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
