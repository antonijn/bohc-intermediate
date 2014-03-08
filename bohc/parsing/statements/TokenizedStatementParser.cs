using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;
using Bohc.Exceptions;
using Bohc.Boh;

namespace Bohc.Parsing.Statements
{
	public class TokenizedStatementParser
	{
		private readonly TokenizedExpressionParser expressions = new TokenizedExpressionParser();
		private TokenizedFileParser parser;

		public TokenizedStatementParser(TokenizedFileParser parser)
		{
			this.parser = parser;

			expressions.init(this);
		}

		public TokenizedExpressionParser getEp()
		{
			return expressions;
		}

		public TokenizedFileParser getFp()
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
				result.Statements.Add(parseNext(ref body, func, vars, result, f));
			}

			vars.Pop();
			return result;
		}

		private Statement parseNext(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			Token c = t.get();
			if (c.value == "{")
			{
				t.next();
				TokenStream bod = t.until("}", new Tuple<string, string>("{", "}"));
				return(new Scope(parseBody(bod, func, vars, result, f)));
			}
			else if (c.isType(TokenType.IF_OR_LOOP))
			{
				return(parseIfOrLoop(ref t, func, vars, result, f));
			}
			else if (c.isType(TokenType.CONTROL_FLOW))
			{
				return(parseControlFlow(t, func, vars, result, f));
			}

			Statement stat;
			if (tryParseVarDec(ref t, out stat, func, vars, result, f))
			{
				return(stat);
			}

			TokenStream ts = t.until(";", new Tuple<string, string>("{", "}"));
			t.prior();
			Expression e = expressions.analyze(ts, vars.SelectMany(x => x), func);
			return(new ExpressionStatement(e));
		}

		private bool tryParseVarDec(ref TokenStream tin, out Statement stat, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			TokenStream t = tin.fork();
			Modifiers mf = Modifiers.None;
			while (t.next() && t.get().isType(TokenType.MODIFIER))
			{
				mf |= ModifierHelper.GetModifierFromString(t.get().value);
			}
			if (!t.get().isType(TokenType.IDENTIFIER) && !t.get().isType(TokenType.PRIMITIVE))
			{
				stat = null;
				return false;
			}
			t.prior();
			TokenRange tyr = TokenizedFileParser.readTypeName(t);
			t.next();
			if (!t.get().isType(TokenType.IDENTIFIER)) // if 'identifier clash', it is a vardec
			{
				stat = null;
				return false;
			}
			tin = t;
			TypeSystem.Type ty = TypeSystem.Type.GetExisting(f.getContext(), tyr.str, parser);
			string id = t.get().value;

			Local variable = new Local(id, ty, mf);
			variable.LamdaLevel = expressions.getLambdaStack();
			vars.Peek().Add(variable);
			result.RegisterLocal(variable);

			if (t.next() && t.get().value == ";")
			{
				stat = new VarDeclaration(variable, null);
				return true;
			}
			else if (t.get().value == "=")
			{
				t.next();
				TokenStream initial = t.until(";", new Tuple<string, string>("{", "}"));
				stat = new VarDeclaration(variable, expressions.analyze(initial, vars.SelectMany(x => x), func));
				t.prior();
				return true;
			}

			t.get().error("unexpected token '{0}', expected '=' or ';'", t.get().value);
			throw new System.Exception();
		}

		private Statement parseIfOrLoop(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			switch (t.get().value)
			{
				case "if":
					return parseIf(ref t, func, vars, result, f);
				case "while":
					return parseWhile(ref t, func, vars, result, f);
				case "do":
					return parseDo(ref t, func, vars, result, f);
				case "for":
					return parseFor(ref t, func, vars, result, f);
				case "try":
					return parseTry(ref t, func, vars, result, f);
			}
			throw new NotImplementedException();
		}

		private Statement parseTry(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			t.next();
			Statement body = parseNext(ref t, func, vars, result, f);
			//t.next();

			List<CatchStatement> css = new List<CatchStatement>();
			CatchStatement cs;

			int i;
			for (i = 0; parseCatch(ref t, out cs, func, vars, result, f); ++i)
			{
				css.Add(cs);
			}

			FinallyStatement fs;
			if (parseFinally(ref t, out fs, func, vars, result, f))
			{
				++i;
			}

			return new TryStatement(body, css, fs);
		}

		private bool parseFinally(ref TokenStream t, out FinallyStatement fs, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			if (t.get().value != "finally")
			{
				fs = null;
				return false;
			}

			t.next();
			Statement body = parseNext(ref t, func, vars, result, f);
			fs = new FinallyStatement(body);
			return true;
		}

		private bool parseCatch(ref TokenStream t, out CatchStatement cs, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			if (t.get().value != "catch")
			{
				cs = null;
				return false;
			}
			t.next();

			Modifiers mf = Modifiers.None;
			while (t.next() && t.get().isType(TokenType.MODIFIER))
			{
				mf |= ModifierHelper.GetModifierFromString(t.get().value);
			}
			if (!t.get().isType(TokenType.IDENTIFIER) && !t.get().isType(TokenType.PRIMITIVE))
			{
				t.get().error("expected type name");
			}
			t.prior();
			TokenRange tyr = TokenizedFileParser.readTypeName(t);
			t.next();
			if (!t.get().isType(TokenType.IDENTIFIER)) // if 'identifier clash', it is a vardec
			{
				t.get().error("expected identifier");
			}
			TypeSystem.Type ty = TypeSystem.Type.GetExisting(f.getContext(), tyr.str, parser);
			string id = t.get().value;

			Parameter param = new Parameter(null, mf, id, ty);

			t.next();
			t.next();

			vars.Push(new List<Variable>());
			vars.Peek().Add(param);

			Statement c = parseNext(ref t, func, vars, result, f);
			cs = new CatchStatement(param, c);

			vars.Peek().RemoveAt(vars.Peek().Count - 1);

			return true;
		}

		private Statement parseFor(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			t.next();
			t.next();
			TokenStream initialr = t.until(";", new Tuple<string, string>("{", "}"));
			initialr.next();
			TokenStream condr = t.until(";", new Tuple<string, string>("{", "}"));
			TokenStream finalr = t.until(")", new Tuple<string, string>("{", "}"), new Tuple<string, string>("(", ")"));
			finalr.next();
			Statement initial = parseNext(ref initialr, func, vars, result, f);
			Expression cond = getEp().analyze(condr, vars.SelectMany(x => x), func);
			Statement final = parseNext(ref finalr, func, vars, result, f);
			Statement body = parseNext(ref t, func, vars, result, f);
			return new ForStatement(initial, cond, final, body);
		}

		private Statement parseIf(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			Expression condition;
			Statement body;
			parseIfLike(ref t, func, vars, result, f, out condition, out body);
			if (t.get().value == "else")
			{
				t.next();
				Statement elseBod = parseNext(ref t, func, vars, result, f);
				return new IfStatement(condition, body, new ElseStatement(elseBod));
			}
			return new IfStatement(condition, body, null);
		}

		private Statement parseDo(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			t.next();
			Statement bod = parseNext(ref t, func, vars, result, f);
			if (t.get().value != "while")
			{
				t.get().error("expected 'while' after 'do' block");
			}
			t.next();
			t.next();
			TokenStream c = t.until(")", new Tuple<string, string>("(", ")"));
			Expression condition = getEp().analyze(c, vars.SelectMany(x => x), func);
			return new DoWhileStatement(condition, bod);
		}

		private Statement parseWhile(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			Expression condition;
			Statement body;
			parseIfLike(ref t, func, vars, result, f, out condition, out body);
			return new WhileStatement(condition, body);
		}

		private void parseIfLike(ref TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f, out Expression condition, out Statement body)
		{
			t.next();
			t.next();
			TokenStream c = t.until(")", new Tuple<string, string>("(", ")"));
			condition = getEp().analyze(c, vars.SelectMany(x => x), func);
			body = parseNext(ref t, func, vars, result, f);
		}

		private Statement parseControlFlow(TokenStream t, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			throw new NotImplementedException();
		}
	}
}

