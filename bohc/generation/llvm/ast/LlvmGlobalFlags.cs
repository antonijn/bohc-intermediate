using System;

namespace Bohc.Generation.Llvm
{
	[Flags]
	public enum LlvmGlobalFlags
	{
		Thread_Local = 0x01,
		Unnamed_addr = 0x02,
		Constant = 0x04,
		Global = 0x08,
	}
}

