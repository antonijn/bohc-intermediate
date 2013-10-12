#include "hey_hi_class.h"
struct c_hey_p_hi_p_Class * c_hey_p_hi_p_Class_m_get(struct c_hey_p_hi_p_Class * self, float p_f);
const struct vtable_c_hey_p_hi_p_Class *instance_vtable_c_hey_p_hi_p_Class = { &c_hey_p_hi_p_Class_m_get };
struct c_hey_p_hi_p_Class * new_c_hey_p_hi_p_Class(void)
{
	struct c_hey_p_hi_p_Class * result = GC_malloc(sizeof(struct c_hey_p_hi_p_Class));
	result->vtable = instance_vtable_c_hey_p_hi_p_Class;
	c_hey_p_hi_p_Class_m_this(result);
	return result;
}
void c_hey_p_hi_p_Class_m_this(struct c_hey_p_hi_p_Class * self)
{
}
struct c_hey_p_hi_p_Class * c_hey_p_hi_p_Class_m_get(struct c_hey_p_hi_p_Class * self, float p_f)
{
}
void c_hey_p_hi_p_Class_m_main(void)
{
	if (4 == 4)
	{
		struct c_hey_p_hi_p_Class * temp0;
		struct c_hey_p_hi_p_Class * temp1;
		(temp0 = (temp1 = new_c_hey_p_hi_p_Class())->vtable->c_hey_p_hi_p_Class_m_get(temp1, 1.0f))->vtable->c_hey_p_hi_p_Class_m_get(temp0, 2)->f_ah;
	}
}
