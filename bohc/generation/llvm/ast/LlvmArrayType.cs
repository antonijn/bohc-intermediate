using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmArrayType : LlvmType
	{
		public int count;
		public LlvmType type;

		public LlvmArrayType(int count, LlvmType type)
		{
			this.count = count;
			this.type = type;
		}

		public override string ToString()
		{
			return "[" + count.ToString() + " x " + type.ToString() + "]";
		}

		public override int size(General.Platform p)
		{
			throw new NotImplementedException();
		}
	}
}

