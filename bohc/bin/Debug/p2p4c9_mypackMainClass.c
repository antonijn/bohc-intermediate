#include "p2p4c9_mypackMainClass.h"


static void p2p4c9_mypackMainClass_m_main_decf77d6(struct p3p3c14_bohstdArray_boh_std_String * const p_args);

const struct vtable_p2p4c9_mypackMainClass instance_vtable_p2p4c9_mypackMainClass = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p2p4c9_mypackMainClass(void)
{
	static struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p2p4c9_mypackMainClass * new_p2p4c9_mypackMainClass_d5aca7eb(void)
{
	struct p2p4c9_mypackMainClass * result = GC_malloc(sizeof(struct p2p4c9_mypackMainClass));
	result->vtable = &instance_vtable_p2p4c9_mypackMainClass;
	p2p4c9_mypackMainClass_m_static_2d2816fe();
	p2p4c9_mypackMainClass_fi(result);
	p2p4c9_mypackMainClass_m_this_d5aca7eb(result);
	return result;
}

void p2p4c9_mypackMainClass_fi(struct p2p4c9_mypackMainClass * const self)
{
}

int main(int argc, char **argv)
{
	p2p4c9_mypackMainClass_m_main_decf77d6();
	return 0;
}
static void p2p4c9_mypackMainClass_m_main_decf77d6(struct p3p3c14_bohstdArray_boh_std_String * const p_args)
{
	struct p3p3c9_bohstdArray_int * l_integers = new_p3p3c9_bohstdArray_int_70fcd6e5((int32_t)(10));
}
void p2p4c9_mypackMainClass_m_this_d5aca7eb(struct p2p4c9_mypackMainClass * const self)
{
}
void p2p4c9_mypackMainClass_m_static_2d2816fe(void)
{
	static _Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	{
	}
}
