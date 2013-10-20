﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public abstract class BodyStatement : Statement
	{
		public readonly Body body;

		public BodyStatement(Body body)
		{
			this.body = body;
		}
	}
}