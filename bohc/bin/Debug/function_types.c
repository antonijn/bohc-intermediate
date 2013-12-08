#include "function_types.h"
float l0(struct lmbd_ctx_0 * ctx, float p_x)
{
	return (p_x / ctx->*ep_param);
}
void l1(struct lmbd_ctx_1 * ctx)
{
	struct f12_p05_floatp05_float temp0;
	temp0 = ctx->*l_function;
	temp0.function(temp0.context, (ctx->*ep_what * ctx->self->f_asdfghjkl));
}
void l2(struct lmbd_ctx_2 * ctx, float p_what)
{
	float* ep_what = GC_malloc(sizeof(float));
	ctx->*ep_what = p_what;
	{
		{
			struct f8_p04_void temp1;
			temp1.function = &l1;
			temp1.context = GC_malloc(sizeof(struct lmbd_ctx_1));
			((struct lmbd_ctx_1 *)temp1.context)->l_function = &ctx->*l_function;
			((struct lmbd_ctx_1 *)temp1.context)->ep_what = &ctx->*ep_what;
			((struct lmbd_ctx_1 *)temp1.context)->self = ctx->self;
			struct f8_p04_void l_other = temp1;
			struct f8_p04_void temp2;
			temp2 = l_other;
			temp2.function(temp2.context);
		}
	}
}
