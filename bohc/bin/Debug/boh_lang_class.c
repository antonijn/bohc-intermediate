#include "boh_lang_class.h"



const struct vtable_c_boh_p_lang_p_Class instance_vtable_c_boh_p_lang_p_Class = { &c_boh_p_lang_p_Class_m_isSubTypeOf };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Class * new_c_boh_p_lang_p_Class(void)
{
	struct c_boh_p_lang_p_Class * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Class));
	result->vtable = &instance_vtable_c_boh_p_lang_p_Class;
	c_boh_p_lang_p_Class_m_this(result);
	return result;
}

bool c_boh_p_lang_p_Class_m_isSubTypeOf(struct c_boh_p_lang_p_Class * const self, struct c_boh_p_lang_p_Type * p_other)
{
	(self->f_antonijn = 10);
	if ((p_other == self))
	{
		return true;
	}
	if ((self->f_base != NULL))
	{
		return c_boh_p_lang_p_Class_m_isSubTypeOf(self->f_base, p_other);
	}
	return false;
}
struct c_boh_p_lang_p_Class * c_boh_p_lang_p_Class_m_getBase(struct c_boh_p_lang_p_Class * const self)
{
	return self->f_base;
}
void c_boh_p_lang_p_Class_m_this(struct c_boh_p_lang_p_Class * const self)
{
	(self->f_base = NULL);
}
