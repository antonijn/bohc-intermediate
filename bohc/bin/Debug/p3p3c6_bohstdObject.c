#include "p3p3c6_bohstdObject.h"



const struct vtable_p3p3c6_bohstdObject instance_vtable_p3p3c6_bohstdObject = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdObject(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c6_bohstdObject * new_p3p3c6_bohstdObject_d5aca7eb(void)
{
	struct p3p3c6_bohstdObject * result = GC_malloc(sizeof(struct p3p3c6_bohstdObject));
	result->vtable = &instance_vtable_p3p3c6_bohstdObject;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	p3p3c6_bohstdObject_fi(result);
	p3p3c6_bohstdObject_m_this_d5aca7eb(result);
	return result;
}

void p3p3c6_bohstdObject_fi(struct p3p3c6_bohstdObject * const self)
{
}

void p3p3c6_bohstdObject_m_static_2d2816fe(void)
{
	static _Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	{
	}
}
_Bool p3p3c6_bohstdObject_m_equals_5289cddf(struct p3p3c6_bohstdObject * const self, struct p3p3c6_bohstdObject * p_other)
{
	return (p_other == self);
}
int64_t p3p3c6_bohstdObject_m_hash_d5aca7eb(struct p3p3c6_bohstdObject * const self)
{
	return boh_force_cast(self);
}
struct p3p3c4_bohstdType * p3p3c6_bohstdObject_m_getType_d5aca7eb(struct p3p3c6_bohstdObject * const self)
{
	return (typeof_p3p3c6_bohstdObject());
}
struct p3p3c6_bohstdString * p3p3c6_bohstdObject_m_toString_d5aca7eb(struct p3p3c6_bohstdObject * const self)
{
	struct p3p3c4_bohstdType * temp0;
	struct p3p3c6_bohstdObject * temp1;
	return (temp0 = (temp1 = self)->vtable->m_getType_d5aca7eb(temp1))->vtable->m_getName_d5aca7eb(temp0);
}
_Bool p3p3c6_bohstdObject_m_valEquals_d237012d(struct p3p3c6_bohstdObject * p_left, struct p3p3c6_bohstdObject * p_right)
{
	p3p3c6_bohstdObject_m_static_2d2816fe();
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
	struct p3p3c6_bohstdObject * temp2;
	return (temp2 = p_left)->vtable->m_equals_5289cddf(temp2, p_right);
}
void p3p3c6_bohstdObject_m_this_d5aca7eb(struct p3p3c6_bohstdObject * const self)
{
}
