using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmVar : LlvmValue
	{
		private LlvmType type;
		public override LlvmType Type()
		{
			return type;
		}

		public LlvmVar(LlvmType type)
		{
			this.type = type;
		}
	}
}

