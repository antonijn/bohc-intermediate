using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.typesys;

namespace bohc.parsing.ts
{
	public class File
	{
		public readonly List<Package> imports = new List<Package>();
		public readonly Package package;

		public typesys.Type type;

		public File(List<Package> imports, Package package)
		{
			this.imports = imports;
			this.package = package;
		}
	}
}
