using System;

namespace Bohc.Generation.Llvm
{
	[Flags]
	public enum LlvmGlobalFlags
	{
		Unnamed_addr = 0x01,
		Constant = 0x02,
		Global = 0x04,
	}
}

