#include "p3p3c6_bohstdString.h"

struct p3p3c6_bohstdString * p3p3c6_bohstdString_sf_empty;

static void p3p3c6_bohstdString_m_this_51708853(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length);
static struct p3p3c6_bohstdString * create_str_by_len(int32_t p_len);
#endif

const struct vtable_p3p3c6_bohstdString instance_vtable_p3p3c6_bohstdString = { &p3p3c6_bohstdString_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdString(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_51708853(struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length)
{
	struct p3p3c6_bohstdString * result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	p3p3c6_bohstdString_m_static_0();
	p3p3c6_bohstdString_fi(result);
	p3p3c6_bohstdString_m_this_51708853(result, p_str, p_offset, p_length);
	return result;
}
struct p3p3c6_bohstdString * new_p3p3c6_bohstdString_adeaa357(int32_t p_length)
{
	struct p3p3c6_bohstdString * result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	p3p3c6_bohstdString_m_static_0();
	p3p3c6_bohstdString_fi(result);
	p3p3c6_bohstdString_m_this_adeaa357(result, p_length);
	return result;
}

void p3p3c6_bohstdString_fi(struct p3p3c6_bohstdString * const self)
{
	self->f_offset = 0;
	self->f_length = 0;
	self->f_chars = new_p3p3p7c8_bohstdinteropPtr_char_35cf4c();
}

void p3p3c6_bohstdString_m_this_51708853(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdString * p_str, int32_t p_offset, int32_t p_length)
{
	(self->f_chars = p_str->f_chars);
	(self->f_offset = (p_offset + p_str->f_offset));
	(self->f_length = p_length);
}
void p3p3c6_bohstdString_m_this_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_length)
{
	(self->f_chars = new_p3p3p7c8_bohstdinteropPtr_char_bf420477(p3p3p7c7_bohstdinteropInterop_m_gcAlloc_799e0023((p_length * sizeof(unsigned char)))));
	(self->f_offset = (int32_t)(0));
	(self->f_length = p_length);
	for (int32_t l_i = (int32_t)(0); (l_i < p_length); (++l_i))
	{
		struct p3p3p7c8_bohstdinteropPtr_char temp6;
		p3p3p7c8_bohstdinteropPtr_char_m_set_d5ad6698((temp6 = self->f_chars, &temp6), ((l_i + self->f_offset) * sizeof(unsigned char)), (u8'\0'));
	}
}
_Bool p3p3c6_bohstdString_m_isNullOrEmpty_ef2d95bf(struct p3p3c6_bohstdString * p_str)
{
	p3p3c6_bohstdString_m_static_0();
	return (((p_str == NULL)) || ((p_str->f_length == 0)));
}
_Bool p3p3c6_bohstdString_m_equals_e9664e21(struct p3p3c6_bohstdString * const self, struct p3p3c6_bohstdObject * p_other)
{
	struct p3p3c6_bohstdObject * temp7;
	if ((!p3p3c6_bohstdObject_m_valEquals_4eb476e0((struct p3p3c6_bohstdObject *)((temp7 = p_other)->vtable->m_getType_35cf4c(temp7)), (struct p3p3c6_bohstdObject *)((typeof_p3p3c6_bohstdString())))))
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
		if ((!(p3p3c6_bohstdString_m_get_adeaa357(l_str, l_i) == p3p3c6_bohstdString_m_get_adeaa357(self, l_i))))
		{
			return 0;
		}
	}
	return 1;
}
unsigned char p3p3c6_bohstdString_m_get_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_i)
{
	return (*(unsigned char *)((int8_t *)((self->f_chars + ((self->f_offset + p_i) * sizeof(unsigned char))))));
}
struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_adeaa357(struct p3p3c6_bohstdString * const self, int32_t p_idx)
{
	return new_p3p3c6_bohstdString_51708853(self, p_idx, (self->f_length - p_idx));
}
struct p3p3c6_bohstdString * p3p3c6_bohstdString_m_substring_dd8c3cec(struct p3p3c6_bohstdString * const self, int32_t p_idx, int32_t p_len)
{
	return new_p3p3c6_bohstdString_51708853(self, p_idx, (p_len - p_idx));
}
int32_t p3p3c6_bohstdString_m_indexOf_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if (((p3p3c6_bohstdString_m_get_adeaa357(self, l_i) == p_ch)))
		{
			return l_i;
		}
	}
	return (-1);
}
int32_t p3p3c6_bohstdString_m_count_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	int32_t l_result = (int32_t)(0);
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if (((p3p3c6_bohstdString_m_get_adeaa357(self, l_i) == p_ch)))
		{
			(++l_result);
		}
	}
	return l_result;
}
struct p3p3c14_bohstdArray_boh_std_String * p3p3c6_bohstdString_m_split_111bcd8d(struct p3p3c6_bohstdString * const self, unsigned char p_ch)
{
	struct p3p3c14_bohstdArray_boh_std_String * l_res = new_p3p3c14_bohstdArray_boh_std_String_adeaa357((p3p3c6_bohstdString_m_count_111bcd8d(self, p_ch) + 1));
	int32_t l_i = (int32_t)(0);
	int32_t l_idx = (int32_t)(0);
	int32_t l_prev = (int32_t)(0);
	while ((!((l_idx = p3p3c6_bohstdString_m_indexOf_111bcd8d(self, p_ch)) == (-1))))
	{
		p3p3c14_bohstdArray_boh_std_String_m_set_d881ed08(l_res, l_i, p3p3c6_bohstdString_m_substring_dd8c3cec(self, l_prev, (l_idx - l_prev)));
		(++l_i);
		(l_prev = l_idx);
	}
	p3p3c14_bohstdArray_boh_std_String_m_set_d881ed08(l_res, l_i, p3p3c6_bohstdString_m_substring_dd8c3cec(self, l_idx, (self->f_length - l_idx)));
	return l_res;
}
struct p3p3c6_bohstdString * p3p3c6_bohstdString_op_add_5264d1a0(struct p3p3c6_bohstdString * p_left, struct p3p3c6_bohstdString * p_right)
{
	p3p3c6_bohstdString_m_static_0();
	struct p3p3c6_bohstdString * l_res = create_str_by_len((p_left->f_length + p_right->f_length));
	for (int32_t l_i = (int32_t)(0); (l_i < p_left->f_length); (++l_i))
	{
		struct p3p3p7c8_bohstdinteropPtr_char temp8;
		p3p3p7c8_bohstdinteropPtr_char_m_set_d5ad6698((temp8 = l_res->f_chars, &temp8), ((l_i + self->f_offset) * sizeof(unsigned char)), p3p3c6_bohstdString_m_get_adeaa357(p_left, l_i));
	}
	for (int32_t l_i = (int32_t)(0); (l_i < p_right->f_length); (++l_i))
	{
		struct p3p3p7c8_bohstdinteropPtr_char temp9;
		p3p3p7c8_bohstdinteropPtr_char_m_set_d5ad6698((temp9 = l_res->f_chars, &temp9), (((l_i + self->f_offset) + p_left->f_length) * sizeof(unsigned char)), p3p3c6_bohstdString_m_get_adeaa357(p_right, l_i));
	}
	return l_res;
}
void p3p3c6_bohstdString_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	p3p3c6_bohstdString_sf_empty = boh_create_string(u"", 0);
	{
	}
}
