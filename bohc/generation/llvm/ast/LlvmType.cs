using System;

using Bohc.TypeSystem;

namespace Bohc.Generation.Llvm
{
	public abstract class LlvmType
	{
		public override bool Equals(object obj)
		{
			return obj.ToString() == ToString();
		}

		public abstract int size(General.Platform p);
		public virtual int padding(General.Platform p)
		{
			return size(p);
		}
	}
}

