using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmUndef : LlvmValue
	{
		private LlvmType type;

		public LlvmUndef(LlvmType type)
		{
			this.type = type;
		}

		public override LlvmType Type()
		{
			return type;
		}

		public override string ToString()
		{
			return "undef";
		}
	}
}

