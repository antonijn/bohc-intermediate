#include "p3p3c6_bohstdString.h"

struct p3p3c6_bohstdString * p3p3c6_bohstdString_sf_empty;

static void p3p3c6_bohstdString_m_this_70fcd6e5(struct p3p3c6_bohstdString * const self, int32_t p_size);
static unsigned char p3p3c6_bohstdString_m_set_f076e8e0(struct p3p3c6_bohstdString * const self, int32_t p_i, unsigned char p_ch);

const struct vtable_p3p3c6_bohstdString instance_vtable_p3p3c6_bohstdString = { &p3p3c6_bohstdString_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdString(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_70fcd6e5(int32_t p_size)
{
	struct p3p3c6_bohstdString * result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	p3p3c6_bohstdString_m_static_2d2816fe();
	p3p3c6_bohstdString_fi(result);
	p3p3c6_bohstdString_m_this_70fcd6e5(result, p_size);
	return result;
}

void p3p3c6_bohstdString_fi(struct p3p3c6_bohstdString * const self)
{
	self->f_offset = 0;
	self->f_length = 0;
	self->f_chars = NULL;
}

static void p3p3c6_bohstdString_m_this_70fcd6e5(struct p3p3c6_bohstdString * const self, int32_t p_size)
{
	p3p3c6_bohstdObject_m_this_d5aca7eb(self);
	unsigned char l_dummy = (u8'\0');
	(self->f_chars = (boh_char*)(GC_malloc((p_size * sizeof(l_dummy)))));
	(self->f_offset = (int32_t)(0));
	(self->f_length = p_size);
}
_Bool p3p3c6_bohstdString_m_isNullOrEmpty_5bf6fcab(struct p3p3c6_bohstdString * p_str)
{
	p3p3c6_bohstdString_m_static_2d2816fe();
	return (((p_str == NULL)) || ((p_str->f_length == 0)));
}
_Bool p3p3c6_bohstdString_m_equals_5289cddf(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdObject * p_other)
{
	struct p3p3c6_bohstdObject * temp3;
	if ((!p3p3c6_bohstdObject_m_valEquals_d237012d((struct p3p3c6_bohstdObject *)((temp3 = p_other)->vtable->m_getType_d5aca7eb(temp3)), (struct p3p3c6_bohstdObject *)((typeof_p3p3c6_bohstdString())))))
	{
		return 0;
	}
	struct p3p3c6_bohstdString * l_str = (struct p3p3c6_bohstdString *)(p_other);
	if ((!(l_str->f_length == self->f_length)))
	{
		return 0;
	}
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if ((!(p3p3c6_bohstdString_m_get_70fcd6e5(l_str, l_i) == p3p3c6_bohstdString_m_get_70fcd6e5(self, l_i))))
		{
			return 0;
		}
	}
	{
		int32_t l_i = (int32_t)(0);
	}
	return 1;
}
unsigned char p3p3c6_bohstdString_m_get_70fcd6e5(struct p3p3c6_bohstdString * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_chars, (self->f_offset + p_i));
}
static unsigned char p3p3c6_bohstdString_m_set_f076e8e0(struct p3p3c6_bohstdString * const self, int32_t p_i, unsigned char p_ch)
{
	return boh_set_deref(self->f_chars, (self->f_offset + p_i), p_ch);
}
void p3p3c6_bohstdString_m_static_2d2816fe(void)
{
	static _Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	p3p3c6_bohstdString_sf_empty = boh_create_string(u"", 0);
	{
	}
}
