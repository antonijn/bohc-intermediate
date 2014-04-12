using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;

namespace Bohc.Parsing.Statements
{
	public interface IStatementParser
	{
		Body parseBody(object body, Function func, Body parent, File f);
		Body parseBody(object body, Function func, Stack<List<Variable>> vars, Body parent, File f);
		void init(IFileParser parser);
		IFileParser getParser();
		IExpressionParser getExpressions();
	}
}
