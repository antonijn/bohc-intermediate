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
			NativeType nt = type as NativeType;
			if (nt != null)
			{
				return nt.crep;
			}

			if (type is Class || type is Interface)
			{
				return getCStructName(type) + " *";
			}

			if (type is Primitive)
			{
				return ((Primitive)type).cname;
			}

			if (type is NullType)
			{
				return "NULL";
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

			if (variable.identifier == "this")
			{
				return "self";
			}

			throw new NotImplementedException();
		}

		private static string getVFuncName(Function func)
		{
			return "m_" + func.identifier;
		}

		private static string getOpName(Operator op)
		{
			return op.overloadName;
		}

		private static string getCFuncName(Function func)
		{
			if (func is OverloadedOperator)
			{
				return getCName(func.owner) + "_op_" + getOpName(((OverloadedOperator)func).which);
			}
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

			if (!System.IO.File.Exists("boh_internal.h"))
			{
				System.IO.File.Copy(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "boh_internal.h", "boh_internal.h");
			}
		}

		private static string generateHeader(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			Class c = type as Class;

			if (c != null)
			{
				return generateClassHeader(c, others);
			}

			Interface i = type as Interface;
			if (i != null)
			{
				return generateInterfaceHeader(i, others);
			}

			throw new NotImplementedException();
		}

		private static string generateInterfaceHeader(Interface iface, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			addIncludeGuard(builder, iface);
			builder.AppendLine();

			builder.Append(getCStructName(iface));
			builder.AppendLine(";");
			builder.AppendLine();

			addIncludes(builder, others);
			builder.AppendLine();

			addInterfaceNewOpSig(builder, iface);
			builder.AppendLine();

			addInterfaceStruct(builder, iface);
			builder.AppendLine();

			builder.AppendLine("#endif");

			return builder.ToString();
		}

		private static void addInterfaceFunc(StringBuilder builder, Function f)
		{
			builder.Append(getCTypeName(f.returnType));
			builder.Append(" (*");
			builder.Append(getVFuncName(f));
			builder.Append(")(");
			addFunctionParams(builder, f);
			builder.Append(")");
		}

		private static void addInterfaceFuncs(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.implements)
			{
				addInterfaceFuncs(builder, impl);
			}

			foreach (Function f in iface.functions)
			{
				builder.Append("\t");
				addInterfaceFunc(builder, f);
				builder.AppendLine(";");
			}
		}

		private static void addInterfaceStruct(StringBuilder builder, Interface iface)
		{
			builder.AppendLine(getCStructName(iface));
			builder.AppendLine("{");

			builder.Append("\t");
			builder.Append(getCTypeName(StdType.obj));
			builder.AppendLine(" object;");
			addInterfaceFuncs(builder, iface);
			builder.AppendLine("};");
		}

		private static void addInterfaceNewOpParams(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.implements)
			{
				addInterfaceNewOpParams(builder, impl);
			}

			foreach (Function f in iface.functions)
			{
				addInterfaceFunc(builder, f);
				builder.Append(", ");
			}
		}

		private static void addInterfaceNewOpSig(StringBuilder builder, Interface iface)
		{
			builder.Append("extern ");
			builder.Append(getCTypeName(iface));
			builder.Append(" new_");
			builder.Append(getCName(iface));
			builder.Append("(");
			builder.Append(getCTypeName(StdType.obj));
			builder.Append(" object, ");

			addInterfaceNewOpParams(builder, iface);

			builder.Remove(builder.Length - 2, 2);

			builder.AppendLine(");");
		}

		private static void addIncludeGuard(StringBuilder builder, typesys.Type type)
		{
			builder.AppendLine("#ifndef " + getIncludeGuardName(type));
			builder.AppendLine("#define " + getIncludeGuardName(type));
		}

		private static void addIncludes(StringBuilder builder, IEnumerable<typesys.Type> others)
		{
			builder.AppendLine("#include \"boh_internal.h\"");
			builder.AppendLine("#include <stdint.h>");
			builder.AppendLine("#include <stddef.h>");
			builder.AppendLine("#include <uchar.h>");
			builder.AppendLine("#include <setjmp.h>");

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
				if (func.owner is Interface)
				{
					builder.Append(getCTypeName(StdType.obj));
				}
				else
				{
					builder.Append(getCTypeName(func.owner));
				}
				builder.Append(" const self, ");
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

		private static void addTypeOfSig(StringBuilder builder, typesys.Type type)
		{
			string cname = getCName(type);
			string ctype = getCTypeName(StdType.type);

			builder.Append("extern ");
			builder.Append(ctype);
			builder.Append(" typeof_");
			builder.Append(cname);
			builder.AppendLine("(void);");
			
		}

		private static void addTypeOfDef(StringBuilder builder, typesys.Type type)
		{
			string cname = getCName(type);
			string ctype = getCTypeName(StdType.type);

			builder.Append(ctype);
			builder.Append(" typeof_");
			builder.Append(cname);
			builder.AppendLine("(void)");
			builder.AppendLine("{");
			builder.Append("\tstatic ");
			builder.Append(ctype);
			builder.AppendLine(" result = NULL;");
			builder.AppendLine("\tif (result == NULL)");
			builder.AppendLine("\t{");
			// TODO: initialise the type
			builder.AppendLine("\t}");
			builder.AppendLine("\treturn result;");
			builder.Append("}");
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

			builder.Append("\tresult->vtable = &instance_");
			builder.Append(getVtableName((typesys.Class)constr.owner));
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
				builder.Append(getVFuncName(f));
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
			builder.AppendLine("};");
			builder.AppendLine();
			builder.Append("extern const struct ");
			builder.Append(getVtableName(c));
			builder.Append(" instance_");
			builder.Append(getVtableName(c));
			builder.AppendLine(";");
		}

		private static void addStructFields(StringBuilder builder, typesys.Class c)
		{
			if (c.super != null)
			{
				addStructFields(builder, c.super);
			}

			foreach (Field f in c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				builder.Append("\t");
				builder.Append(getCTypeName(f.type));
				builder.Append(" ");
				builder.Append(getVarName(f));
				builder.AppendLine(";");
			}
		}

		private static void addStructDefinition(StringBuilder builder, typesys.Class c)
		{
			addVtable(builder, c);

			builder.AppendLine();

			builder.AppendLine(getCStructName(c));
			builder.AppendLine("{");

			builder.Append("\tconst struct ");
			builder.Append(getVtableName(c));
			builder.AppendLine(" * vtable;");

			addStructFields(builder, c);

			builder.AppendLine("};");
		}

		private static string generateClassHeader(typesys.Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			addIncludeGuard(builder, c);
			builder.AppendLine();

			addStructPrototype(builder, c);
			builder.AppendLine();

			addIncludes(builder, others);
			builder.AppendLine();

			addTypeOfSig(builder, c);
			builder.AppendLine();

			addNewOperatorSigs(builder, c);
			builder.AppendLine();
	
			addFunctionSigs(builder, c.functions.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();
			
			addStaticFieldProtos(builder, c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();
	
			addStructDefinition(builder, c);
			builder.AppendLine();

			builder.AppendLine("#endif");

			return builder.ToString();
		}

		private class FEqualComp : IEqualityComparer<Function>
		{
			public bool Equals(Function f0, Function f1)
			{
				return f0.identifier == f1.identifier &&
					f0.parameters.Select(x => x.type).SequenceEqual(f1.parameters.Select(x => x.type));
			}

			public int GetHashCode(Function f)
			{
				return f.GetHashCode();
			}
		}

		private static void addVtableInit(StringBuilder builder, Class c, IEnumerable<Function> overriden)
		{
			Class super = c.super;
			if (super != null)
			{
				addVtableInit(builder, super, overriden.Union(
					c.super.functions.Where(x => x.modifiers.HasFlag(Modifiers.OVERRIDE)),
					new FEqualComp()));
			}

			foreach (Function f in c.functions.Where(x => x.modifiers.HasFlag(Modifiers.ABSTRACT) || x.modifiers.HasFlag(Modifiers.VIRTUAL)))
			{
				Function overridenf = overriden.SingleOrDefault(x => new FEqualComp().Equals(x, f));
				if (overridenf != null)
				{
					builder.Append("&" + getCFuncName(overridenf) + ", ");
				}
				else
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
		}

		private static void addVtableDefinition(StringBuilder builder, Class c)
		{
			builder.Append("const struct " + getVtableName(c));
			builder.Append(" instance_" + getVtableName(c));
			builder.Append(" = { ");
			addVtableInit(builder, c, c.functions.Where(x => x.modifiers.HasFlag(Modifiers.OVERRIDE)));
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(" };");
		}

		private static void addFunctionDef(StringBuilder builder, Function func)
		{
			if (func.modifiers.HasFlag(Modifiers.PRIVATE))
			{
				builder.Append("static ");
			}
			builder.Append(getCTypeName(func.returnType));
			builder.Append(" ");
			builder.Append(getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);

			builder.AppendLine(")");

			if (func.identifier == "this")
			{
				foreach (Field f in ((typesys.Class)func.owner).fields.Where(x => !x.modifiers.HasFlag(Modifiers.STATIC)))
				{
					ExprVariable ev = new ExprVariable(f, new ThisVar((typesys.Class)func.owner));
					if (f.initial == null)
					{
						// TODO: add default value
						f.initial = f.type.defaultVal();
					}
					BinaryOperation assignment = new BinaryOperation(ev, f.initial, BinaryOperation.ASSIGN);
					func.body.statements.Insert(0, new ExpressionStatement(assignment));
				}
			}
			addBody(builder, func.body);
		}

		private static void addFunctionDefs(StringBuilder builder, IEnumerable<Function> funcs)
		{
			foreach (Function f in funcs)
			{
				addFunctionDef(builder, f);
			}
		}

		#region Misc

		private static int indentation = 0;

		public const string INDENT_SEQ = "\t";

		private static void addIndent(StringBuilder builder)
		{
			for (int i = 0; i < indentation; ++i)
			{
				builder.Append(INDENT_SEQ);
			}
		}

		private static Stack<List<string>> prestatstack = new Stack<List<string>>();
		private static int tempvcounter = 0;

		private static void addPreStatStatement(string txt)
		{
			prestatstack.Peek().Add(txt);
		}

		private static string addTemp(string prefix, string suffix)
		{
			string name = "temp" + tempvcounter++;
			addPreStatStatement(prefix + name + suffix);
			return name;
		}

		private static string addTemp(Expression value)
		{
			return addTemp(getCTypeName(value.getType()) + " ", ";");
		}

		#endregion

		#region Expressions

		private static void addInterfaceNewCallFunc(StringBuilder builder, Function f, string tempn)
		{
			builder.Append("&");
			if (f.modifiers.HasFlag(Modifiers.OVERRIDE) || f.modifiers.HasFlag(Modifiers.VIRTUAL) || f.modifiers.HasFlag(Modifiers.ABSTRACT))
			{
				builder.Append(tempn);
				builder.Append("->vtable->");
				builder.Append(getVFuncName(f));
			}
			else
			{
				builder.Append(getCFuncName(f));
			}
		}

		private static void addInterfaceNewCallFuncs(StringBuilder builder, Interface iface, Class c, string tempn)
		{
			// leaves ", " at the end of the string

			foreach (Interface i in iface.implements)
			{
				addInterfaceNewCallFuncs(builder, i, c, tempn);
			}

			foreach (Function other in iface.functions)
			{
				foreach (Function f in c.getAllFuncs())
				{
					if (f.identifier == other.identifier &&
						f.modifiers.HasFlag(Modifiers.PUBLIC) &&
						f.parameters.Select(x => x.type).SequenceEqual(other.parameters.Select(x => x.type)))
					{
						addInterfaceNewCallFunc(builder, f, tempn);
						builder.Append(", ");
						break;
					}
				}
			}
		}

		private static void addExpressionImplCon(StringBuilder builder, Expression expression, typesys.Type towhat)
		{
			typesys.Type type = expression.getType();
			if (type == towhat)
			{
				addExpression(builder, expression);
			}
			else if (type is Interface && towhat is Class)
			{
				builder.Append("(");
				builder.Append(getCTypeName(towhat));
				builder.Append(")");
				addExpression(builder, expression);
				builder.Append("->object");
			}
			else if (type is Class && towhat is Interface)
			{
				string tempn = addTemp(getCTypeName(type), ";");

				Interface towi = towhat as Interface;
				builder.Append("new_");
				builder.Append(getCName(towhat));
				builder.Append("(");
				builder.Append(tempn);
				builder.Append(" = (");
				addExpression(builder, expression);
				builder.Append("), ");
				addInterfaceNewCallFuncs(builder, towi, type as Class, tempn);
				builder.Remove(builder.Length - 2, 2);
				builder.Append(")");
			}
			else
			{
				builder.Append("(");
				builder.Append(getCTypeName(towhat));
				builder.Append(")(");
				addExpression(builder, expression);
				builder.Append(")");
			}
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

			UnaryOperation unop = expression as UnaryOperation;
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
		}

		private static int getStrLitLen(string str)
		{
			int len = 0;
			for (int i = 0; i < str.Length; ++i)
			{
				char ch = str[i];
				if (ch == '\\')
				{
					if (++i >= str.Length || str[i] != '\\')
					{
					}
					else
					{
						--i;
					}
				}
				++len;
			}
			return len - 2;
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
					builder.Append(lit.representation == "true" ? 1 : 0);
				}
				else if (prim.isInt())
				{
					long lval = long.Parse(lit.representation);
					builder.Append(lval.ToString());
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
			else
			{
				if (lit.type == StdType.str && lit.representation != "NULL")
				{
					builder.Append("boh_create_string(u");
					builder.Append(lit.representation);
					builder.Append(", ");
					builder.Append(getStrLitLen(lit.representation));
					builder.Append(")");
				}
				else
				{
					builder.Append(lit.representation);
				}
			}
		}

		private static void addCCall(StringBuilder builder, ConstructorCall ccall)
		{
			builder.Append(getNewName(ccall.function.owner));
			builder.Append("(");

			IEnumerator<Parameter> fparams = ccall.function.parameters.GetEnumerator();
			foreach (Expression param in ccall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, fparams.Current.type);
				builder.Append(", ");
			}

			if (ccall.parameters.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private static void addNativeExpression(StringBuilder builder, NativeExpression nexpr)
		{
			builder.Append(nexpr.str);
		}

		private static void addNativeFCall(StringBuilder builder, NativeFCall fcall)
		{
			builder.Append(fcall.str);
			builder.Append("(");
			foreach (Expression e in fcall.parameters)
			{
				addExpression(builder, e);
				builder.Append(", ");
			}

			if (fcall.parameters.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private static void addFCall(StringBuilder builder, FunctionCall fcall)
		{
			if (fcall.refersto.modifiers.HasFlag(Modifiers.VIRTUAL) ||
				fcall.refersto.modifiers.HasFlag(Modifiers.ABSTRACT) ||
				fcall.refersto.modifiers.HasFlag(Modifiers.OVERRIDE))
			{
				if (fcall.belongsto is SuperVar)
				{
					builder.Append("instance_");
					builder.Append(getVtableName((Class)fcall.belongsto.getType()));
					builder.Append(".");
					builder.Append(getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append("self, ");
				}
				else
				{
					string temp = addTemp(fcall.belongsto);

					builder.Append("(");
					builder.Append(temp);
					builder.Append(" = ");
					addExpression(builder, fcall.belongsto);
					builder.Append(")->vtable->");

					builder.Append(getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append(temp);
					builder.Append(", ");
				}
			}
			else if (!fcall.refersto.modifiers.HasFlag(Modifiers.STATIC))
			{
				if (fcall.refersto.owner is Class)
				{
					builder.Append(getCFuncName(fcall.refersto));
					builder.Append("(");
					addExpression(builder, fcall.belongsto);
					builder.Append(", ");
				}
				else
				{
					string temp = addTemp(fcall.belongsto);
					builder.Append("(");
					builder.Append(temp);
					builder.Append(" = ");
					addExpression(builder, fcall.belongsto);
					builder.Append(")->");
					builder.Append(getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append(temp);
					builder.Append("->object, ");
				}
			}
			else
			{
				builder.Append(getCFuncName(fcall.refersto));
				builder.Append("(");
			}

			IEnumerator<Parameter> fparams = fcall.refersto.parameters.GetEnumerator();
			foreach (Expression param in fcall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, fparams.Current.type);
				builder.Append(", ");
			}

			if (!fcall.refersto.modifiers.HasFlag(Modifiers.STATIC) || fcall.parameters.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private static void addEqualsBinOp(StringBuilder builder, Expression left, Expression right)
		{
			Function objEq = StdType.obj.functions.Single(x => x.identifier == "valEquals");
			builder.Append(getCFuncName(objEq));
			builder.Append("(");
			addExpression(builder, left);
			builder.Append(", ");
			addExpression(builder, right);
			builder.Append(")");
		}

		private static string getOpRep(Operator op)
		{
			if (op == BinaryOperation.R_EQ)
			{
				return "==";
			}

			return op.representation;
		}

		private static void addBinOp(StringBuilder builder, BinaryOperation binop)
		{
			if (binop.operation == BinaryOperation.EQUAL)
			{
				addEqualsBinOp(builder, binop.left, binop.right);
			}
			else if (binop.overloaded != null)
			{
				builder.Append(getCFuncName(binop.overloaded));
				builder.Append("(");
				addExpression(builder, binop.left);
				builder.Append(", ");
				addExpression(builder, binop.right);
				builder.Append(")");
			}
			else
			{
				addExpression(builder, binop.left);
				builder.Append(" ");
				builder.Append(getOpRep(binop.operation));
				builder.Append(" ");
				if (binop.isAssignment())
				{
					addExpressionImplCon(builder, binop.right, binop.left.getType());
				}
				else
				{
					addExpression(builder, binop.right);
				}
			}
		}

		private static void addTypeOf(StringBuilder builder, typesys.Type type)
		{
			builder.Append("typeof_");
			builder.Append(getCName(type));
			builder.Append("()");
		}

		private static void addUnOp(StringBuilder builder, UnaryOperation unop)
		{
			if (unop.operation == UnaryOperation.TYPEOF)
			{
				addTypeOf(builder, (unop.onwhat as ExprType).type);
			}
			else if (unop.operation == UnaryOperation.INCREMENT_POST ||
				unop.operation == UnaryOperation.DECREMENT_POST)
			{
				addExpression(builder, unop.onwhat);
				builder.Append(unop.operation);
			}
			else
			{
				builder.Append(unop.operation);
				addExpression(builder, unop.onwhat);
			}
		}

		private static void addExprVar(StringBuilder builder, ExprVariable exprvar)
		{
			if (exprvar.belongsto is ExprVariable)
			{
				addExpression(builder, exprvar.belongsto);
				builder.Append("->");
			}
			builder.Append(getVarName(exprvar.refersto));
		}

		#endregion

		#region Statements

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

		private static bool addSpecStat<T>(StringBuilder builder, Statement stat, Action<StringBuilder, T> act)
			where T : Statement
		{
			T spec = stat as T;
			if (spec != null)
			{
				act(builder, spec);
				return true;
			}

			return false;
		}

		private static void addStatement(StringBuilder builder, Statement statement)
		{
			prestatstack.Push(new List<string>());

			StringBuilder statb = new StringBuilder();

			do
			{
				if (addSpecStat<IfStatement>(statb, statement, addIfStat)) { break; }
				if (addSpecStat<WhileStatement>(statb, statement, addWhileStat)) { break; }
				if (addSpecStat<DoWhileStatement>(statb, statement, addDoWhile)) { break; }
				if (addSpecStat<ForStatement>(statb, statement, addFor)) { break; }
				if (addSpecStat<ExpressionStatement>(statb, statement, addEStat)) { break; }
				if (addSpecStat<VarDeclaration>(statb, statement, addVarDec)) { break; }
				if (addSpecStat<ReturnStatement>(statb, statement, addReturn)) { break; }
				if (addSpecStat<BreakStatement>(statb, statement, (x, y) => addSingleString(x, "break;"))) { break; }
				if (addSpecStat<ContinueStatement>(statb, statement, (x, y) => addSingleString(x, "continue;"))) { break; }
				if (addSpecStat<TryStatement>(statb, statement, addTry)) { break; }
				if (addSpecStat<ThrowStatement>(statb, statement, addThrow)) { break; }
			}
			while (false);

			foreach (string str in prestatstack.Pop())
			{
				addIndent(builder);
				builder.AppendLine(str);
			}

			builder.Append(statb.ToString());
		}

		private static void addThrow(StringBuilder builder, ThrowStatement thr)
		{
			addIndent(builder);
			builder.Append("boh_throw_ex(");
			addExpression(builder, thr.exception);
			builder.AppendLine(");");
		}

		private static void addReturn(StringBuilder builder, ReturnStatement ret)
		{
			addIndent(builder);
			builder.Append("return ");
			addExpression(builder, ret.returns);
			builder.AppendLine(";");
		}

		// used for continue and break, usage otherwise discouraged
		private static void addSingleString(StringBuilder builder, string str)
		{
			addIndent(builder);
			builder.AppendLine(str);
		}

		private static void addCatch(StringBuilder builder, CatchStatement cstat, string finname)
		{
			builder.Append("if (exception_type == typeof_");
			builder.Append(getCName(cstat.param.type));
			builder.AppendLine("())");

			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			string typeusename = getCTypeName(cstat.param.type);
			addIndent(builder);
			builder.Append(typeusename);
			builder.Append(" ");
			builder.Append(getVarName(cstat.param));
			builder.Append(" = (");
			builder.Append(typeusename);
			builder.AppendLine(")exception;");

			addBody(builder, cstat.body);

			if (finname != null)
			{
				addIndent(builder);
				builder.Append(finname);
				builder.AppendLine("();");
			}

			--indentation;
			addIndent(builder);
			builder.AppendLine("}");
			addIndent(builder);
			builder.Append("else ");
		}

		private static void addTry(StringBuilder builder, TryStatement trys)
		{
			string finname = null;
			if (trys.fin != null)
			{
				addIndent(builder);
				finname = "temp" + tempvcounter++;
				builder.AppendLine("void " + finname + "(void)");

				addBody(builder, trys.fin.body);
			}

			addIndent(builder);
			string tempname = addTemp("jmp_buf ", ";");
			builder.Append("memcpy(&");
			builder.Append(tempname);
			builder.AppendLine(", &exception_buf, sizeof(jmp_buf));");
			
			addIndent(builder);
			builder.AppendLine("if (setjmp(exception_buf) == 0)");
			
			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			addBody(builder, trys.body);

			addJmpReset(builder, tempname);

			if (finname != null)
			{
				addIndent(builder);
				builder.Append(finname);
				builder.AppendLine("();");
			}

			--indentation;
			addIndent(builder);
			builder.AppendLine("}");

			addIndent(builder);
			builder.AppendLine("else");
			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			addJmpReset(builder, tempname);

			addIndent(builder);
			foreach (CatchStatement cstat in trys.catches)
			{
				addCatch(builder, cstat, finname);
			}

			builder.AppendLine();
			addIndent(builder);
			builder.AppendLine("{");
			++indentation;
			if (finname != null)
			{
				addIndent(builder);
				builder.Append(finname);
				builder.AppendLine("();");
			}
			addIndent(builder);
			builder.AppendLine("longjmp(exception_buf, 1);");
			--indentation;
			addIndent(builder);
			builder.AppendLine("}");

			--indentation;
			addIndent(builder);
			builder.AppendLine("}");
		}

		private static void addJmpReset(StringBuilder builder, string tempname)
		{
			addIndent(builder);
			builder.Append("memcpy(&exception_buf, &");
			builder.Append(tempname);
			builder.AppendLine(", sizeof(jmp_buf));");
		}

		private static void addDoWhile(StringBuilder builder, DoWhileStatement dostat)
		{
			addIndent(builder);
			builder.AppendLine("do");
			addBody(builder, dostat.body);
			addIndent(builder);
			builder.Append("while (");
			addExpression(builder, dostat.condition);
			builder.AppendLine(");");
		}

		private static void addFor(StringBuilder builder, ForStatement forstat)
		{
			addIndent(builder);
			builder.Append("for (");

			StringBuilder sbuild = new StringBuilder();
			addStatement(sbuild, forstat.initial);
			string str = sbuild.ToString().Trim();
			builder.Append(str);
			builder.Append(" ");

			addExpression(builder, forstat.condition);
			builder.Append("; ");

			sbuild = new StringBuilder();
			addStatement(sbuild, forstat.final);
			str = sbuild.ToString().Trim();
			builder.Append(str.Substring(0, str.Length - 1));
			builder.AppendLine(")");

			addBody(builder, forstat.body);
		}

		private static void addElse(StringBuilder builder, ElseStatement elsestat)
		{
			addIndent(builder);
			builder.AppendLine("else");
			addBody(builder, elsestat.body);
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
				addExpressionImplCon(builder, vdec.initial, vdec.refersto.type);
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
			builder.Append("while (");
			addExpression(builder, stat.condition);
			builder.AppendLine(")");
			addBody(builder, stat.body);
		}

		private static void addIfStat(StringBuilder builder, IfStatement stat)
		{
			addIndent(builder);
			builder.Append("if (");
			addExpression(builder, stat.condition);
			builder.AppendLine(")");
			addBody(builder, stat.body);
			if (stat.elsestat != null)
			{
				addElse(builder, stat.elsestat);
			}
		}

		#endregion

		private static string generateCode(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			Class c = type as Class;
			if (c != null)
			{
				return generateClassCode(c, others);
			}

			Interface i = type as Interface;
			if (i != null)
			{
				return generateInterfaceCode(i, others);
			}

			throw new NotImplementedException();
		}

		private static void addInterfaceConstructor(StringBuilder builder, Interface iface)
		{
			builder.Append(getCTypeName(iface));
			builder.Append(" new_");
			builder.Append(getCName(iface));
			builder.Append("(");
			builder.Append(getCTypeName(StdType.obj));
			builder.Append(" object, ");
			addInterfaceNewOpParams(builder, iface);
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(")");
			builder.AppendLine("{");
			builder.Append("\t");
			builder.Append(getCTypeName(iface));
			builder.Append(" result = GC_malloc(sizeof(");
			builder.Append(getCStructName(iface));
			builder.AppendLine("));");
			builder.AppendLine("\tresult->object = object;");
			addInterfaceAssignments(builder, iface);
			builder.AppendLine("\treturn result;");
			builder.AppendLine("}");
		}

		private static void addInterfaceAssignments(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.implements)
			{
				addInterfaceAssignments(builder, impl);
			}

			foreach (Function f in iface.functions)
			{
				builder.Append("\tresult->");
				builder.Append(getVFuncName(f));
				builder.Append(" = ");
				builder.Append(getVFuncName(f));
				builder.AppendLine(";");
			}
		}

		private static string generateInterfaceCode(Interface iface, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(getHeaderName(iface));
			builder.AppendLine("\"");
			builder.AppendLine();

			addInterfaceConstructor(builder, iface);

			return builder.ToString();
		}

		private static string generateClassCode(Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(getHeaderName(c));
			builder.AppendLine("\"");
			builder.AppendLine();

			addStaticFieldProtos(builder, c.fields, string.Empty);
			builder.AppendLine();

			addFunctionSigs(builder, c.functions.Where(x => x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "static ");
			builder.AppendLine();
			
			addVtableDefinition(builder, c);
			builder.AppendLine();

			addTypeOfDef(builder, c);
			builder.AppendLine();

			addNewOperators(builder, c);
			builder.AppendLine();
			
			addFunctionDefs(builder, c.functions.Where(x => !x.modifiers.HasFlag(Modifiers.ABSTRACT)));

			return builder.ToString();
		}
	}
}
