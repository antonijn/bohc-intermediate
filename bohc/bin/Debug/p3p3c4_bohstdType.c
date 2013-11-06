#include "p3p3c4_bohstdType.h"



const struct vtable_p3p3c4_bohstdType instance_vtable_p3p3c4_bohstdType = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb, &p3p3c4_bohstdType_m_getName_d5aca7eb, &p3p3c4_bohstdType_m_isSubTypeOf_46dba1cc };

struct p3p3c4_bohstdType * typeof_p3p3c4_bohstdType(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c4_bohstdType * new_p3p3c4_bohstdType_125bf9a2(struct p3p3c6_bohstdString * p_name)
{
	struct p3p3c4_bohstdType * result = GC_malloc(sizeof(struct p3p3c4_bohstdType));
	result->vtable = &instance_vtable_p3p3c4_bohstdType;
	p3p3c4_bohstdType_m_static_2d2816fe();
	p3p3c4_bohstdType_fi(result);
	p3p3c4_bohstdType_m_this_125bf9a2(result, p_name);
	return result;
}

void p3p3c4_bohstdType_fi(struct p3p3c4_bohstdType * const self)
{
	self->f_name = NULL;
	self->f_base = NULL;
}

void p3p3c4_bohstdType_m_this_125bf9a2(struct p3p3c4_bohstdType * const self, struct p3p3c6_bohstdString * p_name)
{
	p3p3c6_bohstdObject_m_this_d5aca7eb(self);
	(self->f_name = p_name);
}
struct p3p3c6_bohstdString * p3p3c4_bohstdType_m_getName_d5aca7eb(struct p3p3c4_bohstdType * const self)
{
	return self->f_name;
}
_Bool p3p3c4_bohstdType_m_isSubTypeOf_46dba1cc(struct p3p3c4_bohstdType * const self, struct p3p3c4_bohstdType * p_t)
{
	return (p3p3c6_bohstdObject_m_valEquals_d237012d((struct p3p3c6_bohstdObject *)(self->f_base), (struct p3p3c6_bohstdObject *)(p_t)));
}
void p3p3c4_bohstdType_m_static_2d2816fe(void)
{
	static _Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	{
	}
}
