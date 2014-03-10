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

using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public class File
	{
		public readonly List<Package> imports = new List<Package>();
		public readonly Package package;
		public string filename;
		public readonly object parserinfo;
		public bool ignore = false;

		public ParserState state;

		public IEnumerable<Package> getContext()
		{
			return imports.Concat(new[] { package });
		}

		public IType type;

		public File(List<Package> imports, Package package, object content)
		{
			this.imports = imports;
			this.package = package;
			this.parserinfo = content;
		}

		public override string ToString()
		{
			return type.ToString();
		}
	}
}
