#include "p2p4c9_mypackMainClass.h"



const struct vtable_p2p4c9_mypackMainClass instance_vtable_p2p4c9_mypackMainClass = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p2p4c9_mypackMainClass(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p2p4c9_mypackMainClass * new_p2p4c9_mypackMainClass(struct p3p3c6_bohstdString * p_str)
{
	struct p2p4c9_mypackMainClass * result = GC_malloc(sizeof(struct p2p4c9_mypackMainClass));
	result->f_str = NULL;
	result->vtable = &instance_vtable_p2p4c9_mypackMainClass;
	p2p4c9_mypackMainClass_m_this_125bf9a2(result, p_str);
	return result;
}

void p2p4c9_mypackMainClass_m_this_125bf9a2(struct p2p4c9_mypackMainClass * const self, struct p3p3c6_bohstdString * p_str)
{
	(self->f_str = p_str);
}
void p2p4c9_mypackMainClass_m_main_2d2816fe(void)
{
}
