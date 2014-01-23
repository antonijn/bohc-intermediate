using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.TypeSystem
{
	public class LambdaParam : Variable
	{
		public LambdaParam(string identifier, Type type)
			: base(identifier, type)
		{
		}
	}
}
