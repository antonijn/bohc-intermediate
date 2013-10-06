using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Function : IMember
	{
		public readonly Modifiers modifiers;
		public readonly typesys.Type returnType;
		public readonly string identifier;
		public readonly List<Parameter> parameters;

		Modifiers IMember.getModifiers()
		{
			return modifiers;
		}

		string IMember.getName()
		{
			return identifier;
		}

		public Function(Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters)
		{
			this.modifiers = modifiers;
			this.returnType = returnType;
			this.identifier = identifier;
			this.parameters = parameters;
		}
	}
}
