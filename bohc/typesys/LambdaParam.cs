using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class LambdaParam : Variable
	{
		public LambdaParam(string identifier, Type type)
			: base(identifier, type)
		{
		}
	}
}
