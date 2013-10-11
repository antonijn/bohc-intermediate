using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ThisVar : ExprVariable
	{
		public ThisVar(typesys.Class c)
			: base(c.THIS, null)
		{
		}
	}
}
