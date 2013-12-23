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
	public sealed class ForStatement : BodyStatement
	{
		public readonly Statement initial;
		public readonly Expression condition;
		public readonly Statement final;

		public ForStatement(Statement initial, Expression condition, Statement final, Body body)
			: base(body)
		{
			if (condition == null)
			{
				condition = new Literal(typesys.Primitive.BOOLEAN, "true");
			}
			boh.Exception.require<exceptions.ParserException>(condition.getType() == typesys.Primitive.BOOLEAN, "Condition must be boolean");

			this.initial = initial;
			this.condition = condition;
			this.final = final;
		}
	}
}
