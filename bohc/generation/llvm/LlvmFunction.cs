using System;
using System.Linq;
using System.Collections.Generic;

namespace Bohc.Generation.Llvm
{
	public class LlvmFunction : LlvmValue
	{
		public string id;
		public LlvmType ret;
		public List<LlvmParam> parameters;
		public LlvmLinkage linkage;
		public bool unwind;

		public LlvmFunction(LlvmType ret, string id, List<LlvmParam> parameters,
		                    LlvmLinkage linkage, bool unwind)
		{
			this.id = id;
			this.ret = ret;
			this.unwind = unwind;
			this.linkage = linkage;
			this.parameters = parameters;
		}

		public override LlvmType Type()
		{
			return new LlvmFunctionPtrType(ret, parameters.Select(x => x.Type()).ToArray());
		}

		public override string ToString()
		{
			return id;
		}
	}
}

