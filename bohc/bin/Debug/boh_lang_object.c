#include "boh_lang_object.h"



const struct vtable_c_boh_p_lang_p_Object instance_vtable_c_boh_p_lang_p_Object = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Object(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Object * new_c_boh_p_lang_p_Object(void)
{
	struct c_boh_p_lang_p_Object * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Object));
	result->vtable = &instance_vtable_c_boh_p_lang_p_Object;
	c_boh_p_lang_p_Object_m_this_3526476(result);
	return result;
}

struct c_boh_p_lang_p_String * c_boh_p_lang_p_Object_m_toString_3526476(struct c_boh_p_lang_p_Object * const self)
{
	struct c_boh_p_lang_p_Vector3_float l_floats = new_c_boh_p_lang_p_Vector3_float((float)(10), (float)(20), (float)(30));
	struct c_boh_p_lang_p_Vector3_boh_lang_String l_strings = new_c_boh_p_lang_p_Vector3_boh_lang_String(boh_create_string(u"Hello, ", 7), boh_create_string(u"world!", 6), (struct c_boh_p_lang_p_String *)(NULL));
	jmp_buf temp6;
	void temp5(void)
	{
		int32_t l_third = (int32_t)(3);
	}
	memcpy(&temp6, &exception_buf, sizeof(jmp_buf));
	if (setjmp(exception_buf) == 0)
	{
		{
			_Decimal64 l_dec = 100DD;
			int32_t l_first = (int32_t)(1);
		}
		memcpy(&exception_buf, &temp6, sizeof(jmp_buf));
		temp5();
	}
	else
	{
		memcpy(&exception_buf, &temp6, sizeof(jmp_buf));
		if (exception_type == typeof_c_boh_p_lang_p_Exception())
		{
			struct c_boh_p_lang_p_Exception * p_e = (struct c_boh_p_lang_p_Exception *)exception;
			{
				int32_t l_second = (int32_t)(2);
			}
			temp5();
		}
		else 
		{
			temp5();
			longjmp(exception_buf, 1);
		}
	}
	struct c_boh_p_lang_p_Object * temp7;
	return c_boh_p_lang_p_Type_m_getName_3526476((temp7 = self)->vtable->m_getType_3526476(temp7));
}
int64_t c_boh_p_lang_p_Object_m_hash_3526476(struct c_boh_p_lang_p_Object * const self)
{
	return boh_force_cast(self);
}
struct c_boh_p_lang_p_Type * c_boh_p_lang_p_Object_m_getType_3526476(struct c_boh_p_lang_p_Object * const self)
{
	return (typeof_c_boh_p_lang_p_Object());
}
_Bool c_boh_p_lang_p_Object_m_equals_2378881924(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other)
{
	return (p_other == self);
}
_Bool c_boh_p_lang_p_Object_m_is_713218619(struct c_boh_p_lang_p_Object * p_o, struct c_boh_p_lang_p_Type * p_t)
{
	if (((c_boh_p_lang_p_Object_m_valEquals_2338730496(p_o, (struct c_boh_p_lang_p_Object *)(NULL))) || (c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(p_t), (struct c_boh_p_lang_p_Object *)(NULL)))))
	{
		return 0;
	}
	struct c_boh_p_lang_p_Type * temp8;
	struct c_boh_p_lang_p_Object * temp9;
	return (temp8 = (temp9 = p_o)->vtable->m_getType_3526476(temp9))->vtable->m_isSubTypeOf_4199290047(temp8, p_t);
}
_Bool c_boh_p_lang_p_Object_m_valEquals_2338730496(struct c_boh_p_lang_p_Object * p_l, struct c_boh_p_lang_p_Object * p_r)
{
	_Bool l_lNull = (p_l == NULL);
	_Bool l_rNull = (p_r == NULL);
	if ((l_lNull && l_rNull))
	{
		return 1;
	}
	if ((l_lNull || l_rNull))
	{
		return 0;
	}
	struct c_boh_p_lang_p_Object * temp10;
	return (temp10 = p_l)->vtable->m_equals_2378881924(temp10, p_r);
}
void c_boh_p_lang_p_Object_m_this_3526476(struct c_boh_p_lang_p_Object * const self)
{
}
