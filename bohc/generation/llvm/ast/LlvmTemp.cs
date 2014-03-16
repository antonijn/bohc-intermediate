using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmTemp : LlvmVar
	{
		private int tmp;
		private Llvm ll;

		public LlvmTemp(LlvmType type, Llvm ll)
			: base(type)
		{
			this.ll = ll;
		}

		private bool used = false;
		public override void use()
		{
			if (used)
				return;
			used = true;
			tmp = ll.nums++;
		}

		public override string ToString()
		{
			return "%" + tmp;
		}
	}
}

