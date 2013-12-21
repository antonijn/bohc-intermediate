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
		NONE         = 0x00000000,
		PRIVATE      = 0x00000001,
		PROTECTED    = 0x00000002,
		PUBLIC       = 0x00000004,
		STATIC       = 0x00000008,
		FINAL        = 0x00000010,
		ABSTRACT     = 0x00000020,
		VIRTUAL      = 0x00000040,
		OVERRIDE     = 0x00000080,
		NOCONTEXT    = 0x00000100,
		CVISIBLE     = 0x00000200,
		REF          = 0x00000400,
						   
		PF_WINDOWS   = 0x00000800,
		PF_LINUX     = 0x00001000,
		PF_OSX       = 0x00002000,
		PF_ANDROID   = 0x00004000,
		PF_IOS       = 0x00008000,
		PF_DESKTOP   = 0x00010000,
		PO_WEB       = 0x00020000,

		PF_WINDOWS64 = 0x00040000,
		PF_LINUX64   = 0x00080000,
		PF_OSX64     = 0x00100000,
		PF_DESKTOP64 = 0x00200000,

		PF_WINDOWS32 = 0x00400000,
		PF_LINUX32   = 0x00800000,
		PF_OSX32     = 0x01000000,
		PF_DESKTOP32 = 0x02000000,
	}
}
