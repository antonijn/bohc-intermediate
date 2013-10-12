using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.boh;
using bohc.exceptions;
using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.ts;
using bohc.typesys;

namespace bohc
{
	public class CodeGen
	{
		#region Name Transformation

		private static string getIncludeGuardName(typesys.Type type)
		{
			return type.fullName().Replace('.', '_').ToUpperInvariant() + "_H";
		}

		private static string getHeaderName(typesys.Type type)
		{
			return type.fullName().Replace('.', '_').ToLowerInvariant() + ".h";
		}

		private static string getCodeFileName(typesys.Type type)
		{
			return type.fullName().Replace('.', '_').ToLowerInvariant() + ".c";
		}

		private static string getCTypeName(typesys.Type type)
		{
			if (type is Class)
			{
				return getCStructName(type) + " *";
			}

			if (type is Primitive)
			{
				return ((Primitive)type).cname;
			}

			throw new NotImplementedException();
		}

		private static string getCStructName(typesys.Type type)
		{
			return "struct " + getCName(type);
		}

		private static string getCName(typesys.Type type)
		{
			string fname = type.fullName();
			return "c_" + fname.Replace(".", "_p_");
		}

		private static string getNewName(typesys.Type type)
		{
			return "new_" + getCName(type);
		}

		private static string getVarName(Variable variable)
		{
			if (variable is Local)
			{
				return "l_" + variable.identifier;
			}

			if (variable is Parameter)
			{
				return "p_" + variable.identifier;
			}

			if (variable is Field)
			{
				Field f = (Field)variable;
				if (f.modifiers.HasFlag(Modifiers.STATIC))
				{
					return getCName(f.owner) + "_sf_" + variable.identifier;
				}
				return "f_" + variable.identifier;
			}

			throw new NotImplementedException();
		}

		private static string getCFuncName(Function func)
		{
			return getCName(func.owner) + "_m_" + func.identifier;
		}

		private static string getVtableName(Class c)
		{
			return "vtable_" + getCName(c);
		}

		#endregion

		public static void generateFor(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			others = others.Except(new[] { type });

			string header = generateHeader(type, others);
			System.IO.File.WriteAllText(getHeaderName(type), header);

			string code = generateCode(type, others);
			System.IO.File.WriteAllText(getCodeFileName(type), code);
		}

		private static string generateHeader(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			Class c = type as Class;

			if (c != null)
			{
				return generateClassHeader(c, others);
			}

			throw new NotImplementedException();
		}

		private static void addIncludeGuard(StringBuilder builder, typesys.Type type)
		{
			builder.AppendLine("#ifndef " + getIncludeGuardName(type));
			builder.AppendLine("#define " + getIncludeGuardName(type));
		}

		private static void addIncludes(StringBuilder builder, IEnumerable<typesys.Type> others)
		{
			builder.AppendLine("#include <stdint.h>");
			builder.AppendLine("#include <stdbool.h>");
			builder.AppendLine("#include <stddef.h>");

			foreach (typesys.Type type in others)
			{
				builder.AppendLine("#include \"" + getHeaderName(type) + "\"");
			}
		}

		private static void addStructPrototype(StringBuilder builder, typesys.Class c)
		{
			builder.Append(getCStructName(c));
			builder.AppendLine(";");
		}

		private static void addFunctionParams(StringBuilder builder, Function func)
		{
			if (!func.modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append(getCTypeName(func.owner));
				builder.Append(" self, ");
			}

			foreach (Parameter p in func.parameters)
			{
				builder.Append(getCTypeName(p.type));
				builder.Append(" ");
				builder.Append(getVarName(p));
				builder.Append(", ");
			}

			if (!func.modifiers.HasFlag(Modifiers.STATIC) || func.parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			else
			{
				builder.Append("void");
			}
		}

		private static void addFunctionSig(StringBuilder builder, Function func, string prefix)
		{
			builder.Append(prefix);
			builder.Append(getCTypeName(func.returnType));
			builder.Append(" ");
			builder.Append(getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);
			
			builder.Append(");");
			builder.AppendLine();
		}

		private static void addFunctionSigs(StringBuilder builder, IEnumerable<Function> funcs, string prefix)
		{
			foreach (Function f in funcs)
			{
				addFunctionSig(builder, f, prefix);
			}
		}

		private static void addNewOperatorSig(StringBuilder builder, Constructor constr)
		{
			builder.Append("extern ");
			builder.Append(getCTypeName(constr.owner));
			builder.Append(" ");
			builder.Append(getNewName(constr.owner));
			builder.Append("(");
			foreach (Parameter p in constr.parameters)
			{
				builder.Append(getCTypeName(p.type));
				builder.Append(" ");
				builder.Append(getVarName(p));
				builder.Append(", ");
			}
			if (constr.parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			else
			{
				builder.Append("void");
			}
			builder.AppendLine(");");
		}

		private static void addNewOperatorSigs(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.constructors)
			{
				addNewOperatorSig(builder, constr);
			}
		}

		private static void addNewOperator(StringBuilder builder, Constructor constr)
		{
			builder.Append(getCTypeName(constr.owner));
			builder.Append(" ");
			builder.Append(getNewName(constr.owner));
			builder.Append("(");
			foreach (Parameter p in constr.parameters)
			{
				builder.Append(getCTypeName(p.type));
				builder.Append(" ");
				builder.Append(getVarName(p));
				builder.Append(", ");
			}
			if (constr.parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			else
			{
				builder.Append("void");
			}
			builder.AppendLine(")");

			builder.AppendLine("{");

			builder.Append("\t");
			builder.Append(getCTypeName(constr.owner));
			builder.Append(" result = GC_malloc(sizeof(");
			builder.Append(getCStructName(constr.owner));
			builder.AppendLine("));");

			builder.Append("\tresult->vtable = ");
			builder.Append("instance_" + getVtableName(constr.owner));
			builder.AppendLine(";");

			builder.Append("\t");
			builder.Append(getCFuncName(constr));
			builder.Append("(result");

			foreach (Parameter p in constr.parameters)
			{
				builder.Append(", ");
				builder.Append(getVarName(p));
			}

			builder.AppendLine(");");

			builder.AppendLine("\treturn result;");
			builder.AppendLine("}");
		}

		private static void addNewOperators(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.constructors)
			{
				addNewOperator(builder, constr);
			}
		}

		private static void addStaticFieldProto(StringBuilder builder, Field field, string prefix)
		{
			builder.Append(prefix);
			builder.Append(getCTypeName(field.type));
			builder.Append(" ");
			builder.Append(getVarName(field));
			builder.AppendLine(";");
		}

		private static void addStaticFieldProtos(StringBuilder builder, IEnumerable<Field> fields, string prefix)
		{
			foreach (Field f in fields.Where(x => x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				addStaticFieldProto(builder, f, prefix);
			}
		}

		private static void addVtableMembers(StringBuilder builder, typesys.Class c)
		{
			typesys.Class super = c.super;
			if (super != null)
			{
				addVtableMembers(builder, super);
			}

			foreach (Function f in c.functions.Where(x => x.modifiers.HasFlag(Modifiers.VIRTUAL) || x.modifiers.HasFlag(Modifiers.ABSTRACT)))
			{
				builder.Append("\t");
				builder.Append(getCTypeName(f.returnType));
				builder.Append(" (*");
				builder.Append(getCFuncName(f));
				builder.Append(")(");
				addFunctionParams(builder, f);
				builder.AppendLine(");");
			}
		}

		private static void addVtable(StringBuilder builder, Class c)
		{
			builder.Append("struct ");
			builder.AppendLine(getVtableName(c));
			builder.AppendLine("{");
			addVtableMembers(builder, c);
			builder.AppendLine("}");
			builder.Append("extern const instance_");
			builder.Append(getVtableName(c));
			builder.AppendLine(";");
		}

		private static void addStructDefinition(StringBuilder builder, typesys.Class c)
		{
			addVtable(builder, c);

			builder.AppendLine(getCStructName(c));
			builder.AppendLine("{");

			builder.Append("\tstruct ");
			builder.Append(getVtableName(c));
			builder.AppendLine(" * vtable;");

			foreach (Field f in c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				builder.Append("\t");
				builder.Append(getCTypeName(f.type));
				builder.Append(" ");
				builder.Append(getVarName(f));
				builder.AppendLine(";");
			}

			builder.AppendLine("};");
		}

		private static string generateClassHeader(typesys.Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			addIncludeGuard(builder, c);

			addIncludes(builder, others);

			addStructPrototype(builder, c);

			addNewOperatorSigs(builder, c);
			addFunctionSigs(builder, c.functions.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE)), "extern ");
			addStaticFieldProtos(builder, c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE)), "extern ");
			addStructDefinition(builder, c);

			builder.AppendLine("#endif");

			return builder.ToString();
		}

		private static void addVtableInit(StringBuilder builder, Class c)
		{
			Class super = c.super;
			if (super != null)
			{
				addVtableInit(builder, super);
			}

			foreach (Function f in c.functions.Where(x => x.modifiers.HasFlag(Modifiers.ABSTRACT) || x.modifiers.HasFlag(Modifiers.VIRTUAL)))
			{
				if (f.modifiers.HasFlag(Modifiers.ABSTRACT))
				{
					builder.Append("NULL, ");
				}
				else
				{
					builder.Append("&" + getCFuncName(f) + ", ");
				}
			}
		}

		private static void addVtableDefinition(StringBuilder builder, Class c)
		{
			builder.Append("const struct " + getVtableName(c) + " *");
			builder.Append("instance_" + getVtableName(c));
			builder.Append(" = { ");
			addVtableInit(builder, c);
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(" };");
		}

		private static void addFunctionDef(StringBuilder builder, Function func)
		{
			builder.Append(getCTypeName(func.returnType));
			builder.Append(" ");
			builder.Append(getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);

			builder.AppendLine(")");

			addBody(builder, func.body);
		}

		private static int indentation = 0;

		private static void addIndent(StringBuilder builder)
		{
			for (int i = 0; i < indentation; ++i)
			{
				builder.Append("\t");
			}
		}

		private static void addBody(StringBuilder builder, Body body)
		{
			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			foreach (Statement s in body.statements)
			{
				addStatement(builder, s);
			}

			--indentation;
			addIndent(builder);
			builder.AppendLine("}");
		}

		private static Stack<List<string>> tempstack = new Stack<List<string>>();
		private static int tempvcounter = 0;

		private static string addTemp(Expression value)
		{
			List<string> l = tempstack.Peek();

			string name = "temp" + tempvcounter++;

			StringBuilder builder = new StringBuilder();
			builder.Append(getCTypeName(value.getType()));
			builder.Append(" ");
			builder.Append(name);
			builder.Append(";");

			l.Add(builder.ToString());

			return name;
		}

		private static void addExpression(StringBuilder builder, Expression expression)
		{
			BinaryOperation binop = expression as BinaryOperation;
			if (binop != null)
			{
				builder.Append("(");
				addBinOp(builder, binop);
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
		}

		private static void addLiteral(StringBuilder builder, Literal lit)
		{
			typesys.Type type = lit.getType();

			Primitive prim = type as Primitive;
			if (prim != null)
			{
				if (prim == Primitive.CHAR)
				{
					builder.Append("u");
					builder.Append(lit.representation);
				}
				else if (prim == Primitive.BOOLEAN)
				{
					builder.Append(lit.representation);
				}
				else if (prim.isInt())
				{
					long lval = long.Parse(lit.representation);
					builder.Append(lval.ToString("X"));
					if (lval > int.MaxValue)
					{
						builder.Append("L");
					}
				}
				else if (prim.isFloat())
				{
					if (prim == Primitive.DOUBLE)
					{
						builder.Append(lit.representation);
					}
					else
					{
						string str = lit.representation.Substring(0, lit.representation.Length - 1);
						
						builder.Append(str);
						if (!str.Contains('.'))
						{
							builder.Append(".0");
						}
						builder.Append("f");
					}
				}
			}
		}

		private static void addCCall(StringBuilder builder, ConstructorCall ccall)
		{
			builder.Append(getNewName(ccall.function.owner));
			builder.Append("(");

			foreach (Expression param in ccall.parameters)
			{
				addExpression(builder, param);
				builder.Append(", ");
			}

			if (ccall.parameters.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private static void addFCall(StringBuilder builder, FunctionCall fcall)
		{
			if (fcall.refersto.modifiers.HasFlag(Modifiers.VIRTUAL) || fcall.refersto.modifiers.HasFlag(Modifiers.VIRTUAL))
			{
				string temp = addTemp(fcall.belongsto);

				builder.Append("(");
				builder.Append(temp);
				builder.Append(" = ");
				addExpression(builder, fcall.belongsto);
				builder.Append(")->vtable->");

				builder.Append(getCFuncName(fcall.refersto));
				builder.Append("(");
				builder.Append(temp);
				builder.Append(", ");
			}
			else if (!fcall.refersto.modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append(getCFuncName(fcall.refersto));
				builder.Append("(");
				addExpression(builder, fcall.belongsto);
				builder.Append(", ");
			}
			else
			{
				builder.Append(getCFuncName(fcall.refersto));
				builder.Append("(");
			}

			foreach (Expression param in fcall.parameters)
			{
				addExpression(builder, param);
				builder.Append(", ");
			}

			if (!fcall.refersto.modifiers.HasFlag(Modifiers.STATIC) || fcall.parameters.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private static void addBinOp(StringBuilder builder, BinaryOperation binop)
		{
			addExpression(builder, binop.left);
			builder.Append(" ");
			builder.Append(binop.operation);
			builder.Append(" ");
			addExpression(builder, binop.right);
		}

		private static void addExprVar(StringBuilder builder, ExprVariable exprvar)
		{
			if (exprvar.belongsto != null)
			{
				addExpression(builder, exprvar.belongsto);
				builder.Append("->");
			}
			builder.Append(getVarName(exprvar.refersto));
		}

		private static void addStatement(StringBuilder builder, Statement statement)
		{
			tempstack.Push(new List<string>());

			StringBuilder statb = new StringBuilder();

			do
			{
				IfStatement ifs = statement as IfStatement;
				if (ifs != null)
				{
					addIfStat(statb, ifs);
					break;
				}

				WhileStatement whiles = statement as WhileStatement;
				if (whiles != null)
				{
					addWhileStat(statb, whiles);
					break;
				}

				ExpressionStatement estat = statement as ExpressionStatement;
				if (estat != null)
				{
					addEStat(statb, estat);
					break;
				}

				VarDeclaration vdec = statement as VarDeclaration;
				if (vdec != null)
				{
					addVarDec(statb, vdec);
					break;
				}
			}
			while (false);

			foreach (string str in tempstack.Pop())
			{
				addIndent(builder);
				builder.AppendLine(str);
			}

			builder.Append(statb.ToString());
		}

		private static void addVarDec(StringBuilder builder, VarDeclaration vdec)
		{
			addIndent(builder);
			builder.Append(getCTypeName(vdec.refersto.type));
			builder.Append(" ");
			builder.Append(getVarName(vdec.refersto));

			if (vdec.initial != null)
			{
				builder.Append(" = ");
				addExpression(builder, vdec.initial);
			}

			builder.AppendLine(";");
		}

		private static void addEStat(StringBuilder builder, ExpressionStatement estat)
		{
			addIndent(builder);
			addExpression(builder, estat.expression);
			builder.AppendLine(";");
		}

		private static void addWhileStat(StringBuilder builder, WhileStatement stat)
		{
			addIndent(builder);
			builder.Append("while ");
			addExpression(builder, stat.condition);
			builder.AppendLine();
			addBody(builder, stat.body);
		}

		private static void addIfStat(StringBuilder builder, IfStatement stat)
		{
			addIndent(builder);
			builder.Append("if ");
			addExpression(builder, stat.condition);
			builder.AppendLine();
			addBody(builder, stat.body);
		}

		private static void addFunctionDefs(StringBuilder builder, IEnumerable<Function> funcs)
		{
			foreach (Function f in funcs)
			{
				addFunctionDef(builder, f);
			}
		}

		private static string generateCode(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			Class c = type as Class;
			if (c != null)
			{
				return generateClassCode(c, others);
			}

			throw new NotImplementedException();
		}

		private static string generateClassCode(Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(getHeaderName(c));
			builder.AppendLine("\"");

			addStaticFieldProtos(builder, c.fields, string.Empty);
			addFunctionSigs(builder, c.functions.Where(x => x.modifiers.HasFlag(Modifiers.PRIVATE)), string.Empty);
			addVtableDefinition(builder, c);
			addNewOperators(builder, c);
			addFunctionDefs(builder, c.functions);

			return builder.ToString();
		}
	}
}
