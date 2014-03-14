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
	public class ExprVariable : Expression
	{
		public readonly Bohc.TypeSystem.Variable refersto;

		// NOTE:
		// belongsto is null for static variables
		// the class to which it belongs should be sought through "refersto"
		// refersto will be a field in that case, so the container class can be found out
		public readonly Expression belongsto;

		public ExprVariable(Bohc.TypeSystem.Variable refersto, Expression belongsto)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return refersto.Type;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			Bohc.TypeSystem.Field f = refersto as Bohc.TypeSystem.Field;
			if (f != null)
			{
				// type is not modifiable if final, unless in constructor
				return !f.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Final) || 
					(!ctx.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Static) && ctx is Bohc.TypeSystem.Constructor) ||
					(ctx.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Static) && ctx is Bohc.TypeSystem.StaticConstructor);
			}

			Bohc.TypeSystem.Local l = refersto as Bohc.TypeSystem.Local;
			if (l != null)
			{
				return !(l.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Final) && l.Type is Bohc.TypeSystem.Primitive);
			}

			Bohc.TypeSystem.Parameter param = refersto as Bohc.TypeSystem.Parameter;
			if (param != null)
			{
				return !param.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Final);
			}

			return true;
		}

		public override Expression useAsLvalue(BinaryOperation binop)
		{
			TypeSystem.Local l = refersto as TypeSystem.Local;
			if (l != null && binop.operation == BinaryOperation.ASSIGN)
			{
				l.assignedTo.Pop();
				l.assignedTo.Push(true);
			}

			return binop;
		}

		public override void useAsRvalue()
		{
			TypeSystem.Local l = refersto as TypeSystem.Local;
			if (l != null)
			{
				Boh.Exception.require<Exceptions.ParserException>(l.wasAssignedTo(), l.Identifier + " was not yet assigned to");
				++l.usageCount;
			}
		}

		public override bool isStatement()
		{
			return false;
		}

		public override bool shouldCheckNull()
		{
			return refersto.Identifier != "this" && refersto.Identifier != "super";
		}

		public override string ToString()
		{
			if (belongsto != null)
			{
				return belongsto.ToString() + "." + refersto.Identifier;
			}
			return refersto.Identifier;
		}
	}
}
