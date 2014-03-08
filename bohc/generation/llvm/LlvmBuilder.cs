using System;
using System.Linq;
using System.Collections.Generic;

namespace Bohc.Generation.Llvm
{
	public class LlvmBuilder
	{
		private List<object> objects = new List<object>();

		public LlvmBuilder Append(object o)
		{
			objects.Add(o);
			return this;
		}

		public LlvmBuilder AppendLine(object o)
		{
			return Append(o).Append(Environment.NewLine);
		}

		public LlvmBuilder AppendLine()
		{
			return AppendLine(string.Empty);
		}

		public void RemoveLast()
		{
			objects.RemoveAt(objects.Count - 1);
		}

		public override string ToString()
		{
			return new string(objects.SelectMany(x => x.ToString()).ToArray());
		}
	}
}

