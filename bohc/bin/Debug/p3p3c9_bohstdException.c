#include "p3p3c9_bohstdException.h"



const struct vtable_p3p3c9_bohstdException instance_vtable_p3p3c9_bohstdException = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb, &p3p3c9_bohstdException_m_what_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdException(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_125bf9a2(struct p3p3c6_bohstdString * p_description)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_2d2816fe();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_125bf9a2(result, p_description);
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_d5aca7eb(void)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_2d2816fe();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_d5aca7eb(result);
	return result;
}

void p3p3c9_bohstdException_fi(struct p3p3c9_bohstdException * const self)
{
	self->f_description = NULL;
}

struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_what_d5aca7eb(struct p3p3c9_bohstdException * const self)
{
	return boh_create_string(u"Something went wrong in the application", 39);
}
struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_getDescription_d5aca7eb(struct p3p3c9_bohstdException * const self)
{
	return self->f_description;
}
void p3p3c9_bohstdException_m_this_125bf9a2(struct p3p3c9_bohstdException * const self, struct p3p3c6_bohstdString * p_description)
{
	(self->f_description = p_description);
}
void p3p3c9_bohstdException_m_this_d5aca7eb(struct p3p3c9_bohstdException * const self)
{
	p3p3c9_bohstdException_m_this_125bf9a2(self, boh_create_string(u"", 0));
}
void p3p3c9_bohstdException_m_static_2d2816fe(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	{
	}
}
