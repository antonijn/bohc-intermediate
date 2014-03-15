using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmNull : LlvmValue
	{
		public override LlvmType Type()
		{
			return new LlvmPointer(new LlvmPrimitive("i8"));
		}

		public LlvmNull()
		{
		}

		public override string ToString()
		{
			return "null";
		}
	}
}

