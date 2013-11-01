#include "boh_lang_package.h"

struct c_boh_p_lang_p_Package * c_boh_p_lang_p_Package_sf_GLOBAL;


const struct vtable_c_boh_p_lang_p_Package instance_vtable_c_boh_p_lang_p_Package = { &c_boh_p_lang_p_Package_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Package(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Package * new_c_boh_p_lang_p_Package(struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_owner)
{
	struct c_boh_p_lang_p_Package * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Package));
	result->f_name = NULL;
	result->f_owner = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Package;
	c_boh_p_lang_p_Package_m_this_3863939627(result, p_name, p_owner);
	return result;
}

void c_boh_p_lang_p_Package_m_this_3863939627(struct c_boh_p_lang_p_Package * const self, struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_owner)
{
	(self->f_name = p_name);
	(self->f_owner = p_owner);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Package_m_getName_3526476(struct c_boh_p_lang_p_Package * const self)
{
	return self->f_name;
}
struct c_boh_p_lang_p_Package * c_boh_p_lang_p_Package_m_getOwner_3526476(struct c_boh_p_lang_p_Package * const self)
{
	return self->f_owner;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Package_m_toString_3526476(struct c_boh_p_lang_p_Package * const self)
{
	if ((!c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(self->f_owner), (struct c_boh_p_lang_p_Object *)(NULL))))
	{
		struct c_boh_p_lang_p_Package * temp12;
		return (c_boh_p_lang_p_String_op_add_4275178606((c_boh_p_lang_p_String_op_add_4275178606((temp12 = self->f_owner)->vtable->m_toString_3526476(temp12), boh_create_string(u".", 1))), self->f_name));
	}
	return self->f_name;
}
