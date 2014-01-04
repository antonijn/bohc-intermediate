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

using bohc.boh;
using bohc.exceptions;
using bohc.parsing;
using bohc.parsing.statements;
using bohc.typesys;
using bohc.generation.mangling;

namespace bohc.generation.c
{
	public class CCodeGen : ICodeGen
	{
		private readonly IMangler mangler;

		public CCodeGen(IMangler mangler)
		{
			this.mangler = mangler;
		}

		public void generateGeneralBit(IEnumerable<typesys.Type> others)
		{
			//Primitive.figureOutFunctionsForAll();
			System.IO.Directory.CreateDirectory(".c");

			if (!System.IO.File.Exists(".c/boh_internal.h"))
			{
				System.IO.File.Copy(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "boh_internal.h", ".c/boh_internal.h");
			}
			if (!System.IO.File.Exists(".c/boh_internal.c"))
			{
				System.IO.File.Copy(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "boh_internal.c", ".c/boh_internal.c");
			}

			generateFunctionRefTypes(others);
		}

		// associates lambdas with the names of their context types
		private readonly Dictionary<Lambda, string> lambdaCtxNames = new Dictionary<Lambda, string>();
		// unanonyfies the lambdas
		private readonly Dictionary<Lambda, string> lambdaNames = new Dictionary<Lambda, string>();

		private void generateFunctionRefTypes(IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("#pragma once");
			builder.AppendLine("#include \"all.h\"");

			builder.AppendLine("struct boh_fp_null_type { void *one; void *two; };");
			builder.AppendLine("extern struct boh_fp_null_type BOH_FP_NULL;");

			foreach (FunctionRefType fRefType in FunctionRefType.instances)
			{
				//addFunctionRefTypeDef(builder, fRefType);
			}

			generateLambdas(builder);

			System.IO.File.WriteAllText(".c/function_types.h", builder.ToString());

			builder.Clear();

			builder.AppendLine("#include \"function_types.h\"");
			builder.AppendLine("struct boh_fp_null_type BOH_FP_NULL = { NULL, NULL };");
			foreach (Lambda l in Lambda.lambdas)
			{
				lambdaStack = l.lambdaLevel;
				builder.Append(mangler.getCTypeName(l.type.retType)).Append(" " + lambdaNames[l]).Append("(struct ").Append(lambdaCtxNames[l] + " * ctx, ");
				foreach (LambdaParam lParam in l.lambdaParams)
				{
					builder.Append(mangler.getCTypeName(lParam.type)).Append(" ").Append(mangler.getVarName(lParam)).Append(", ");
				}
				builder.Remove(builder.Length - 2, 2);
				builder.AppendLine(")");


				if (l.body != null)
				{
					addFnHeapParams(builder, l.lambdaParams);
					addBody(builder, l.body);
					addFnHeapParamClose(builder, l.lambdaParams);
				}
				else
				{
					//builder.AppendLine("{");

					//++indentation;
					addFnHeapParams(builder, l.lambdaParams);
					if (l.type.retType == Primitive.VOID)
					{
						addIndent(builder);
						builder.AppendLine("{");
						++indentation;
						addStatement(builder, new ExpressionStatement(l.expression));
						--indentation;
						addIndent(builder);
						builder.AppendLine("}");
					}
					else
					{
						addIndent(builder);
						builder.AppendLine("{");
						++indentation;
						addStatement(builder, new ReturnStatement(l.expression));
						--indentation;
						addIndent(builder);
						builder.AppendLine("}");
					}
					//--indentation;
					//builder.AppendLine("}");
					addFnHeapParamClose(builder, l.lambdaParams);
				}
			}
			lambdaStack = 0;
			System.IO.File.WriteAllText(".c/function_types.c", builder.ToString());
		}

		private void addFunctionRefTypeDef(StringBuilder builder, FunctionRefType fRefType)
		{
			builder.Append("struct ");
			builder.AppendLine(mangler.getCName(fRefType));
			builder.AppendLine("{");

			++indentation;
			addIndent(builder);
			builder.Append(mangler.getCTypeName(fRefType.retType)).Append(" (* function)(void *, ");

			foreach (typesys.Type t in fRefType.paramTypes)
			{
				builder.Append(mangler.getCTypeName(t)).Append(", ");
			}

			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(");");

			addIndent(builder);
			builder.AppendLine("void * context;");

			--indentation;
			builder.AppendLine("};");
		}

		private void generateLambdas(StringBuilder builder)
		{
			int i = 0;
			foreach (Lambda l in Lambda.lambdas)
			{
				//figureOutEnclosed(l);

				string ctxName = "lmbd_ctx_" + lambdaCtxNames.Count;
				lambdaCtxNames[l] = ctxName;

				builder.AppendLine("struct " + ctxName);
				builder.AppendLine("{");

				++indentation;
				foreach (Variable v in l.enclosed)
				{
					addIndent(builder);
					builder.Append(mangler.getCTypeName(v.type));
					builder.Append(" ");
					builder.Append(mangler.getHeapVarDeclName(v));
					builder.AppendLine(";");
				}
				--indentation;
				builder.AppendLine("};");

				lambdaNames[l] = "l" + i.ToString();

				builder.Append(mangler.getCTypeName(l.type.retType));
				builder.Append(" l" + (i++).ToString());
				builder.Append("(struct ");
				builder.Append(ctxName + " * ctx, ");
				foreach (LambdaParam lParam in l.lambdaParams)
				{
					builder.Append(mangler.getParamTypeName(lParam));
					builder.Append(" ");
					builder.Append(mangler.getVarName(lParam));
					builder.Append(", ");
				}
				builder.Remove(builder.Length - 2, 2);
				builder.AppendLine(");");
			}
		}

		private class DependencyBasedTypeSorter : IComparer<typesys.Type>
		{
			private static bool containsTypeOther(typesys.Type type, typesys.Type other)
			{
				Struct str = type as Struct;
				if (str != null)
				{
					return containsTypeOther(str, other);
				}

				FunctionRefType frt = type as FunctionRefType;
				if (frt != null)
				{
					return containsTypeOther(frt, other);
				}

				return false;
			}

			private static bool containsTypeOther(Struct str, typesys.Type other)
			{
				foreach (Field f in str.fields)
				{
					if (f.type == other)
					{
						return true;
					}

					if (containsTypeOther(f.type, other))
					{
						return true;
					}
				}
				return false;
			}

			private static bool containsTypeOther(FunctionRefType frt, typesys.Type other)
			{
				return containsTypeOther(frt.retType, other) || frt.paramTypes.Any(x => containsTypeOther(x, other));
			}

			int IComparer<typesys.Type>.Compare(typesys.Type l, typesys.Type r)
			{
				if (containsTypeOther(l, r))
				{
					return -1;
				}

				return 0;
			}
		}

		public void finish(IEnumerable<typesys.Type> types)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("#pragma once");
			builder.AppendLine("#include \"boh_internal.h\"");
			builder.AppendLine("#include <gc/gc.h>");
			builder.AppendLine("#include <stdint.h>");
			builder.AppendLine("#include <stddef.h>");
			builder.AppendLine("#include <setjmp.h>");
			builder.AppendLine("#include \"function_types.h\"");

			foreach (typesys.Type c in types.Where(x => (x is Class && !(x is Struct)) || x is Interface))
			{
				addStructPrototype(builder, c);
			}

			foreach (typesys.Type t in types.Where(x => (!(x is Class) || x is Struct) && !(x is Interface)).Concat(FunctionRefType.instances).OrderBy(x => x, new DependencyBasedTypeSorter()))
			{
				if (t is Struct)
				{
					builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".type.h");
					builder.AppendLine();
				}
				else
				{
					addFunctionRefTypeDef(builder, t as FunctionRefType);
				}
			}

			foreach (typesys.Type t in types)
			{
				builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".type.h");
				builder.AppendLine();

				builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".sigs.h");
				builder.AppendLine();
				builder.AppendLine();
			}

			System.IO.File.WriteAllText(".c/all.h", builder.ToString());
		}

		public void generateFor(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			others = others.Except(new[] { type });

			generateHeaders(type, others);

			string code = generateCode(type, others);
			System.IO.File.WriteAllText(".c/" + mangler.getCodeFileName(type), code);
		}

		private void generateHeaders(typesys.Type type, IEnumerable<typesys.Type> others)
		{
			Struct s = type as Struct;
			if (s != null)
			{
				generateStructHeaders(s, others);
				return;
			}

			Class c = type as Class;
			if (c != null)
			{
				generateClassHeaders(c, others);
				return;
			}

			Interface i = type as Interface;
			if (i != null)
			{
				generateInterfaceHeaders(i, others);
				return;
			}

			typesys.Enum e = type as typesys.Enum;
			if (e != null)
			{
				generateEnumHeaders(e, others);
				return;
			}

			throw new NotImplementedException();
		}

		private void generateEnumHeaders(typesys.Enum e, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			addIncludeGuard(builder, e);
			builder.AppendLine();

			builder.AppendLine(mangler.getCStructName(e));
			builder.AppendLine("{");
			
			++indentation;

			int i = 0;
			foreach (Enumerator enumerator in e.enumerators)
			{
				addIndent(builder);
				builder.Append(mangler.getEnumeratorName(enumerator));
				builder.Append(" = ");
				builder.Append(i++);
				builder.AppendLine(",");
			}

			if (e.enumerators.Count > 0)
			{
				builder.Remove(builder.Length - Environment.NewLine.Length - 1, 1);
			}

			builder.AppendLine("};");

			--indentation;

			System.IO.File.WriteAllText(".c/" + mangler.getCName(e) + ".enum.h", builder.ToString());
		}

		private void generateInterfaceHeaders(Interface iface, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			addIncludeGuard(builder, iface);
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();

			addInterfaceNewOpSig(builder, iface);
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(iface) + ".sigs.h", builder.ToString());
			builder.Clear();

			addIncludeGuard(builder, iface);
			builder.AppendLine();

			builder.Append(mangler.getCStructName(iface));
			builder.AppendLine(";");
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();
			
			addInterfaceStruct(builder, iface);
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(iface) + ".type.h", builder.ToString());

		}

		private void addInterfaceFunc(StringBuilder builder, Function f)
		{
			builder.Append(mangler.getCTypeName(f.returnType));
			builder.Append(" (*");
			builder.Append(mangler.getVFuncName(f));
			builder.Append(")(");
			addFunctionParams(builder, f);
			builder.Append(")");
		}

		private void addInterfaceFuncs(StringBuilder builder, Interface iface)
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

		private void addInterfaceStruct(StringBuilder builder, Interface iface)
		{
			builder.AppendLine(mangler.getCStructName(iface));
			builder.AppendLine("{");

			builder.Append("\t");
			builder.Append(mangler.getCTypeName(StdType.obj));
			builder.AppendLine(" object;");
			addInterfaceFuncs(builder, iface);
			builder.AppendLine("};");
		}

		private void addInterfaceNewOpParams(StringBuilder builder, Interface iface)
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

		private void addInterfaceNewOpSig(StringBuilder builder, Interface iface)
		{
			builder.Append("extern ");
			builder.Append(mangler.getCTypeName(iface));
			builder.Append(" new_");
			builder.Append(mangler.getCName(iface));
			builder.Append("(");
			builder.Append(mangler.getCTypeName(StdType.obj));
			builder.Append(" object, ");

			addInterfaceNewOpParams(builder, iface);

			builder.Remove(builder.Length - 2, 2);

			builder.AppendLine(");");
		}

		private void addIncludeGuard(StringBuilder builder, typesys.Type type)
		{
			builder.AppendLine("#pragma once");
		}

		private void addStructPrototypes(StringBuilder builder, IEnumerable<typesys.Type> others)
		{
			builder.AppendLine("#include \"all.h\"");
			/*builder.AppendLine("#include \"boh_internal.h\"");
			builder.AppendLine("#include \"function_types.h\"");
			builder.AppendLine("#include <gc/gc.h>");
			builder.AppendLine("#include <stdint.h>");
			builder.AppendLine("#include <stddef.h>");
			builder.AppendLine("#include <uchar.h>");
			builder.AppendLine("#include <setjmp.h>");

			foreach (typesys.Type type in others)
			{
				addStructPrototype(builder, type);
			}*/
		}

		/*private void addIncludesOnlySpecified(StringBuilder builder, IEnumerable<typesys.Type> others)
		{
			foreach (typesys.Type type in others.OrderBy(x => x.fullName()))
			{
				builder.AppendLine("#include \"" + mangler.getHeaderName(type) + "\"");
			}
		}*/

		private void addIncludes(StringBuilder builder, IEnumerable<typesys.Type> others)
		{
			//addIncludesOnlySpecified(builder, others);
			builder.AppendLine("#include \"all.h\"");
		}

		private void addStructPrototype(StringBuilder builder, typesys.Type c)
		{
			if (c is Struct)
			{
				builder.Append("#include \"");
				builder.Append(mangler.getHeaderName(c));
				builder.AppendLine("\"");
			}
			else
			{
				builder.Append(mangler.getCStructName(c));
				builder.AppendLine(";");
			}
		}

		private void addFunctionParams(StringBuilder builder, Function func)
		{
			if (!func.modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append(mangler.getThisParamTypeName(func.owner));
				builder.Append(" self, ");
			}
			else if (!func.modifiers.HasFlag(Modifiers.NATIVE))
			{
				builder.Append("void * const ");
				if (Program.Options.gccOnly)
				{
					builder.Append(" __attribute__((unused)) ");
				}
				builder.Append("dummy, ");
			}

			foreach (Parameter p in func.parameters)
			{
				builder.Append(mangler.getParamTypeName(p));
				builder.Append(" ");
				builder.Append(mangler.getVarName(p));
				builder.Append(", ");
			}

			if (!func.modifiers.HasFlag(Modifiers.NATIVE) || func.parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
		}

		private void addPlatformModifiers(StringBuilder builder, Modifiers modifiers)
		{
			modifiers = ModifierHelper.getPfMods(modifiers);
			if (modifiers != Modifiers.NONE)
			{
				builder.Append("#if defined(");
				builder.Append(modifiers.ToString().Replace(", ", ") || defined("));
				builder.AppendLine(")");
			}
		}

		private void addPlatformModClose(StringBuilder builder, Modifiers modifiers)
		{
			modifiers = ModifierHelper.getPfMods(modifiers);
			if (modifiers != Modifiers.NONE)
			{
				builder.AppendLine("#endif");
			}
		}

		private void addFunctionSig(StringBuilder builder, Function func, string prefix)
		{
			if (func.modifiers.HasFlag(Modifiers.NATIVE) &&
				(func.identifier == "boh_gc_alloc" ||
				func.identifier == "boh_gc_realloc" ||
				func.identifier == "boh_gc_collect" ||
				func.identifier == "malloc" ||
				func.identifier == "free" ||
				func.identifier == "realloc"))
			{
				return;
			}

			addPlatformModifiers(builder, func.modifiers);
			builder.Append(prefix);
			builder.Append(mangler.getCTypeName(func.returnType));
			builder.Append(" ");
			builder.Append(mangler.getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);
			
			builder.Append(");");
			builder.AppendLine();
			addPlatformModClose(builder, func.modifiers);
		}

		private void addFunctionSigs(StringBuilder builder, IEnumerable<Function> funcs, string prefix)
		{
			foreach (Function f in funcs)
			{
				addFunctionSig(builder, f, prefix);
			}
		}

		private void addTypeOfSig(StringBuilder builder, typesys.Type type)
		{
			string cname = mangler.getCName(type);
			string ctype = mangler.getCTypeName(StdType.type);

			builder.Append("extern ");
			builder.Append(ctype);
			builder.Append(" typeof_");
			builder.Append(cname);
			builder.AppendLine("(void);");
			
		}

		private void addTypeOfDef(StringBuilder builder, typesys.Type type)
		{
			string cname = mangler.getCName(type);
			string ctype = mangler.getCTypeName(StdType.type);

			builder.Append(ctype);
			builder.Append(" typeof_");
			builder.Append(cname);
			builder.AppendLine("(void)");
			builder.AppendLine("{");

			++indentation;
			addIndent(builder);
			builder.Append("static ");
			builder.Append(ctype);
			builder.AppendLine(" result = NULL;");
			addIndent(builder);
			builder.AppendLine("if (result == NULL)");
			addIndent(builder);
			builder.AppendLine("{");
			// TODO: initialise the type
			addIndent(builder);
			builder.AppendLine("}");
			addIndent(builder);
			builder.AppendLine("return result;");
			--indentation;

			builder.Append("}");
		}

		private void addFieldInitSig(StringBuilder builder, Class c)
		{
			builder.Append("extern void ");
			builder.Append(mangler.getFieldInitName(c));
			builder.Append("(");
			builder.Append(mangler.getCTypeName(c));
			builder.AppendLine(" const self);");
		}

		private void addNewOperatorSig(StringBuilder builder, Constructor constr)
		{
			addPlatformModifiers(builder, constr.modifiers);

			builder.Append("extern ");
			builder.Append(mangler.getCTypeName(constr.owner));
			builder.Append(" ");
			builder.Append(mangler.getNewName(constr));
			builder.Append("(");
			foreach (Parameter p in constr.parameters)
			{
				builder.Append(mangler.getCTypeName(p.type));
				builder.Append(" ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
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

			addPlatformModClose(builder, constr.modifiers);
		}

		private void addNewOperatorSigs(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.constructors)
			{
				addNewOperatorSig(builder, constr);
			}
		}

		private void addFieldInit(StringBuilder builder, Class c)
		{
			builder.Append("void ");
			builder.Append(mangler.getFieldInitName(c));
			builder.Append("(");
			builder.Append(mangler.getThisParamTypeName(c));
			builder.AppendLine(" self)");
			builder.AppendLine("{");
			++indentation;
			if (c.super != null && c.super != StdType.obj)
			{
				addIndent(builder);
				builder.Append(mangler.getFieldInitName(c.super));
				builder.AppendLine("(self);");
			}

			foreach (Field f in c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				addIndent(builder);
				builder.Append("self->");
				builder.Append(mangler.getVarUsageName(f, lambdaStack));
				builder.Append(" = ");
				if (f.initial != null)
				{
					addExpression(builder, f.initial);
				}
				else
				{
					addExpression(builder, f.type.defaultVal());
				}
				builder.AppendLine(";");
			}

			--indentation;
			builder.AppendLine("}");
		}

		private void addNewOperator(StringBuilder builder, Constructor constr)
		{
			addPlatformModifiers(builder, constr.modifiers);

			builder.Append(mangler.getCTypeName(constr.owner));
			builder.Append(" ");
			builder.Append(mangler.getNewName(constr));
			builder.Append("(");
			foreach (Parameter p in constr.parameters)
			{
				builder.Append(mangler.getCTypeName(p.type));
				builder.Append(" ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
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

			++indentation;
			addIndent(builder);
			builder.Append(mangler.getCTypeName(constr.owner));
			builder.Append(" result");
			if (constr.owner is Struct)
			{
				builder.AppendLine(";");
			}
			else
			{
				builder.Append(" = boh_gc_alloc(sizeof(");
				builder.Append(mangler.getCStructName(constr.owner));
				builder.AppendLine("));");
			}

			if (!(constr.owner is Struct))
			{
				addIndent(builder);
				builder.Append("result->vtable = &instance_");
				builder.Append(mangler.getVtableName((typesys.Class)constr.owner));
				builder.AppendLine(";");
			}

			addIndent(builder);
			builder.Append(mangler.getCFuncName(((Class)constr.owner).staticConstr));
			builder.AppendLine("(NULL);");

			addIndent(builder);
			builder.Append(mangler.getFieldInitName((Class)constr.owner));
			builder.Append("(");
			if (constr.owner is Struct)
			{
				builder.Append("&");
			}
			builder.AppendLine("result);");

			addIndent(builder);
			builder.Append(mangler.getCFuncName(constr));
			builder.Append("(");
			if (constr.owner is Struct)
			{
				builder.Append("&");
			}
			builder.Append("result");

			foreach (Parameter p in constr.parameters)
			{
				builder.Append(", ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
			}

			builder.AppendLine(");");

			addIndent(builder);
			builder.AppendLine("return result;");
			--indentation;
			
			builder.AppendLine("}");

			addPlatformModClose(builder, constr.modifiers);
		}

		private void addNewOperators(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.constructors)
			{
				addNewOperator(builder, constr);
			}
		}

		private void addStaticFieldProto(StringBuilder builder, Field field, string prefix)
		{
			addPlatformModifiers(builder, field.modifiers);
			builder.Append(prefix);
			builder.Append(mangler.getCTypeName(field.type));
			if (field.modifiers.HasFlag(Modifiers.FINAL) && field.type is Primitive && field.initial != null)
			{
				builder.Append(" const");
			}
			builder.Append(" ");
			builder.Append(mangler.getVarUsageName(field, lambdaStack));
			if (prefix == string.Empty && field.modifiers.HasFlag(Modifiers.FINAL) && field.type is Primitive && field.initial != null)
			{
				builder.Append(" = ");
				addExpressionImplCon(builder, field.initial, field.type);
			}
			builder.AppendLine(";");
			addPlatformModClose(builder, field.modifiers);
		}

		private void addStaticFieldProtos(StringBuilder builder, IEnumerable<Field> fields, string prefix)
		{
			foreach (Field f in fields.Where(x => x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				addStaticFieldProto(builder, f, prefix);
			}
		}

		private void addVtableMembers(StringBuilder builder, typesys.Class c)
		{
			typesys.Class super = c.super;
			if (super != null)
			{
				addVtableMembers(builder, super);
			}

			++indentation;
			foreach (Function f in c.functions.Where(x => x.modifiers.HasFlag(Modifiers.VIRTUAL) || x.modifiers.HasFlag(Modifiers.ABSTRACT)))
			{
				addPlatformModifiers(builder, f.modifiers);
				addIndent(builder);
				builder.Append(mangler.getCTypeName(f.returnType));
				builder.Append(" (*");
				builder.Append(mangler.getVFuncName(f));
				builder.Append(")(");
				addFunctionParams(builder, f);
				builder.AppendLine(");");
				addPlatformModClose(builder, f.modifiers);
			}
			--indentation;
		}

		private void addVtable(StringBuilder builder, Class c)
		{
			builder.Append("struct ");
			builder.AppendLine(mangler.getVtableName(c));
			builder.AppendLine("{");
			addVtableMembers(builder, c);
			builder.AppendLine("};");
			builder.AppendLine();
			builder.Append("extern const struct ");
			builder.Append(mangler.getVtableName(c));
			builder.Append(" instance_");
			builder.Append(mangler.getVtableName(c));
			builder.AppendLine(";");
		}

		private void addStructFields(StringBuilder builder, typesys.Class c)
		{
			if (c.super != null)
			{
				addStructFields(builder, c.super);
			}

			++indentation;
			foreach (Field f in c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.STATIC)))
			{
				addPlatformModifiers(builder, f.modifiers);
				addIndent(builder);
				builder.Append(mangler.getCTypeName(f.type));
				builder.Append(" ");
				builder.Append(mangler.getVarUsageName(f, lambdaStack));
				builder.AppendLine(";");
				addPlatformModClose(builder, f.modifiers);
			}
			--indentation;
		}

		private void addStructDefinition(StringBuilder builder, typesys.Class c)
		{
			if (!(c is Struct))
			{
				addVtable(builder, c);

				builder.AppendLine();
			}

			builder.AppendLine(mangler.getCStructName(c));
			builder.AppendLine("{");

			if (!(c is Struct))
			{
				builder.Append("\tconst struct ");
				builder.Append(mangler.getVtableName(c));
				builder.AppendLine(" * vtable;");
			}

			addStructFields(builder, c);

			builder.AppendLine("};");
		}

		private void generateStructHeaders(typesys.Struct c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			addIncludeGuard(builder, c);
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();

			addStructDefinition(builder, c);
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(c) + ".type.h", builder.ToString());
			builder.Clear();

			addIncludeGuard(builder, c);
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();

			addTypeOfSig(builder, c);
			builder.AppendLine();

			addNewOperatorSigs(builder, c);
			builder.AppendLine();

			addFunctionSigs(builder, c.functions.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();

			addStaticFieldProtos(builder, c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(c) + ".sigs.h", builder.ToString());
		}

		private void generateClassHeaders(typesys.Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			addIncludeGuard(builder, c);
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();

			addTypeOfSig(builder, c);
			builder.AppendLine();

			addNewOperatorSigs(builder, c);
			builder.AppendLine();

			addFieldInitSig(builder, c);
			builder.AppendLine();
	
			addFunctionSigs(builder, c.functions.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();
			
			addStaticFieldProtos(builder, c.fields.Where(x => !x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "extern ");
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(c) + ".sigs.h", builder.ToString());
			builder.Clear();

			addIncludeGuard(builder, c);
			builder.AppendLine();

			addStructPrototypes(builder, others);
			builder.AppendLine();

			addStructDefinition(builder, c);
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(c) + ".type.h", builder.ToString());
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

		private void addVtableInit(StringBuilder builder, Class c, IEnumerable<Function> overriden)
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
					builder.Append("&" + mangler.getCFuncName(overridenf) + ", ");
				}
				else
				{
					if (f.modifiers.HasFlag(Modifiers.ABSTRACT))
					{
						builder.Append("NULL, ");
					}
					else
					{
						builder.Append("&" + mangler.getCFuncName(f) + ", ");
					}
				}
			}
		}

		private void addVtableDefinition(StringBuilder builder, Class c)
		{
			builder.Append("const struct " + mangler.getVtableName(c));
			builder.Append(" instance_" + mangler.getVtableName(c));
			builder.Append(" = { ");
			addVtableInit(builder, c, c.functions.Where(x => x.modifiers.HasFlag(Modifiers.OVERRIDE)));
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(" };");
		}

		private void addFunctionDef(StringBuilder builder, Function func)
		{
			addPlatformModifiers(builder, func.modifiers);

			// TODO: String[] also allowed
			if (func.identifier == "main")
			{
				// add real main function
				builder.AppendLine("int main(int argc, char **argv)");
				builder.AppendLine("{");
				++indentation;
				addIndent(builder);
				builder.AppendLine("GC_INIT();");
				addIndent(builder);
				builder.Append(mangler.getCFuncName(func));
				builder.AppendLine("(NULL);");
				addIndent(builder);
				builder.AppendLine("return 0;");
				--indentation;
				builder.AppendLine("}");
			}

			if (func.modifiers.HasFlag(Modifiers.PRIVATE))
			{
				builder.Append("");
			}
			builder.Append(mangler.getCTypeName(func.returnType));
			builder.Append(" ");
			builder.Append(mangler.getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);

			builder.AppendLine(")");

			bool hasSuperBeenCalled = (func.body.hasSuperBeenCalled() || (!(func.owner is Class) || ((Class)func.owner).super == null));
			if (!hasSuperBeenCalled && func is Constructor && ((Class)func.owner).super != StdType.obj)
			{
				func.body.statements.Insert(0,
					new ExpressionStatement(
					new FunctionCall(
						((Class)func.owner).super.constructors.Single(x => !(x.modifiers.HasFlag(Modifiers.PRIVATE) || x.modifiers.HasFlag(Modifiers.CVISIBLE)) && x.parameters.Count == 0),
						((Class)func.owner).THISVAR, Enumerable.Empty<Expression>())));
			}

			if (func is StaticConstructor)
			{
				addStaticConstrPrebody(builder, func);
			}

			if (!(func is StaticConstructor) &&
				func.modifiers.HasFlag(Modifiers.STATIC) &&
				(func.modifiers.HasFlag(Modifiers.PUBLIC) ||
				//func.modifiers.HasFlag(Modifiers.PACKAGE) ||
				func.modifiers.HasFlag(Modifiers.CVISIBLE)))
			{
				func.body.statements.Insert(0, 
					new ExpressionStatement(
					new FunctionCall(((Class)func.owner).staticConstr, null, Enumerable.Empty<Expression>())));
			}

			addFnHeapParams(builder, func.parameters);

			addBody(builder, func.body);

			addFnHeapParamClose(builder, func.parameters);

			if (!(func is StaticConstructor) &&
				func.modifiers.HasFlag(Modifiers.STATIC) &&
				(func.modifiers.HasFlag(Modifiers.PUBLIC) ||
				//func.modifiers.HasFlag(Modifiers.PACKAGE) ||
				func.modifiers.HasFlag(Modifiers.CVISIBLE)))
			{
				func.body.statements.RemoveAt(0);
			}

			if (func is StaticConstructor)
			{
				--indentation;
				addIndent(builder);
				builder.AppendLine("}");
			}

			if (!hasSuperBeenCalled && func is Constructor && ((Class)func.owner).super != StdType.obj)
			{
				func.body.statements.RemoveAt(0);
			}

			addPlatformModClose(builder, func.modifiers);
		}

		private void addFnHeapParamClose(StringBuilder builder, IEnumerable<Variable> parameters)
		{
			if (parameters.Where(x => x.enclosed).Count() > 0)
			{
				--indentation;
				builder.AppendLine("}");
			}
		}

		private void addFnHeapParams(StringBuilder builder, IEnumerable<Variable> parameters)
		{
			if (parameters.Where(x => x.enclosed).Count() > 0)
			{
				builder.AppendLine("{");

				++indentation;

				foreach (Variable param in parameters.Where(x => x.enclosed))
				{
					addIndent(builder);

					builder.Append(mangler.getCTypeName(param.type));
					builder.Append("* e");
					builder.Append(mangler.getVarName(param));
					builder.Append(" = boh_gc_alloc(sizeof(");
					builder.Append(mangler.getCTypeName(param.type));
					builder.AppendLine("));");
					addIndent(builder);
					builder.Append(mangler.getVarUsageName(param, lambdaStack));
					builder.Append(" = ");
					builder.Append(mangler.getVarName(param));
					builder.AppendLine(";");
				}
			}
		}

		private void addStaticConstrPrebody(StringBuilder builder, Function func)
		{
			builder.AppendLine("{");
			++indentation;
			addIndent(builder);
			builder.AppendLine("static _Bool hasBeenCalled = 0;");
			addIndent(builder);
			builder.AppendLine("if (hasBeenCalled)");
			addIndent(builder);
			builder.AppendLine("{");
			++indentation;
			addIndent(builder);
			builder.AppendLine("return;");
			--indentation;
			addIndent(builder);
			builder.AppendLine("}");
			addIndent(builder);
			builder.AppendLine("hasBeenCalled = 1;");
			if (((Class)func.owner).super != null)
			{
				addIndent(builder);
				builder.Append(mangler.getCFuncName(((Class)func.owner).super.staticConstr));
				builder.AppendLine("(NULL);");
			}

			foreach (Field f in ((Class)func.owner).fields.Where(x => x.modifiers.HasFlag(Modifiers.STATIC) && !(x.modifiers.HasFlag(Modifiers.FINAL) && x.type is Primitive && x.initial != null)))
			{
				addIndent(builder);
				builder.Append(mangler.getVarUsageName(f, lambdaStack));
				builder.Append(" = ");
				if (f.initial != null)
				{
					addExpression(builder, f.initial);
				}
				else
				{
					addExpression(builder, f.type.defaultVal());
				}
				builder.AppendLine(";");
			}
		}

		private void addFunctionDefs(StringBuilder builder, IEnumerable<Function> funcs)
		{
			foreach (Function f in funcs)
			{
				addFunctionDef(builder, f);
			}
		}

		#region Misc

		private int indentation = 0;

		public const string INDENT_SEQ = "\t";

		private void addIndent(StringBuilder builder)
		{
			for (int i = 0; i < indentation; ++i)
			{
				builder.Append(INDENT_SEQ);
			}
		}

		private Stack<List<string>> prestatstack = new Stack<List<string>>();
		private int tempvcounter = 0;
		// the lambda stack, see Expression.cs for exact definition
		private int lambdaStack = 0;

		private void addPreStatStatement(string txt)
		{
			prestatstack.Peek().Add(txt);
		}

		private string addTemp(string prefix, string suffix)
		{
			string name = "temp" + tempvcounter++;
			addPreStatStatement(prefix + name + suffix);
			return name;
		}

		private string addTemp(Expression value)
		{
			return addTemp(mangler.getCTypeName(value.getType()) + " ", ";");
		}

		#endregion

		#region Expressions

		private void addInterfaceNewCallFunc(StringBuilder builder, Function f, string tempn)
		{
			builder.Append("&");
			if (f.modifiers.HasFlag(Modifiers.OVERRIDE) || f.modifiers.HasFlag(Modifiers.VIRTUAL) || f.modifiers.HasFlag(Modifiers.ABSTRACT))
			{
				builder.Append(tempn);
				builder.Append("->vtable->");
				builder.Append(mangler.getVFuncName(f));
			}
			else
			{
				builder.Append(mangler.getCFuncName(f));
			}
		}

		private void addInterfaceNewCallFuncs(StringBuilder builder, Interface iface, Class c, string tempn)
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

		private void addExpressionImplCon(StringBuilder builder, Expression expression, typesys.Type towhat)
		{
			typesys.Type type = expression.getType();
			if (type == towhat)
			{
				addExpression(builder, expression);
			}
			else if (type is Interface && towhat is Class)
			{
				builder.Append("(");
				builder.Append(mangler.getCTypeName(towhat));
				builder.Append(")");
				addExpression(builder, expression);
				builder.Append("->object");
			}
			else if (type is Class && towhat is Interface)
			{
				string tempn = addTemp(mangler.getCTypeName(type), ";");

				Interface towi = towhat as Interface;
				builder.Append("new_");
				builder.Append(mangler.getCName(towhat));
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
				builder.Append(mangler.getCTypeName(towhat));
				builder.Append(")(");
				addExpression(builder, expression);
				builder.Append(")");
			}
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

			NativeFunctionCall nexpr = expression as NativeFunctionCall;
			if (nexpr != null)
			{
				addNativeExpression(builder, nexpr);
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

			throw new NotImplementedException();
		}

		private void addRefExpr(StringBuilder builder, RefExpression refExpr)
		{
			builder.Append("&");
			addExpression(builder, refExpr.onwhat);
		}

		private void addEnumerator(StringBuilder builder, ExprEnumerator en)
		{
			builder.Append(mangler.getEnumeratorName(en.enumerator));
		}

		private void addLambda(StringBuilder builder, Lambda lambda)
		{
			string tmp = addTemp(lambda);
			addPreStatStatement(tmp + ".function = &" + lambdaNames[lambda] + ";");
			addPreStatStatement(tmp + ".context = boh_gc_alloc(sizeof(struct " + lambdaCtxNames[lambda] + "));");
			foreach (Variable v in lambda.enclosed)
			{
				StringBuilder preStat = new StringBuilder();
				preStat.Append("((struct ");
				preStat.Append(lambdaCtxNames[lambda]);
				preStat.Append(" *)");
				preStat.Append(tmp);
				preStat.Append(".context)->");
				preStat.Append(mangler.getHeapVarAssignName(v));
				preStat.Append(" = ");
				if (v.identifier != "this")
				{
					preStat.Append("&");
				}
				preStat.Append(mangler.getVarUsageName(v, lambdaStack));
				preStat.Append(";");
				addPreStatStatement(preStat.ToString());
			}
			builder.Append(tmp);
		}

		private int getStrLitLen(string str)
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

		private void addFvCall(StringBuilder builder, FunctionVarCall fvCall)
		{
			string tmp = addTemp(fvCall.belongsto);
			StringBuilder tempBuild = new StringBuilder();
			tempBuild.Append(tmp + " = ");
			addExpression(tempBuild, fvCall.belongsto);
			tempBuild.Append(";");
			addPreStatStatement(tempBuild.ToString());
			builder.Append(tmp + ".function(" + tmp + ".context, ");

			IEnumerator<typesys.Type> fparams = (fvCall.belongsto.getType() as FunctionRefType).paramTypes.Where(x => true).GetEnumerator();
			foreach (Expression param in fvCall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, (typesys.Type)fparams.Current);
				builder.Append(", ");
			}

			builder.Remove(builder.Length - 2, 2);

			builder.Append(")");
		}

		private void addTypeCast(StringBuilder builder, TypeCast tc)
		{
			// TODO: check if Object.cast<T> is needed
			if (tc.towhat.extends(tc.onwhat.getType()) > 0 ||
				tc.onwhat.getType().extends(tc.towhat) > 0)
			{
				addExpressionImplCon(builder, tc.onwhat, tc.towhat);
			}
			else
			{

			}
		}

		private void addLiteral(StringBuilder builder, Literal lit)
		{
			typesys.Type type = lit.getType();

			Primitive prim = type as Primitive;
			if (prim != null)
			{
                if (prim == Primitive.CHAR)
                {
                    //builder.Append("u8");
                    builder.Append(lit.representation);
                }
                else if (prim == Primitive.BOOLEAN)
                {
                    builder.Append(lit.representation == "true" ? 1 : 0);
                }
                else if (prim.isInt())
                {
					string rep = lit.representation;
					if (rep.EndsWith("L") || rep.EndsWith("l"))
					{
						rep = rep.Substring(0, rep.Length - 1);
					}
                    long lval = long.Parse(rep);
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
                else if (prim == Primitive.DECIMAL)
                {
                    builder.Append(lit.representation.ToUpperInvariant());
                }
			}
			else
			{
				if (lit.type == StdType.str && lit.representation != "NULL")
				{
					builder.Append("boh_create_string(u8");
					builder.Append(lit.representation);
					builder.Append(", ");
					builder.Append(getStrLitLen(lit.representation));
					builder.Append(")");
				}
				else if (lit.representation == "BOH_FP_NULL")
				{
					builder.Append("(*(");
					builder.Append(mangler.getCTypeName((FunctionRefType)lit.type));
					builder.Append(" *)&BOH_FP_NULL)");
				}
				else
				{
					builder.Append(lit.representation);
				}
			}
		}

		private void addCCall(StringBuilder builder, ConstructorCall ccall)
		{
			builder.Append(mangler.getNewName(ccall.function));
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

		private void addNativeExpression(StringBuilder builder, NativeFunctionCall nfcall)
		{
			if (nfcall.function == NativeFunction.NATIVE_DEREF)
			{
				ExprType type = (ExprType)nfcall.parameters [0];
				builder.Append("(*(");
				builder.Append(mangler.getCTypeName(type.type));
				builder.Append(" *)");
				builder.Append("((int8_t *)");
				builder.Append("(");
				addExpression(builder, nfcall.parameters [1]);
				builder.Append(")))");
			}
			else if (nfcall.function == NativeFunction.NATIVE_REF)
			{
				builder.Append("&");
				addExpression(builder, nfcall.parameters [0]);
			}
			else if (nfcall.function == NativeFunction.NATIVE_SIZEOF)
			{
				builder.Append("sizeof(");
				builder.Append(mangler.getCTypeName(((ExprType)nfcall.parameters[0]).type));
				builder.Append(")");
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		private void addNullCheck(StringBuilder builder, Expression expr)
		{
			ThisVar tvar = expr as ThisVar;
			if (!Program.Options.unsafeNullPtr && tvar == null)
			{
				builder.Append("boh_check_null(");
			}
			addExpression(builder, expr);
			if (!Program.Options.unsafeNullPtr && tvar == null)
			{
				builder.Append(", ");
				builder.Append(mangler.getCTypeName(expr.getType()));
				builder.Append(")");
			}
		}
		private void addFCall(StringBuilder builder, FunctionCall fcall)
		{
			if ((fcall.refersto.modifiers.HasFlag(Modifiers.VIRTUAL) ||
				fcall.refersto.modifiers.HasFlag(Modifiers.ABSTRACT) ||
				fcall.refersto.modifiers.HasFlag(Modifiers.OVERRIDE)) &&
				!(fcall.refersto.owner is Struct))
			{
				if (fcall.belongsto is SuperVar)
				{
					builder.Append("instance_");
					builder.Append(mangler.getVtableName((Class)fcall.belongsto.getType()));
					builder.Append(".");
					builder.Append(mangler.getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append("self, ");
				}
				else
				{
					string temp = addTemp(fcall.belongsto);

					builder.Append("(");
					builder.Append(temp);
					builder.Append(" = ");
					addNullCheck(builder, fcall.belongsto);
					builder.Append(")->vtable->");

					builder.Append(mangler.getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append(temp);
					builder.Append(", ");
				}
			}
			else if (!fcall.refersto.modifiers.HasFlag(Modifiers.STATIC))
			{
				if (fcall.refersto.owner is Struct)
				{
					string tempname = addTemp(mangler.getCTypeName(fcall.refersto.owner) + " ", ";");

					builder.Append(mangler.getCFuncName(fcall.refersto));
					builder.Append("((");
					builder.Append(tempname);
					builder.Append(" = ");
					addExpression(builder, fcall.belongsto);
					builder.Append(", &");
					builder.Append(tempname);
					builder.Append("), ");
				}
				else if (fcall.refersto.owner is Class
					|| fcall.refersto.owner is typesys.Enum
					|| fcall.refersto.owner is typesys.Primitive)
				{
					builder.Append(mangler.getCFuncName(fcall.refersto));
					builder.Append("(");
					addNullCheck(builder, fcall.belongsto);
					builder.Append(", ");
				}
				else
				{
					string temp = addTemp(fcall.belongsto);
					builder.Append("(");
					builder.Append(temp);
					builder.Append(" = ");
					addNullCheck(builder, fcall.belongsto);
					builder.Append(")->");
					builder.Append(mangler.getVFuncName(fcall.refersto));
					builder.Append("(");
					builder.Append(temp);
					builder.Append("->object, ");
				}
			}
			else
			{
				builder.Append(mangler.getCFuncName(fcall.refersto));
				if (fcall.refersto.modifiers.HasFlag(Modifiers.NATIVE))
				{
					builder.Append("(");
				}
				else
				{
					builder.Append("(NULL, ");
				}
			}

			IEnumerator<Parameter> fparams = fcall.refersto.parameters.GetEnumerator();
			foreach (Expression param in fcall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, fparams.Current.type);
				builder.Append(", ");
			}

			if (!fcall.refersto.modifiers.HasFlag(Modifiers.NATIVE) || fcall.refersto.parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}

			builder.Append(")");
		}

		private void addEqualsBinOp(StringBuilder builder, Expression left, Expression right)
		{
			if ((left.getType() is Primitive && right.getType() is Primitive) ||
				(left.getType() is NullType || right.getType() is NullType))
			{
				builder.Append("(");
				addExpression(builder, left);
				builder.Append(" == ");
				addExpression(builder, right);
				builder.Append(")");
			}
			else
			{
				Function objEq = StdType.obj.functions.Single(x => x.identifier == "valEquals");
				builder.Append(mangler.getCFuncName(objEq));
				builder.Append("(NULL, ");
				addExpressionImplCon(builder, left, StdType.obj);
				builder.Append(", ");
				addExpressionImplCon(builder, right, StdType.obj);
				builder.Append(")");
			}
		}

		private string getOpRep(Operator op)
		{
			if (op == BinaryOperation.R_EQ)
			{
				return "==";
			}

			return op.representation;
		}

		private void addBinOp(StringBuilder builder, BinaryOperation binop)
		{
			if (binop.operation == BinaryOperation.EQUAL || binop.operation == BinaryOperation.NOT_EQUAL)
			{
				if (binop.operation == BinaryOperation.NOT_EQUAL)
				{
					builder.Append("!");
				}
				addEqualsBinOp(builder, binop.left, binop.right);
			}
			else if (binop.overloaded != null)
			{
				builder.Append(mangler.getCFuncName(binop.overloaded));
				builder.Append("(NULL, ");
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
				if (binop.isAssignment() && !(binop.left.getType() is CompatibleWithAllType))
				{
					addExpressionImplCon(builder, binop.right, binop.left.getType());
				}
				else
				{
					addExpression(builder, binop.right);
				}
			}
		}

		private void addTypeOf(StringBuilder builder, typesys.Type type)
		{
			builder.Append("typeof_");
			builder.Append(mangler.getCName(type));
			builder.Append("()");
		}

		private void addUnOp(StringBuilder builder, UnaryOperation unop)
		{
			if (unop.operation == UnaryOperation.TYPEOF)
			{
				addTypeOf(builder, (unop.onwhat as ExprType).type);
			}
			else if (unop.operation == UnaryOperation.DEFAULT)
			{
				addExpression(builder, (unop.onwhat as ExprType).type.defaultVal());
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

		private void addExprVar(StringBuilder builder, ExprVariable exprvar)
		{
			if (exprvar.belongsto is ExprVariable)
			{
				if (exprvar.belongsto.getType() is Struct)
				{
					addExpression(builder, exprvar.belongsto);
					builder.Append(".");
				}
				else
				{
					addNullCheck(builder, exprvar.belongsto);
					builder.Append("->");
				}
			}

			Field f = exprvar.refersto as Field;
			if (f != null && f.modifiers.HasFlag(Modifiers.STATIC | Modifiers.PUBLIC))
			{
				StringBuilder temp = new StringBuilder();
				temp.Append(mangler.getCFuncName(f.owner.staticConstr));
				temp.Append("(NULL);");
				addPreStatStatement(temp.ToString());
			}
			builder.Append(mangler.getVarUsageName(exprvar.refersto, lambdaStack));
		}

		#endregion

		#region Statements

		private void addBody(StringBuilder builder, Body body)
		{
			int temp = tempvcounter;

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

			tempvcounter = temp;
		}

		private bool addSpecStat<T>(StringBuilder builder, Statement stat, Action<StringBuilder, T> act)
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

		private void addStatement(StringBuilder builder, Statement statement)
		{
			prestatstack.Push(new List<string>());

			StringBuilder statb = new StringBuilder();

			if (addSpecStat<IfStatement>(statb, statement, addIfStat)) { }
			else if (addSpecStat<WhileStatement>(statb, statement, addWhileStat)) { }
			else if (addSpecStat<DoWhileStatement>(statb, statement, addDoWhile)) { }
			else if (addSpecStat<ForStatement>(statb, statement, addFor)) { }
			else if (addSpecStat<ExpressionStatement>(statb, statement, addEStat)) { }
			else if (addSpecStat<VarDeclaration>(statb, statement, addVarDec)) { }
			else if (addSpecStat<ReturnStatement>(statb, statement, addReturn)) { }
			else if (addSpecStat<BreakStatement>(statb, statement, (x, y) => addSingleString(x, "break;"))) { }
			else if (addSpecStat<ContinueStatement>(statb, statement, (x, y) => addSingleString(x, "continue;"))) { }
			else if (addSpecStat<TryStatement>(statb, statement, addTry)) { }
			else if (addSpecStat<ThrowStatement>(statb, statement, addThrow)) { }
			else if (addSpecStat<Scope>(statb, statement, (x, y) => addBody(x, y.body))) { }
			else if (addSpecStat<SwitchStatement>(statb, statement, addSwitchStat)) { }
			else if (addSpecStat<ForeachStatement>(statb, statement, addForeachStat)) { }
			else { throw new NotImplementedException(); }

			foreach (string str in prestatstack.Pop())
			{
				addIndent(builder);
				builder.AppendLine(str);
			}

			builder.Append(statb.ToString());
		}

		private void addForeachStat(StringBuilder builder, ForeachStatement fore)
		{
			addIndent(builder);

			FunctionCall fcall = null;
			if (fore.expr.getType() is typesys.Class)
			{
				fcall = new FunctionCall(((typesys.Class)fore.expr.getType()).functions.Single(x => x.identifier == "iterator" && x.parameters.Count() == 0), fore.expr, Enumerable.Empty<Expression>());
			}
			else
			{
				fcall = new FunctionCall(((typesys.Interface)fore.expr.getType()).getFunctions("iterator").Single(x => x.parameters.Count() == 0), fore.expr, Enumerable.Empty<Expression>());
			}
			string tmp = addTemp(fcall);
			builder.Append(tmp);
			builder.Append(" = ");
			addExpressionImplCon(builder, fcall, StdType.iiterator.getTypeFor(new[] { fore.vardecl.refersto.type }, null));
			builder.AppendLine(";");

			addIndent(builder);
			builder.Append("while (");
			builder.Append(tmp);
			builder.Append("->");
			builder.Append(mangler.getVFuncName(((typesys.Interface)fcall.getType()).functions.Single(x => x.identifier == "next")));
			builder.Append("(");
			builder.Append(tmp);
			builder.Append("->object");
			builder.AppendLine("))");

			addIndent(builder);
			builder.AppendLine("{");

			++indentation;
			addVarDec(builder, fore.vardecl);
			addIndent(builder);
			builder.Append(mangler.getVarUsageName(fore.vardecl.refersto, lambdaStack));
			builder.Append(" = ");
			builder.Append(tmp);
			builder.Append("->");
			builder.Append(mangler.getVFuncName(((typesys.Interface)fcall.getType()).functions.Single(x => x.identifier == "current")));
			builder.Append("(");
			builder.Append(tmp);
			builder.Append("->object");
			builder.AppendLine(");");

			addBody(builder, fore.body);
			--indentation;
			addIndent(builder);
			builder.AppendLine("}");
		}

		private void addSwitchStat(StringBuilder builder, SwitchStatement swstat)
		{
			addIndent(builder);
			builder.Append("switch (");
			addExpression(builder, swstat.expression);
			builder.AppendLine(")");
			
			addIndent(builder);
			builder.AppendLine("{");

			foreach (SwitchLabel slabel in swstat.labels)
			{
				CaseLabel clabel = slabel as CaseLabel;
				if (clabel != null)
				{
					addIndent(builder);
					builder.Append("case ");
					addExpression(builder, clabel.expression);
					builder.AppendLine(":");
					++indentation;
					addBody(builder, clabel.body);
					--indentation;
				}

				DefaultLabel def = slabel as DefaultLabel;
				if (def != null)
				{
					addIndent(builder);
					builder.AppendLine("default:");
					++indentation;
					addBody(builder, def.body);
					--indentation;
				}
			}

			addIndent(builder);
			builder.AppendLine("}");
		}

		private void addThrow(StringBuilder builder, ThrowStatement thr)
		{
			addIndent(builder);
			builder.Append("boh_throw_ex(");
			addExpression(builder, thr.exception);
			builder.AppendLine(");");
		}

		private void addReturn(StringBuilder builder, ReturnStatement ret)
		{
			addIndent(builder);
			if (ret.returns != null)
			{
				builder.Append("return ");
				addExpression(builder, ret.returns);
			}
			else
			{
				builder.Append("return");
			}
			builder.AppendLine(";");
		}

		// used for continue and break, usage otherwise discouraged
		private void addSingleString(StringBuilder builder, string str)
		{
			addIndent(builder);
			builder.AppendLine(str);
		}

		private void addCatch(StringBuilder builder, CatchStatement cstat, string finname)
		{
			builder.Append("if (exception_type == typeof_");
			builder.Append(mangler.getCName(cstat.param.type));
			builder.AppendLine("())");

			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			string typeusename = mangler.getCTypeName(cstat.param.type);
			addIndent(builder);
			builder.Append(typeusename);
			builder.Append(" ");
			builder.Append(mangler.getVarUsageName(cstat.param, lambdaStack));
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

		private void addTry(StringBuilder builder, TryStatement trys)
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

		private void addJmpReset(StringBuilder builder, string tempname)
		{
			addIndent(builder);
			builder.Append("memcpy(&exception_buf, &");
			builder.Append(tempname);
			builder.AppendLine(", sizeof(jmp_buf));");
		}

		private void addDoWhile(StringBuilder builder, DoWhileStatement dostat)
		{
			addIndent(builder);
			builder.AppendLine("do");
			addBody(builder, dostat.body);
			addIndent(builder);
			builder.Append("while (");
			addExpression(builder, dostat.condition);
			builder.AppendLine(");");
		}

		private void addFor(StringBuilder builder, ForStatement forstat)
		{
			addIndent(builder);
			builder.Append("for (");

			StringBuilder sbuild = new StringBuilder();
			addStatement(sbuild, forstat.initial);
			string str = sbuild.ToString().Trim();
			if (str.Length == 0)
			{
				str = ";";
			}
			builder.Append(str);
			builder.Append(" ");

			addExpression(builder, forstat.condition);
			builder.Append("; ");

			sbuild = new StringBuilder();
			addStatement(sbuild, forstat.final);
			str = sbuild.ToString().Trim();
			if (str.Length > 0)
			{
				builder.Append(str.Substring(0, str.Length - 1));
			}
			builder.AppendLine(")");

			addBody(builder, forstat.body);
		}

		private void addElse(StringBuilder builder, ElseStatement elsestat)
		{
			addIndent(builder);
			builder.AppendLine("else");
			addBody(builder, elsestat.body);
		}

		private void addVarDec(StringBuilder builder, VarDeclaration vdec)
		{
			// REMEMBER
			// Enclosed variable are ALWAYS heap allocated
			// ALWAYS
			// If they're already reference types, the reference shall be heap-allocated as well

			// UNLESS
			// They are static variables, in which case the reference operator can be used

			// TODO:
			// UNLESS
			// They are primitive constants

			addIndent(builder);

			if (((Local)vdec.refersto).modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append("static ");
			}

			builder.Append(mangler.getCTypeName(vdec.refersto.type));
			if (vdec.refersto.enclosed && !((Local)vdec.refersto).modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append("*");
			}
			else if (vdec.refersto.modifiers.HasFlag(Modifiers.FINAL) && vdec.refersto.type is Primitive)
			{
				builder.Append(" const");
			}

			builder.Append(" ");
			builder.Append(mangler.getVarName(vdec.refersto));

			if (vdec.refersto.enclosed && !((Local)vdec.refersto).modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append(" = boh_gc_alloc(sizeof(");
				builder.Append(mangler.getCTypeName(vdec.refersto.type));
				builder.Append("))");
				if (vdec.initial != null)
				{
					builder.AppendLine(";");
					addIndent(builder);
					builder.Append(mangler.getVarUsageName(vdec.refersto, lambdaStack));
				}
			}

			if (vdec.initial != null && !((Local)vdec.refersto).modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append(" = ");
				addExpressionImplCon(builder, vdec.initial, vdec.refersto.type);
			}

			builder.AppendLine(";");

			if (vdec.initial != null && ((Local)vdec.refersto).modifiers.HasFlag(Modifiers.STATIC))
			{
				addIndent(builder);
				builder.Append(mangler.getVarUsageName(vdec.refersto, lambdaStack));
				builder.Append(" = ");
				addExpressionImplCon(builder, vdec.initial, vdec.refersto.type);
				builder.AppendLine(";");
			}
		}

		private void addEStat(StringBuilder builder, ExpressionStatement estat)
		{
			addIndent(builder);
			addExpression(builder, estat.expression);
			builder.AppendLine(";");
		}

		private void addWhileStat(StringBuilder builder, WhileStatement stat)
		{
			addIndent(builder);
			builder.Append("while (");
			addExpression(builder, stat.condition);
			builder.AppendLine(")");
			addBody(builder, stat.body);
		}

		private void addIfStat(StringBuilder builder, IfStatement stat)
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

		private string generateCode(typesys.Type type, IEnumerable<typesys.Type> others)
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

			typesys.Enum e = type as typesys.Enum;
			if (e != null)
			{
				return generateEnumCode(e, others);
			}

			throw new NotImplementedException();
		}

		private string generateEnumCode(typesys.Enum e, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(mangler.getHeaderName(e));
			builder.AppendLine("\"");

			addFunctionDef(builder, e.toString);

			return builder.ToString();
		}

		private void addInterfaceConstructor(StringBuilder builder, Interface iface)
		{
			builder.Append(mangler.getCTypeName(iface));
			builder.Append(" new_");
			builder.Append(mangler.getCName(iface));
			builder.Append("(");
			builder.Append(mangler.getCTypeName(StdType.obj));
			builder.Append(" object, ");
			addInterfaceNewOpParams(builder, iface);
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(")");
			builder.AppendLine("{");
			builder.Append("\t");
			builder.Append(mangler.getCTypeName(iface));
			builder.Append(" result = boh_gc_alloc(sizeof(");
			builder.Append(mangler.getCStructName(iface));
			builder.AppendLine("));");
			builder.AppendLine("\tresult->object = object;");
			addInterfaceAssignments(builder, iface);
			builder.AppendLine("\treturn result;");
			builder.AppendLine("}");
		}

		private void addInterfaceAssignments(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.implements)
			{
				addInterfaceAssignments(builder, impl);
			}

			foreach (Function f in iface.functions)
			{
				builder.Append("\tresult->");
				builder.Append(mangler.getVFuncName(f));
				builder.Append(" = ");
				builder.Append(mangler.getVFuncName(f));
				builder.AppendLine(";");
			}
		}

		private string generateInterfaceCode(Interface iface, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(mangler.getHeaderName(iface));
			builder.AppendLine("\"");
			builder.AppendLine();

			addIncludes(builder, others);

			addInterfaceConstructor(builder, iface);

			return builder.ToString();
		}

		private string generateClassCode(Class c, IEnumerable<typesys.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(mangler.getHeaderName(c));
			builder.AppendLine("\"");
			builder.AppendLine();

			addIncludes(builder, others);

			addStaticFieldProtos(builder, c.fields, string.Empty);
			builder.AppendLine();

			addFunctionSigs(builder, c.functions.Where(x => x.modifiers.HasFlag(Modifiers.PRIVATE) && !x.modifiers.HasFlag(Modifiers.ABSTRACT)), "static ");
			builder.AppendLine();

			if (!(c is Struct))
			{
				addVtableDefinition(builder, c);
				builder.AppendLine();
			}

			addTypeOfDef(builder, c);
			builder.AppendLine();

			addNewOperators(builder, c);
			builder.AppendLine();

			addFieldInit(builder, c);
			builder.AppendLine();
			
			addFunctionDefs(builder, c.functions.Where(x => x.body != null));

			return builder.ToString();
		}
	}
}
