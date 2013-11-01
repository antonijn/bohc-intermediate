// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public abstract class Expression
	{
		private static string readNext(string str, ref int pos)
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
			}

			if (i == 0)
			{
				return null;
			}

			return sb.ToString();
		}

		private static Expression analyze(string str, IEnumerable<typesys.Variable> vars, ts.File file, int opprec)
		{
			string next = null;
			int i = 0;

			Expression last = null;
			analyzeOp(ref last, vars, ref i, ref next, str, file, opprec);

			return last;
		}

		private static void analyzeOp(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, ts.File file, int opprec)
		{
			while ((next = readNext(str, ref i)) != null)
			{
				if (analyzeBrackets(ref last, vars, ref i, ref next, str, file)) { continue; }
				OpBreakStat s = analyzeOperator(ref last, vars, ref i, ref next, str, file, opprec);
				if (s == OpBreakStat.BREAK)
				{
					break;
				}
				else if (s == OpBreakStat.CONTINUE)
				{
					continue;
				}
				if (analyzeLiteral(ref last, ref i, ref next, str, file)) { continue; }
				if (analyzeName(ref last, vars, ref i, ref next, str, file)) { continue; }
			}
		}

		public static Expression analyze(string str, IEnumerable<typesys.Variable> vars, ts.File file)
		{
			return analyze(str, vars, file, -1);
		}

		private static bool analyzeBrackets(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, ts.File file)
		{
			if (next != "(")
			{
				return false;
			}

			int closingParam = Parser.getMatchingBraceChar(str, i - 1, ')');
			string between = str.Substring(i, closingParam - i);
			Expression betweenBrackets = analyze(between, vars, file);
			i += between.Length + 1;

			last = betweenBrackets;

			return true;
		}

		private enum OpBreakStat
		{
			BREAK,
			CONTINUE,
			NOTHING,
		}

		private static OpBreakStat analyzeOperator(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string next, string str, ts.File file, int opprec)
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
				analyzeOp(ref right, vars, ref _i, ref nxt, rightstr, file, op.precedence);
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
				analyzeOp(ref right, vars, ref _i, ref nxt, rightstr, file, op.precedence);
				i += _i;

				last = new BinaryOperation(left, right, op);
			}

			return OpBreakStat.CONTINUE;
		}

		private static bool analyzeLiteral(ref Expression last, ref int i, ref string next, string str, ts.File file)
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

		private static bool analyzeNative(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string nxt, string str, ts.File file)
		{
			if (nxt.StartsWith("native."))
			{
				string after = readNext(str, ref i);
				if (after == "(")
				{
					last = new NativeFCall(nxt.Substring("native.".Length), getStringParams(str, i, vars, file).ToArray());
					i = Parser.getMatchingBraceChar(str, i - 1, ')') + 1;
				}
				else
				{
					if (after != null)
					{
						i -= after.Length;
					}
					last = new NativeExpression(nxt.Substring("native.".Length));
				}
				return true;
			}

			return false;
		}

		private static bool analyzeName(ref Expression last, IEnumerable<typesys.Variable> vars, ref int i, ref string nxt, string str, ts.File file)
		{
			if (analyzeNative(ref last, vars, ref i, ref nxt, str, file))
			{
				return true;
			}

			int ibackup = i;

			string next = nxt;
			int idxDot = nxt.IndexOf('.');
			if (idxDot == 0)
			{
				next = nxt.Substring(1);
				//++i;
				return analyzeName(ref last, vars, ref i, ref next, str, file);
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

				string typeConstr = readNext(str, ref i);
				int backup = i;
				try
				{
					i = solveIdentifierForType(ref last, vars, i, "this", str, file, (typesys.Class)typesys.Class.getExisting(file.getContext(), typeConstr));
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
					int greaterThan = Parser.getMatchingBraceChar(str, i - 1, '>');
					string genpars = str.Substring(backup + 1, greaterThan - backup - 1);
					i = greaterThan + 1;
					i = solveIdentifierForType(ref last, vars, i, "this", str, file, (typesys.Class)typesys.Class.getExisting(file.getContext(), typeConstr + "<" + genpars + ">"));
				}
				FunctionCall flast = (FunctionCall)last;
				typesys.Constructor constr = (typesys.Constructor)flast.refersto;
				last = new ConstructorCall(constr, flast.parameters);
			}
			else if (last is ExprVariable)
			{
				ExprVariable exprLast = (ExprVariable)last;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, exprLast.refersto.type);
			}
			else if (last is ExprType)
			{
				ExprType exprLast = (ExprType)last;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, exprLast.type);
			}
			else if (last is FunctionCall || last is ConstructorCall)
			{
				i = solveIdentifierForType(ref last, vars, i, next, str, file, last.getType());
			}
			else if (vars.Any(x => x.identifier == next))
			{
				// TODO: callable variables
				typesys.Variable v = vars.Single(x => x.identifier == next);
				last = new ExprVariable(v, null);
			}
			else if ((type = typesys.Type.getExisting(file.getContext(), next)) != null)
			{
				last = new ExprType(type);
			}
			else
			{
				// thisvar
				last = ((typesys.Class)file.type).THISVAR;
				i = solveIdentifierForType(ref last, vars, i, next, str, file, (typesys.Type)file.type);
			}

			// recursively parse stuff
			if (idxDot != -1 && idxDot != 0)
			{
				string after = nxt.Substring(idxDot + 1);
				int j = ibackup;
				analyzeName(ref last, vars, ref j, ref after, str, file);
				i = j;
			}

			return true;
		}

		private static int solveIdentifierForType(ref Expression expr, IEnumerable<typesys.Variable> vars, int i, string next, string str, ts.File file, typesys.Type type)
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

			string nextnext = readNext(str, ref i);
			if (nextnext == "(")
			{
				// function
				IEnumerable<Expression> parameters;
				typesys.Function f = getCompatibleFunction(ref i, next, str, file, vars, functions, out parameters);
				FunctionCall call = new FunctionCall(f, expr, parameters);
				expr = call;

				//i -= nextnext.Length;
				//i = Parser.getMatchingBraceChar(str, i - 1, ')') + 1;
			}
			else
			{
				if (next == "this")
				{
					expr = ((typesys.Class)file.type).THISVAR;
				}
				else if (next == "super")
				{
					expr = new SuperVar((typesys.Class)file.type);
				}
				else
				{
					expr = new ExprVariable(field, expr);
					if (nextnext != null)
					{
						i -= nextnext.Length;
					}
				}
			}

			return i;
		}

		private static IEnumerable<Expression> getStringParams(string str, int i, IEnumerable<typesys.Variable> locals, ts.File file)
		{
			int close = Parser.getMatchingBraceChar(str, i - 1, ')');
			string paramstring = str.Substring(i - 1, close - i + 2);
			return Parser.split(paramstring, 0, ')', ',')
							.Select(x => x.Trim())
							.Where(x => !string.IsNullOrEmpty(x))
							.Select(x => Expression.analyze(x, locals, file))
							.Where(x => x != null);
		}

		/// <summary>
		/// Selects the function compatible with the given expressions.
		/// </summary>
		private static typesys.Function getCompatibleFunction(ref int i, string next, string str, ts.File file, IEnumerable<typesys.Variable> locals, IEnumerable<typesys.Function> functions, out IEnumerable<Expression> parameters)
		{
			/*typesys.Function compatible = functions
				.Where(x => x.parameters.Count == parameters.Count())
				.SingleOrDefault(x => areParamsCompatible(parameters, x));*/

			parameters = getStringParams(str, i, locals, file);
			IEnumerable<Expression> _parameters = parameters;
			typesys.Function compatible = functions
				.Where(x => x.parameters.Count == _parameters.Count())
				.Select(x => new Tuple<typesys.Function, int[]>(x, getArrayParams(_parameters, x)))
				.Where(x => !x.Item2.Contains(0))
				.Select(x => new Tuple<typesys.Function, int>(x.Item1, x.Item2.Sum()))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.FirstOrDefault();

			boh.Exception.require<exceptions.ParserException>(compatible != default(typesys.Function), "Method not found: " + next);

			int closeParent = Parser.getMatchingBraceChar(str, i - 1, ')');
			i = closeParent + 1;
			return compatible;
		}

		public static int[] getArrayParams(IEnumerable<Expression> parameters, typesys.Function func)
		{
			return getArrayParams(parameters.Select(x => x.getType()), func);
		}
		public static int[] getArrayParams(IEnumerable<typesys.Type> parameters, typesys.Function func)
		{
			int i = 0;
			int[] result = new int[parameters.Count()];
			using (IEnumerator<typesys.Type> exprsns = parameters.GetEnumerator())
			{
				using (IEnumerator<typesys.Parameter> paramsf = func.parameters.GetEnumerator())
				{
					while (exprsns.MoveNext() && paramsf.MoveNext())
					{
						result[i] = exprsns.Current.extends(paramsf.Current.type);
						++i;
					}
				}
			}
			return result;
		}

		public abstract typesys.Type getType();
		public abstract bool isLvalue();
		public abstract bool isStatement();
	}
}
