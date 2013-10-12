#include "boh_lang_object.h"

struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Object_sf_type;


const struct vtable_c_boh_p_lang_p_Object instance_vtable_c_boh_p_lang_p_Object = { &c_boh_p_lang_p_Object_m_getType };


struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Object_m_getType(struct c_boh_p_lang_p_Object * const self)
{
	return self->c_boh_p_lang_p_Object_sf_type;
}
