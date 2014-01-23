using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using Bohc.Generation;
using Bohc.Generation.C;
using Bohc.Generation.Mangling;

using Bohc.Parsing;
using Bohc.Parsing.Statements;

using Bohc.TypeSystem;

namespace Bohc.General
{
	public interface IParserStrategy
	{
		void parse(Project p);
		ParserState getpstate();
	}
}

