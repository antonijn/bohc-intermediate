using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;
using Bohc.Exceptions;
using Bohc.Boh;

namespace Bohc.Parsing.Statements
{
	public class TokenizedStatementParser : IStatementParser
	{
		private readonly IExpressionParser expressions;
		private FileParser parser;

		public TokenizedStatementParser(IExpressionParser expressions)
		{
			this.expressions = expressions;
			this.expressions.init(this);
		}

		public IExpressionParser getExpressions()
		{
			return expressions;
		}

		public void init(FileParser parser)
		{
			this.parser = parser;
		}

		public IFileParser getParser()
		{
			return parser;
		}

		public Body parseBody(object _body, Function func, Body parent, File f)
		{
			TokenStream body = (TokenStream)_body;
			Stack<List<Variable>> vars = new Stack<List<Variable>>();
			// TODO: remove this if it doesn't work?
			if (func != null)
			{
				vars.Push(func.Parameters.ToList<Bohc.TypeSystem.Variable>());

				if (!func.Modifiers.HasFlag(Modifiers.Static))
				{
					vars.Peek().Add(((TypeSystem.Class)func.Owner).This);
					vars.Peek().Add(((TypeSystem.Class)func.Owner).SuperVar);
				}

				Indexer idxer = func as Indexer;
				if (idxer != null && idxer.IsAssignment())
				{
					vars.Peek().Add(idxer.Assignment);
				}
			}
			return parseBody(body, func, vars, parent, f);
		}

		public Body parseBody(object _body, Function func, Stack<List<Variable>> vars, Body parent, File f)
		{
			TokenStream body = (TokenStream)_body;
			Body result = new Body(func.Body);
			vars.Push(new List<Variable>());

			while (body.next())
			{
				parseLine(body, func, vars, result, f);
			}

			vars.Pop();
			return result;
		}

		private void parseLine(TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			Token c = t.get();
			if (c.value == "{")
			{
				TokenStream bod = t.until("}", new Tuple<string, string>("{", "}"));
				result.Statements.Add(new Scope(parseBody(bod, func, vars, result, f)));
			}
		}
	}
}

