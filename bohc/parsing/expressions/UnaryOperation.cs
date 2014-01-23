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
	public class UnaryOperation : Expression
	{
		public static readonly int[] UNARY_PRECENDENCES = new int[] { 7, 8 };

		public static readonly Operator PLUS = new Operator("+", 7, "plus");
		public static readonly Operator MINUS = new Operator("-", 7, "minus");
		public static readonly Operator INCREMENT = new Operator("++", 7, "inc");
		public static readonly Operator INCREMENT_POST = new Operator("++", 7, "inc_post");
		public static readonly Operator DECREMENT = new Operator("--", 7, "dec");
		public static readonly Operator DECREMENT_POST = new Operator("--", 7, "dec_post");
		public static readonly Operator INVERT = new Operator("!", 7, "inv");
		public static readonly Operator NOT = new Operator("~", 7, "not");
		public static readonly Operator TYPEOF = new Operator("typeof", 8, "typeof");
		public static readonly Operator DEFAULT = new Operator("default", 8, "default");

		public readonly Expression onwhat;
		public readonly Operator operation;

		public UnaryOperation(Expression onwhat, Operator operation)
		{
			this.onwhat = onwhat;
			this.operation = operation;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			if (operation == TYPEOF)
			{
				return Bohc.TypeSystem.StdType.Type;
			}
			else if (operation == DEFAULT)
			{
				return (onwhat as ExprType).type;
			}
			return onwhat.getType();
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return operation == INCREMENT_POST ||
			       operation == INCREMENT ||
				   operation == DECREMENT_POST ||
				   operation == DECREMENT;
		}
	}
}
