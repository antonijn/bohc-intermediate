using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmPrimitive : LlvmType
	{
		private string name;

		public LlvmPrimitive(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}
	}
}

