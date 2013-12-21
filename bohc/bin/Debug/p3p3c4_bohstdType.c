#include "p3p3c4_bohstdType.h"



const struct vtable_p3p3c4_bohstdType instance_vtable_p3p3c4_bohstdType = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c, &p3p3c4_bohstdType_m_getName_35cf4c, &p3p3c4_bohstdType_m_isSubTypeOf_490a3fde };

struct p3p3c4_bohstdType * typeof_p3p3c4_bohstdType(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c4_bohstdType * new_p3p3c4_bohstdType_f13b0af3(struct p3p3c6_bohstdString * p_name)
{
	struct p3p3c4_bohstdType * result = GC_malloc(sizeof(struct p3p3c4_bohstdType));
	result->vtable = &instance_vtable_p3p3c4_bohstdType;
	p3p3c4_bohstdType_m_static_0();
	p3p3c4_bohstdType_fi(result);
	p3p3c4_bohstdType_m_this_f13b0af3(result, p_name);
	return result;
}

void p3p3c4_bohstdType_fi(struct p3p3c4_bohstdType * const self)
{
	self->f_name = NULL;
	self->f_base = NULL;
}

void p3p3c4_bohstdType_m_this_f13b0af3(struct p3p3c4_bohstdType * const self, struct p3p3c6_bohstdString * p_name)
{
	(self->f_name = p_name);
}
struct p3p3c6_bohstdString * p3p3c4_bohstdType_m_getName_35cf4c(struct p3p3c4_bohstdType * const self)
{
	return self->f_name;
}
_Bool p3p3c4_bohstdType_m_isSubTypeOf_490a3fde(struct p3p3c4_bohstdType * const self, struct p3p3c4_bohstdType * p_t)
{
	return (p3p3c6_bohstdObject_m_valEquals_4eb476e0((struct p3p3c6_bohstdObject *)(self->f_base), (struct p3p3c6_bohstdObject *)(p_t)));
}
void p3p3c4_bohstdType_m_static_0(void)
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
