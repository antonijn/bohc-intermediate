using System;
using System.Text;

namespace Bohc.Generation.Llvm
{
	public class LlvmStructInit : LlvmValue
	{
		private LlvmType ty;
		public LlvmValue[] members;
		public LlvmStructInit(LlvmType ty, params LlvmValue[] members)
		{
			this.ty = ty;
			this.members = members;
		}

		public override LlvmType Type()
		{
			return ty;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{ ");
			foreach (LlvmValue mem in members)
			{
				sb.Append(mem.Type().ToString()).Append(" ").Append(mem.ToString())
					.Append(", ");
			}
			if (members.Length > 0)
			{
				sb.Remove(sb.Length - 2, 2);
			}
			sb.Append(" }");
			return sb.ToString();
		}
	}
}

