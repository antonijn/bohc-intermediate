using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Field : Variable, IMember
	{
		public parsing.Expression initial = null;

		public readonly Modifiers modifiers;
		public readonly Class owner;
		public readonly string initvalstr;

		Modifiers IMember.getModifiers()
		{
			return modifiers;
		}

		string IMember.getName()
		{
			return identifier;
		}

		public Field(Modifiers modifiers, string identifier, typesys.Type type, Class owner, string initvalstr)
			: base(identifier, type)
		{
			this.modifiers = modifiers;
			this.owner = owner;
			this.initvalstr = initvalstr;
		}
	}
}
