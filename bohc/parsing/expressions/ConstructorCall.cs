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

namespace Bohc.Parsing
{
	public class ConstructorCall : Expression
	{
		public readonly Bohc.TypeSystem.Constructor function;
		public readonly Expression[] parameters;

		public ConstructorCall(Bohc.TypeSystem.Constructor function, Expression[] parameters)
		{
			this.function = function;
			this.parameters = parameters;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return function.Owner;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("new ").Append(function.Owner.FullName()).Append("(");
			foreach (Expression e in parameters)
			{
				sb.Append(e.ToString()).Append(", ");
			}
			if (parameters.Length > 0)
			{
				sb.Remove(sb.Length - 2, 2);
			}
			sb.Append(")");
			return sb.ToString();
		}
	}
}
