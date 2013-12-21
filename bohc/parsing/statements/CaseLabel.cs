using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class CaseLabel : SwitchLabel
	{
		public Expression expression;

		public CaseLabel(Expression expression, Body body)
			: base(body)
		{
			this.expression = expression;
		}
	}
}
