using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

using bohc.typesys;

namespace bohc.general
{
	public interface IParserStrategy
	{
		void parse(Project p);
		ParserState getpstate();
	}
}

