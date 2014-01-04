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
	public class ExprVariable : Expression
	{
		public readonly typesys.Variable refersto;

		// NOTE:
		// belongsto is null for static variables
		// the class to which it belongs should be sought through "refersto"
		// refersto will be a field in that case, so the container class can be found out
		public readonly Expression belongsto;

		public ExprVariable(typesys.Variable refersto, Expression belongsto)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
		}

		public override typesys.Type getType()
		{
			return refersto.type;
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			typesys.Field f = refersto as typesys.Field;
			if (f != null)
			{
				// type is not modifiable if final, unless in constructor
				return !f.modifiers.HasFlag(typesys.Modifiers.FINAL) || 
					(!ctx.modifiers.HasFlag(typesys.Modifiers.STATIC) && ctx is typesys.Constructor) ||
					(ctx.modifiers.HasFlag(typesys.Modifiers.STATIC) && ctx is typesys.StaticConstructor);
			}

			typesys.Local l = refersto as typesys.Local;
			if (l != null)
			{
				return !(l.modifiers.HasFlag(typesys.Modifiers.FINAL) && l.type is typesys.Primitive);
			}

			typesys.Parameter param = refersto as typesys.Parameter;
			if (param != null)
			{
				return !param.modifiers.HasFlag(typesys.Modifiers.FINAL);
			}

			return true;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
