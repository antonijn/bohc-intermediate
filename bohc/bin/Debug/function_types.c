#include "function_types.h"
_Bool l0(struct lmbd_ctx_0 * ctx, int32_t p_item, int32_t p_idx)
{
	struct f12_p07_booleanp03_int temp0;
	temp0 = ctx->*ep_when;
	return temp0.function(temp0.context, p_item);
}
_Bool l1(struct lmbd_ctx_1 * ctx, int32_t p_other)
{
	return ((p_other == ctx->*ep_item));
}
_Bool l2(struct lmbd_ctx_2 * ctx, unsigned char p_item, int32_t p_idx)
{
	struct f13_p07_booleanp04_char temp1;
	temp1 = ctx->*ep_when;
	return temp1.function(temp1.context, p_item);
}
_Bool l3(struct lmbd_ctx_3 * ctx, unsigned char p_other)
{
	return ((p_other == ctx->*ep_item));
}
_Bool l4(struct lmbd_ctx_4 * ctx, int32_t p_item, int32_t p_idx)
{
	struct f12_p07_booleanp03_int temp2;
	temp2 = ctx->*ep_when;
	return temp2.function(temp2.context, p_item);
}
_Bool l5(struct lmbd_ctx_5 * ctx, int32_t p_other)
{
	return ((p_other == ctx->*ep_item));
}
