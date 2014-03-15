using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmZeroinitializer : LlvmValue
	{
		private readonly LlvmType ty;

		public LlvmZeroinitializer(LlvmType ty)
		{
			this.ty = ty;
		}

		public override LlvmType Type()
		{
			return ty;
		}

		public override string ToString()
		{
			return "zeroinitializer";
		}
	}
}

