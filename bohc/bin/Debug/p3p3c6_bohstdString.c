#include "p3p3c6_bohstdString.h"



const struct vtable_p3p3c6_bohstdString instance_vtable_p3p3c6_bohstdString = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3c6_bohstdString(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c6_bohstdString * new_p3p3c6_bohstdString(void)
{
	struct p3p3c6_bohstdString * result = GC_malloc(sizeof(struct p3p3c6_bohstdString));
	result->f_str = NULL;
	result->f_length = 0;
	result->f_first = u'\0';
	result->vtable = &instance_vtable_p3p3c6_bohstdString;
	p3p3c6_bohstdString_m_this_d5aca7eb(result);
	return result;
}

void p3p3c6_bohstdString_m_this_d5aca7eb(struct p3p3c6_bohstdString * const self)
{
	p2p4c9_mypackMainClass_m_this_125bf9a2(self, boh_create_string(u"Hi!", 3));
}
