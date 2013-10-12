#include "boh_lang_type.h"

struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Type_sf_type;


const struct vtable_c_boh_p_lang_p_Type instance_vtable_c_boh_p_lang_p_Type = { &c_boh_p_lang_p_Type_m_getType };

struct c_boh_p_lang_p_Type * new_c_boh_p_lang_p_Type(void)
{
	struct c_boh_p_lang_p_Type * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Type));
	result->vtable = &instance_vtable_c_boh_p_lang_p_Type;
	c_boh_p_lang_p_Type_m_this(result);
	return result;
}

struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Type_m_getType(struct c_boh_p_lang_p_Type * const self)
{
	return self->c_boh_p_lang_p_Type_sf_type;
}
void c_boh_p_lang_p_Type_m_this(struct c_boh_p_lang_p_Type * const self)
{
}
