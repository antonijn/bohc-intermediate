using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.TypeSystem
{
	public class GenericFunction : IFunction
	{
		public string Identifier;
		public Modifiers Modifiers;
		public string[] TypeNames;
		public object ParseInfo;
		public Class Owner;

		Modifiers IMember.GetModifiers()
		{
			return Modifiers;
		}

		string IMember.GetName()
		{
			return Identifier;
		}

		public GenericFunction(string id, Modifiers mods, string[] typenames, object parseinfo, Class owner)
		{
			Identifier = id;
			Modifiers = mods;
			TypeNames = typenames;
			ParseInfo = parseinfo;
			Owner = owner;
		}
	}
}
