using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmStackLocal : LlvmVar
	{
		private LlvmType type;
		private string str;

		public LlvmStackLocal(LlvmType type, string str)
			: base(new LlvmPointer(type))
		{
			this.type = type;
			this.str = str;
		}

		public override string ToString()
		{
			return "%" + str;
		}
	}
}

