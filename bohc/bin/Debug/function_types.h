#pragma once
#include "p2p4eB_mypackEnumExample.h"
#include "p2p4c9_mypackMainClass.h"
#include "p3p3c6_bohstdObject.h"
#include "p3p3c6_bohstdString.h"
#include "p3p3c4_bohstdType.h"
#include "p3p3c9_bohstdException.h"
#include "p3p3cD_bohstdStringBuilder.h"
#include "p3p3p7c7_bohstdinteropVoidPtr.h"
#include "p3p3p7c7_bohstdinteropInterop.h"
#include "p3p3c17_bohstdBox_my_pack_EnumExample.h"
#include "p3p3c14_bohstdArray_boh_std_String.h"
#include "p3p3cA_bohstdArray_char.h"
#include "p3p3c9_bohstdArray_int.h"
#include "p3p3c22_bohstdArray_boh_std_Array_boh_std_String.h"
#include "p3p3i1A_bohstdICollection_boh_std_String.h"
#include "p3p3i10_bohstdICollection_char.h"
#include "p3p3iF_bohstdICollection_int.h"
#include "p3p3i28_bohstdICollection_boh_std_Array_boh_std_String.h"
#include "p3p3i21_bohstdIIndexedCollection_boh_std_String.h"
#include "p3p3i17_bohstdIIndexedCollection_char.h"
#include "p3p3i16_bohstdIIndexedCollection_int.h"
#include "p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String.h"
#include "p3p3i18_bohstdIIterator_boh_std_String.h"
#include "p3p3iE_bohstdIIterator_char.h"
#include "p3p3iD_bohstdIIterator_int.h"
#include "p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String.h"
#include "p3p3c9_bohstdList_char.h"
#include "p3p3c8_bohstdList_int.h"
#include "p3p3p7c8_bohstdinteropPtr_char.h"
struct f1A_p07_booleanp04_charp03_int
{
	_Bool (* function)(void *, unsigned char, int32_t);
	void * context;
};
struct f13_p07_booleanp04_char
{
	_Bool (* function)(void *, unsigned char);
	void * context;
};
struct f19_p07_booleanp03_intp03_int
{
	_Bool (* function)(void *, int32_t, int32_t);
	void * context;
};
struct f12_p07_booleanp03_int
{
	_Bool (* function)(void *, int32_t);
	void * context;
};
struct lmbd_ctx_0
{
	struct f12_p07_booleanp03_int *ep_when;
};
_Bool l0(struct lmbd_ctx_0 * ctx, int32_t p_item, int32_t p_idx);
struct lmbd_ctx_1
{
	int32_t *ep_item;
};
_Bool l1(struct lmbd_ctx_1 * ctx, int32_t p_other);
struct lmbd_ctx_2
{
	struct f13_p07_booleanp04_char *ep_when;
};
_Bool l2(struct lmbd_ctx_2 * ctx, unsigned char p_item, int32_t p_idx);
struct lmbd_ctx_3
{
	unsigned char *ep_item;
};
_Bool l3(struct lmbd_ctx_3 * ctx, unsigned char p_other);
struct lmbd_ctx_4
{
};
_Bool l4(struct lmbd_ctx_4 * ctx, int32_t p_item, int32_t p_idx);
struct lmbd_ctx_5
{
};
_Bool l5(struct lmbd_ctx_5 * ctx, int32_t p_other);
