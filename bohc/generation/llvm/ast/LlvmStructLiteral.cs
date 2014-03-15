using System;
using System.Text;

namespace Bohc.Generation.Llvm
{
	public class LlvmStructLiteral : LlvmValue
	{
		private readonly LlvmType ty;
		private readonly LlvmValue[] init;

		public LlvmStructLiteral(LlvmType ty, params LlvmValue[] init)
		{
			this.ty = ty;
			this.init = init;
		}

		public override LlvmType Type()
		{
			return ty;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("{ ");
			foreach (LlvmValue v in init)
			{
				builder.Append(v.Type().ToString()).Append(" ").Append(v.ToString()).Append(", ");
			}
			builder.Remove(builder.Length - 2, 2);
			builder.Append(" }");
			return builder.ToString();
		}
	}
}

