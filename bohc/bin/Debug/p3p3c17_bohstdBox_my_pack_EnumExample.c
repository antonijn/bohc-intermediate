#include "p3p3c17_bohstdBox_my_pack_EnumExample.h"



const struct vtable_p3p3c17_bohstdBox_my_pack_EnumExample instance_vtable_p3p3c17_bohstdBox_my_pack_EnumExample = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c17_bohstdBox_my_pack_EnumExample_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c17_bohstdBox_my_pack_EnumExample(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c17_bohstdBox_my_pack_EnumExample * new_p3p3c17_bohstdBox_my_pack_EnumExample_3b9b8234(enum p2p4eB_mypackEnumExample p_value)
{
	struct p3p3c17_bohstdBox_my_pack_EnumExample * result = GC_malloc(sizeof(struct p3p3c17_bohstdBox_my_pack_EnumExample));
	result->vtable = &instance_vtable_p3p3c17_bohstdBox_my_pack_EnumExample;
	p3p3c17_bohstdBox_my_pack_EnumExample_m_static_0();
	p3p3c17_bohstdBox_my_pack_EnumExample_fi(result);
	p3p3c17_bohstdBox_my_pack_EnumExample_m_this_3b9b8234(result, p_value);
	return result;
}

void p3p3c17_bohstdBox_my_pack_EnumExample_fi(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self)
{
	self->f_value = 0;
}

void p3p3c17_bohstdBox_my_pack_EnumExample_m_this_3b9b8234(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self, enum p2p4eB_mypackEnumExample p_value)
{
	(self->f_value = p_value);
}
struct p3p3c6_bohstdString * p3p3c17_bohstdBox_my_pack_EnumExample_m_toString_35cf4c(struct p3p3c17_bohstdBox_my_pack_EnumExample * const self)
{
	return p2p4eB_mypackEnumExample_m_toString_35cf4c(self->f_value);
}
void p3p3c17_bohstdBox_my_pack_EnumExample_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	{
	}
}
