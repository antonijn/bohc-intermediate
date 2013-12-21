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
	public class NativeType : Type
	{
		public readonly string crep;

		public NativeType(string str)
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, "native." + str)
		{
			this.crep = str;
		}

		public override parsing.Expression defaultVal()
		{
			if (crep.Contains('*'))
			{
				return NullType.NULL.defaultVal();
			}

			return new parsing.Literal(this, "0");
		}
	}
}
