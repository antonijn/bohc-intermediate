#include "p2p4c9_mypackMainClass.h"


static void p2p4c9_mypackMainClass_m_main_f366fe7e(struct p3p3c14_bohstdArray_boh_std_String * const p_args);

const struct vtable_p2p4c9_mypackMainClass instance_vtable_p2p4c9_mypackMainClass = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c, &p2p4c9_mypackMainClass_m_vfun_35cf4c };

struct p3p3c4_bohstdType * typeof_p2p4c9_mypackMainClass(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p2p4c9_mypackMainClass * new_p2p4c9_mypackMainClass_35cf4c(void)
{
	struct p2p4c9_mypackMainClass * result = GC_malloc(sizeof(struct p2p4c9_mypackMainClass));
	result->vtable = &instance_vtable_p2p4c9_mypackMainClass;
	p2p4c9_mypackMainClass_m_static_0();
	p2p4c9_mypackMainClass_fi(result);
	p2p4c9_mypackMainClass_m_this_35cf4c(result);
	return result;
}

void p2p4c9_mypackMainClass_fi(struct p2p4c9_mypackMainClass * const self)
{
	self->f_i = 10;
}

#if defined(PF_LINUX)
void p2p4c9_mypackMainClass_m_vfun_35cf4c(struct p2p4c9_mypackMainClass * const self)
{
	struct p3p3c6_bohstdString * l_str = (p3p3c6_bohstdString_op_add_5264d1a0(boh_create_string(u"Hello, ", 7), boh_create_string(u"world!", 6)));
	(l_str = (p3p3c6_bohstdString_op_add_5264d1a0(l_str, boh_create_string(u"From antonijn", 13))));
}
#endif
int main(int argc, char **argv)
{
	p2p4c9_mypackMainClass_m_main_f366fe7e();
	return 0;
}
void p2p4c9_mypackMainClass_m_main_f366fe7e(struct p3p3c14_bohstdArray_boh_std_String * const p_args)
{
	struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String * l_strstsrts = (struct p3p3c22_bohstdArray_boh_std_Array_boh_std_String *)(new_p3p3c14_bohstdArray_boh_std_String_adeaa357((int32_t)(10)));
	p3p3c22_bohstdArray_boh_std_Array_boh_std_String_m_set_9c920c15(l_strstsrts, (int32_t)(0), p_args);
}
void p2p4c9_mypackMainClass_m_this_35cf4c(struct p2p4c9_mypackMainClass * const self)
{
}
void p2p4c9_mypackMainClass_m_static_0(void)
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
