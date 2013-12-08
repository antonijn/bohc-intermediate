#pragma once
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
struct f12_p05_floatp05_float
{
	float (* function)(void *, float);
	void * context;
};
struct f11_p04_voidp05_float
{
	void (* function)(void *, float);
	void * context;
};
struct f8_p04_void
{
	void (* function)(void *);
	void * context;
};
struct lmbd_ctx_0
{
	float *ep_param;
};
float l0(struct lmbd_ctx_0 * ctx, float p_x);
struct lmbd_ctx_1
{
	struct f12_p05_floatp05_float *l_function;
	float *ep_what;
	struct p2p4c9_mypackMainClass * self;
};
void l1(struct lmbd_ctx_1 * ctx);
struct lmbd_ctx_2
{
};
void l2(struct lmbd_ctx_2 * ctx, float p_what);
