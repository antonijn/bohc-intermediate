using System;
using System.Collections.Generic;

namespace Bohc.Generation.Llvm
{
	public class LlvmLabel
	{
		public string id;
		public List<LlvmLabel> preds = new List<LlvmLabel>();

		public LlvmLabel()
		{
			id = "0";
		}

		public override string ToString()
		{
			return "%" + id;
		}
	}
}

