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
	public class Literal : Expression
	{
		public readonly typesys.Type type;
		public readonly string representation;

		public Literal(typesys.Type type, string representation)
		{
			this.type = type;
			this.representation = representation;
		}

		public override typesys.Type getType()
		{
			return type;
		}

		public override bool isLvalue()
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
