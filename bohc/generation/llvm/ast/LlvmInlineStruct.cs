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

		public int Offset(string mem)
		{
			int o = 0;
			foreach (string str in members.Keys)
			{
				if (mem == str)
				{
					return o;
				}
				++o;
			}
			throw new NotImplementedException();
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

		private void pad(ref int s, int w)
		{
			while ((s % w) != 0)
				++s;
		}

		public override int size(General.Platform p)
		{
			int size = 0;
			int max = 1;
			foreach (LlvmType ty in members.Values)
			{
				pad(ref size, ty.padding(p));
				int ns = ty.size(p);
				size += ns;
				max = Math.Max(max, ns);
			}
			if (size == 0)
			{
				return 1;
			}
			pad(ref size, max);
			return size;
		}
	}
}

