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
		public readonly string content;

		public IEnumerable<Package> getContext()
		{
			return imports.Concat(new[] { package });
		}

		public IType type;

		public File(List<Package> imports, Package package, string content)
		{
			this.imports = imports;
			this.package = package;
			this.content = content;
		}
	}
}
