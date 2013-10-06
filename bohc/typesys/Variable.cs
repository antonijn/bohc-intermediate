using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Variable
	{
		public readonly string identifier;
		public readonly typesys.Type type;

		public Variable(string identifier, typesys.Type type)
		{
			this.identifier = identifier;
			this.type = type;
		}
	}
}
