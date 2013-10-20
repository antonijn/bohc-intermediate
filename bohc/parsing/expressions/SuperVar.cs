using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class SuperVar : ExprVariable
	{
		public SuperVar(typesys.Class c)
			: base(c.super.THIS, null)
		{
		}
	}
}
