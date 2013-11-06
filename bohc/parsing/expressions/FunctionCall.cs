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

namespace bohc.parsing
{
	public class FunctionCall : Expression
	{
		public readonly typesys.Function refersto;
		public readonly Expression belongsto;
		public readonly Expression[] parameters;

		public FunctionCall(typesys.Function refersto, Expression belongsto, IEnumerable<Expression> parameters)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
			this.parameters = parameters.ToArray();
		}

		public override typesys.Type getType()
		{
			return refersto.returnType;
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return true;
		}
	}
}
