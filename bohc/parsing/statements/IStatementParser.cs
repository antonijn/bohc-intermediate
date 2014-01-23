using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;

namespace Bohc.Parsing.Statements
{
	public interface IStatementParser
	{
		Body parseBody(string body, Function func, File f);
		Body parseBody(string body, Function func, Stack<List<Variable>> vars, File f);
		void init(FileParser parser);
		IFileParser getParser();
		IExpressionParser getExpressions();
	}
}
