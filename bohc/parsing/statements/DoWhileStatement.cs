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

namespace Bohc.Parsing.Statements
{
	public sealed class DoWhileStatement : BodyStatement
	{
		public readonly Expression condition;

		public DoWhileStatement(Expression condition, Body body)
			: base(body)
		{
			Boh.Exception.require<Exceptions.ParserException>(condition.getType() == Bohc.TypeSystem.Primitive.Boolean, "Condition must be boolean");

			this.condition = condition;
		}
	}
}
