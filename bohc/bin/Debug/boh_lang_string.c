#include "boh_lang_string.h"


static void c_boh_p_lang_p_String_m_set_3873787583(struct c_boh_p_lang_p_String * const self, int32_t p_i, struct c_boh_p_lang_p_Character p_ch);
static struct c_boh_p_lang_p_Character c_boh_p_lang_p_String_m_get_2607005255(struct c_boh_p_lang_p_String * const self, int32_t p_i);

const struct vtable_c_boh_p_lang_p_String instance_vtable_c_boh_p_lang_p_String = { &c_boh_p_lang_p_String_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_String * new_c_boh_p_lang_p_String(void)
{
	struct c_boh_p_lang_p_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_String));
	result->f_length = 0;
	result->f_first = new_c_boh_p_lang_p_Character();
	result->vtable = &instance_vtable_c_boh_p_lang_p_String;
	c_boh_p_lang_p_String_m_this_3526476(result);
	return result;
}

struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_toString_3526476(struct c_boh_p_lang_p_String * const self)
{
	(self->f_first.f_ch0 = (char16_t)(0));
	struct c_boh_p_lang_p_Character temp0;
	struct c_boh_p_lang_p_String * l_str = c_boh_p_lang_p_Character_m_toString_3526476((temp0 = self->f_first, &temp0));
	return self;
}
int32_t c_boh_p_lang_p_String_m_size_3526476(struct c_boh_p_lang_p_String * const self)
{
	return self->f_length;
}
static void c_boh_p_lang_p_String_m_set_3873787583(struct c_boh_p_lang_p_String * const self, int32_t p_i, struct c_boh_p_lang_p_Character p_ch)
{
	boh_str_set_ch(self, p_i, p_ch);
}
static struct c_boh_p_lang_p_Character c_boh_p_lang_p_String_m_get_2607005255(struct c_boh_p_lang_p_String * const self, int32_t p_i)
{
	return boh_str_get_ch(self, p_i);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_substring_3255497772(struct c_boh_p_lang_p_String * const self, int32_t p_first, int32_t p_len)
{
	struct c_boh_p_lang_p_String * l_result = (struct c_boh_p_lang_p_String *)(boh_create_string_empty(p_len));
	for (int32_t l_i = (int32_t)(0); (l_i < p_len); (++l_i))
	{
		c_boh_p_lang_p_String_m_set_3873787583(l_result, l_i, c_boh_p_lang_p_String_m_get_2607005255(self, (p_first + l_i)));
	}
	return l_result;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_substring_2607005255(struct c_boh_p_lang_p_String * const self, int32_t p_first)
{
	int32_t l_len = (self->f_length - p_first);
	return c_boh_p_lang_p_String_m_substring_3255497772(self, p_first, l_len);
}
int32_t c_boh_p_lang_p_String_m_indexOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch)
{
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if ((c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(c_boh_p_lang_p_String_m_get_2607005255(self, l_i)), (struct c_boh_p_lang_p_Object *)(p_ch))))
		{
			return l_i;
		}
	}
	return (-1);
}
int32_t c_boh_p_lang_p_String_m_lastIndexOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch)
{
	for (int32_t l_i = (self->f_length - 1); (l_i >= 0); (--l_i))
	{
		if ((c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(c_boh_p_lang_p_String_m_get_2607005255(self, l_i)), (struct c_boh_p_lang_p_Object *)(p_ch))))
		{
			return l_i;
		}
	}
	return (-1);
}
_Bool c_boh_p_lang_p_String_m_stringAtPos_247466877(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str, int32_t p_pos)
{
	for (int32_t l_i = (int32_t)(0); (l_i < p_str->f_length); (++l_i))
	{
		if ((!c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(c_boh_p_lang_p_String_m_get_2607005255(self, (l_i + p_pos))), (struct c_boh_p_lang_p_Object *)(c_boh_p_lang_p_String_m_get_2607005255(p_str, l_i)))))
		{
			return 0;
		}
	}
	return 1;
}
int32_t c_boh_p_lang_p_String_m_indexOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str)
{
	for (int32_t l_i = (int32_t)(0); (l_i < (self->f_length - p_str->f_length)); (++l_i))
	{
		if (c_boh_p_lang_p_String_m_stringAtPos_247466877(self, p_str, l_i))
		{
			return l_i;
		}
	}
	return (-1);
}
int32_t c_boh_p_lang_p_String_m_lastIndexOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str)
{
	for (int32_t l_i = ((self->f_length - p_str->f_length) - 1); (l_i >= 0); (--l_i))
	{
		if (c_boh_p_lang_p_String_m_stringAtPos_247466877(self, p_str, l_i))
		{
			return l_i;
		}
	}
	return (-1);
}
struct c_boh_p_lang_p_Array_int * c_boh_p_lang_p_String_m_indicesOf_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch)
{
}
struct c_boh_p_lang_p_Array_int * c_boh_p_lang_p_String_m_indicesOf_2510264406(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_String * p_str)
{
}
struct c_boh_p_lang_p_Array_boh_lang_String * c_boh_p_lang_p_String_m_split_1069932740(struct c_boh_p_lang_p_String * const self, struct c_boh_p_lang_p_Character p_ch)
{
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trim_3526476(struct c_boh_p_lang_p_String * const self)
{
	int32_t l_start;
	for ((l_start = (int32_t)(0)); ((l_start < self->f_length) && (c_boh_p_lang_p_String_m_get_2607005255(self, l_start) < 32)); (++l_start))
	{
	}
	int32_t l_finish;
	for ((l_finish = (self->f_length - 1)); ((l_finish >= 0) && (c_boh_p_lang_p_String_m_get_2607005255(self, l_finish) < 32)); (--l_finish))
	{
	}
	int32_t l_len = ((l_finish + 1) - l_start);
	return c_boh_p_lang_p_String_m_substring_3255497772(self, l_start, l_len);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trimStart_3526476(struct c_boh_p_lang_p_String * const self)
{
	int32_t l_start;
	for ((l_start = (int32_t)(0)); ((l_start < self->f_length) && (c_boh_p_lang_p_String_m_get_2607005255(self, l_start) < 32)); (++l_start))
	{
	}
	return c_boh_p_lang_p_String_m_substring_2607005255(self, l_start);
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_m_trimEnd_3526476(struct c_boh_p_lang_p_String * const self)
{
	int32_t l_finish;
	for ((l_finish = (self->f_length - 1)); ((l_finish >= 0) && (c_boh_p_lang_p_String_m_get_2607005255(self, l_finish) < 32)); (--l_finish))
	{
	}
	return c_boh_p_lang_p_String_m_substring_3255497772(self, (int32_t)(0), (l_finish + 1));
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_String_op_add_4275178606(struct c_boh_p_lang_p_String * p_left, struct c_boh_p_lang_p_Object * p_robj)
{
	struct c_boh_p_lang_p_Object * temp1;
	struct c_boh_p_lang_p_String * l_right = (temp1 = p_robj)->vtable->m_toString_3526476(temp1);
	struct c_boh_p_lang_p_String * l_result = (struct c_boh_p_lang_p_String *)(boh_create_string_empty((p_left->f_length + l_right->f_length)));
	for (int32_t l_i = (int32_t)(0); (l_i < p_left->f_length); (++l_i))
	{
		c_boh_p_lang_p_String_m_set_3873787583(l_result, l_i, c_boh_p_lang_p_String_m_get_2607005255(p_left, l_i));
	}
	for (int32_t l_i = (int32_t)(0); (l_i < l_right->f_length); (++l_i))
	{
		c_boh_p_lang_p_String_m_set_3873787583(l_result, l_i, c_boh_p_lang_p_String_m_get_2607005255(l_right, l_i));
	}
	return l_result;
}
void c_boh_p_lang_p_String_m_this_3526476(struct c_boh_p_lang_p_String * const self)
{
}
