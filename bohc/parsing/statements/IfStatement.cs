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
	public sealed class IfStatement : BodyStatement
	{
		public readonly Expression condition;
		public readonly ElseStatement elsestat;

		public IfStatement(Expression condition, Body body, ElseStatement elsestat)
			: this(condition, new Scope(body), elsestat) { }
		public IfStatement(Expression condition, Statement body, ElseStatement elsestat)
			: base(body)
		{
			Boh.Exception.require<Exceptions.ParserException>(condition.getType() == Bohc.TypeSystem.Primitive.Boolean, "Condition must be boolean");

			this.condition = condition;
			this.elsestat = elsestat;
		}

		public override bool hasReturned()
		{
			return body.hasReturned() && (elsestat == null || elsestat.body.hasReturned());
		}

		public override bool hasSuperBeenCalled()
		{
			return body.hasSuperBeenCalled() && (elsestat == null || elsestat.body.hasSuperBeenCalled());
		}
	}
}
