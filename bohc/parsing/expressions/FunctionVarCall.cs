﻿// Copyright (c) 2013 Antonie Blom
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
	public class FunctionVarCall : Expression
	{
		public readonly Expression belongsto;
		public readonly Expression[] parameters;

		public FunctionVarCall(Expression belongsto, IEnumerable<Expression> parameters)
		{
			this.belongsto = belongsto;
			this.parameters = parameters.ToArray();
		}

		public override typesys.Type getType()
		{
			return ((typesys.FunctionRefType)belongsto.getType()).retType;
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