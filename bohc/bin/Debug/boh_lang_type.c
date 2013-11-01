#include "boh_lang_type.h"



const struct vtable_c_boh_p_lang_p_Type instance_vtable_c_boh_p_lang_p_Type = { &c_boh_p_lang_p_Type_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924, NULL };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Type * new_c_boh_p_lang_p_Type(struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_pack)
{
	struct c_boh_p_lang_p_Type * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Type));
	result->f_name = NULL;
	result->f_pack = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Type;
	c_boh_p_lang_p_Type_m_this_3863939627(result, p_name, p_pack);
	return result;
}

void c_boh_p_lang_p_Type_m_this_3863939627(struct c_boh_p_lang_p_Type * const self, struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_pack)
{
	(self->f_name = p_name);
	(self->f_pack = p_pack);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Type_m_getName_3526476(struct c_boh_p_lang_p_Type * const self)
{
	return self->f_name;
}
struct c_boh_p_lang_p_Package * c_boh_p_lang_p_Type_m_getPackage_3526476(struct c_boh_p_lang_p_Type * const self)
{
	return self->f_pack;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Type_m_toString_3526476(struct c_boh_p_lang_p_Type * const self)
{
	if ((c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(self->f_pack), (struct c_boh_p_lang_p_Object *)(c_boh_p_lang_p_Package_sf_GLOBAL))))
	{
		return self->f_name;
	}
	struct c_boh_p_lang_p_Package * temp11;
	return (c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((temp11 = self->f_pack)->vtable->m_toString_3526476(temp11), boh_create_string(u".", 1))), self->f_name));
}
