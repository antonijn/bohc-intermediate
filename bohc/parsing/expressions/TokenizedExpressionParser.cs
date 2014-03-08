using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Parsing.Statements;
using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public class TokenizedExpressionParser
	{
		public TokenizedExpressionParser()
		{
		}

		private Tuple<string, string>[] scopes = new Tuple<string, string>[]
		{
			new Tuple<string, string>("(", ")"),
			new Tuple<string, string>("[", "]"),
			new Tuple<string, string>("{", "}"),
		};

		private IStatementParser istats;

		public void init(IStatementParser statements)
		{
			istats = statements;
		}

		public IStatementParser getStats()
		{
			return istats;
		}

		public int getLambdaStack()
		{
			throw new NotImplementedException();
		}

		public Expression analyze(TokenStream t, IEnumerable<Bohc.TypeSystem.Variable> vars, Bohc.TypeSystem.Function ctx)
		{
			try
			{
				Expression expr = null;
				analyze(ref expr, t, vars, null, ctx);
				return expr;
			}
			catch (TokenException e)
			{
				e.display();
			}
			return null;
		}

		public Expression analyze(TokenStream t, IEnumerable<Bohc.TypeSystem.Variable> vars)
		{
			return analyze(t, vars, null);
		}

		public void analyze(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			while (t.next())
			{
				if (analyzeBrackets(ref expr, t, vars, opprec, ctx)) { continue; }
				if (analyzeLiteral(ref expr, t, vars, opprec, ctx)) { continue; }
				OpBreakStat obs = analyzeOperator(ref expr, t, vars, opprec, ctx);
				if (obs == OpBreakStat.BREAK)
				{
					break;
				}
				else if (obs == OpBreakStat.CONTINUE)
				{
					continue;
				}
				if (analyzeName(ref expr, t, vars, opprec, ctx)) { continue; }
			}
		}

		private bool analyzeName(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			Token to = t.get();

			if (!to.isType(TokenType.IDENTIFIER) && !to.isType(TokenType.PRIMITIVE))
			{
				return false;
			}

			Variable local = vars.SingleOrDefault(x => x.Identifier == to.value);
			if (local != null)
			{
				expr = new ExprVariable(local, null);
				return true;
			}

			Package p = Package.GetFromStringExisting(to.value);
			if (p != Package.Global && p != null)
			{
				expr = new ExprPackage(p);
				return true;
			}

			/*TypeSystem.Type ty = TypeSystem.Type.GetExisting(to.value, getStats().getParser());
			if (ty != null)
			{
				expr = new ExprType(ty);
				return true;
			}*/

			try
			{
				solveIdentifier(ref expr, t, vars, ctx, expr.getType(), true, true);
			}
			catch
			{
				throw new TokenException(to, "invalid identifier in context: '{0}'", to.value);
			}
			return true;
		}

		

		private bool analyzeBrackets(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			if (t.get().value != "(")
			{
				return false;
			}

			t.next();
			analyze(ref expr, t.until(")", scopes), vars, opprec, ctx);
			return true;
		}

		private bool analyzeLiteral(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			Token to = t.get();
			if (to.isType(TokenType.DEC_INTEGER) || to.isType(TokenType.BIN_INTEGER) || to.isType(TokenType.HEX_INTEGER))
			{
				expr = new Literal(Primitive.Int, to.value);
				return true;
			}
			else if (to.isType(TokenType.FLOAT))
			{
				expr = new Literal(Primitive.Float, to.value);
				return true;
			}
			else if (to.isType(TokenType.DOUBLE))
			{
				expr = new Literal(Primitive.Double, to.value);
				return true;
			}
			else if (to.isType(TokenType.BOOLEAN))
			{
				expr = new Literal(Primitive.Boolean, to.value);
				return true;
			}
			else if (to.isType(TokenType.CHAR))
			{
				expr = new Literal(Primitive.Boolean, to.value);
				return true;
			}
			else if (to.isType(TokenType.STRING))
			{
				expr = new Literal(StdType.Str, to.value);
				return true;
			}
			else if (to.isType(TokenType.NULL))
			{
				expr = NullType.Null.DefaultVal();
				return true;
			}
			return false;
		}

		private IEnumerable<Expression> getParameters(TokenStream t, IEnumerable<Variable> vars, Function ctx)
		{
			t.next();
			TokenStream ps = t.until(")", scopes);
			foreach (TokenStream ts in ps.split(",", scopes))
			{
				yield return analyze(ts, vars, ctx);
			}
		}

		private void solveIdentifier(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Function ctx, TypeSystem.Type owner, bool maybeStatic, bool maybeInstance)
		{
			Token to = t.get();

			IEnumerable<Function> functions = owner.GetFunctions(to.value, ctx.Owner)
				.Where(x => (!maybeStatic && x.Modifiers.HasFlag(Modifiers.Static) ||
				        (!maybeInstance && !x.Modifiers.HasFlag(Modifiers.Static)))).ToArray();
			Field field = owner.GetField(to.value, ctx.Owner);
			if (field != null && ((field.Modifiers.HasFlag(Modifiers.Static) && !maybeStatic) ||
			    (!field.Modifiers.HasFlag(Modifiers.Static) && !maybeInstance)))
			{
				field = null;
			}

			if (owner is Bohc.TypeSystem.Enum)
			{
				Bohc.TypeSystem.Enum _enum = (Bohc.TypeSystem.Enum)owner;
				Bohc.TypeSystem.Enumerator enumerator = _enum.Enumerators.SingleOrDefault(x => x.Name == to.value);
				if (enumerator != null)
				{
					expr = new ExprEnumerator(enumerator);
					return;
				}
			}

			Token next = t.peek(1);
			if (next.value == "(" && functions.Count() > 0)
			{
				t.next();

				Expression[] parameters = getParameters(t, vars, ctx).ToArray();
				Function f = Function.GetCompatibleFunction(ctx.Owner.File, vars, functions, ctx, parameters);
				expr = new FunctionCall(f, expr, parameters);
				return;
			}

			if (field != null)
			{
				expr = new ExprVariable(field, expr);
				return;
			}

			throw new Exception();
			//to.error("'{0}' has no member named '{1}'", owner, to.value);
		}

		private bool analyzeDot(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			t.next();

			Token to = t.get();
			if (!to.isType(TokenType.IDENTIFIER))
			{
				throw new TokenException(to, "not an identifier: '{0}'", to.value);
			}

			if (expr is ExprPackage)
			{
				// to.value is either a package or type
				Package p = ((ExprPackage)expr).refersto;

				Package n = Package.GetFromStringExisting(p.ToString() + "." + to.value);
				if (n != Package.Global && n != null)
				{
					expr = new ExprPackage(n);
					return true;
				}

				TypeSystem.Type ty = TypeSystem.Type.GetExisting(p.ToString() + "." + to.value, null /*getStats().getParser()*/);
				if (ty != null)
				{
					expr = new ExprType(ty);
					return true;
				}

				throw new TokenException(to, "package '{0}' has no member named '{1}'", p.ToString(), to.value);
			}

			if (expr is ExprType)
			{
				try
				{
					solveIdentifier(ref expr, t, vars, ctx, expr.getType(), true, false);
				}
				catch
				{
					throw new TokenException(to, "'{0}' has no member named '{1}'", expr.getType().FullName(), to.value);
				}
				return true;
			}

			try
			{
				solveIdentifier(ref expr, t, vars, ctx, expr.getType(), false, true);
			}
			catch
			{
				throw new TokenException(to, "'{0}' has no member named '{1}'", expr.getType().FullName(), to.value);
			}
			return true;
		}

		private enum OpBreakStat
		{
			BREAK,
			CONTINUE,
			NOTHING,
		}
	
		private OpBreakStat analyzeOperator(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			Token to = t.get();
			if (!to.isType(TokenType.OPERATOR))
			{
				return OpBreakStat.NOTHING;
			}

			if (to.value == ".")
			{
				analyzeDot(ref expr, t, vars, opprec, ctx);
				return OpBreakStat.CONTINUE;
			}

			if (expr == null) // unary
			{
				Operator op = Operator.getExisting(to.value, OperationType.UNARY);
				if (opprec == null && op.precedence < opprec.precedence)
				{
					t.prior();
					return OpBreakStat.BREAK;
				}

				Expression right = null;
				analyze(ref right, t, vars, op, ctx);
				expr = new UnaryOperation(right, op);
			}
			else
			{
				if (to.value == "++")
				{
					expr = new UnaryOperation(expr, UnaryOperation.INCREMENT_POST);
					return OpBreakStat.CONTINUE;
				}
				else if (to.value == "--")
				{
					expr = new UnaryOperation(expr, UnaryOperation.DECREMENT_POST);
					return OpBreakStat.CONTINUE;
				}

				Operator op = Operator.getExisting(to.value, OperationType.BINARY);
				if (opprec != null && op.precedence <= opprec.precedence)
				{
					t.prior();
					return OpBreakStat.BREAK;
				}

				Expression right = null;
				analyze(ref right, t, vars, op, ctx);
				expr = expr.useAsLvalue(new BinaryOperation(expr, right, op));
			}

			return OpBreakStat.CONTINUE;
		}
	}
}

