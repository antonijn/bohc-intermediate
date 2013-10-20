using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class CompatibleWithAllType : Type
	{
		public CompatibleWithAllType()
			: base(Package.GLOBAL, Modifiers.FINAL | Modifiers.PUBLIC, "\0")
		{
		}

		public override parsing.Expression defaultVal()
		{
			throw new NotImplementedException();
		}

		public override int extends(Type other)
		{
			return 1;
		}
	}
}
