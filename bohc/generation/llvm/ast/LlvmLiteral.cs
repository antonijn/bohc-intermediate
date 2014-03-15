using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmLiteral : LlvmValue
	{
		private LlvmType t;
		private string str;

		public override LlvmType Type()
		{
			return t;
		}

		public LlvmLiteral(LlvmType t, string str)
		{
			this.t = t;
			this.str = str;
		}

		public override string ToString()
		{
			// TODO: do this properly
			return str;
		}
	}
}

