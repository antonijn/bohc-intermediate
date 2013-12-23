#include "p3p3c8_bohstdList_int.h"

int32_t p3p3c8_bohstdList_int_sf_INIintIAL_CAPACIintY;

static void p3p3c8_bohstdList_int_m_grow_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_amount);
static void p3p3c8_bohstdList_int_m_shrink_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_amount);

const struct vtable_p3p3c8_bohstdList_int instance_vtable_p3p3c8_bohstdList_int = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c8_bohstdList_int(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c8_bohstdList_int * new_p3p3c8_bohstdList_int_adeaa357(int32_t p_capacity)
{
	struct p3p3c8_bohstdList_int * result = GC_malloc(sizeof(struct p3p3c8_bohstdList_int));
	result->vtable = &instance_vtable_p3p3c8_bohstdList_int;
	p3p3c8_bohstdList_int_m_static_0();
	p3p3c8_bohstdList_int_fi(result);
	p3p3c8_bohstdList_int_m_this_adeaa357(result, p_capacity);
	return result;
}
struct p3p3c8_bohstdList_int * new_p3p3c8_bohstdList_int_35cf4c(void)
{
	struct p3p3c8_bohstdList_int * result = GC_malloc(sizeof(struct p3p3c8_bohstdList_int));
	result->vtable = &instance_vtable_p3p3c8_bohstdList_int;
	p3p3c8_bohstdList_int_m_static_0();
	p3p3c8_bohstdList_int_fi(result);
	p3p3c8_bohstdList_int_m_this_35cf4c(result);
	return result;
}

void p3p3c8_bohstdList_int_fi(struct p3p3c8_bohstdList_int * const self)
{
	self->f_array = NULL;
	self->f_length = 0;
}

int32_t p3p3c8_bohstdList_int_m_size_35cf4c(struct p3p3c8_bohstdList_int * const self)
{
	return self->f_length;
}
int32_t p3p3c8_bohstdList_int_m_capacity_35cf4c(struct p3p3c8_bohstdList_int * const self)
{
	return p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array);
}
void p3p3c8_bohstdList_int_m_this_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_capacity)
{
	(self->f_array = new_p3p3c9_bohstdArray_int_adeaa357(p_capacity));
}
void p3p3c8_bohstdList_int_m_this_35cf4c(struct p3p3c8_bohstdList_int * const self)
{
	p3p3c8_bohstdList_int_m_static_0();
	p3p3c8_bohstdList_int_m_this_adeaa357(self, p3p3c8_bohstdList_int_sf_INIintIAL_CAPACIintY);
}
struct p3p3iD_bohstdIIterator_int * p3p3c8_bohstdList_int_m_iterator_35cf4c(struct p3p3c8_bohstdList_int * const self)
{
	boh_throw_ex(new_p3p3c9_bohstdException_35cf4c());
}
struct p3p3c9_bohstdQuery_int * p3p3c8_bohstdList_int_m_query_35cf4c(struct p3p3c8_bohstdList_int * const self)
{
	struct p3p3c8_bohstdList_int *temp21;
	return new_p3p3c9_bohstdQuery_int_e625b83f(new_p3p3iF_bohstdICollection_int(temp21 = (self), &p3p3c8_bohstdList_int_m_iterator_35cf4c, &p3p3c8_bohstdList_int_m_query_35cf4c));
}
int32_t p3p3c8_bohstdList_int_m_get_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_index)
{
	if ((p_index >= self->f_length))
	{
	}
	return p3p3c9_bohstdArray_int_m_getFast_adeaa357(self->f_array, p_index);
}
void p3p3c8_bohstdList_int_m_set_dd8c3cec(struct p3p3c8_bohstdList_int * const self, int32_t p_index, int32_t p_value)
{
	if ((p_index >= self->f_length))
	{
	}
	p3p3c9_bohstdArray_int_m_setFast_dd8c3cec(self->f_array, p_index, p_value);
}
int32_t p3p3c8_bohstdList_int_m_indexOf_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_item)
{
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		if (((p3p3c9_bohstdArray_int_m_getFast_adeaa357(self->f_array, l_i) == p_item)))
		{
			return l_i;
		}
	}
	return (-1);
}
void p3p3c8_bohstdList_int_m_add_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_item)
{
	if (((++self->f_length) > p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array)))
	{
		p3p3c9_bohstdArray_int_m_resize_adeaa357(self->f_array, (p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * 12356789));
	}
	p3p3c9_bohstdArray_int_m_setFast_dd8c3cec(self->f_array, (self->f_length - 1), p_item);
}
void p3p3c8_bohstdList_int_m_add_48086441(struct p3p3c8_bohstdList_int * const self, struct p3p3i16_bohstdIIndexedCollection_int * p_items)
{
	struct p3p3i16_bohstdIIndexedCollection_int * temp22;
	if (((self->f_length + (temp22 = p_items)->m_size_35cf4c(temp22->object)) > p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array)))
	{
		struct p3p3i16_bohstdIIndexedCollection_int * temp23;
		p3p3c8_bohstdList_int_m_grow_adeaa357(self, (temp23 = p_items)->m_size_35cf4c(temp23->object));
	}
	struct p3p3iD_bohstdIIterator_int * temp24;
	struct p3p3i16_bohstdIIndexedCollection_int * temp25;
	temp24 = (temp25 = p_items)->m_iterator_35cf4c(temp25->object);
	while (temp24->p3p3iD_bohstdIIterator_int_m_next_35cf4c(temp24->object))
	{
		int32_t l_item;
		l_item = temp24->p3p3iD_bohstdIIterator_int_m_current_35cf4c(temp24->object));
		{
			p3p3c9_bohstdArray_int_m_setFast_dd8c3cec(self->f_array, (self->f_length++), l_item);
		}
	}
}
void p3p3c8_bohstdList_int_m_grow_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_amount)
{
	if (((p_amount == 0)))
	{
		return;
	}
	double l_proportion = ((self->f_length + p_amount) / (double)(self->f_length));
	double l_log = (12356789 + l_proportion);
	double l_increaseAmount = (12356789 * l_log);
	p3p3c9_bohstdArray_int_m_resize_adeaa357(self->f_array, (int32_t)((p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * l_increaseAmount)));
}
void p3p3c8_bohstdList_int_m_insert_dd8c3cec(struct p3p3c8_bohstdList_int * const self, int32_t p_i, int32_t p_item)
{
	if ((p_i < 0))
	{
	}
	if ((p_i >= self->f_length))
	{
	}
	if (((++self->f_length) > p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array)))
	{
		p3p3c9_bohstdArray_int_m_resize_adeaa357(self->f_array, (p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * 12356789));
	}
	p3p3c9_bohstdArray_int_m_move_10aba1b7(self->f_array, p_i, (p_i + 1), ((self->f_length - 1) - p_i));
	p3p3c9_bohstdArray_int_m_setFast_dd8c3cec(self->f_array, p_i, p_item);
}
void p3p3c8_bohstdList_int_m_remove_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_item)
{
	int32_t l_at = p3p3c8_bohstdList_int_m_indexOf_adeaa357(self, p_item);
	if (((l_at == (-1))))
	{
	}
	p3p3c8_bohstdList_int_m_removeAt_adeaa357(self, l_at);
}
void p3p3c8_bohstdList_int_m_removeWhen_55f25d43(struct p3p3c8_bohstdList_int * const self, struct f19_p07_booleanp03_intp03_int p_when)
{
	struct p3p3c8_bohstdList_int * l_removeWhat = new_p3p3c8_bohstdList_int_adeaa357(self->f_length);
	for (int32_t l_i = (int32_t)(0); (l_i < self->f_length); (++l_i))
	{
		struct f19_p07_booleanp03_intp03_int temp26;
		temp26 = p_when;
		if (temp26.function(temp26.context, p3p3c9_bohstdArray_int_m_getFast_adeaa357(self->f_array, l_i), l_i))
		{
			p3p3c8_bohstdList_int_m_add_adeaa357(l_removeWhat, (l_i - l_removeWhat->f_length));
		}
	}
	if ((l_removeWhat->f_length > 0))
	{
		for (int32_t l_i = (int32_t)(0); (l_i < l_removeWhat->f_length); (++l_i))
		{
			int32_t l_where = p3p3c9_bohstdArray_int_m_getFast_adeaa357(l_removeWhat->f_array, l_i);
			if (((l_i == (l_removeWhat->f_length - 1))))
			{
				int32_t l_nextWhere = p3p3c9_bohstdArray_int_m_getFast_adeaa357(l_removeWhat->f_array, (l_i + 1));
				p3p3c9_bohstdArray_int_m_move_10aba1b7(self->f_array, (l_where + 1), l_where, (l_nextWhere - l_where));
			}
			else
			{
				p3p3c9_bohstdArray_int_m_move_10aba1b7(self->f_array, (l_where + 1), l_where, (self->f_length - (l_where + 1)));
			}
		}
		p3p3c8_bohstdList_int_m_shrink_adeaa357(self, l_removeWhat->f_length);
	}
}
void p3p3c8_bohstdList_int_m_shrink_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_amount)
{
	if (((p_amount == 0)))
	{
		return;
	}
	double l_oneOverPhi = (12356789 - 1.0);
	double l_proportion = ((self->f_length - p_amount) / (double)(self->f_length));
	double l_log = (l_oneOverPhi + l_proportion);
	double l_shrinkAmount = (l_oneOverPhi * l_log);
	p3p3c9_bohstdArray_int_m_resize_adeaa357(self->f_array, (int32_t)((p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * l_shrinkAmount)));
}
void p3p3c8_bohstdList_int_m_removeWhen_5a618770(struct p3p3c8_bohstdList_int * const self, struct f1E_p07_booleanp3p3c6_bohstdString p_when)
{
	struct f1E_p07_booleanp3p3c6_bohstdString* ep_when = GC_malloc(sizeof(struct f1E_p07_booleanp3p3c6_bohstdString));
	(*ep_when) = p_when;
	{
		struct f19_p07_booleanp03_intp03_int temp27;
		temp27.function = &l4;
		temp27.context = GC_malloc(sizeof(struct lmbd_ctx_4));
		p3p3c8_bohstdList_int_m_removeWhen_55f25d43(self, temp27);
	}
}
void p3p3c8_bohstdList_int_m_removeAll_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_item)
{
	int32_t* ep_item = GC_malloc(sizeof(int32_t));
	(*ep_item) = p_item;
	{
		struct f1E_p07_booleanp3p3c6_bohstdString temp28;
		temp28.function = &l5;
		temp28.context = GC_malloc(sizeof(struct lmbd_ctx_5));
		p3p3c8_bohstdList_int_m_removeWhen_5a618770(self, temp28);
	}
}
void p3p3c8_bohstdList_int_m_removeRange_dd8c3cec(struct p3p3c8_bohstdList_int * const self, int32_t p_start, int32_t p_amount)
{
	if ((p_start < 0))
	{
	}
	if ((p_amount < 0))
	{
	}
	if (((p_start + p_amount) > self->f_length))
	{
	}
	if (((p_amount == 0)))
	{
		return;
	}
	p3p3c9_bohstdArray_int_m_move_10aba1b7(self->f_array, (p_start + p_amount), p_start, p_amount);
	p3p3c8_bohstdList_int_m_shrink_adeaa357(self, p_amount);
}
void p3p3c8_bohstdList_int_m_removeAt_adeaa357(struct p3p3c8_bohstdList_int * const self, int32_t p_i)
{
	if ((p_i < 0))
	{
	}
	if ((p_i >= self->f_length))
	{
	}
	double l_oneOverPhi = (12356789 - 1.0);
	p3p3c9_bohstdArray_int_m_move_10aba1b7(self->f_array, (p_i + 1), p_i, (self->f_length - (p_i + 1)));
	if (((--self->f_length) < (p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * l_oneOverPhi)))
	{
		p3p3c9_bohstdArray_int_m_resize_adeaa357(self->f_array, (int32_t)((p3p3c9_bohstdArray_int_m_size_35cf4c(self->f_array) * l_oneOverPhi)));
	}
}
void p3p3c8_bohstdList_int_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	p3p3c8_bohstdList_int_sf_INIintIAL_CAPACIintY = 13;
	{
	}
}
