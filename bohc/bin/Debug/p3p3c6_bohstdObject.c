#include "p3p3c6_bohstdObject.h"



const struct vtable_p3p3c6_bohstdObject instance_vtable_p3p3c6_bohstdObject = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdObject(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c6_bohstdObject * new_p3p3c6_bohstdObject_35cf4c(void)
{
	struct p3p3c6_bohstdObject * result = GC_malloc(sizeof(struct p3p3c6_bohstdObject));
	result->vtable = &instance_vtable_p3p3c6_bohstdObject;
	p3p3c6_bohstdObject_m_static_0();
	p3p3c6_bohstdObject_fi(result);
	p3p3c6_bohstdObject_m_this_35cf4c(result);
	return result;
}

void p3p3c6_bohstdObject_fi(struct p3p3c6_bohstdObject * const self)
{
}

_Bool p3p3c6_bohstdObject_m_equals_e9664e21(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other)
{
	return (p_other == self);
}
int64_t p3p3c6_bohstdObject_m_hash_35cf4c(struct p3p3c6_bohstdObject * const self)
{
	return &(*(int32_t *)((int8_t *)(self)));
}
struct p3p3c4_bohstdType * p3p3c6_bohstdObject_m_getType_35cf4c(struct p3p3c6_bohstdObject * const self)
{
	return (typeof_p3p3c6_bohstdObject());
}
struct p3p3c6_bohstdString * p3p3c6_bohstdObject_m_toString_35cf4c(struct p3p3c6_bohstdObject * const self)
{
	struct p3p3c4_bohstdType * temp3;
	struct p3p3c6_bohstdObject * temp4;
	return (temp3 = (temp4 = self)->vtable->m_getType_35cf4c(temp4))->vtable->m_getName_35cf4c(temp3);
}
_Bool p3p3c6_bohstdObject_m_valEquals_4eb476e0(struct p3p3c6_bohstdObject * p_left, struct p3p3c6_bohstdObject * p_right)
{
	p3p3c6_bohstdObject_m_static_0();
	_Bool l_lNull = (p_left == NULL);
	_Bool l_rNull = (p_right == NULL);
	if ((l_lNull && l_rNull))
	{
		return 1;
	}
	if ((l_lNull || l_rNull))
	{
		return 0;
	}
	struct p3p3c6_bohstdObject * temp5;
	return (temp5 = p_left)->vtable->m_equals_e9664e21(temp5, p_right);
}
void p3p3c6_bohstdObject_m_this_35cf4c(struct p3p3c6_bohstdObject * const self)
{
}
void p3p3c6_bohstdObject_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	{
	}
}
