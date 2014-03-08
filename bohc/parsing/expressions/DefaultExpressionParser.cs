using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Parsing.Statements;

namespace Bohc.Parsing
{
	public class DefaultExpressionParser : IExpressionParser
	{
		// the stack to which enclosed variables may be added
		private readonly Stack<List<Bohc.TypeSystem.Variable>> enclosedVars = new Stack<List<Bohc.TypeSystem.Variable>>();

		// lambda stack
		// if greater than 0, lambdas are being generated
		public int lambdaStack = 0;

		public int getLambdaStack()
		{
			return lambdaStack;
		}

		// push lambda onto lambda stack
		private void pushLambda()
		{
			enclosedVars.Push(new List<Bohc.TypeSystem.Variable>());
			lambdaStack += 2;
		}

		// pop lambda off of lambda stack
		private void popLambda()
		{
			enclosedVars.Pop();
			lambdaStack -= 2;
		}

		private string readNext(string str, ref int pos)
		{
			bool instring = false;

			StringBuilder sb = new StringBuilder();
			int i = 0;
			for (; pos < str.Length; ++pos)
			{
				char ch = str[pos];
				if (ch == ' ')
				{
					if (!instring)
					{
						if (i > 0)
						{
							break;
						}
						continue;
					}
				}
				else if (ch == '"')
				{
					if (sb.Length > 0)
					{
						if (!instring)
						{
							//sb.Append("\"");
							//--pos;
							break;
						}
						else
						{
							++pos;
							sb.Append("\"");
							break;
						}
					}
					instring = !instring;
				}

				if (pos > 0 && i > 0)
				{
					if (!instring)
					{
						char prev = str[pos - 1];
						if ((((char.IsLetterOrDigit(prev) || prev == '_') && !(char.IsLetterOrDigit(ch) || ch == '_')) ||
							((char.IsLetterOrDigit(ch) || ch == '_') && !(char.IsLetterOrDigit(prev) || prev == '_')) ||
							prev == '(' || prev == ')' || prev == '[' || (prev == ']'))
							&& ch != '.' && prev != '.')
						{
							if (ch == '*' && sb.ToString().StartsWith("native."))
							{
								// yeah, it's supposed to be empty
							}
							else
							{
								break;
							}
						}
					}
				}

				sb.Append(ch);
				++i;
				if (ch == '\'')
				{
					++pos;
					break;
				}
			}

			if (i == 0)
			{
				return null;
			}

			return sb.ToString();
		}

		public IStatementParser Statements;

		public void init(IStatementParser Statements)
		{
			this.Statements = Statements;
		}

		public IStatementParser getStats()
		{
			return Statements;
		}

		public Expression analyze(string str, IEnumerable<Bohc.TypeSystem.Variable> vars, Bohc.Parsing.File file, int opprec, Bohc.TypeSystem.Function ctx)
		{
			string next = null;
			int i = 0;

			Expression last = null;
			analyzeOp(ref last, vars, ref i, ref next, str, file, opprec, ctx);

			return last;
		}

		private void analyzeOp(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, int opprec, Bohc.TypeSystem.Function ctx)
		{
			while ((next = readNext(str, ref i)) != null)
			{
				if (analyzeLambda(ref last, vars, ref i, ref next, str, file, opprec, ctx)) { continue; }
				if (analyzeBrackets(ref last, vars, ref i, ref next, str, file, opprec, ctx)) { continue; }
				OpBreakStat s = analyzeOperator(ref last, vars, ref i, ref next, str, file, opprec, ctx);
				if (s == OpBreakStat.BREAK)
				{
					break;
				}
				else if (s == OpBreakStat.CONTINUE)
				{
					continue;
				}
				if (analyzeLiteral(ref last, ref i, ref next, str, file)) { continue; }
				if (analyzeRef(ref last, vars, ref i, ref next, str, file, ctx)) { continue; }
				if (analyzeIndexer(ref last, vars, ref i, ref next, str, file, ctx)) { continue; }
				if (analyzeName(ref last, vars, ref i, ref next, str, file, ctx)) { continue; }
			}

			if (last != null)
			{
				last.useAsRvalue();
			}
		}

		private bool analyzeIndexer(ref Expression last, IEnumerable<TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{
			if (next != "[")
			{
				return false;
			}

			int temp = i;

			IEnumerable<Expression> parameters = null;
			TypeSystem.Indexer getter = null;

			try
			{
				IEnumerable<TypeSystem.Indexer> idxers = ((TypeSystem.Class)last.getType())
					.GetAllFuncs()
					.OfType<TypeSystem.Indexer>()
					.Where(x => !x.IsAssignment()).ToArray();
				getter = (TypeSystem.Indexer)TypeSystem.Function.GetCompatibleFunction(
					ref temp, "indexer", str, file, vars, idxers,
					out parameters, ctx, this, ']');
			}
			catch
			{
			}

			TypeSystem.Indexer setter = null;
			try
			{
				IEnumerable<TypeSystem.Indexer> idxers = ((TypeSystem.Class)last.getType())
					.GetAllFuncs()
					.OfType<TypeSystem.Indexer>()
					.Where(x => x.IsAssignment()).ToArray();
				setter = (TypeSystem.Indexer)TypeSystem.Function.GetCompatibleFunction(
					ref i, "indexer", str, file, vars, idxers,
					out parameters, ctx, this, ']');
			}
			catch
			{
			}

			last = new IndexerCall(getter, setter, last, parameters);

			return true;
		}

		private bool analyzeRef(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{
			if (next != "ref")
			{
				return false;
			}

			string after = str.Substring(i);
			Expression onwhat = analyze(after, vars, file, ctx);

			last = new RefExpression(onwhat, ctx);
			i = str.Length;
			return true;
		}

		private void parseParam(File f, string part, out string identifier, out TypeSystem.Modifiers mods, out Bohc.TypeSystem.Type type)
		{
			string[] paramParts = part.Split(' ');
			Boh.Exception.require<Exceptions.ParserException>(paramParts.Length >= 2, "Parameter type expected");
			Boh.Exception.require<Exceptions.ParserException>(paramParts.Length <= 3, "Parameters may only have one modifier");

			identifier = paramParts[paramParts.Length - 1];
			string typeName = paramParts[paramParts.Length - 2];

			mods = TypeSystem.Modifiers.None;
			if (paramParts.Length > 2)
			{
				Boh.Exception.require<Exceptions.ParserException>(
					paramParts.First() == "final" || paramParts.First() == "ref", "'final' and 'ref' are the only legal modifiers for parameters");
				mods = TypeSystem.ModifierHelper.GetModifierFromString(paramParts.First());
			}

			type = Bohc.TypeSystem.Type.GetExisting(f.getContext(), typeName, getStats().getParser());
			Boh.Exception.require<Exceptions.ParserException>(type != null, "type doesn't exist");
		}

		private bool analyzeLambda(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, int opprec, Bohc.TypeSystem.Function ctx)
		{
			if (next != "(")
			{
				return false;
			}

			if (!(last is ExprType))
			{
				return false;
			}

			pushLambda();
			IEnumerable<Bohc.TypeSystem.LambdaParam> parameters = ParserTools.split(str, i - 1, ')', ',').Select(x =>
			{
				if (string.IsNullOrWhiteSpace(x))
				{
					return null;
				}

				string id;
				Bohc.TypeSystem.Modifiers mods;
				Bohc.TypeSystem.Type type;
				parseParam(file, x.Trim(), out id, out mods, out type);
				Bohc.TypeSystem.LambdaParam res = new Bohc.TypeSystem.LambdaParam(id, type);
				res.LamdaLevel = lambdaStack/* - 1*/;
				return res;
			}).Where(x => x != null).ToArray();

			Bohc.TypeSystem.Type retType = ((last as ExprType).type);
			Bohc.TypeSystem.Type[] paramTypes = parameters.Select(x => x.Type).ToArray();

			Bohc.TypeSystem.FunctionRefType fRefType = Bohc.TypeSystem.FunctionRefType.Get(retType, paramTypes);
			i = ParserTools.getMatchingBraceChar(str, i - 1, ')') + 1;
			string afterParams = str.Substring(i).TrimStart();
			i += str.Length - afterParams.Length - i + 2;
			Boh.Exception.require<Exceptions.ParserException>(afterParams.StartsWith("->"), "Lambda operator expected");
			string exprOrBod = afterParams.Substring(2);
			if (exprOrBod.TrimStart().StartsWith("{"))
			{
				Stack<List<Bohc.TypeSystem.Variable>> varStack = new Stack<List<Bohc.TypeSystem.Variable>>();
				varStack.Push(parameters.Cast<Bohc.TypeSystem.Variable>().ToList());
				varStack.Push(vars.ToList());
				last = new Lambda(fRefType, Statements.parseBody(exprOrBod.TrimStart(), ctx, varStack, null, file), null, parameters);
				((Lambda)last).lambdaLevel = lambdaStack - 1;
				((Lambda)last).enclosed = enclosedVars.Peek();
				i += exprOrBod.Length;
			}
			else
			{
				last = new Lambda(fRefType, null,
					analyze(exprOrBod.TrimStart(), vars.Concat(parameters), file), parameters);
				((Lambda)last).lambdaLevel = lambdaStack - 1;
				((Lambda)last).enclosed = enclosedVars.Peek();
				i += exprOrBod.Length;
			}
			popLambda();
			return true;
		}

		public Expression analyze(string str, IEnumerable<Bohc.TypeSystem.Variable> vars, Bohc.Parsing.File file)
		{
			return analyze(str, vars, file, null);
		}

		public Expression analyze(string str, IEnumerable<Bohc.TypeSystem.Variable> vars, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{
			return analyze(str, vars, file, -1, ctx);
		}

		private Expression analyzeFunctionCall(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, Bohc.Parsing.File file, string parameters)
		{
			Expression[] exprs = ParserTools.split("(" + parameters + ")", 0, ')', ',')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => analyze(x, vars, file)).ToArray();
			return new FunctionVarCall(last, exprs);
		}

		private bool analyzeBrackets(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, int opprec, Bohc.TypeSystem.Function ctx)
		{
			if (next != "(")
			{
				return false;
			}

			int closingParam = ParserTools.getMatchingBraceChar(str, i - 1, ')');
			string between = str.Substring(i, closingParam - i);

			if (last != null)
			{
				last = analyzeFunctionCall(ref last, vars, file, between);
				i += between.Length + 1;
			}
			else
			{
				Expression betweenBrackets = analyze(between, vars, file, ctx);
				i += between.Length + 1;

				last = betweenBrackets;

				// typecast
				ExprType type = betweenBrackets as ExprType;
				if (type != null && opprec != UnaryOperation.TYPEOF.precedence)
				{
					// a typecast has operator precedence 8, hence the 8
					// TODO: or is it 7?
					Expression e = null;
					analyzeOp(ref e, vars, ref i, ref next, str, file, 8, ctx);
					// only apply if it's actually followed by something
					if (e != null)
					{
						last = new TypeCast(type.type, e);
					}
				}
			}

			return true;
		}

		private enum OpBreakStat
		{
			BREAK,
			CONTINUE,
			NOTHING,
		}

		private OpBreakStat analyzeOperator(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string next, string str, Bohc.Parsing.File file, int opprec, Bohc.TypeSystem.Function ctx)
		{
			// TODO: unary operators
			if (!Operator.isOperator(next))
			{
				return OpBreakStat.NOTHING;
			}

			if (last == null) // normal unary operator, including prefix increment and decrement
			{
				Operator op = Operator.getExisting(next, OperationType.UNARY);
				if (op.precedence < opprec)
				{
					i -= op.representation.Length;
					return OpBreakStat.BREAK;
				}

				string rightstr = str.Substring(i);
				string nxt = null;
				int _i = 0;
				Expression right = null;
				analyzeOp(ref right, vars, ref _i, ref nxt, rightstr, file, op.precedence, ctx);
				i += _i;

				if (op == UnaryOperation.MINUS && right is Literal)
				{
					if (right.getType() == TypeSystem.Primitive.Byte)
					{
						last = new Literal(TypeSystem.Primitive.Short, "-" + ((Literal)right).representation);
					}
					else
					{
						last = new Literal(right.getType(), "-" + ((Literal)right).representation);
					}
				}
				else
				{
					last = new UnaryOperation(right, op);
				}
			}
			else // binary operator, or suffix increment/decrement
			{
				if (next == "++")
				{
					last = new UnaryOperation(last, UnaryOperation.INCREMENT_POST);
					return OpBreakStat.CONTINUE;
				}
				else if (next == "--")
				{
					last = new UnaryOperation(last, UnaryOperation.DECREMENT_POST);
					return OpBreakStat.CONTINUE;
				}

				Operator op = Operator.getExisting(next, OperationType.BINARY);
				if (op.precedence <= opprec)
				{
					i -= op.representation.Length;
					return OpBreakStat.BREAK;
				}

				Expression left = last;
				string rightstr = str.Substring(i);
				string nxt = null;
				int _i = 0;
				Expression right = null;
				analyzeOp(ref right, vars, ref _i, ref nxt, rightstr, file, op.precedence, ctx);
				i += _i;

				BinaryOperation binop = new BinaryOperation(left, right, op, ctx);
				if (binop.isAssignment())
				{
					last = binop.left.useAsLvalue(binop);
				}
				else
				{
					last = binop;
				}
			}

			return OpBreakStat.CONTINUE;
		}

		private bool analyzeLiteral(ref Expression last, ref int i, ref string next, string str, Bohc.Parsing.File file)
		{
			// TODO: chars and strings

			if (next == "'")
			{
				StringBuilder builder = new StringBuilder("'");

				char ch = str[i++];
				char close = str[i++];
				if (ch == '\\') // control character
				{
					builder.Append(ch);
					builder.Append(close);
					close = str[i++];
				}
				else
				{
					builder.Append(ch);
				}

				Boh.Exception.require<Exceptions.ParserException>(close == '\'', "Chars may only be one character wide");

				builder.Append("'");
				last = new Literal(Bohc.TypeSystem.Primitive.Char, builder.ToString());
				return true;
			}

			if (next.StartsWith("\""))
			{
				last = new Literal(Bohc.TypeSystem.StdType.Str, next);
				return true;
			}

			if (next == "true" || next == "false")
			{
				last = new Literal(Bohc.TypeSystem.Primitive.Boolean, next);
				return true;
			}

			if (next == "null")
			{
				last = Bohc.TypeSystem.NullType.Null.DefaultVal();
				return true;
			}

			return analyzeNumericLiteral(ref last, next);
		}

		private static bool analyzeNumericLiteral(ref Expression last, string next)
		{
			int _base = next.StartsWith("0x") ? 16 : 10;
			if (next.StartsWith("0x"))
			{
				next = next.Substring(2);
			}

			byte b;
			short s;
			int _i;
			long l;
			float f;
			double d;
			try
			{
				b = Convert.ToByte(next, _base);
				last = new Literal(Bohc.TypeSystem.Primitive.Byte, b.ToString());
				return true;
			}
			catch
			{
			}
			try
			{
				s = Convert.ToInt16(next, _base);
				last = new Literal(Bohc.TypeSystem.Primitive.Short, s.ToString());
				return true;
			}
			catch
			{
			}
			try
			{
				_i = Convert.ToInt32(next, _base);
				last = new Literal(Bohc.TypeSystem.Primitive.Int, _i.ToString());
				return true;
			}
			catch
			{
			}
			try
			{
				l = Convert.ToInt64(next, _base);
				last = new Literal(Bohc.TypeSystem.Primitive.Long, l.ToString());
				return true;
			}
			catch
			{
			}
			if (double.TryParse(next, out d))
			{
				last = new Literal(Bohc.TypeSystem.Primitive.Double, next);
				return true;
			}
			else if (next.EndsWith("f") && float.TryParse(next.Substring(0, next.Length - 1), out f))
			{
				last = new Literal(Bohc.TypeSystem.Primitive.Float, next);
				return true;
			}
			else if (next.EndsWith("dd") && double.TryParse(next.Substring(0, next.Length - 2), out d))
			{
				last = new Literal(Bohc.TypeSystem.Primitive.Decimal, next);
				return true;
			}

			return false;
		}

		private bool analyzeNative(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string nxt, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{
			Bohc.TypeSystem.NativeFunction nf;
			if (Bohc.TypeSystem.NativeFunction.Funcs.TryGetValue(nxt, out nf))
			{
				string openParent = readNext(str, ref i);
				Boh.Exception.require<Exceptions.ParserException>(openParent == "(", "Native function must be followed by an open paranthesis");
				last = new NativeFunctionCall(nf, Bohc.TypeSystem.Function.GetStringParams(str, i, vars, file, ctx, this, ')').ToArray());
				i = ParserTools.getMatchingBraceChar(str, i - 1, ')');
				return true;
			}

			return false;
		}

		private bool analyzeName(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, ref string nxt, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{
			if (analyzeNative(ref last, vars, ref i, ref nxt, str, file, ctx))
			{
				return true;
			}

			ExprPackage exprPack = last as ExprPackage;
			int ibackup = i;

			string next = nxt;
			int idxDot = nxt.IndexOf('.');
			if (idxDot == 0)
			{
				next = nxt.Substring(1);
				//++i;
				return analyzeName(ref last, vars, ref i, ref next, str, file, ctx);
			}
			else if (idxDot != -1)
			{
				i -= idxDot + 1;
				next = nxt.Substring(0, idxDot);
			}

			if (!Bohc.TypeSystem.Type.IsValidIdentifier(next))
			{
				return false;
			}

			Bohc.TypeSystem.Type type;

			if (next == "new")
			{
				// constructor call
				analyzeConstrCall(ref last, vars, ref i, str, file, ctx);
			}
			else if (last is ExprVariable)
			{
				ExprVariable exprLast = (ExprVariable)last;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, exprLast.refersto.Type, ctx);
			}
			else if (last is Literal)
			{
				i = solveIdentifierForType(ref last, vars, i, next, str, file, last.getType(), ctx);
			}
			else if (last is ExprType)
			{
				ExprType exprLast = (ExprType)last;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, exprLast.type, ctx);
			}
			else if (last is FunctionCall || last is ConstructorCall)
			{
				i = solveIdentifierForType(ref last, vars, i, next, str, file, last.getType(), ctx);
			}
			else if (vars.Any(x => x.Identifier == next))
			{
				last = analyzeLocal(last, vars, next);
			}
			else if ((exprPack != null && (type = Bohc.TypeSystem.Type.GetExisting(exprPack.refersto, next, Statements.getParser())) != null) ||
				((type = Bohc.TypeSystem.Type.GetExisting(file.getContext(), next, Statements.getParser())) != null))
			{
				last = new ExprType(type);
			}
			else if (Bohc.TypeSystem.GenericType.AllGenTypes.Any(x => x.Name == next))
			{
				analyzeGenType(ref last, ref i, str, file, exprPack, next);
			}
			else
			{
				// it's either a package, field
				analyzeFieldOrPackage(ref last, vars, ref i, str, file, ctx, next);
			}

			// recursively parse stuff
			if (idxDot != -1 && idxDot != 0)
			{
				string after = nxt.Substring(idxDot + 1);
				int j = ibackup;
				analyzeName(ref last, vars, ref j, ref after, str, file, ctx);
				i = j;
			}

			return true;
		}

		private Expression analyzeLocal(Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, string next)
		{
			Bohc.TypeSystem.Variable v = vars.Single(x => x.Identifier == next);
			if (v.LamdaLevel < lambdaStack/* && !(v is typesys.LambdaParam)*/)
			{
				Bohc.TypeSystem.Parameter param = v as Bohc.TypeSystem.Parameter;
				Boh.Exception.require<Exceptions.ParserException>(param == null || !param.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Ref),
					"ref parameters may not be used inside lambdas");
				v.Enclosed = true;
				enclosedVars.Peek().Add(v);
			}
			last = new ExprVariable(v, null);
			return last;
		}

		private void analyzeFieldOrPackage(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx, string next)
		{
			Expression lastBackup = last;
			try
			{
				i = solveIdentifierForType(ref last, vars, i, next, str, file, (Bohc.TypeSystem.Type)file.type, ctx);
			}
			catch
			{
				last = lastBackup;
				ExprPackage prevPack = last as ExprPackage;
				if (prevPack != null)
				{
					Bohc.TypeSystem.Package pack = Bohc.TypeSystem.Package.GetFromStringExisting(prevPack.refersto.ToString() + "." + next);
					Boh.Exception.require<Exceptions.ParserException>(pack != null, "Invalid identifier: " + next);
					last = new ExprPackage(pack);
				}
				else
				{
					Bohc.TypeSystem.Package pack = Bohc.TypeSystem.Package.GetFromStringExisting(next);
					Boh.Exception.require<Exceptions.ParserException>(pack != null, "Invalid identifier: " + next);
					last = new ExprPackage(pack);
				}
			}
		}

		private void analyzeGenType(ref Expression last, ref int i, string str, Bohc.Parsing.File file, ExprPackage exprPack, string next)
		{
			int backup = i;
			string after = readNext(str, ref i);
			Boh.Exception.require<Exceptions.ParserException>(after == "<", "Generic type must be followed by <");
			int greaterThan = ParserTools.getMatchingBraceChar(str, i - 1, '>');
			string typename = str.Substring(backup - next.Length, greaterThan - backup + next.Length + 1);
			last = new ExprType(Bohc.TypeSystem.Type.GetExisting(exprPack != null ? new Bohc.TypeSystem.Package[] { exprPack.refersto } : file.getContext(), typename, getStats().getParser()));
			i = greaterThan + 1;
		}

		private void analyzeConstrCall(ref Expression last, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx)
		{

			int idxOpen = str.IndexOf('(', i);
			int idxIndexer = str.IndexOf('[', i);

			if (idxIndexer != -1 && idxIndexer < idxOpen || idxOpen == -1)
			{
				string typeStr = str.Substring(i + 1, idxIndexer - i - 1);
				Bohc.TypeSystem.Type typeExpr = Bohc.TypeSystem.Type.GetExisting(file.getContext(), typeStr, Statements.getParser());
				Bohc.TypeSystem.Class arrayType = (Bohc.TypeSystem.Class)Bohc.TypeSystem.StdType.Array.GetTypeFor(new[] { typeExpr }, Statements.getParser());
				int idxClose = ParserTools.getMatchingBraceChar(str, idxIndexer, ']');
				int len = idxClose - idxIndexer;
				string paramStr = str.Substring(idxIndexer + 1, len - 1);
				Expression paramExpr = analyze(paramStr, vars, file, ctx);
				Boh.Exception.require<Exceptions.ParserException>((paramExpr.getType() as Bohc.TypeSystem.Primitive).IsInt(),
					"Array size must be an integer");
				last = new ConstructorCall(
					arrayType.Constructors.Single(
					x => x.Parameters.Count == 1 && x.Parameters.Single().Type == Bohc.TypeSystem.Primitive.Int),
					new[] { paramExpr });
				i = idxClose + 1;
			}
			else
			{
				string typeStr = str.Substring(i + 1, idxOpen - i - 1);
				Bohc.TypeSystem.Type typeExpr = Bohc.TypeSystem.Type.GetExisting(file.getContext(), typeStr, Statements.getParser());
				i += typeStr.Length + 1;
				int backup = i;
				try
				{
					i = solveIdentifierForType(ref last, vars, i, "this", str, file, (Bohc.TypeSystem.Class)typeExpr, ctx);
				}
				catch (NullReferenceException)
				{
					// generic type
					i = backup;
					string after = readNext(str, ref i);
					if (after != "<")
					{
						throw;
					}
					int greaterThan = ParserTools.getMatchingBraceChar(str, i - 1, '>');
					//string genpars = str.Substring(backup + 1, greaterThan - backup - 1);
					i = greaterThan + 1;

					i = solveIdentifierForType(ref last, vars, i, "this", str, file, (Bohc.TypeSystem.Class)typeExpr, ctx);
				}
				FunctionCall flast = (FunctionCall)last;
				Bohc.TypeSystem.Constructor constr = (Bohc.TypeSystem.Constructor)flast.refersto;
				last = new ConstructorCall(constr, flast.parameters);
			}
		}

		private int solveIdentifierForType(ref Expression expr, IEnumerable<Bohc.TypeSystem.Variable> vars, int i, string next, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Type type, Bohc.TypeSystem.Function ctx)
		{
			// TOOD: callable variables

			// check which functions/fields are compatible
			// change expr accordingly

			IEnumerable<Bohc.TypeSystem.Function> functions = type.GetFunctions(next, type);
			Bohc.TypeSystem.Variable field = type.GetField(next, type);

			if (type is Bohc.TypeSystem.Enum)
			{
				Bohc.TypeSystem.Enum _enum = (Bohc.TypeSystem.Enum)type;
				Bohc.TypeSystem.Enumerator enumerator = _enum.Enumerators.SingleOrDefault(x => x.Name == next);
				if (enumerator != null)
				{
					expr = new ExprEnumerator(enumerator);
					return i;
				}
			}

			if (field != null && !((TypeSystem.Field)field).Modifiers.HasFlag(TypeSystem.Modifiers.Static) && expr == null)
			{
				expr = ((Bohc.TypeSystem.Class)type).ThisVarExpr;
			}

			Boh.Exception.require<Exceptions.ParserException>(
				(functions != null && functions.Count() != 0) || field != null,
				"Invalid identifier: " + next);

			solveIdentifierWithInfo(ref expr, vars, ref i, next, str, file, ctx, functions, field);

			return i;
		}

		private void solveIdentifierWithInfo(ref Expression expr, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, string next, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx, IEnumerable<Bohc.TypeSystem.Function> functions, Bohc.TypeSystem.Variable field)
		{
			string nextnext = readNext(str, ref i);
			if (field == null)
			{
				if (nextnext == "(")
				{
					solveFunctionCall(ref expr, vars, ref i, next, str, file, ctx, functions, field);
				}
			}
			else
			{
				solveVar(ref expr, ref i, next, file, field, nextnext);
			}
		}

		private bool isThisVar(Expression expr)
		{
			ExprVariable exprvar = (expr as ExprVariable);
			return exprvar != null && (exprvar.refersto.Identifier == "this" || exprvar.refersto.Identifier == "super");
		}

		private void solveVar(ref Expression expr, ref int i, string next, Bohc.Parsing.File file, Bohc.TypeSystem.Variable field, string nextnext)
		{
			// if belongsto is thisvar, the thisvar has to be enclosed
			if (isThisVar(expr))
			{
				TypeSystem.Variable tv = ((Bohc.TypeSystem.Class)file.type).This;
				if (lambdaStack > tv.LamdaLevel)
				{
					tv.Enclosed = true;
					enclosedVars.Peek().Add(tv);
				}
			}

			// TODO: maybe uncomment this and find a proper solution or something?
			//expr = new ExprVariable(field, ((Bohc.TypeSystem.Field)field).Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Static) ? null : expr);
			expr = new ExprVariable(field, expr /*((Bohc.TypeSystem.Field)field).Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Static) ? null : expr*/);
			if (nextnext != null)
			{
				i -= nextnext.Length;
			}
		}

		private void solveFunctionCall(ref Expression expr, IEnumerable<Bohc.TypeSystem.Variable> vars, ref int i, string next, string str, Bohc.Parsing.File file, Bohc.TypeSystem.Function ctx, IEnumerable<Bohc.TypeSystem.Function> functions, Bohc.TypeSystem.Variable field)
		{
			// function
			IEnumerable<Expression> parameters;
			if (field == null)
			{
				Bohc.TypeSystem.Function f = Bohc.TypeSystem.Function.GetCompatibleFunction(ref i, next, str, file, vars, functions, out parameters, ctx, this, ')');
				// if static, remove thisvar, so it doesn't get enclosed
				// TODO: why is it [the thisvar] even there?!
				/*if (f.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Static) && expr is ThisVar)
				{
					expr = null;
				}*/
				if (!f.Modifiers.HasFlag(TypeSystem.Modifiers.Static) && expr == null)
				{
					expr = ((TypeSystem.Class)file.type).ThisVarExpr;
				}
				FunctionCall call = new FunctionCall(f, expr, parameters);
				expr = call;
			}

			// if belongsto is thisvar, the thisvar has to be enclosed
			if (isThisVar(expr))
			{
				TypeSystem.Variable tv = ((Bohc.TypeSystem.Class)file.type).This;
				if (lambdaStack > tv.LamdaLevel)
				{
					tv.Enclosed = true;
					enclosedVars.Peek().Add(tv);
				}
			}

			//i -= nextnext.Length;
			//i = Parser.getMatchingBraceChar(str, i - 1, ')') + 1;
		}
	}
}
