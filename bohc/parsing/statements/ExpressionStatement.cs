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
	public sealed class ExpressionStatement : Statement
	{
		public readonly Expression expression;

		public ExpressionStatement(Expression expression)
		{
			Boh.Exception.require<Exceptions.ParserException>(expression.isStatement(), "Expression is not a statement");

			this.expression = expression;
		}
	}
}
