﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class ElseStatement : BodyStatement
	{
		public ElseStatement(Body body)
			: base(body)
		{
		}
	}
}
