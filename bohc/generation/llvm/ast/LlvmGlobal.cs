using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmGlobal : LlvmVar
	{
		public LlvmLinkage linkage;
		public LlvmGlobalFlags flags;
		public string id;
		public LlvmValue initial;

		public LlvmGlobal(LlvmLinkage linkage, LlvmGlobalFlags flags, string id, LlvmType t, LlvmValue initial)
			: base(new LlvmPointer(t))
		{
			this.linkage = linkage;
			this.flags = flags;
			this.id = id.Replace('`', '.');
			this.initial = initial;
		}

		public override string ToString()
		{
			return id;
		}
	}
}

