using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class DefaultLabel : SwitchLabel
	{
		public DefaultLabel(Body body)
			: base(body)
		{
		}
	}
}
