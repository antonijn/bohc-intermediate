#include "p3p3c6_bohstdString.h"

struct p3p3c6_bohstdString * p3p3c6_bohstdString_sf_empty;

static void p3p3c6_bohstdString_m_this_e2fdbca8(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length);
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
struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_e2fdbca8(struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length)
{
	struct p3p3c6_bohstdString * result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	p3p3c6_bohstdString_m_static_2d2816fe();
	p3p3c6_bohstdString_fi(result);
	p3p3c6_bohstdString_m_this_e2fdbca8(result, p_str, p_offset, p_length);
	return result;
}

void p3p3c6_bohstdString_fi(struct p3p3c6_bohstdString * const self)
{
	self->f_offset = 0;
	self->f_length = 0;
	self->f_chars = NULL;
}

static void p3p3c6_bohstdString_m_this_e2fdbca8(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length)
{
	unsigned char l_dummy = (u8'\0');
	(self->f_chars = p_str->f_chars);
	(self->f_offset = (p_offset + p_str->f_offset));
	(self->f_length = p_length);
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
struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_70fcd6e5(struct p3p3c6_bohstdString * const self, int32_t p_idx)
{
	return new_p3p3c6_bohstdString_e2fdbca8(self, p_idx, (self->f_length - p_idx));
}
struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_e5adf5a9(struct p3p3c6_bohstdString * const self, int32_t p_idx, int32_t p_len)
{
	return new_p3p3c6_bohstdString_e2fdbca8(self, p_idx, (p_len - p_idx));
}
int32_t p3p3c6_bohstdString_m_indexOf_d8de2e33(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if (((p3p3c6_bohstdString_m_get_70fcd6e5(self, l_i) == p_ch)))
		{
			return l_i;
		}
	}
	return (-1);
}
int32_t p3p3c6_bohstdString_m_count_d8de2e33(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	int32_t l_result = (int32_t)(0);
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if (((p3p3c6_bohstdString_m_get_70fcd6e5(self, l_i) == p_ch)))
		{
			(++l_result);
		}
	}
	return l_result;
}
struct p3p3c14_bohstdArray_boh_std_String * p3p3c6_bohstdString_m_split_d8de2e33(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	struct p3p3c14_bohstdArray_boh_std_String * l_res = (struct p3p3c14_bohstdArray_boh_std_String *)(new_p3p3c6_bohstdObject_d5aca7eb());
	int32_t l_i = (int32_t)(0);
	int32_t l_idx = (int32_t)(0);
	int32_t l_prev = (int32_t)(0);
	while ((!((l_idx = p3p3c6_bohstdString_m_indexOf_d8de2e33(self, p_ch)) == (-1))))
	{
		p3p3c14_bohstdArray_boh_std_String_m_set_77f0ba2b(l_res, l_i, p3p3c6_bohstdString_m_substring_e5adf5a9(self, l_prev, (l_idx - l_prev)));
		(++l_i);
		(l_prev = l_idx);
	}
	p3p3c14_bohstdArray_boh_std_String_m_set_77f0ba2b(l_res, l_i, p3p3c6_bohstdString_m_substring_e5adf5a9(self, l_idx, (self->f_length - l_idx)));
	return l_res;
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
