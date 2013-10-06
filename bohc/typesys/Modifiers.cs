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
	}
}
