using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Parsing.Statements;
using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public interface IExpressionParser
	{
		Expression analyze(string str, IEnumerable<Variable> vars, File file, int opprec, Function ctx);
		Expression analyze(string str, IEnumerable<Variable> vars, File file, Function ctx);
		Expression analyze(string str, IEnumerable<Variable> vars, File file);

		void init(IStatementParser Statements);
		IStatementParser getStats();

		int getLambdaStack();
	}
}
