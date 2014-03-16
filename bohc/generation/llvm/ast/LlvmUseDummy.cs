using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmUseDummy : LlvmValue
	{
		private Llvm ll;
		public LlvmUseDummy(Llvm ll)
		{
			this.ll = ll;
		}

		public override LlvmType Type()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return string.Empty;
		}

		private bool used = false;
		public override void use()
		{
			if (used)
				return;
			used = true;
			++ll.nums;
		}
	}
}

