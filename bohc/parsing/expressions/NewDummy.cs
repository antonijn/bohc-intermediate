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
	public sealed class NewDummy : Expression
	{
		public override typesys.Type getType()
		{
			return null;
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			throw new NotImplementedException();
		}

		public override bool isStatement()
		{
			throw new NotImplementedException();
		}
	}
}
