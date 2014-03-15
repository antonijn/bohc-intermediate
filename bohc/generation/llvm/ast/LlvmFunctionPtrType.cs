using System;
using System.Linq;
using System.Text;

namespace Bohc.Generation.Llvm
{
	public class LlvmFunctionPtrType : LlvmType
	{
		public LlvmType ret;
		public LlvmType[] parameters;

		public LlvmFunctionPtrType(LlvmType ret, LlvmType[] parameters)
		{
			this.ret = ret;
			this.parameters = parameters;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(ret.ToString()).Append(" (");
			foreach (LlvmType l in parameters)
			{
				sb.Append(l.ToString()).Append(",");
			}
			if (parameters.Length > 0)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			return sb.Append(")*").ToString();
		}

		public override int size(General.Platform p)
		{
			return p.longType().getSizeofBytes(p);
		}
	}
}

