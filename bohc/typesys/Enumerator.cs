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
	public sealed class Enumerator
	{
		public readonly string Name;
		public readonly Enum EnumType;

		public Enumerator(string name, Enum enumType)
		{
			this.Name = name;
			this.EnumType = enumType;
		}
	}
}
