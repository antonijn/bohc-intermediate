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
	}
}

