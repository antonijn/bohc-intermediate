using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmInline : LlvmValue
	{
		private LlvmType ty;
		private LlvmBuilder str;

		public LlvmInline(LlvmType ty, LlvmBuilder str)
		{
			this.ty = ty;
			this.str = str;
		}

		public override LlvmType Type()
		{
			return ty;
		}

		public override string ToString()
		{
			return str.ToString();
		}
	}
}

