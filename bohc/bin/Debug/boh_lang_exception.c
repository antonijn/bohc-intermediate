#include "boh_lang_exception.h"

struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Exception_sf_type;


const struct vtable_c_boh_p_lang_p_Exception instance_vtable_c_boh_p_lang_p_Exception = { &c_boh_p_lang_p_Exception_m_getType };

struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(void)
{
	struct c_boh_p_lang_p_Exception * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Exception));
	result->vtable = &instance_vtable_c_boh_p_lang_p_Exception;
	c_boh_p_lang_p_Exception_m_this(result);
	return result;
}

struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Exception_m_getType(struct c_boh_p_lang_p_Exception * const self)
{
	return self->c_boh_p_lang_p_Exception_sf_type;
}
void c_boh_p_lang_p_Exception_m_this(struct c_boh_p_lang_p_Exception * const self)
{
}
