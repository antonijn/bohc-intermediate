using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class DefaultLabel : SwitchLabel
	{
		public DefaultLabel(Body body)
			: base(body)
		{
		}
	}
}
