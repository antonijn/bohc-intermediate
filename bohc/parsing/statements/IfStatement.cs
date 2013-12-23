// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class IfStatement : BodyStatement
	{
		public readonly Expression condition;
		public readonly ElseStatement elsestat;

		public IfStatement(Expression condition, Body body, ElseStatement elsestat)
			: base(body)
		{
			boh.Exception.require<exceptions.ParserException>(condition.getType() == typesys.Primitive.BOOLEAN, "Condition must be boolean");

			this.condition = condition;
			this.elsestat = elsestat;
		}
	}
}
