using System;
using System.Linq;
using System.Collections.Generic;

namespace Bohc.Generation.Llvm
{
	public class LlvmBuilder
	{
		private Llvm llvm;
		public LlvmBuilder(Llvm llvm)
		{
			this.llvm = llvm;
		}

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

		public void Insert(int i, object o)
		{
			objects.Insert(i, o);
		}

		public string expand()
		{
			foreach (LlvmValue o in objects.OfType<LlvmValue>())
			{
				o.use();
			}
			return new string(objects.SelectMany(x => x.ToString()).ToArray());
		}
	}
}

