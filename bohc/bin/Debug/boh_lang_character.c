#include "boh_lang_character.h"



const struct vtable_c_boh_p_lang_p_Character instance_vtable_c_boh_p_lang_p_Character = { &c_boh_p_lang_p_Character_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Character(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Character new_c_boh_p_lang_p_Character(void)
{
	struct c_boh_p_lang_p_Character result;
	result.f_ch0 = u'\0';
	result.f_ch1 = u'\0';
	c_boh_p_lang_p_Character_m_this_3526476(&result);
	return result;
}

struct c_boh_p_lang_p_String * c_boh_p_lang_p_Character_m_toString_3526476(struct c_boh_p_lang_p_Character * const self)
{
	return NULL;
}
void c_boh_p_lang_p_Character_m_this_3526476(struct c_boh_p_lang_p_Character * const self)
{
}
