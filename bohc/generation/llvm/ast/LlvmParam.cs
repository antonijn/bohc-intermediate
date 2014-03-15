using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmParam : LlvmVar
	{
		private string str;

		public LlvmParam(string str, LlvmType ty)
			: base(ty)
		{
			this.str = str;
		}

		public override string ToString()
		{
			return str;
		}
	}
}

