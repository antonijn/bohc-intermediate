using System;

namespace Bohc.Generation.Llvm
{
	public abstract class LlvmValue
	{
		public abstract LlvmType Type();

		public LlvmValue()
		{
		}
	}
}

