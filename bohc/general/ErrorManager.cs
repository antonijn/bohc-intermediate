using System;

namespace Bohc.General
{
	public class ErrorManager
	{
		public int errors;
		public int warnings;

		public readonly Project p;

		public ErrorManager(Project p)
		{
			this.p = p;
			p.emanager = this;
		}
	}
}
