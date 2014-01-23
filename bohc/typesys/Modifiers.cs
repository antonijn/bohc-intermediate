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
	[Flags]
	public enum Modifiers
	{
		None         = 0x00000000,
		Private      = 0x00000001,
		Protected    = 0x00000002,
		Public       = 0x00000004,
		Static       = 0x00000008,
		Final        = 0x00000010,
		Abstract     = 0x00000020,
		Virtual      = 0x00000040,
		Override     = 0x00000080,
		NoContext    = 0x00000100,
		CVisible     = 0x00000200,
		Ref          = 0x00000400,
						   
		Pf_Windows   = 0x00000800,
		Pf_Linux     = 0x00001000,
		Pf_Osx       = 0x00002000,
		Pf_Android   = 0x00004000,
		Pf_Ios       = 0x00008000,
		Pf_Desktop   = 0x00010000,
		Pf_Web       = 0x00020000,

		Pf_Windows64 = 0x00040000,
		Pf_Linux64   = 0x00080000,
		Pf_Osx64     = 0x00100000,
		Pf_Desktop64 = 0x00200000,

		Pf_Windows32 = 0x00400000,
		Pf_Linux32   = 0x00800000,
		Pf_Osx32     = 0x01000000,
		Pf_Desktop32 = 0x02000000,

		Pf_Clr       = 0x04000000,
		Pf_Jvm       = 0x08000000,

		Native       = 0x10000000,
	}
}
