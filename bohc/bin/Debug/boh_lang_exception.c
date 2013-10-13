#include "boh_lang_exception.h"



const struct vtable_c_boh_p_lang_p_Exception instance_vtable_c_boh_p_lang_p_Exception =  };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Type(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Exception * new_c_boh_p_lang_p_Exception(void)
{
	struct c_boh_p_lang_p_Exception * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Exception));
	result->vtable = &instance_vtable_c_boh_p_lang_p_Exception;
	c_boh_p_lang_p_Exception_m_this(result);
	return result;
}

void c_boh_p_lang_p_Exception_m_this(struct c_boh_p_lang_p_Exception * const self)
{
	jmp_buf temp1;
	void temp0(void)
	{
		double l_third = 2.0f;
	}
	memcpy(&temp1, &exception_buf, sizeof(jmp_buf));
	if (setjmp(exception_buf) == 0)
	{
		{
			boh_throw_ex(new_c_boh_p_lang_p_Exception());
			int32_t l_first = 0;
		}
		temp0();
	}
	else
	{
		memcpy(&exception_buf, &temp1, sizeof(jmp_buf));
		struct c_boh_p_lang_p_Type * exception_type = exception->vtable->getType(exception);
		if (exception_type == typeof_c_boh_p_lang_p_Exception())
		{
			struct c_boh_p_lang_p_Exception * p_e = (struct c_boh_p_lang_p_Exception *)exception;
			{
				float l_second = 1.0;
			}
			temp0();
			longjmp(exception_buf, 1);
		}
	}
}
