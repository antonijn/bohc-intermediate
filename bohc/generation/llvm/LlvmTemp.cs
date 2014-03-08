using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmTemp : LlvmVar
	{
		private int tmp;

		public LlvmTemp(LlvmType type)
			: base(type)
		{
			tmp = Llvm.tempvc.Peek()[0]++;
		}

		public override string ToString()
		{
			return "%" + (tmp);
		}
	}
}

