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

namespace bohc.typesys
{
	public class Function : IFunction
	{
		public readonly typesys.Type owner;
		public readonly Modifiers modifiers;
		public readonly typesys.Type returnType;
		public readonly string identifier;
		public readonly List<Parameter> parameters;

		public readonly string bodystr;
		public parsing.statements.Body body;

		Modifiers IMember.getModifiers()
		{
			return modifiers;
		}

		string IMember.getName()
		{
			return identifier;
		}

		public Function(typesys.Type owner, Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters, string bodystr)
		{
			this.owner = owner;
			this.modifiers = modifiers;
			this.returnType = returnType;
			this.identifier = identifier;
			this.parameters = parameters;
			this.bodystr = bodystr;
		}
	}
}
