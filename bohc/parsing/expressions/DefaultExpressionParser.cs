using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing.statements;

namespace bohc.parsing.expressions
{
	public class DefaultExpressionParser : IExpressionParser
	{
		// the stack to which enclosed variables may be added
		private readonly Stack<List<typesys.Variable>> enclosedVars = new Stack<List<typesys.Variable>>();

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
			enclosedVars.Push(new List<typesys.Variable>());
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
							prev == '(' || prev == ')')
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

		public IStatementParser statements;

		public void init(IStatementParser statements)
		{
			this.statements = statements;
		}

		public IStatementParser getStats()
		{
			return statements;
		}

		public Expression analyze(string str, IEnumerable<typesys.Variable> vars, parsing.File file, int opprec, typesys.Function ctx)
		{
			string next = null;
			int i = 0;

			Expression last = null;
			analyzeOp(ref last, vars, ref i, ref next, str, file, opprec, ctx);

			return last;
		}

		private void analyzeOp(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, parsing.File file, int opprec, typesys.Function ctx)
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
				if (analyzeName(ref last, vars, ref i, ref next, str, file, ctx)) { continue; }
			}
		}

		private bool analyzeRef(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, parsing.File file, typesys.Function ctx)
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

		private bool analyzeLambda(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, parsing.File file, int opprec, typesys.Function ctx)
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
			IEnumerable<typesys.LambdaParam> parameters = ParserTools.split(str, i - 1, ')', ',').Select(x =>
			{
				if (string.IsNullOrWhiteSpace(x))
				{
					return null;
				}

				string id;
				typesys.Modifiers mods;
				typesys.Type type;
				statements.getParser().parseParam(file, x.Trim(), out id, out mods, out type);
				typesys.LambdaParam res = new typesys.LambdaParam(id, type);
				res.lambdaLevel = lambdaStack/* - 1*/;
				return res;
			}).Where(x => x != null).ToArray();

			typesys.Type retType = ((last as ExprType).type);
			typesys.Type[] paramTypes = parameters.Select(x => x.type).ToArray();

			typesys.FunctionRefType fRefType = typesys.FunctionRefType.get(retType, paramTypes);
			i = ParserTools.getMatchingBraceChar(str, i - 1, ')') + 1;
			string afterParams = str.Substring(i).TrimStart();
			i += str.Length - afterParams.Length - i + 2;
			boh.Exception.require<exceptions.ParserException>(afterParams.StartsWith("->"), "Lambda operator expected");
			string exprOrBod = afterParams.Substring(2);
			if (exprOrBod.TrimStart().StartsWith("{"))
			{
				Stack<List<typesys.Variable>> varStack = new Stack<List<typesys.Variable>>();
				varStack.Push(parameters.Cast<typesys.Variable>().ToList());
				varStack.Push(vars.ToList());
				last = new Lambda(fRefType, statements.parseBody(exprOrBod.TrimStart(), ctx, varStack, file), null, parameters);
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

		public Expression analyze(string str, IEnumerable<typesys.Variable> vars, parsing.File file)
		{
			return analyze(str, vars, file, null);
		}

		public Expression analyze(string str, IEnumerable<typesys.Variable> vars, parsing.File file, typesys.Function ctx)
		{
			return analyze(str, vars, file, -1, ctx);
		}

		private Expression analyzeFunctionCall(ref Expression last, IEnumerable<typesys.Variable> vars, parsing.File file, string parameters)
		{
			Expression[] exprs = ParserTools.split("(" + parameters + ")", 0, ')', ',')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => analyze(x, vars, file)).ToArray();
			return new FunctionVarCall(last, exprs);
		}

		private bool analyzeBrackets(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, parsing.File file, int opprec, typesys.Function ctx)
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

		private OpBreakStat analyzeOperator(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, parsing.File file, int opprec, typesys.Function ctx)
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

				last = new UnaryOperation(right, op);
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

				last = new BinaryOperation(left, right, op, ctx);
			}

			return OpBreakStat.CONTINUE;
		}

		private bool analyzeLiteral(ref Expression last, ref int i, ref string next, string str, parsing.File file)
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

				boh.Exception.require<exceptions.ParserException>(close == '\'', "Chars may only be one character wide");

				builder.Append("'");
				last = new Literal(typesys.Primitive.CHAR, builder.ToString());
				return true;
			}

			if (next.StartsWith("\""))
			{
				last = new Literal(typesys.StdType.str, next);
				return true;
			}

			if (next == "true" || next == "false")
			{
				last = new Literal(typesys.Primitive.BOOLEAN, next);
				return true;
			}

			if (next == "null")
			{
				last = typesys.NullType.NULL.defaultVal();
				return true;
			}

			byte b;
			short s;
			int _i;
			long l;
			float f;
			double d;
			if (byte.TryParse(next, out b))
			{
				last = new Literal(typesys.Primitive.BYTE, next);
				return true;
			}
			if (short.TryParse(next, out s))
			{
				last = new Literal(typesys.Primitive.SHORT, next);
				return true;
			}
			else if (int.TryParse(next, out _i))
			{
				last = new Literal(typesys.Primitive.INT, next);
				return true;
			}
			else if (long.TryParse(next, out l))
			{
				last = new Literal(typesys.Primitive.LONG, next);
				return true;
			}
			else if (double.TryParse(next, out d))
			{
				last = new Literal(typesys.Primitive.DOUBLE, next);
				return true;
			}
			else if (next.EndsWith("f") && float.TryParse(next.Substring(0, next.Length - 1), out f))
			{
				last = new Literal(typesys.Primitive.FLOAT, next);
				return true;
			}
			else if (next.EndsWith("dd") && double.TryParse(next.Substring(0, next.Length - 2), out d))
			{
				last = new Literal(typesys.Primitive.DECIMAL, next);
				return true;
			}

			return false;
		}

		private bool analyzeNative(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string nxt, string str, parsing.File file, typesys.Function ctx)
		{
			typesys.NativeFunction nf;
			if (typesys.NativeFunction.funcs.TryGetValue(nxt, out nf))
			{
				string openParent = readNext(str, ref i);
				boh.Exception.require<exceptions.ParserException>(openParent == "(", "Native function must be followed by an open paranthesis");
				last = new NativeFunctionCall(nf, typesys.Function.getStringParams(str, i, vars, file, ctx, this).ToArray());
				i = ParserTools.getMatchingBraceChar(str, i - 1, ')');
				return true;
			}

			return false;
		}

		private bool analyzeName(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string nxt, string str, parsing.File file, typesys.Function ctx)
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

			if (!typesys.Type.isValidIdentifier(next))
			{
				return false;
			}

			typesys.Type type;

			if (next == "new")
			{
				// constructor call

				int idxOpen = str.IndexOf('(', i);
				int idxIndexer = str.IndexOf('[', i);

				if (idxIndexer != -1 && idxIndexer < idxOpen || idxOpen == -1)
				{
					string typeStr = str.Substring(i + 1, idxIndexer - i - 1);
					typesys.Type typeExpr = typesys.Type.getExisting(file.getContext(), typeStr, statements.getParser());
					typesys.Class arrayType = (typesys.Class)typesys.StdType.array.getTypeFor(new[] { typeExpr }, statements.getParser());
					int idxClose = ParserTools.getMatchingBraceChar(str, idxIndexer, ']');
					int len = idxClose - idxIndexer;
					string paramStr = str.Substring(idxIndexer, len);
					Expression paramExpr = analyze(paramStr, vars, file, ctx);
					boh.Exception.require<exceptions.ParserException>((paramExpr.getType() as typesys.Primitive).isInt(),
						"Array size must be an integer");
					last = new ConstructorCall(
						arrayType.constructors.Single(
						x => x.parameters.Count == 1 && x.parameters.Single().type == typesys.Primitive.INT),
						new[] { paramExpr });
					i = idxClose + 1;
				}
				else
				{
					string typeStr = str.Substring(i + 1, idxOpen - i - 1);
					typesys.Type typeExpr = typesys.Type.getExisting(file.getContext(), typeStr, statements.getParser());
					i += typeStr.Length + 1;
					int backup = i;
					try
					{
						i = solveIdentifierForType(ref last, vars, i, "this", str, file, (typesys.Class)typeExpr, ctx);
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
						string genpars = str.Substring(backup + 1, greaterThan - backup - 1);
						i = greaterThan + 1;

						i = solveIdentifierForType(ref last, vars, i, "this", str, file, (typesys.Class)typeExpr, ctx);
					}
					FunctionCall flast = (FunctionCall)last;
					typesys.Constructor constr = (typesys.Constructor)flast.refersto;
					last = new ConstructorCall(constr, flast.parameters);
				}
			}
			else if (last is ExprVariable)
			{
				ExprVariable exprLast = (ExprVariable)last;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, exprLast.refersto.type, ctx);
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
			else if (vars.Any(x => x.identifier == next))
			{
				typesys.Variable v = vars.Single(x => x.identifier == next);
				if (v.lambdaLevel < lambdaStack && !v.enclosed/* && !(v is typesys.LambdaParam)*/)
				{
					typesys.Parameter param = v as typesys.Parameter;
					boh.Exception.require<exceptions.ParserException>(param == null || !param.modifiers.HasFlag(typesys.Modifiers.REF),
						"ref parameters may not be used inside lambdas");
					v.enclosed = true;
					enclosedVars.Peek().Add(v);
				}
				last = new ExprVariable(v, null);
			}
			else if ((exprPack != null && (type = typesys.Type.getExisting(exprPack.refersto, next, statements.getParser())) != null) ||
				((type = typesys.Type.getExisting(file.getContext(), next, statements.getParser())) != null))
			{
				last = new ExprType(type);
			}
			else if (typesys.GenericType.allGenTypes.Any(x => x.name == next))
			{
				int backup = i;
				string after = readNext(str, ref i);
				boh.Exception.require<exceptions.ParserException>(after == "<", "Generic type must be followed by <");
				int greaterThan = ParserTools.getMatchingBraceChar(str, i - 1, '>');
				string typename = str.Substring(backup - next.Length, greaterThan - backup + next.Length + 1);
				last = new ExprType(typesys.Type.getExisting(exprPack != null ? new typesys.Package[] { exprPack.refersto } : file.getContext(), typename, getStats().getParser()));
				i = greaterThan + 1;
			}
			else
			{
				// it's either a package, field
				Expression lastBackup = last;
				try
				{
					// thisvar
					last = ((typesys.Class)file.type).THISVAR;
					i = solveIdentifierForType(ref last, vars, i, next, str, file, (typesys.Type)file.type, ctx);
				}
				catch
				{
					last = lastBackup;
					ExprPackage prevPack = last as ExprPackage;
					if (prevPack != null)
					{
						typesys.Package pack = typesys.Package.getFromStringExisting(prevPack.refersto.ToString() + "." + next);
						boh.Exception.require<exceptions.ParserException>(pack != null, "Invalid identifier: " + next);
						last = new ExprPackage(pack);
					}
					else
					{
						typesys.Package pack = typesys.Package.getFromStringExisting(next);
						boh.Exception.require<exceptions.ParserException>(pack != null, "Invalid identifier: " + next);
						last = new ExprPackage(pack);
					}
				}
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

		private int solveIdentifierForType(ref Expression expr, IEnumerable<typesys.Variable> vars, int i, string next, string str, parsing.File file, typesys.Type type, typesys.Function ctx)
		{
			// TOOD: callable variables

			// check which functions/fields are compatible
			// change expr accordingly

			IEnumerable<typesys.Function> functions = null;
			typesys.Variable field = null;

			if (type is typesys.Interface)
			{
				typesys.Interface iface = (typesys.Interface)type;
				functions = iface.getFunctions(next);
			}
			else if (type is typesys.Class)
			{
				typesys.Class _class = (typesys.Class)type;
				functions = _class.getFunctions(next, (typesys.Class)file.type);
				field = _class.getField(next, (typesys.Class)file.type);
				if (field == null)
				{
					field = vars.SingleOrDefault(x => x.identifier == next);
				}
			}
			else if (type is typesys.Enum)
			{
				typesys.Enum _enum = (typesys.Enum)type;
				typesys.Enumerator enumerator = _enum.enumerators.SingleOrDefault(x => x.name == next);
				if (enumerator != null)
				{
					expr = new ExprEnumerator(enumerator);
					return i;
				}

				switch (next)
				{
					case "toString":
						functions = new[] { _enum.toString };
						break;
					case "getType":
						functions = new[] { _enum.getType };
						break;
					case "equals":
						functions = new[] { _enum.equals };
						break;
					case "hash":
						functions = new[] { _enum.hash };
						break;
				}
			}
			else if (vars.Any(x => x.identifier == next))
			{
				field = vars.SingleOrDefault(x => x.identifier == next);
			}
			else
			{
				field = ((typesys.Class)type).fields.SingleOrDefault(x => x.identifier == next);
			}

			boh.Exception.require<exceptions.ParserException>(
				(next == "this" || next == "super") || (functions != null && functions.Count() != 0) || field != null,
				"Invalid identifier: " + next);

			solveIdentifierWithInfo(ref expr, vars, ref i, next, str, file, ctx, functions, field);

			return i;
		}

		private void solveIdentifierWithInfo(ref Expression expr, IEnumerable<typesys.Variable> vars, ref int i, string next, string str, parsing.File file, typesys.Function ctx, IEnumerable<typesys.Function> functions, typesys.Variable field)
		{
			string nextnext = readNext(str, ref i);
			if (nextnext == "(" && field == null)
			{

				// if belongsto is thisvar, the thisvar has to be enclosed
				if (expr is ThisVar)
				{
					ThisVar tv = ((typesys.Class)file.type).THISVAR;
					if (lambdaStack > tv.refersto.lambdaLevel)
					{
						tv.refersto.enclosed = true;
						enclosedVars.Peek().Add(tv.refersto);
					}
				}

				// function
				IEnumerable<Expression> parameters;
				if (field == null)
				{
					typesys.Function f = typesys.Function.getCompatibleFunction(ref i, next, str, file, vars, functions, out parameters, ctx, this);
					FunctionCall call = new FunctionCall(f, expr, parameters);
					expr = call;
				}
				else
				{
					//FunctionVarCall call = new FunctionVarCall(expr, typesys.Function.getStringParams(str, i, vars, file, ctx, this));
					//expr = call;
					//i = ParserTools.getMatchingBraceChar(str, i - 1, ')') + 1;
				}

				//i -= nextnext.Length;
				//i = Parser.getMatchingBraceChar(str, i - 1, ')') + 1;
			}
			else
			{
				if (next == "this")
				{
					ThisVar tv = ((typesys.Class)file.type).THISVAR;
					if (lambdaStack > tv.refersto.lambdaLevel)
					{
						tv.refersto.enclosed = true;
						enclosedVars.Peek().Add(tv.refersto);
					}
					expr = tv;
				}
				else if (next == "super")
				{
					SuperVar sv = new SuperVar((typesys.Class)file.type);
					if (lambdaStack > sv.refersto.lambdaLevel)
					{
						sv.refersto.enclosed = true;
						enclosedVars.Peek().Add(sv.refersto);
					}
					expr = sv;
				}
				else
				{
					// if belongsto is thisvar, the thisvar has to be enclosed
					if (expr is ThisVar)
					{
						ThisVar tv = ((typesys.Class)file.type).THISVAR;
						if (lambdaStack > tv.refersto.lambdaLevel)
						{
							tv.refersto.enclosed = true;
							enclosedVars.Peek().Add(tv.refersto);
						}
					}

					expr = new ExprVariable(field, ((typesys.Field)field).modifiers.HasFlag(typesys.Modifiers.STATIC) ? null : expr);
					if (nextnext != null)
					{
						i -= nextnext.Length;
					}
				}
			}
		}
	}
}
