using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.boh;
using bohc.exceptions;
using bohc.parsing;
using bohc.parsing.statements;
using bohc.typesys;
using bohc.generation.mangling;

namespace bohc.generation.cs
{
	public class CSCodeGen : ICodeGen
	{
		private readonly IMangler mangler;
		private int indentation = 0;

		public CSCodeGen(IMangler mangler)
		{
			this.mangler = mangler;
		}

		private void addIndent(StringBuilder builder)
		{
			for (int i = 0; i < indentation; ++i)
			{
				builder.Append('\t');
			}
		}
		
		public void generateGeneralBit(IEnumerable<bohc.typesys.Type> types)
		{
		}

		public void generateFor(bohc.typesys.Type t, IEnumerable<bohc.typesys.Type> others)
		{
			throw new System.NotImplementedException();
		}
	
		public void generateFor(typesys.Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine();

			if (c.modifiers.HasFlag(Modifiers.PUBLIC))
			{
				builder.Append("public ");
			}
			if (c.modifiers.HasFlag(Modifiers.PRIVATE))
			{
				builder.Append("internal ");
			}

			if (c.modifiers.HasFlag(Modifiers.ABSTRACT))
			{
				builder.Append("abstract ");
			}
			if (c.modifiers.HasFlag(Modifiers.FINAL))
			{
				builder.Append("sealed ");
			}

			builder.Append("class ");
			builder.Append(mangler.getCName(c));
			if (c.super != null || c.implements.Count > 0)
			{
				builder.Append(" : ");
			}
			if (c.super != null)
			{
				builder.Append(mangler.getCTypeName(c.super));
				if (c.implements.Count > 0)
				{
					builder.Append(", ");
				}
			}
			foreach (Interface iface in c.implements)
			{
				builder.Append(mangler.getCTypeName(iface));
				builder.Append(", ");
			}
			if (c.implements.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			builder.AppendLine();
			builder.AppendLine("{");

			++indentation;

			foreach (Field f in c.fields)
			{
				addIndent(builder);

				addFieldModifiers(c, builder);
				builder.Append(f.identifier);

				if (f.initial != null)
				{
					builder.Append(" = ");
					addExpression(builder, f.initial);
				}

				builder.AppendLine(";");
			}
		}

		private void addFieldModifiers(Class c, StringBuilder builder)
		{
			if (c.modifiers.HasFlag(Modifiers.PUBLIC))
			{
				builder.Append("public ");
			}
			if (c.modifiers.HasFlag(Modifiers.PRIVATE))
			{
				builder.Append("private ");
			}
			if (c.modifiers.HasFlag(Modifiers.PRIVATE))
			{
				builder.Append("protected ");
			}
			if (c.modifiers.HasFlag(Modifiers.FINAL))
			{
				builder.Append("readonly ");
			}
			if (c.modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append("static ");
			}
		}

		private void addBinOp(StringBuilder builder, BinaryOperation binop)
		{
			builder.Append("(");
			addExpression(builder, binop.left);
			builder.Append(" ");
			//builder.Append();
		}

		private void addExpression(StringBuilder builder, Expression expression)
		{
			BinaryOperation binop = expression as BinaryOperation;
			if (binop != null)
			{
				builder.Append("(");
				addBinOp(builder, binop);
				builder.Append(")");
				return;
			}

			/*UnaryOperation unop = expression as UnaryOperation;
			if (unop != null)
			{
				builder.Append("(");
				addUnOp(builder, unop);
				builder.Append(")");
				return;
			}

			FunctionCall fcall = expression as FunctionCall;
			if (fcall != null)
			{
				addFCall(builder, fcall);
				return;
			}

			ConstructorCall ccall = expression as ConstructorCall;
			if (ccall != null)
			{
				addCCall(builder, ccall);
				return;
			}

			Literal lit = expression as Literal;
			if (lit != null)
			{
				addLiteral(builder, lit);
				return;
			}

			ExprVariable exprvar = expression as ExprVariable;
			if (exprvar != null)
			{
				addExprVar(builder, exprvar);
				return;
			}

			NativeExpression nexpr = expression as NativeExpression;
			if (nexpr != null)
			{
				addNativeExpression(builder, nexpr);
				return;
			}

			NativeFCall nfcall = expression as NativeFCall;
			if (nfcall != null)
			{
				addNativeFCall(builder, nfcall);
				return;
			}

			TypeCast tc = expression as TypeCast;
			if (tc != null)
			{
				addTypeCast(builder, tc);
				return;
			}

			FunctionVarCall fvCall = expression as FunctionVarCall;
			if (fvCall != null)
			{
				addFvCall(builder, fvCall);
				return;
			}

			Lambda lambda = expression as Lambda;
			if (lambda != null)
			{
				addLambda(builder, lambda);
				return;
			}

			ExprEnumerator en = expression as ExprEnumerator;
			if (en != null)
			{
				addEnumerator(builder, en);
				return;
			}

			RefExpression refExpr = expression as RefExpression;
			if (refExpr != null)
			{
				addRefExpr(builder, refExpr);
				return;
			}

			NativeMember mem = expression as NativeMember;
			if (mem != null)
			{
				addExpression(builder, mem.owner);
				builder.Append('.');
				builder.Append(mem.representation);
				return;
			}*/

			throw new NotImplementedException();
		}
	}
}

