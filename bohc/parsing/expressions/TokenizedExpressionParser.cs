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

		private TokenizedStatementParser istats;
		private int lambdaStack = 0;

		public void init(TokenizedStatementParser statements)
		{
			istats = statements;
		}

		public TokenizedStatementParser getSp()
		{
			return istats;
		}

		public int getLambdaStack()
		{
			return lambdaStack;
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
				if (analyzeIndexer(ref expr, t, vars, opprec, ctx)) { continue; }
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

			if (expr != null)
			{
				try
				{
					expr.useAsRvalue();
				}
				catch
				{
					throw new TokenException(getSp().getFp().getEM(), t.get(), "use of unassigned local variable");
				}
			}
		}

		private bool analyzeIndexer(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			Token to = t.get();
			if (to.value != "[")
			{
				return false;
			}

			Expression[] parameters = getParameters(t, vars, ctx, ']').ToArray();
			IEnumerable<Indexer> funcs = expr.getType().GetFunctions("this", (TypeSystem.Type)ctx.Owner.File.type).OfType<Indexer>().ToArray();
			expr = new IndexerCall(
				funcs.SingleOrDefault(x => !x.IsAssignment()),
				funcs.SingleOrDefault(x => x.IsAssignment()),
				expr, parameters);
			t.prior();
			return true;
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

			TypeSystem.Type ty = TypeSystem.Type.GetExisting(ctx.Owner.File.getContext(), to.value, getSp().getFp());
			if (ty != null)
			{
				expr = new ExprType(ty);
				return true;
			}

			// if nothing prior, it might be a static or instance variable belonging to the class
			if (expr == null)
			{
				try
				{
					expr = new ExprType(ctx.Owner);
					solveIdentifier(ref expr, t, vars, ctx, ctx.Owner, true, false);
					return true;
				}
				catch
				{
					if (!ctx.Modifiers.HasFlag(Modifiers.Static))
					{
						expr = ((Class)ctx.Owner).ThisVarExpr;
						solveIdentifier(ref expr, t, vars, ctx, ctx.Owner, false, true);
						return true;
					}
				}
			}

			try
			{
				solveIdentifier(ref expr, t, vars, ctx, expr.getType(), true, true);
			}
			catch
			{
				throw new TokenException(getSp().getFp().getEM(), to, "invalid identifier in context: '{0}'", to.value);
			}
			return true;
		}

		

		private bool analyzeBrackets(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			if (t.get().value != "(")
			{
				return false;
			}

			ExprVariable ev = expr as ExprVariable;
			if (ev != null)
			{
				if (ev.refersto == ((Class)ctx.Owner).This)
				{
					Expression[] parameters = getParameters(t, vars, ctx).ToArray();
					Function f = Function.GetCompatibleFunction(ctx.Owner.File, vars, ((Class)ctx.Owner).Constructors, ctx, parameters);
					expr = new FunctionCall(f, expr, parameters);
					return true;
				}
				else if (ev.refersto == ((Class)ctx.Owner).SuperVar)
				{
					Expression[] parameters = getParameters(t, vars, ctx).ToArray();
					Function f = Function.GetCompatibleFunction(ctx.Owner.File, vars, ((Class)ctx.Owner).Super.Constructors, ctx, parameters);
					expr = new FunctionCall(f, expr, parameters);
					return true;
				}
			}

			t.next();
			analyze(ref expr, t.until(")", scopes), vars, null, ctx);
			t.prior();

			ExprType type = expr as ExprType;
			if (type != null && opprec != UnaryOperation.TYPEOF)
			{
				// a typecast has operator precedence 8, hence the 8
				// TODO: or is it 7?
				Expression e = null;
				t.prior();
				analyze(ref e, t, vars, UnaryOperation.TYPE_CAST, ctx);
				// only apply if it's actually followed by something
				if (e != null)
				{
					expr = new TypeCast(type.type, e);
				}
			}

			return true;
		}

		private string intToDecStr(Token to)
		{
			string i = to.value;
			if (i.EndsWith("l") || i.EndsWith("L"))
			{
				i = i.Substring(0, i.Length - 1);
			}
			if (to.isType(TokenType.DEC_INTEGER))
			{
				return i;
			}
			bool sign = i.StartsWith("-");
			if (sign)
			{
				i = i.Substring(1);
			}
			if (to.isType(TokenType.BIN_INTEGER))
			{
				i = i.Substring(2);
				return (Convert.ToInt64(i, 2) * (sign ? -1 : 1)).ToString();
			}
			if (to.isType(TokenType.HEX_INTEGER))
			{
				i = i.Substring(2);
				return (Convert.ToInt64(i, 16) * (sign ? -1 : 1)).ToString();
			}
			if (to.isType(TokenType.OCT_INTEGER))
			{
				i = i.Substring(1);
				return (Convert.ToInt64(i, 8) * (sign ? -1 : 1)).ToString();
			}
			throw new NotImplementedException();
		}

		private bool analyzeLiteral(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			Token to = t.get();
			if (to.isType(TokenType.DEC_INTEGER) || to.isType(TokenType.BIN_INTEGER) || to.isType(TokenType.HEX_INTEGER) || to.isType(TokenType.OCT_INTEGER))
			{
				expr = new Literal(Primitive.Int, intToDecStr(to));
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

		private IEnumerable<Expression> getParameters(TokenStream t, IEnumerable<Variable> vars, Function ctx, char close = ')')
		{
			t.next();
			TokenStream ps = t.until(close.ToString(), scopes);
			ps.next();
			TokenStream[] tss = ps.split(",", scopes).ToArray();
			if (tss.Length == 1 && tss.Single().size() == 0)
			{
				yield break;
			}
			foreach (TokenStream ts in tss)
			{
				yield return analyze(ts, vars, ctx);
			}
		}

		private void solveIdentifier(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Function ctx, TypeSystem.Type owner, bool maybeStatic, bool maybeInstance)
		{
			Token to = t.get();

			IEnumerable<Function> functions = owner.GetFunctions(to.value, ctx.Owner)
				.Where(x => (maybeStatic && x.Modifiers.HasFlag(Modifiers.Static) ||
				        (maybeInstance && !x.Modifiers.HasFlag(Modifiers.Static)))).ToArray();
			Field field = owner.GetField(to.value, ctx.Owner);
			if (field != null && !((field.Modifiers.HasFlag(Modifiers.Static) && maybeStatic) ||
			    (!field.Modifiers.HasFlag(Modifiers.Static) && maybeInstance)))
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
				t.prior();
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
				throw new TokenException(getSp().getFp().getEM(), to, "not an identifier: '{0}'", to.value);
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

				throw new TokenException(getSp().getFp().getEM(), to, "package '{0}' has no member named '{1}'", p.ToString(), to.value);
			}

			if (expr is ExprType)
			{
				try
				{
					solveIdentifier(ref expr, t, vars, ctx, expr.getType(), true, false);
				}
				catch
				{
					throw new TokenException(getSp().getFp().getEM(), to, "'{0}' has no member named '{1}'", expr.getType().FullName(), to.value);
				}
				return true;
			}

			try
			{
				solveIdentifier(ref expr, t, vars, ctx, expr.getType(), false, true);
			}
			catch
			{
				throw new TokenException(getSp().getFp().getEM(), to, "'{0}' has no member named '{1}'", expr.getType().FullName(), to.value);
			}
			return true;
		}

		private enum OpBreakStat
		{
			BREAK,
			CONTINUE,
			NOTHING,
		}

		private void analyzeNewOp(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			TokenRange tyr = TokenizedFileParser.readTypeName(getSp().getFp().getEM(), t);
			TypeSystem.Type ty = TypeSystem.Type.GetExisting(ctx.Owner.File.getContext(), tyr.str, getSp().getFp());
			t.next();
			if (t.get().value == "(")
			{
				Expression[] parameters = getParameters(t, vars, ctx).ToArray();
				Constructor c = (Constructor)Function.GetCompatibleFunction(ctx.Owner.File, vars, ((Class)ty).Constructors, ctx, parameters);
				expr = new ConstructorCall(c, parameters);
				t.prior();
			}
			else if (t.get().value == "[")
			{
				Token to = t.peek(1);
				Expression[] parameters = getParameters(t, vars, ctx, ']').ToArray();
				if (parameters.Length > 1 || (parameters.Length == 1 && parameters.Single().getType().Extends(Primitive.Int) == 0))
				{
					throw new TokenException(getSp().getFp().getEM(), to, "array initializers take one or less integer parameters");
				}
				t.prior();
				// TODO: check for '{'
				expr = new ConstructorCall(((Class)StdType.Array.GetTypeFor(new[] { ty }, getSp().getFp()))
					.Constructors.Single(x => x.Parameters.Count == parameters.Length),
					parameters);
			}
			else
			{
				throw new TokenException(getSp().getFp().getEM(), t.get(), "unexpected token '{0}', expected '(' or '['", t.get().value);
			}
		}
	
		private OpBreakStat analyzeOperator(ref Expression expr, TokenStream t, IEnumerable<Variable> vars, Operator opprec, Function ctx)
		{
			BinaryOperation.ADD.GetType();
			UnaryOperation.DECREMENT.GetType();

			Token to = t.get();

			if (to.value == "new")
			{
				analyzeNewOp(ref expr, t, vars, opprec, ctx);
				return OpBreakStat.CONTINUE;
			}

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
				if (opprec != null && op.precedence < opprec.precedence)
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
				if (op == BinaryOperation.ASSIGN)
				{
					right.useAsRvalue();
					expr = expr.useAsLvalue(new BinaryOperation(expr, right, op));
				}
				else
				{
					expr.useAsRvalue();
					expr = new BinaryOperation(expr, right, op);
				}
			}

			return OpBreakStat.CONTINUE;
		}
	}
}

