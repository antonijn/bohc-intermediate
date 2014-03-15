using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmPointer : LlvmType
	{
		public LlvmType t;

		public LlvmPointer(LlvmType t)
		{
			this.t = t;
		}

		public override string ToString()
		{
			return t.ToString() + "*";
		}

		public override int size(General.Platform p)
		{
			return p.longType().getSizeofBytes(p);
		}
	}
}

