using System;
using System.Text;

namespace Bohc.Generation.Llvm
{
	public class LlvmStringLiteral : LlvmLiteral
	{
		private static string getStr(byte[] chars)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("c\"");
			foreach (byte b in chars)
			{
				if ((b & 0x80) == 0 && !char.IsControl((char)b) && b != '\\' && b != '"')
				{
					sb.Append((char)b);
				}
				else
				{
					sb.Append("\\");
					sb.AppendFormat("{0:X2}", b);
				}
			}
			sb.Append("\"");
			return sb.ToString();
		}

		public LlvmStringLiteral(byte[] chars)
			: base(new LlvmArrayType(chars.Length, new LlvmPrimitive("i8")), getStr(chars))
		{
		}
	}
}

