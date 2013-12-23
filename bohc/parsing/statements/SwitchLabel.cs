using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class SwitchLabel
	{
		public Body body;

		public SwitchLabel(Body body)
		{
			this.body = body;
		}
	}
}
