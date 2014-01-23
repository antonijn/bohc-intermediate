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

namespace Bohc.TypeSystem
{
	public class Field : Variable, IMember
	{
		public Parsing.Expression Initial = null;

		public readonly Modifiers Modifiers;
		public readonly Class Owner;
		public readonly string InitValStr;

		Modifiers IMember.GetModifiers()
		{
			return Modifiers;
		}

		string IMember.GetName()
		{
			return Identifier;
		}

		public Field(Modifiers modifiers, string identifier, Bohc.TypeSystem.Type type, Class owner, string initvalstr)
			: base(identifier, type)
		{
			this.Modifiers = modifiers;
			this.Owner = owner;
			this.InitValStr = initvalstr;
		}
	}
}
