using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing.statements;
using bohc.typesys;

namespace bohc.parsing
{
	public interface IExpressionParser
	{
		Expression analyze(string str, IEnumerable<Variable> vars, File file, int opprec, Function ctx);
		Expression analyze(string str, IEnumerable<Variable> vars, File file, Function ctx);
		Expression analyze(string str, IEnumerable<Variable> vars, File file);

		void init(IStatementParser statements);
		IStatementParser getStats();

		int getLambdaStack();
	}
}
