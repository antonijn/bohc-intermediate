#include "p2p4c9_mypackMainClass.h"


static void p2p4c9_mypackMainClass_m_func_24994b9(struct p2p4c9_mypackMainClass * const self, float p_param);
static void p2p4c9_mypackMainClass_m_main_2d2816fe(void);

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
	self->f_asdfghjkl = 2.0f;
}

static void p2p4c9_mypackMainClass_m_func_24994b9(struct p2p4c9_mypackMainClass * const self, float p_param)
{
	float* ep_param = GC_malloc(sizeof(float));
	(*ep_param) = p_param;
	{
		struct f12_p05_floatp05_float temp3;
		temp3.function = &l0;
		temp3.context = GC_malloc(sizeof(struct lmbd_ctx_0));
		((struct lmbd_ctx_0 *)temp3.context)->ep_param = &(*ep_param);
		struct f12_p05_floatp05_float* l_function = GC_malloc(sizeof(struct f12_p05_floatp05_float));
		(*l_function) = temp3;
		struct f12_p05_floatp05_float temp4;
		temp4 = (*l_function);
		float l_result = temp4.function(temp4.context, (float)(10));
		struct f11_p04_voidp05_float temp5;
		temp5.function = &l2;
		temp5.context = GC_malloc(sizeof(struct lmbd_ctx_2));
		struct f11_p04_voidp05_float l_action = temp5;
	}
}
int main(int argc, char **argv)
{
	p2p4c9_mypackMainClass_m_main_2d2816fe();
	return 0;
}
static void p2p4c9_mypackMainClass_m_main_2d2816fe(void)
{
	struct p2p4c9_mypackMainClass * l_mc = new_p2p4c9_mypackMainClass_d5aca7eb();
	p2p4c9_mypackMainClass_m_func_24994b9(l_mc, 1.0f);
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
