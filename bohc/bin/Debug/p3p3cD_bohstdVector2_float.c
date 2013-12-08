#include "p3p3cD_bohstdVector2_float.h"



const struct vtable_p3p3cD_bohstdVector2_float instance_vtable_p3p3cD_bohstdVector2_float = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3cD_bohstdVector2_float(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3cD_bohstdVector2_float new_p3p3cD_bohstdVector2_float_312b65cc(float p_x, float p_y)
{
	struct p3p3cD_bohstdVector2_float result;
	p3p3cD_bohstdVector2_float_m_static_2d2816fe();
	p3p3cD_bohstdVector2_float_fi(&result)
	p3p3cD_bohstdVector2_float_m_this_312b65cc(&result, p_x, p_y);
	return result;
}

void p3p3cD_bohstdVector2_float_fi(struct p3p3cD_bohstdVector2_float * const self)
{
	self->f_x = 0.0f;
	self->f_y = 0.0f;
}

void p3p3cD_bohstdVector2_float_m_this_312b65cc(struct p3p3cD_bohstdVector2_float * const self, float p_x, float p_y)
{
	((*self).f_x = p_x);
	((*self).f_y = p_y);
}
void p3p3cD_bohstdVector2_float_m_static_2d2816fe(void)
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
