using System;
using System.Collections.Generic;
using System.Text;

namespace Bohc.Generation.Llvm
{
	public class LlvmInlineStruct : LlvmType
	{
		public Dictionary<string, LlvmType> members;
		public LlvmInlineStruct()
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{ ");
			foreach (var v in members)
			{
				sb.Append(v.Value.ToString()).Append(", ");
			}
			if (members.Count > 0)
			{
				sb.Remove(sb.Length - 2, 2);
			}
			sb.Append(" }");
			return sb.ToString();
		}
	}
}

