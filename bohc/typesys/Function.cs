using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Function : IFunction
	{
		public readonly typesys.Class owner;
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

		public Function(typesys.Class owner, Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters, string bodystr)
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
