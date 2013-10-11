using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public sealed class Local : Variable
	{
		public Local(string identifier, Type type)
			: base(identifier, type)
		{
		}
	}
}
