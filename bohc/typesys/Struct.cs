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
	public class Struct : Class
	{
		public Struct(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.ConstructorCall(Constructors.Single(x => x.Parameters.Count == 0), new Parsing.Expression[] { });
		}

		public override bool IsReferenceType()
		{
			return false;
		}
	}
}
