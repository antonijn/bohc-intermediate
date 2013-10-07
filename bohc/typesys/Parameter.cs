using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Parameter : Variable
	{
		public readonly Function function;
		public readonly Modifiers modifiers;

		public Parameter(Function function, Modifiers modifiers, string identifier, typesys.Type type)
			: base(identifier, type)
		{
			this.function = function;
			this.modifiers = modifiers;
		}
	}
}
