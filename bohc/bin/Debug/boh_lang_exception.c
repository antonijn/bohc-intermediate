#include "boh_lang_exception.h"



const struct vtable_c_boh_p_lang_p_Exception instance_vtable_c_boh_p_lang_p_Exception = { &c_boh_p_lang_p_Exception_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924, &c_boh_p_lang_p_Exception_m_what_3526476 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Exception(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(void)
{
	struct c_boh_p_lang_p_Exception * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Exception));
	result->f_stackTrace = NULL;
	result->f_description = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Exception;
	c_boh_p_lang_p_Exception_m_this_3526476(result);
	return result;
}
struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(struct c_boh_p_lang_p_String * p_description)
{
	struct c_boh_p_lang_p_Exception * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Exception));
	result->f_stackTrace = NULL;
	result->f_description = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Exception;
	c_boh_p_lang_p_Exception_m_this_2510264406(result, p_description);
	return result;
}

void c_boh_p_lang_p_Exception_m_this_3526476(struct c_boh_p_lang_p_Exception * const self)
{
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_stackTrace_3526476(struct c_boh_p_lang_p_Exception * const self)
{
	return self->f_stackTrace;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_getDescription_3526476(struct c_boh_p_lang_p_Exception * const self)
{
	return self->f_description;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_what_3526476(struct c_boh_p_lang_p_Exception * const self)
{
	return boh_create_string(u"Something went wrong in the application", 39);
}
void c_boh_p_lang_p_Exception_m_this_2510264406(struct c_boh_p_lang_p_Exception * const self, struct c_boh_p_lang_p_String * p_description)
{
	(self->f_description = p_description);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Exception_m_toString_3526476(struct c_boh_p_lang_p_Exception * const self)
{
	struct c_boh_p_lang_p_Type * temp2;
	struct c_boh_p_lang_p_Exception * temp3;
	struct c_boh_p_lang_p_Exception * temp4;
	return (c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((temp2 = (temp3 = self)->vtable->m_getType_3526476(temp3))->vtable->m_toString_3526476(temp2), boh_create_string(u"; ", 2))), (temp4 = self)->vtable->m_what_3526476(temp4))), boh_create_string(u": ", 2))), self->f_description)), boh_create_string(u"\n", 1))), self->f_stackTrace));
}
