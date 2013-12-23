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
	public class CompatibleWithAllType : Type
	{
		public CompatibleWithAllType()
			: base(Package.GLOBAL, Modifiers.FINAL | Modifiers.PUBLIC, "\0")
		{
		}

		public override parsing.Expression defaultVal()
		{
			throw new NotImplementedException();
		}

		public override int extends(Type other)
		{
			return 1;
		}
	}
}
