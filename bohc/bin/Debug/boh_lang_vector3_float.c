#include "boh_lang_vector3_float.h"



const struct vtable_c_boh_p_lang_p_Vector3_float instance_vtable_c_boh_p_lang_p_Vector3_float = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Vector3_float(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Vector3_float new_c_boh_p_lang_p_Vector3_float(float p_x, float p_y, float p_z)
{
	struct c_boh_p_lang_p_Vector3_float result;
	result.f_x = 0.0f;
	result.f_y = 0.0f;
	result.f_z = 0.0f;
	c_boh_p_lang_p_Vector3_float_m_this_2532104116(&result, p_x, p_y, p_z);
	return result;
}

void c_boh_p_lang_p_Vector3_float_m_this_2532104116(struct c_boh_p_lang_p_Vector3_float * const self, float p_x, float p_y, float p_z)
{
	(self.f_x = p_x);
	(self.f_y = p_y);
	(self.f_z = p_z);
}
struct c_boh_p_lang_p_Vector3_float c_boh_p_lang_p_Vector3_float_op_add_1884407032(struct c_boh_p_lang_p_Vector3_float p_left, struct c_boh_p_lang_p_Vector3_float p_right)
{
	return new_c_boh_p_lang_p_Vector3_float((p_left.f_x + p_right.f_x), (p_left.f_y + p_right.f_y), (p_left.f_z + p_right.f_z));
}
