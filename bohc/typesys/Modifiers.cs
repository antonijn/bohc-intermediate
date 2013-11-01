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
	[Flags]
	public enum Modifiers
	{
		NONE         = 0x0000,
		PRIVATE      = 0x0001,
		PROTECTED    = 0x0002,
		PUBLIC       = 0x0004,
		STATIC       = 0x0008,
		FINAL        = 0x0010,
		ABSTRACT     = 0x0020,
		VIRTUAL      = 0x0040,
		OVERRIDE     = 0x0080,
		NOCONTEXT    = 0x0100,
		CVISIBLE     = 0x0200,
	}
}
