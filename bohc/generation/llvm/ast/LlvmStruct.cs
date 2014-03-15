using System;
using System.Linq;
using System.Collections.Generic;

namespace Bohc.Generation.Llvm
{
	public class LlvmStruct : LlvmType
	{
		public string strname;
		public Dictionary<string, LlvmType> members;

		public LlvmStruct(string strname)
		{
			this.strname = strname.Replace('`', '.');
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
			return strname;
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

