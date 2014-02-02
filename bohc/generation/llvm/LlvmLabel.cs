using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmLabel
	{
		public static int tempcount = 0;

		public readonly string id;

		public LlvmLabel(string desc)
		{
			id = "." + desc + tempcount++;
		}

		public LlvmLabel()
		{
			id = "." + tempcount++;
		}

		public override string ToString()
		{
			return "%" + id;
		}
	}
}

