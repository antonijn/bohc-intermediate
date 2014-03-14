using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;
using Bohc.Exceptions;
using Bohc.Boh;

namespace Bohc.Parsing.Statements
{
	public class DefaultStatementParser : IStatementParser
	{
		private readonly IExpressionParser expressions;
		private FileParser parser;

		public DefaultStatementParser(IExpressionParser expressions)
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
			string body = (string)_body;
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

		public Body parseBodyTillCase(ref string body, Function func, Stack<List<Variable>> vars, File f)
		{
			Body result = new Body(func.Body);
			vars.Push(new List<Variable>());

			while (true)
			{
				body = body.TrimStart();
				if (string.IsNullOrWhiteSpace(body))
				{
					break;
				}
				if (body.StartsWith("case ") || body.StartsWith("default ") || body.StartsWith("default:"))
				{
					break;
				}
				parseLine(ref body, func, vars, result, f);
			}

			vars.Pop();
			return result;
		}

		public Body parseBody(object _body, Function func, Stack<List<Variable>> vars, Body parent, File f)
		{
			string body = (string)_body;
			Body result = new Body(parent);
			vars.Push(new List<Variable>());

			while (true)
			{
				body = body.TrimStart();
				if (string.IsNullOrWhiteSpace(body))
				{
					break;
				}
				parseLine(ref body, func, vars, result, f);
			}

			vars.Pop();
			return result;
		}

		private bool startsWithKW(string line, string word)
		{
			return line.StartsWith(word) && (line.Length == word.Length || !char.IsLetterOrDigit(line[word.Length]));
		}

		private void parseLine(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			if (line.StartsWith("{"))
			{
				int closing = ParserTools.getMatchingBraceChar(line, 0, '}');
				string bod = line.Substring(1, closing - 1);
				result.Statements.Add(new Scope(parseBody(bod, func, vars, result, f)));
				line = line.Substring(closing + 1).TrimStart();
			}
			else if (startsWithKW(line, "if"))
			{
				result.Statements.Add(parseIf(ref line, func, vars, result, f));
			}
			else if (startsWithKW(line, "while"))
			{
				result.Statements.Add(parseWhile(ref line, func, vars, result, f));
			}
			else if (startsWithKW(line, "for"))
			{
				result.Statements.Add(parseFor(ref line, func, vars, result, f));
			}
			else if (startsWithKW(line, "do"))
			{
				result.Statements.Add(parseDoWhile(ref line, func, vars, result, f));
			}
			else if (startsWithKW(line, "try"))
			{
				result.Statements.Add(parseTry(ref line, func, vars, result, f));
			}
			else if (startsWithKW(line, "switch"))
			{
				result.Statements.Add(parseSwitch(ref line, func, vars, f));
			}
			else if (startsWithKW(line, "foreach"))
			{
				result.Statements.Add(parseForeach(ref line, func, vars, result, f));
			}
			else
			{
				// FIXME: void() f = void () -> { doSth(); };
				int semicol1 = ParserTools.indexOf(line, '(', ')', ';');
				int semicol2 = ParserTools.indexOf(line, '{', '}', ';');
				int semicol = Math.Max(semicol1, semicol2);
				string stat = line.Substring(0, semicol);
				result.Statements.Add(parseStatement(stat, func, vars, result, f));
				line = line.Substring(semicol + 1).TrimStart();
			}
		}

		private Statement parseForeach(ref string line, Function func, Stack<List<Variable>> vars, Body b, File f)
		{
			line = line.Substring("foreach".Length).TrimStart();
			
			Boh.Exception.require<ParserException>(line.StartsWith("("), "foreach must be followed by an open bracket");

			vars.Push(new List<Variable>());

			string betw = getBrackets(line);
			int idxSemi = betw.IndexOf(';');
			Boh.Exception.require<ParserException>(idxSemi != -1, "Invalid foreach statement");
			string b4semi = betw.Substring(0, idxSemi).Trim();
			string after = betw.Substring(idxSemi + 1);

			VarDeclaration vardec = parseVarDec(b4semi, func, vars, f, b4semi.LastIndexOf(' '), b4semi.Substring(0, b4semi.LastIndexOf(' ')), b, Modifiers.None);
			Expression expr = expressions.analyze(after, vars.SelectMany(x => x), f, func);

			Boh.Exception.require<ParserException>(
				expr.getType().Extends(StdType.ICollection.GetTypeFor(new[] { vardec.refersto.Type }, parser)) > 0, "foreach expression type must match variable type");

			line = line.Substring(betw.Length + 2).TrimStart();

			string bodyStr = getCurlies(line);
			line = line.Substring(bodyStr.Length + 2).TrimStart();
			Body body = parseBody(bodyStr, func, vars, b, f);

			vars.Pop();

			return new ForeachStatement(vardec, expr, body);
		}

		private SwitchStatement parseSwitch(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("switch".Length).TrimStart();

			Boh.Exception.require<ParserException>(line.StartsWith("("), "Expression must be placed between parentheses");

			string condstr = getBrackets(line);
			Expression expr = expressions.analyze(condstr, vars.SelectMany(x => x), f, func);
			line = line.Substring(condstr.Length + 2).TrimStart();

			Boh.Exception.require<ParserException>(line.StartsWith("{"), "Switch block must use curly brackets");

			return parseSwitchBody(ref line, func, vars, f, expr);
		}

		private SwitchStatement parseSwitchBody(ref string line, Function func, Stack<List<Variable>> vars, File f, Expression expr)
		{
			string bodyStr = getCurlies(line);
			line = line.Substring(bodyStr.Length + 2);
			bodyStr = bodyStr.TrimStart();

			List<SwitchLabel> labels = new List<SwitchLabel>();

			while (!string.IsNullOrWhiteSpace(bodyStr))
			{
				if (bodyStr.StartsWith("case "))
				{
					bodyStr = bodyStr.Substring("case ".Length).TrimStart();
					int idxColon = ParserTools.indexOf(bodyStr, new[] { '(', '{' }, new[] { ')', '}' }, ':');
					string exprStr = bodyStr.Substring(0, idxColon);
					Expression caseExpr = expressions.analyze(exprStr, vars.SelectMany(x => x), f, func);
					Boh.Exception.require<ParserException>(caseExpr.getType().Extends(expr.getType()) != 0, "case label expression type must extend base switch expression");
					bodyStr = bodyStr.Substring(idxColon + 1);
					Body body = parseBodyTillCase(ref bodyStr, func, vars, f);
					labels.Add(new CaseLabel(caseExpr, body));
				}
				else if (bodyStr.StartsWith("default ") || bodyStr.StartsWith("default:"))
				{
					int idxColon = bodyStr.IndexOf(':');
					bodyStr = bodyStr.Substring(idxColon + 1);
					Body body = parseBodyTillCase(ref bodyStr, func, vars, f);
					labels.Add(new DefaultLabel(body));
				}
			}

			return new SwitchStatement(expr, labels);
		}

		private bool parseFullVarDec(string line, Function func, Stack<List<Variable>> vars, File f, Modifiers modifiers, Body body, out Statement s)
		{
			int wspace = Math.Max(ParserTools.indexOf(line, '<', '>', ' '), ParserTools.indexOf(line, '(', ')', ' '));
			if (wspace != -1)
			{
				string beforeWspace = line.Substring(0, wspace);
				if (Bohc.TypeSystem.Type.Exists(f.getContext(), beforeWspace, parser))
				{
					s = parseVarDec(line, func, vars, f, wspace, beforeWspace, body, modifiers);
					return true;
				}
				else if (beforeWspace == "final")
				{
					return parseFullVarDec(line.Substring("final".Length).TrimStart(), func, vars, f, modifiers | Modifiers.Final, body, out s);
				}
				else if (beforeWspace == "static")
				{
					return parseFullVarDec(line.Substring("static".Length).TrimStart(), func, vars, f, modifiers | Modifiers.Static, body, out s);
				}
			}
			s = null;
			return false;
		}

		private Statement parseStatement(string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			Statement s;
			if (parseFullVarDec(line, func, vars, f, Modifiers.None, result, out s))
			{
				return s;
			}

			if (startsWithKW(line, "return"))
			{
				return parseReturn(line, func, vars, f);
			}
			else if (startsWithKW(line, "throw"))
			{
				return parseThrow(line, func, vars, f);
			}
			else if (func is Constructor && line.Replace(" ", string.Empty).StartsWith("this("))
			{
				int i = line.IndexOf("(") + 1;
				IEnumerable<Expression> parameters;
				Bohc.TypeSystem.Function constr = Function.GetCompatibleFunction(ref i, "this", line, f, vars.SelectMany(x => x), ((Class)func.Owner).Constructors, out parameters, func, expressions, ')');
				line = line.Substring(i);
				return new ExpressionStatement(new FunctionCall(constr, ((Class)func.Owner).ThisVarExpr, parameters));
			}
			else if (func is Constructor && line.StartsWith("super("))
			{
				int i = line.IndexOf("(") + 1;
				IEnumerable<Expression> parameters;
				Bohc.TypeSystem.Function constr = Function.GetCompatibleFunction(ref i, "this", line, f, vars.SelectMany(x => x), ((Class)func.Owner).Super.Constructors, out parameters, func, expressions, ')');
				line = line.Substring(i);
				return new ExpressionStatement(new FunctionCall(constr, ((Class)func.Owner).ThisVarExpr, parameters));
			}
			else if (startsWithKW(line, "break"))
			{
				// TODO: make sure that in loop
				return new BreakStatement();
			}
			else if (startsWithKW(line, "continue"))
			{
				// TODO: make sure that in loop
				return new ContinueStatement();
			}

			Expression expr = expressions.analyze(line, vars.SelectMany(x => x), f, func);
			if (expr != null)
			{
				return new ExpressionStatement(expr);
			}

			return new EmptyStatement();
		}

		private VarDeclaration parseVarDec(string line, Function ctx, Stack<List<Variable>> vars, File f, int wspace, string beforeWspace, Body body, Modifiers modifiers)
		{
			Bohc.TypeSystem.Type type = Bohc.TypeSystem.Type.GetExisting(f.getContext(), beforeWspace, parser);
			string id = null;

			Expression initial = null;

			int eq = ParserTools.indexOf(line, '(', ')', '=');
			if (eq != -1)
			{
				id = line.Substring(wspace + 1, eq - wspace - 1).Trim();
				Boh.Exception.require<ParserException>(Bohc.TypeSystem.Type.IsValidIdentifier(id), id + ": is not a valid identifier");
				string initstr = line.Substring(eq + 1);
				initial = expressions.analyze(initstr, vars.SelectMany(x => x), f);
			}
			else
			{
				id = line.Substring(wspace + 1).Trim();
			}

			Boh.Exception.require<ParserException>(!modifiers.HasFlag(Modifiers.Final) || !(type is Primitive) || initial != null, "final local primitive must be initialized upon declaration");

			Local variable = new Local(id, type, modifiers);
			variable.LamdaLevel = expressions.getLambdaStack();
			vars.Peek().Add(variable);
			body.RegisterLocal(variable);

			return new VarDeclaration(variable, initial);
		}

		private ThrowStatement parseThrow(string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("throw ".Length);
			Expression thr = expressions.analyze(line, vars.SelectMany(x => x), f, func);
			Boh.Exception.require<ParserException>(thr.getType().Extends(Bohc.TypeSystem.Type.GetExisting(StdType.BohLang, "Exception", parser)) != 0, "Expression after throw must be an exception");
			return new ThrowStatement(thr);
		}

		private ReturnStatement parseReturn(string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("return".Length);
			Expression ret = expressions.analyze(line, vars.SelectMany(x => x), f, func);
			Boh.Exception.require<ParserException>(ret == null || ret.getType().Extends(func.ReturnType) != 0, "Return statement incompatible with function return type");
			return new ReturnStatement(ret);
		}

		private void parseStatBody(ref string line, Function func, Stack<List<Variable>> vars, File f, Body result, out Body body)
		{
			body = null;
			if (line.StartsWith("{"))
			{
				string curlies = getCurlies(line);
				body = parseBody(curlies, func, vars, result, f);
				line = line.TrimStart().Substring(curlies.Length + 2);
			}
			else
			{
				body = new Body(func.Body);
				parseLine(ref line, func, vars, body, f);
			}
		}

		private void parseIfLike(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f, out Expression condition, out Body body)
		{
			Boh.Exception.require<ParserException>(line.StartsWith("("), "Condition must be placed between parentheses");

			string condstr = getBrackets(line);
			condition = expressions.analyze(condstr, vars.SelectMany(x => x), f, func);
			line = line.Substring(condstr.Length + 2).TrimStart();

			parseStatBody(ref line, func, vars, f, result, out body);
		}

		private TryStatement parseTry(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("try".Length).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(curlies, func, vars, result, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			List<CatchStatement> catches = new List<CatchStatement>();
			while (line.StartsWith("catch"))
			{
				catches.Add(parseCatch(ref line, func, vars, result, f));
			}

			FinallyStatement fin = null;
			if (line.StartsWith("finally"))
			{
				line = line.Substring("finally".Length).TrimStart();
				curlies = getCurlies(line);
				line = line.Substring(curlies.Length + 2).TrimStart();
				fin = new FinallyStatement(parseBody(curlies, func, result, f));
			}

			return new TryStatement(body, catches, fin);
		}

		private CatchStatement parseCatch(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("catch".Length).TrimStart();

			string bracks = getBrackets(line);
			string[] parts = bracks.Split(' ');

			Boh.Exception.require<ParserException>(parts.Length == 2, "Invalid parameter for catch block");

			string typestr = parts[0];
			string idstr = parts[1];

			Bohc.TypeSystem.Type type = Bohc.TypeSystem.Type.GetExisting(f.getContext(), typestr, parser);

			Boh.Exception.require<ParserException>(Bohc.TypeSystem.Type.IsValidIdentifier(idstr), idstr + " is not a valid identifier");

			Parameter param = new Parameter(func, Modifiers.None, idstr, type);
			vars.Push(new List<Variable>());
			vars.Peek().Add(param);

			line = line.Substring(bracks.Length + 2).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(curlies, func, vars, result, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			vars.Pop();

			return new CatchStatement(param, body);
		}

		private IfStatement parseIf(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("if".Length).TrimStart();

			Expression condition;
			Body body;
			parseIfLike(ref line, func, vars, result, f, out condition, out body);

			line = line.TrimStart();
			if (line.StartsWith("else"))
			{
				return new IfStatement(condition, body, parseElse(ref line, func, vars, result, f));
			}

			return new IfStatement(condition, body, null);
		}

		private ElseStatement parseElse(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("else".Length).TrimStart();
			Body body;
			parseStatBody(ref line, func, vars, f, result, out body);

			return new ElseStatement(body);
		}

		private WhileStatement parseWhile(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("while".Length).TrimStart();

			Expression condition;
			Body body;
			parseIfLike(ref line, func, vars, result, f, out condition, out body);

			return new WhileStatement(condition, body);
		}

		private ForStatement parseFor(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			vars.Push(new List<Variable>());

			line = line.Substring("for".Length).TrimStart();

			string betwBracks = "(" + getBrackets(line) + ")";
			IEnumerable<string> strs = ParserTools.split(betwBracks, 0, ')', ';');

			Boh.Exception.require<ParserException>(strs.Count() == 3, "For loops require three parts between brackets");

			string[] strparts = strs.ToArray();

			Statement initial = parseStatement(strparts[0], func, vars, result, f);
			Expression condition = expressions.analyze(strparts[1], vars.SelectMany(x => x), f);
			Statement post = parseStatement(strparts[2], func, vars, result, f);

			line = line.Substring(betwBracks.Length).TrimStart();

			Body body;
			parseStatBody(ref line, func, vars, f, result, out body);

			vars.Pop();

			return new ForStatement(initial, condition, post, body);
		}

		private DoWhileStatement parseDoWhile(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			line = line.Substring("do".Length).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(getCurlies(line), func, result, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			if (line.StartsWith("while"))
			{
				line = line.Substring("while".Length).TrimStart();
				string pars = getBrackets(line);
				Expression condition = expressions.analyze(pars, vars.SelectMany(x => x), f, func);

				line = line.Substring(pars.Length + 2).TrimStart().Substring(1);

				return new DoWhileStatement(condition, body);
			}

			return new DoWhileStatement(new Bohc.Parsing.Literal(Primitive.Boolean, "false"), body);
		}

		public string getBrackets(string line)
		{
			int lbrack = line.IndexOf('(');
			int rbrack = ParserTools.getMatchingBraceChar(line, lbrack, ')');

			return line.Substring(lbrack + 1, rbrack - lbrack - 1);
		}

		private string getCurlies(string line)
		{
			int lbrack = line.IndexOf('{');
			int rbrack = ParserTools.getMatchingBraceChar(line, lbrack, '}');

			return line.Substring(lbrack + 1, rbrack - lbrack - 1);
		}
	}
}
