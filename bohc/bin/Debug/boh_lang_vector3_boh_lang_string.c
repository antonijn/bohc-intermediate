#include "boh_lang_vector3_boh_lang_string.h"



const struct vtable_c_boh_p_lang_p_Vector3_boh_lang_String instance_vtable_c_boh_p_lang_p_Vector3_boh_lang_String = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Vector3_boh_lang_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Vector3_boh_lang_String new_c_boh_p_lang_p_Vector3_boh_lang_String(struct c_boh_p_lang_p_String * p_x, struct c_boh_p_lang_p_String * p_y, struct c_boh_p_lang_p_String * p_z)
{
	struct c_boh_p_lang_p_Vector3_boh_lang_String result;
	result.f_x = NULL;
	result.f_y = NULL;
	result.f_z = NULL;
	c_boh_p_lang_p_Vector3_boh_lang_String_m_this_54760982(&result, p_x, p_y, p_z);
	return result;
}

void c_boh_p_lang_p_Vector3_boh_lang_String_m_this_54760982(struct c_boh_p_lang_p_Vector3_boh_lang_String * const self, struct c_boh_p_lang_p_String * p_x, struct c_boh_p_lang_p_String * p_y, struct c_boh_p_lang_p_String * p_z)
{
	(self.f_x = p_x);
	(self.f_y = p_y);
	(self.f_z = p_z);
}
struct c_boh_p_lang_p_Vector3_boh_lang_String c_boh_p_lang_p_Vector3_boh_lang_String_op_add_239811068(struct c_boh_p_lang_p_Vector3_boh_lang_String p_left, struct c_boh_p_lang_p_Vector3_boh_lang_String p_right)
{
	return new_c_boh_p_lang_p_Vector3_boh_lang_String((c_boh_p_lang_p_String_op_add_4275178606(p_left.f_x, p_right.f_x)), (c_boh_p_lang_p_String_op_add_4275178606(p_left.f_y, p_right.f_y)), (c_boh_p_lang_p_String_op_add_4275178606(p_left.f_z, p_right.f_z)));
}
