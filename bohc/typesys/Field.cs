using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Field : Variable, IMember
	{
		public readonly Modifiers modifiers;

		Modifiers IMember.getModifiers()
		{
			return modifiers;
		}

		string IMember.getName()
		{
			return identifier;
		}

		public Field(Modifiers modifiers, string identifier, typesys.Type type)
			: base(identifier, type)
		{
			this.modifiers = modifiers;
		}
	}
}
