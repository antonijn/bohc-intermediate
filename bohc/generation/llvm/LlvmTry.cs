using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmTry
	{
		public LlvmLabel landingpad;

		public LlvmTry(LlvmLabel landingpads)
		{
			this.landingpad = landingpad;
		}
	}
}

