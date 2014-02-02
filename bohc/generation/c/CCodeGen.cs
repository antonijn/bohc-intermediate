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

using Bohc.Boh;
using Bohc.Exceptions;
using Bohc.Parsing;
using Bohc.Parsing.Statements;
using Bohc.TypeSystem;
using Bohc.Generation.Mangling;

namespace Bohc.Generation.C
{
	public class CCodeGen : ICodeGen
	{
		private readonly IMangler mangler;
		private readonly Bohc.General.Project proj;

		public CCodeGen(IMangler mangler, Bohc.General.Project p)
		{
			this.mangler = mangler;
			this.proj = p;
		}

		public void generateGeneralBit(IEnumerable<Bohc.TypeSystem.Type> others)
		{
			//Primitive.figureOutFunctionsForAll();
			System.IO.Directory.CreateDirectory(".c");

			if (!System.IO.File.Exists(".c/boh_internal.h"))
			{
				System.IO.File.Copy(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "boh_internal.h", ".c/boh_internal.h");
			}
			if (proj.noStd && !System.IO.File.Exists(".c/boh_internal.c"))
			{
				System.IO.File.Copy(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "boh_internal.c", ".c/boh_internal.c");
			}

			generateFunctionRefTypes(others);
		}

		// associates lambdas with the names of their context types
		private readonly Dictionary<Lambda, string> lambdaCtxNames = new Dictionary<Lambda, string>();
		// unanonyfies the lambdas
		private readonly Dictionary<Lambda, string> lambdaNames = new Dictionary<Lambda, string>();

		private void generateFunctionRefTypes(IEnumerable<Bohc.TypeSystem.Type> others)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("#pragma once");
			builder.AppendLine("#include \"all.h\"");

			builder.AppendLine("struct boh_fp_null_type { void *one; void *two; };");
			builder.AppendLine("extern struct boh_fp_null_type BOH_FP_NULL;");

			foreach (FunctionRefType fRefType in FunctionRefType.Instances)
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
				builder.Append(mangler.getCTypeName(l.type.RetType)).Append(" " + lambdaNames[l]).Append("(struct ").Append(lambdaCtxNames[l] + " * ctx, ");
				foreach (LambdaParam lParam in l.lambdaParams)
				{
					builder.Append(mangler.getCTypeName(lParam.Type)).Append(" ").Append(mangler.getVarName(lParam)).Append(", ");
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
					if (l.type.RetType == Primitive.Void)
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
			builder.Append(mangler.getCTypeName(fRefType.RetType)).Append(" (* function)(void *, ");

			foreach (Bohc.TypeSystem.Type t in fRefType.ParamTypes)
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
					builder.Append(mangler.getCTypeName(v.Type));
					builder.Append(" ");
					builder.Append(mangler.getHeapVarDeclName(v));
					builder.AppendLine(";");
				}
				--indentation;
				builder.AppendLine("};");

				lambdaNames[l] = "l" + i.ToString();

				builder.Append(mangler.getCTypeName(l.type.RetType));
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

		private class DependencyBasedTypeSorter : IComparer<Bohc.TypeSystem.Type>
		{
			private static bool containsTypeOther(Bohc.TypeSystem.Type type, Bohc.TypeSystem.Type other)
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

			private static bool containsTypeOther(Struct str, Bohc.TypeSystem.Type other)
			{
				foreach (Field f in str.Fields)
				{
					if (f.Type == other)
					{
						return true;
					}

					if (containsTypeOther(f.Type, other))
					{
						return true;
					}
				}
				return false;
			}

			private static bool containsTypeOther(FunctionRefType frt, Bohc.TypeSystem.Type other)
			{
				return containsTypeOther(frt.RetType, other) || frt.ParamTypes.Any(x => containsTypeOther(x, other));
			}

			int IComparer<Bohc.TypeSystem.Type>.Compare(Bohc.TypeSystem.Type l, Bohc.TypeSystem.Type r)
			{
				if (containsTypeOther(l, r))
				{
					return -1;
				}

				return 0;
			}
		}

		public void finish(IEnumerable<Bohc.TypeSystem.Type> types)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("#pragma once");
			builder.AppendLine("#include \"boh_internal.h\"");
			builder.AppendLine("#include <gc/gc.h>");
			builder.AppendLine("#include <stdint.h>");
			builder.AppendLine("#include <stddef.h>");
			builder.AppendLine("#include <setjmp.h>");
			builder.AppendLine("#include \"function_types.h\"");

			foreach (Bohc.TypeSystem.Type c in types.Where(x => (x is Class && !(x is Struct)) || x is Interface))
			{
				addStructPrototype(builder, c);
			}

			foreach (Bohc.TypeSystem.Type t in types
			         .Where(x => !(x is Primitive))
			         .Where(x => (!(x is Class) || x is Struct) && !(x is Interface)).Concat(FunctionRefType.Instances).OrderBy(x => x, new DependencyBasedTypeSorter()))
			{
				if (t is Struct || t is Bohc.TypeSystem.Enum)
				{
					builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".type.h");
					builder.AppendLine();
				}
				else
				{
					addFunctionRefTypeDef(builder, t as FunctionRefType);
				}
			}

			foreach (Bohc.TypeSystem.Type t in types.Where(x => !(x is FunctionRefType)))
			{
				builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".type.h");
				builder.AppendLine();

				builder.AppendFormat("#include \"{0}\"", mangler.getCName(t) + ".sigs.h");
				builder.AppendLine();
				builder.AppendLine();
			}

			System.IO.File.WriteAllText(".c/all.h", builder.ToString());
		}

		public void generateFor(Bohc.TypeSystem.Type type, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			others = others.Except(new[] { type });

			generateHeaders(type, others);

			if (!type.IsExtern() && !(type is Bohc.TypeSystem.Primitive))
			{
				string code = generateCode(type, others);
				System.IO.File.WriteAllText(".c/" + mangler.getCodeFileName(type), code);
			}
		}

		private void generateHeaders(Bohc.TypeSystem.Type type, IEnumerable<Bohc.TypeSystem.Type> others)
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

			Bohc.TypeSystem.Enum e = type as Bohc.TypeSystem.Enum;
			if (e != null)
			{
				generateEnumHeaders(e, others);
				return;
			}

			if (type is Bohc.TypeSystem.Primitive)
			{
				return;
			}

			throw new NotImplementedException();
		}

		private void generateEnumHeaders(Bohc.TypeSystem.Enum e, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			addIncludeGuard(builder, e);
			builder.AppendLine();

			builder.AppendLine(mangler.getCStructName(e));
			builder.AppendLine("{");
			
			++indentation;

			int i = 0;
			foreach (Enumerator enumerator in e.Enumerators)
			{
				addIndent(builder);
				builder.Append(mangler.getEnumeratorName(enumerator));
				builder.Append(" = ");
				builder.Append(i++);
				builder.AppendLine(",");
			}

			if (e.Enumerators.Count > 0)
			{
				builder.Remove(builder.Length - Environment.NewLine.Length - 1, 1);
			}

			builder.AppendLine("};");

			--indentation;

			System.IO.File.WriteAllText(".c/" + mangler.getCName(e) + ".type.h", builder.ToString());
			System.IO.File.WriteAllText(".c/" + mangler.getCName(e) + ".sigs.h", string.Empty);
		}

		private void generateInterfaceHeaders(Interface iface, IEnumerable<Bohc.TypeSystem.Type> others)
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
			builder.Append(mangler.getCTypeName(f.ReturnType));
			builder.Append(" (*");
			builder.Append(mangler.getVFuncName(f));
			builder.Append(")(");
			addFunctionParams(builder, f);
			builder.Append(")");
		}

		private void addInterfaceFuncs(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.Implements)
			{
				addInterfaceFuncs(builder, impl);
			}

			foreach (Function f in iface.Functions)
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
			builder.Append(mangler.getCTypeName(StdType.Obj));
			builder.AppendLine(" object;");
			addInterfaceFuncs(builder, iface);
			builder.AppendLine("};");
		}

		private void addInterfaceNewOpParams(StringBuilder builder, Interface iface)
		{
			foreach (Interface impl in iface.Implements)
			{
				addInterfaceNewOpParams(builder, impl);
			}

			foreach (Function f in iface.Functions)
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
			builder.Append(mangler.getCTypeName(StdType.Obj));
			builder.Append(" object, ");

			addInterfaceNewOpParams(builder, iface);

			builder.Remove(builder.Length - 2, 2);

			builder.AppendLine(");");
		}

		private void addIncludeGuard(StringBuilder builder, Bohc.TypeSystem.Type type)
		{
			builder.AppendLine("#pragma once");
		}

		private void addStructPrototypes(StringBuilder builder, IEnumerable<Bohc.TypeSystem.Type> others)
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

		private void addIncludes(StringBuilder builder, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			//addIncludesOnlySpecified(builder, others);
			builder.AppendLine("#include \"all.h\"");
		}

		private void addStructPrototype(StringBuilder builder, Bohc.TypeSystem.Type c)
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
			int paramCount = 0;
			if (!func.Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append(mangler.getThisParamTypeName(func.Owner));
				builder.Append(" self, ");
				++paramCount;
			}
			else if (!func.Modifiers.HasFlag(Modifiers.Native))
			{
				builder.Append("void * const ");
				if (proj.gccOnly)
				{
					builder.Append(" __attribute__((unused)) ");
				}
				builder.Append("dummy, ");
				++paramCount;
			}

			foreach (Parameter p in func.Parameters)
			{
				builder.Append(mangler.getParamTypeName(p));
				builder.Append(" ");
				builder.Append(mangler.getVarName(p));
				builder.Append(", ");
				++paramCount;
			}

			Indexer idxer = func as Indexer;
			if (idxer != null && idxer.IsAssignment())
			{
				builder.Append(mangler.getParamTypeName(idxer.Assignment));
				builder.Append(" ");
				builder.Append(mangler.getVarName(idxer.Assignment));
				builder.Append(", ");
				++paramCount;
			}

			if (paramCount > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
		}

		private void addPlatformModifiers(StringBuilder builder, Modifiers modifiers)
		{
			modifiers = ModifierHelper.GetPfMods(modifiers);
			if (modifiers != Modifiers.None)
			{
				builder.Append("#if defined(");
				builder.Append(modifiers.ToString().ToUpperInvariant().Replace(", ", ") || defined("));
				builder.AppendLine(")");
			}
		}

		private void addPlatformModClose(StringBuilder builder, Modifiers modifiers)
		{
			modifiers = ModifierHelper.GetPfMods(modifiers);
			if (modifiers != Modifiers.None)
			{
				builder.AppendLine("#endif");
			}
		}

		private void addFunctionSig(StringBuilder builder, Function func, string prefix)
		{
			if (func.Modifiers.HasFlag(Modifiers.Native) &&
				(func.Identifier == "boh_gc_alloc" ||
				func.Identifier == "boh_gc_realloc" ||
				func.Identifier == "boh_gc_collect" ||
				func.Identifier == "malloc" ||
				func.Identifier == "free" ||
				func.Identifier == "realloc"))
			{
				return;
			}

			addPlatformModifiers(builder, func.Modifiers);
			builder.Append(prefix);
			builder.Append(mangler.getCTypeName(func.ReturnType));
			builder.Append(" ");
			builder.Append(mangler.getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);
			
			builder.Append(");");
			builder.AppendLine();
			addPlatformModClose(builder, func.Modifiers);
		}

		private void addFunctionSigs(StringBuilder builder, IEnumerable<Function> funcs, string prefix)
		{
			foreach (Function f in funcs)
			{
				addFunctionSig(builder, f, prefix);
			}
		}

		private void addArrayInitSig(Class c, StringBuilder builder)
		{
			builder.Append("extern ");
			builder.Append(mangler.getCTypeName(c));
			builder.Append(" init_");
			builder.Append(mangler.getCName(c));
			builder.AppendLine("(int32_t count, ...);");
		}

		private void addArrayInitDef(Class c, StringBuilder builder)
		{
			builder.Append(mangler.getCTypeName(c));
			builder.Append(" init_");
			builder.Append(mangler.getCName(c));
			builder.AppendLine("(int32_t count, ...)");
			builder.AppendLine("{");
			++indentation;

			addIndent(builder);
			builder.AppendLine("int32_t i;");
			addIndent(builder);
			builder.AppendLine("va_list ap;");
			addIndent(builder);
			builder.Append(mangler.getCTypeName(c));
			builder.Append(" a = ");
			builder.Append(mangler.getNewName(c.Constructors.Single(x => x.Parameters.Count == 1 && x.Parameters.Single().Type == Primitive.Int)));
			builder.AppendLine("(count);");
			addIndent(builder);
			builder.AppendLine("va_start(ap, count);");
			addIndent(builder);
			builder.AppendLine("for (i = 0; i < count; ++i)");

			addIndent(builder);

			builder.AppendLine("{");
			++indentation;
			addIndent(builder);

			Indexer idx = c.Functions.OfType<Indexer>().Single(x => x.IsAssignment());

			builder.Append(mangler.getCFuncName(idx));
			builder.Append("(a, i, va_arg(ap, ");
			builder.Append(mangler.getCTypeName(idx.ReturnType));
			builder.AppendLine("));");

			--indentation;
			addIndent(builder);

			builder.AppendLine("}");
			addIndent(builder);
			builder.AppendLine("va_end(ap);");
			addIndent(builder);
			builder.AppendLine("return a;");

			--indentation;
			builder.AppendLine("}");
		}

		private void addTypeOfSig(StringBuilder builder, Bohc.TypeSystem.Type type)
		{
			string cname = mangler.getCName(type);
			string ctype = mangler.getCTypeName(StdType.Type);

			builder.Append("extern ");
			builder.Append(ctype);
			builder.Append(" typeof_");
			builder.Append(cname);
			builder.AppendLine("(void);");
			
		}

		private void addTypeOfDef(StringBuilder builder, Bohc.TypeSystem.Type type)
		{
			string cname = mangler.getCName(type);
			string ctype = mangler.getCTypeName(StdType.Type);

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
			addPlatformModifiers(builder, constr.Modifiers);

			builder.Append("extern ");
			builder.Append(mangler.getCTypeName(constr.Owner));
			builder.Append(" ");
			builder.Append(mangler.getNewName(constr));
			builder.Append("(");
			foreach (Parameter p in constr.Parameters)
			{
				builder.Append(mangler.getCTypeName(p.Type));
				builder.Append(" ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
				builder.Append(", ");
			}
			if (constr.Parameters.Count > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			else
			{
				builder.Append("void");
			}
			builder.AppendLine(");");

			addPlatformModClose(builder, constr.Modifiers);
		}

		private void addNewOperatorSigs(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.Constructors)
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
			if (c.Super != null && c.Super != StdType.Obj)
			{
				addIndent(builder);
				builder.Append(mangler.getFieldInitName(c.Super));
				builder.AppendLine("(self);");
			}

			foreach (Field f in c.Fields.Where(x => !x.Modifiers.HasFlag(Modifiers.Static)))
			{
				addIndent(builder);
				builder.Append("self->");
				builder.Append(mangler.getVarUsageName(f, lambdaStack));
				builder.Append(" = ");
				if (f.Initial != null)
				{
					addExpression(builder, f.Initial);
				}
				else
				{
					addExpression(builder, f.Type.DefaultVal());
				}
				builder.AppendLine(";");
			}

			--indentation;
			builder.AppendLine("}");
		}

		private void addNewOperator(StringBuilder builder, Constructor constr)
		{
			addPlatformModifiers(builder, constr.Modifiers);

			builder.Append(mangler.getCTypeName(constr.Owner));
			builder.Append(" ");
			builder.Append(mangler.getNewName(constr));
			builder.Append("(");
			foreach (Parameter p in constr.Parameters)
			{
				builder.Append(mangler.getCTypeName(p.Type));
				builder.Append(" ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
				builder.Append(", ");
			}
			if (constr.Parameters.Count > 0)
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
			builder.Append(mangler.getCTypeName(constr.Owner));
			builder.Append(" result");
			if (constr.Owner is Struct)
			{
				builder.AppendLine(";");
			}
			else
			{
				builder.Append(" = boh_gc_alloc(sizeof(");
				builder.Append(mangler.getCStructName(constr.Owner));
				builder.AppendLine("));");
			}

			if (!(constr.Owner is Struct))
			{
				addIndent(builder);
				builder.Append("result->vtable = &instance_");
				builder.Append(mangler.getVtableName((Bohc.TypeSystem.Class)constr.Owner));
				builder.AppendLine(";");
			}

			addIndent(builder);
			builder.Append(mangler.getCFuncName(((Class)constr.Owner).StaticConstr));
			builder.AppendLine("(NULL);");

			addIndent(builder);
			builder.Append(mangler.getFieldInitName((Class)constr.Owner));
			builder.Append("(");
			if (constr.Owner is Struct)
			{
				builder.Append("&");
			}
			builder.AppendLine("result);");

			addIndent(builder);
			builder.Append(mangler.getCFuncName(constr));
			builder.Append("(");
			if (constr.Owner is Struct)
			{
				builder.Append("&");
			}
			builder.Append("result");

			foreach (Parameter p in constr.Parameters)
			{
				builder.Append(", ");
				builder.Append(mangler.getVarUsageName(p, lambdaStack));
			}

			builder.AppendLine(");");

			addIndent(builder);
			builder.AppendLine("return result;");
			--indentation;
			
			builder.AppendLine("}");

			addPlatformModClose(builder, constr.Modifiers);
		}

		private void addNewOperators(StringBuilder builder, Class c)
		{
			foreach (Constructor constr in c.Constructors)
			{
				addNewOperator(builder, constr);
			}
		}

		private void addStaticFieldProto(StringBuilder builder, Field field, string prefix)
		{
			addPlatformModifiers(builder, field.Modifiers);
			builder.Append(prefix);
			builder.Append(mangler.getCTypeName(field.Type));
			if (field.Modifiers.HasFlag(Modifiers.Final) && field.Type is Primitive && field.Initial != null)
			{
				builder.Append(" const");
			}
			builder.Append(" ");
			builder.Append(mangler.getVarUsageName(field, lambdaStack));
			if (prefix == string.Empty && field.Modifiers.HasFlag(Modifiers.Final) && field.Type is Primitive && field.Initial != null)
			{
				builder.Append(" = ");
				addExpressionImplCon(builder, field.Initial, field.Type);
			}
			builder.AppendLine(";");
			addPlatformModClose(builder, field.Modifiers);
		}

		private void addStaticFieldProtos(StringBuilder builder, IEnumerable<Field> fields, string prefix)
		{
			foreach (Field f in fields.Where(x => x.Modifiers.HasFlag(Modifiers.Static)))
			{
				addStaticFieldProto(builder, f, prefix);
			}
		}

		private void addVtableMembers(StringBuilder builder, Bohc.TypeSystem.Class c)
		{
			Bohc.TypeSystem.Class super = c.Super;
			if (super != null)
			{
				addVtableMembers(builder, super);
			}

			++indentation;
			foreach (Function f in c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Virtual) || x.Modifiers.HasFlag(Modifiers.Abstract)))
			{
				addPlatformModifiers(builder, f.Modifiers);
				addIndent(builder);
				builder.Append(mangler.getCTypeName(f.ReturnType));
				builder.Append(" (*");
				builder.Append(mangler.getVFuncName(f));
				builder.Append(")(");
				addFunctionParams(builder, f);
				builder.AppendLine(");");
				addPlatformModClose(builder, f.Modifiers);
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

		private int addStructFields(StringBuilder builder, Bohc.TypeSystem.Class c, bool hideprivs)
		{
			int i = 0;
			if (c.Super != null)
			{
				i = addStructFields(builder, c.Super, true);
			}

			++indentation;
			foreach (Field f in c.Fields.Where(x => !x.Modifiers.HasFlag(Modifiers.Static)))
			{
				addPlatformModifiers(builder, f.Modifiers);
				addIndent(builder);
				builder.Append(mangler.getCTypeName(f.Type));
				builder.Append(" ");
				if (hideprivs && f.Modifiers.HasFlag(Modifiers.Private))
				{
					builder.Append("_" + i++);
				}
				else
				{
					builder.Append(mangler.getVarUsageName(f, lambdaStack));
				}
				builder.AppendLine(";");
				addPlatformModClose(builder, f.Modifiers);
			}
			--indentation;
			return i;
		}

		private void addStructDefinition(StringBuilder builder, Bohc.TypeSystem.Class c)
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

			addStructFields(builder, c, false);

			builder.AppendLine("};");
		}

		private void generateStructHeaders(Bohc.TypeSystem.Struct c, IEnumerable<Bohc.TypeSystem.Type> others)
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

			addFunctionSigs(builder, c.Functions.Where(x => !x.Modifiers.HasFlag(Modifiers.Private) && !x.Modifiers.HasFlag(Modifiers.Abstract)), "extern ");
			builder.AppendLine();

			addStaticFieldProtos(builder, c.Fields.Where(x => !x.Modifiers.HasFlag(Modifiers.Private) && !x.Modifiers.HasFlag(Modifiers.Abstract)), "extern ");
			builder.AppendLine();

			System.IO.File.WriteAllText(".c/" + mangler.getCName(c) + ".sigs.h", builder.ToString());
		}

		private void generateClassHeaders(Bohc.TypeSystem.Class c, IEnumerable<Bohc.TypeSystem.Type> others)
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

			if (c.OriginalGenType == StdType.Array)
			{
				addArrayInitSig(c, builder);
				builder.AppendLine();
			}
	
			addFunctionSigs(builder, c.Functions.Where(x => !x.Modifiers.HasFlag(Modifiers.Private) && !x.Modifiers.HasFlag(Modifiers.Abstract)), "extern ");
			builder.AppendLine();
			
			addStaticFieldProtos(builder, c.Fields.Where(x => !x.Modifiers.HasFlag(Modifiers.Private) && !x.Modifiers.HasFlag(Modifiers.Abstract)), "extern ");
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

		public class FEqualComp : IEqualityComparer<Function>
		{
			public bool Equals(Function f0, Function f1)
			{
				return f0.Identifier == f1.Identifier &&
					f0.Parameters.Select(x => x.Type).SequenceEqual(f1.Parameters.Select(x => x.Type));
			}

			public int GetHashCode(Function f)
			{
				return f.GetHashCode();
			}
		}

		private void addVtableInit(StringBuilder builder, Class c, IEnumerable<Function> overriden)
		{
			Class super = c.Super;
			if (super != null)
			{
				addVtableInit(builder, super, overriden.Union(
					c.Super.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Override)),
					new FEqualComp()));
			}

			foreach (Function f in c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Abstract) || x.Modifiers.HasFlag(Modifiers.Virtual)))
			{
				// TODO: make sure this try-catch is appropriate
				Function overridenf = null;
				try
				{
					overridenf = overriden.Single(x => new FEqualComp().Equals(x, f));
				}
				catch
				{
					//Console.WriteLine(f.Identifier);
					//throw;
				}
				if (overridenf != null)
				{
					builder.Append("&" + mangler.getCFuncName(overridenf) + ", ");
				}
				else
				{
					if (f.Modifiers.HasFlag(Modifiers.Abstract))
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
			addVtableInit(builder, c, c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Override)));
			builder.Remove(builder.Length - 2, 2);
			builder.AppendLine(" };");
		}

		private void addFunctionDef(StringBuilder builder, Function func)
		{
			addPlatformModifiers(builder, func.Modifiers);

			// TODO: String[] also allowed
			if (func.Identifier == "main")
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

			if (func.Modifiers.HasFlag(Modifiers.Private))
			{
				builder.Append("");
			}
			builder.Append(mangler.getCTypeName(func.ReturnType));
			builder.Append(" ");
			builder.Append(mangler.getCFuncName(func));
			builder.Append("(");

			addFunctionParams(builder, func);

			builder.AppendLine(")");

			bool hasSuperBeenCalled = (func.Body.hasSuperBeenCalled() || (!(func.Owner is Class) || ((Class)func.Owner).Super == null));
			if (!hasSuperBeenCalled && func is Constructor && ((Class)func.Owner).Super != StdType.Obj)
			{
				func.Body.Statements.Insert(0,
					new ExpressionStatement(
					new FunctionCall(
						((Class)func.Owner).Super.Constructors.Single(x => !(x.Modifiers.HasFlag(Modifiers.Private) || x.Modifiers.HasFlag(Modifiers.CVisible)) && x.Parameters.Count == 0),
						((Class)func.Owner).ThisVarExpr, Enumerable.Empty<Expression>())));
			}

			if (func is StaticConstructor)
			{
				addStaticConstrPrebody(builder, func);
			}

			if (!(func is StaticConstructor) &&
				func.Modifiers.HasFlag(Modifiers.Static) &&
				(func.Modifiers.HasFlag(Modifiers.Public) ||
				//func.modifiers.HasFlag(Modifiers.PACKAGE) ||
				func.Modifiers.HasFlag(Modifiers.CVisible)))
			{
				func.Body.Statements.Insert(0, 
					new ExpressionStatement(
					new FunctionCall(((Class)func.Owner).StaticConstr, null, Enumerable.Empty<Expression>())));
			}

			addFnHeapParams(builder, func.Parameters);

			addBody(builder, func.Body);

			addFnHeapParamClose(builder, func.Parameters);

			if (!(func is StaticConstructor) &&
				func.Modifiers.HasFlag(Modifiers.Static) &&
				(func.Modifiers.HasFlag(Modifiers.Public) ||
				//func.modifiers.HasFlag(Modifiers.PACKAGE) ||
				func.Modifiers.HasFlag(Modifiers.CVisible)))
			{
				func.Body.Statements.RemoveAt(0);
			}

			if (func is StaticConstructor)
			{
				--indentation;
				addIndent(builder);
				builder.AppendLine("}");
			}

			if (!hasSuperBeenCalled && func is Constructor && ((Class)func.Owner).Super != StdType.Obj)
			{
				func.Body.Statements.RemoveAt(0);
			}

			addPlatformModClose(builder, func.Modifiers);
		}

		private void addFnHeapParamClose(StringBuilder builder, IEnumerable<Variable> parameters)
		{
			if (parameters.Where(x => x.Enclosed).Count() > 0)
			{
				--indentation;
				builder.AppendLine("}");
			}
		}

		private void addFnHeapParams(StringBuilder builder, IEnumerable<Variable> parameters)
		{
			if (parameters.Where(x => x.Enclosed).Count() > 0)
			{
				builder.AppendLine("{");

				++indentation;

				foreach (Variable param in parameters.Where(x => x.Enclosed))
				{
					addIndent(builder);

					builder.Append(mangler.getCTypeName(param.Type));
					builder.Append("* e");
					builder.Append(mangler.getVarName(param));
					builder.Append(" = boh_gc_alloc(sizeof(");
					builder.Append(mangler.getCTypeName(param.Type));
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
			if (((Class)func.Owner).Super != null)
			{
				addIndent(builder);
				builder.Append(mangler.getCFuncName(((Class)func.Owner).Super.StaticConstr));
				builder.AppendLine("(NULL);");
			}

			foreach (Field f in ((Class)func.Owner).Fields.Where(x => x.Modifiers.HasFlag(Modifiers.Static) && !(x.Modifiers.HasFlag(Modifiers.Final) && x.Type is Primitive && x.Initial != null)))
			{
				addIndent(builder);
				builder.Append(mangler.getVarUsageName(f, lambdaStack));
				builder.Append(" = ");
				if (f.Initial != null)
				{
					addExpression(builder, f.Initial);
				}
				else
				{
					addExpression(builder, f.Type.DefaultVal());
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
			if (f.Modifiers.HasFlag(Modifiers.Override) || f.Modifiers.HasFlag(Modifiers.Virtual) || f.Modifiers.HasFlag(Modifiers.Abstract))
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

			foreach (Interface i in iface.Implements)
			{
				addInterfaceNewCallFuncs(builder, i, c, tempn);
			}

			foreach (Function other in iface.Functions)
			{
				foreach (Function f in c.GetAllFuncs())
				{
					if (f.Identifier == other.Identifier &&
						f.Modifiers.HasFlag(Modifiers.Public) &&
						f.Parameters.Select(x => x.Type).SequenceEqual(other.Parameters.Select(x => x.Type)))
					{
						addInterfaceNewCallFunc(builder, f, tempn);
						builder.Append(", ");
						break;
					}
				}
			}
		}

		private void addExpressionImplCon(StringBuilder builder, Expression expression, Bohc.TypeSystem.Type towhat)
		{
			Bohc.TypeSystem.Type type = expression.getType();
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

			IndexerCall ic = expression as IndexerCall;
			if (ic != null)
			{
				addIndexerCall(builder, ic);
				return;
			}

			SetIndexerCall sic = expression as SetIndexerCall;
			if (sic != null)
			{
				addSetIndexerCall(builder, sic);
				return;
			}

			throw new NotImplementedException();
		}

		private void addSetIndexerCall(StringBuilder builder, SetIndexerCall sic)
		{
			addFCall(builder, new FunctionCall(sic.setter, sic.belongsto, sic.parameters.Concat(new [] { sic.towhat })));
		}

		private void addIndexerCall(StringBuilder builder, IndexerCall ic)
		{
			addFCall(builder, new FunctionCall(ic.getter, ic.belongsto, ic.parameters));
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
				if (v.Identifier != "this")
				{
					preStat.Append("&");
				}
				preStat.Append(mangler.getVarUsageName(v, lambdaStack));
				preStat.Append(";");
				addPreStatStatement(preStat.ToString());
			}
			builder.Append(tmp);
		}

		private void addFvCall(StringBuilder builder, FunctionVarCall fvCall)
		{
			string tmp = addTemp(fvCall.belongsto);
			StringBuilder tempBuild = new StringBuilder();
			tempBuild.Append(tmp + " = ");
			addNullCheck(tempBuild, fvCall.belongsto);
			tempBuild.Append(";");
			addPreStatStatement(tempBuild.ToString());
			builder.Append(tmp + ".function(" + tmp + ".context, ");

			IEnumerator<Bohc.TypeSystem.Type> fparams = (fvCall.belongsto.getType() as FunctionRefType).ParamTypes.Where(x => true).GetEnumerator();
			foreach (Expression param in fvCall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, (Bohc.TypeSystem.Type)fparams.Current);
				builder.Append(", ");
			}

			builder.Remove(builder.Length - 2, 2);

			builder.Append(")");
		}

		private void addTypeCast(StringBuilder builder, TypeCast tc)
		{
			// TODO: check if Object.cast<T> is needed
			if (tc.towhat.Extends(tc.onwhat.getType()) > 0 ||
				tc.onwhat.getType().Extends(tc.towhat) > 0)
			{
				addExpressionImplCon(builder, tc.onwhat, tc.towhat);
			}
			else
			{

			}
		}

		private void addLiteral(StringBuilder builder, Literal lit)
		{
			Bohc.TypeSystem.Type type = lit.getType();

			Primitive prim = type as Primitive;
			if (prim != null)
			{
                if (prim == Primitive.Char)
                {
					addCharLit(builder, lit);
				}
                else if (prim == Primitive.Boolean)
                {
                    builder.Append(lit.representation == "true" ? 1 : 0);
                }
                else if (prim.IsInt())
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
                else if (prim.IsFloat())
                {
                    if (prim == Primitive.Double)
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
                else if (prim == Primitive.Decimal)
                {
					Boh.Exception.require<CodeGenException>(proj.gccOnly, "decimal literals may only be used in gcc mode");
                    builder.Append(lit.representation.ToUpperInvariant());
                }
			}
			else
			{
				if (lit.type == StdType.Str && lit.representation != "NULL")
				{
					addStrLit(builder, lit);
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

		private void addCharLit(StringBuilder builder, Literal lit)
		{
			builder.Append("'");
			string actStr = lit.representation.Substring(1, lit.representation.Length - 2);
			UTF8Encoding encoding = new UTF8Encoding();

			int byteCount = 0;
			for (int i = 0; i < actStr.Length; ++i)
			{
				char ch = actStr[i];
				if (ch == '\\')
				{
					foreach (byte b in encoding.GetBytes(getEscSeq(actStr, ++i)))
					{
						builder.AppendFormat("\\x{0:X2}", b);
						++byteCount;
					}
					continue;
				}
				foreach (byte b in encoding.GetBytes(new string(ch, 1)))
				{
					builder.AppendFormat("\\x{0:X2}", b);
					++byteCount;
				}
			}

			builder.Append("'");
		}

		private void addStrLit(StringBuilder builder, Literal lit)
		{
			builder.Append("boh_create_string(\"");
			string actStr = lit.representation.Substring(1, lit.representation.Length - 2);
			UTF8Encoding encoding = new UTF8Encoding();

			int byteCount = 0;
			for (int i = 0; i < actStr.Length; ++i)
			{
				char ch = actStr[i];
				if (ch == '\\')
				{
					foreach (byte b in encoding.GetBytes(getEscSeq(actStr, ++i)))
					{
						builder.AppendFormat("\\x{0:X2}", b);
						++byteCount;
					}
					continue;
				}
				foreach (byte b in encoding.GetBytes(new string(ch, 1)))
				{
					builder.AppendFormat("\\x{0:X2}", b);
					++byteCount;
				}
			}

			builder.Append("\", ");
			builder.Append(byteCount);
			builder.Append(")");
		}

		private static string getEscSeq(string str, int i)
		{
			char next = str[i];
			switch (next)
			{
				case '0':
					return "\0";
				case 'a':
					return "\a";
				case 'b':
					return "\b";
				case 'f':
					return "\f";
				case 'n':
					return "\n";
				case 'r':
					return "\r";
				case 't':
					return "\t";
				case 'v':
					return "\v";
				case '\'':
					return "\'";
				case '"':
					return "\"";
				case '\\':
					return "\\";
				case 'u':
					++i;
					string rep = str.Substring(i, 2);
					byte b = byte.Parse(rep);

					UTF8Encoding enc = new UTF8Encoding();
					return enc.GetString(new[] { b });
			}
			throw new CodeGenException();
		}

		private void addCCall(StringBuilder builder, ConstructorCall ccall)
		{
			builder.Append(mangler.getNewName(ccall.function));
			builder.Append("(");

			IEnumerator<Parameter> fparams = ccall.function.Parameters.GetEnumerator();
			foreach (Expression param in ccall.parameters)
			{
				fparams.MoveNext();

				addExpressionImplCon(builder, param, fparams.Current.Type);
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
			if (nfcall.function == NativeFunction.NativeDeref)
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
			else if (nfcall.function == NativeFunction.NativeRef)
			{
				builder.Append("&");
				addExpression(builder, nfcall.parameters [0]);
			}
			else if (nfcall.function == NativeFunction.NativeSizeof)
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
			ExprVariable evar = expr as ExprVariable;

			// TODO: check if null check is required
			// not required for thisvars
			if (!proj.unsafeNullPtr && (evar == null || !evar.refersto.NullChecked))
			{
				if (expr.getType() is FunctionRefType)
				{
					builder.Append("boh_fp_check_null(");
				}
				else
				{
					builder.Append("boh_check_null(");
				}

				string tmp = addTemp(expr);
				StringBuilder b = new StringBuilder();
				b.Append(tmp);
				b.Append(" = ");
				addExpression(b, expr);
				b.Append(";");
				addPreStatStatement(b.ToString());

				builder.Append(tmp);
			}
			else
			{
				addExpression(builder, expr);
			}

			if (!proj.unsafeNullPtr && (evar == null || !evar.refersto.NullChecked))
			{
				builder.Append(", ");
				builder.Append(mangler.getCTypeName(expr.getType()));
				builder.Append(")");
			}

			if (evar != null)
			{
				evar.refersto.NullChecked = true;
			}
		}

		private void addArrayInit(StringBuilder builder, Expression[] exprs, TypeSystem.Type itemtype)
		{
			TypeSystem.Class array = (TypeSystem.Class)StdType.Array.GetTypeFor(new [] { itemtype }, null);
			builder.Append("init_");
			builder.Append(mangler.getCName(array));
			builder.Append("(");
			builder.Append(exprs.Count());
			builder.Append(", ");
			foreach (Expression expr in exprs)
			{
				addExpressionImplCon(builder, expr, itemtype);
				builder.Append(", ");
			}
			if (exprs.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			builder.Append(")");
		}

		private void addVirtCallStart(StringBuilder builder, FunctionCall fcall)
		{
			ExprVariable svar = fcall.belongsto as ExprVariable;
			if (svar != null && svar.refersto.Identifier == "super")
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

		private void addInstanceCallStart(StringBuilder builder, FunctionCall fcall)
		{
			if (fcall.refersto.Owner is Struct)
			{
				string tempname = addTemp(mangler.getCTypeName(fcall.refersto.Owner) + " ", ";");
				builder.Append(mangler.getCFuncName(fcall.refersto));
				builder.Append("((");
				builder.Append(tempname);
				builder.Append(" = ");
				addExpression(builder, fcall.belongsto);
				builder.Append(", &");
				builder.Append(tempname);
				builder.Append("), ");
			}
			else if (fcall.refersto.Owner is Class || fcall.refersto.Owner is Bohc.TypeSystem.Enum || fcall.refersto.Owner is Bohc.TypeSystem.Primitive)
			{
				builder.Append(mangler.getCFuncName(fcall.refersto));
				builder.Append("(");
				if (fcall.refersto.Owner.IsReferenceType())
				{
					addNullCheck(builder, fcall.belongsto);
				}
				else
				{
					addExpression(builder, fcall.belongsto);
				}
				builder.Append(", ");
			}
			else
			{
				string temp = addTemp(fcall.belongsto);
				builder.Append("(");
				builder.Append(temp);
				builder.Append(" = ");
				if (fcall.refersto.Owner.IsReferenceType())
				{
					addNullCheck(builder, fcall.belongsto);
				}
				else
				{
					addExpression(builder, fcall.belongsto);
				}
				builder.Append(")->");
				builder.Append(mangler.getVFuncName(fcall.refersto));
				builder.Append("(");
				builder.Append(temp);
				builder.Append("->object, ");
			}
		}

		void addStaticCallStart(StringBuilder builder, FunctionCall fcall)
		{
			builder.Append(mangler.getCFuncName(fcall.refersto));
			if (fcall.refersto.Modifiers.HasFlag(Modifiers.Native))
			{
				builder.Append("(");
			}
			else
			{
				builder.Append("(NULL, ");
			}
		}

		private void addFCall(StringBuilder builder, FunctionCall fcall)
		{
			if (((fcall.refersto.Modifiers.HasFlag(Modifiers.Virtual) ||
				fcall.refersto.Modifiers.HasFlag(Modifiers.Abstract) ||
				(fcall.refersto.Modifiers.HasFlag(Modifiers.Override) &&
			 	!fcall.refersto.Modifiers.HasFlag(Modifiers.Final))) &&
			 	
				!fcall.belongsto.getType().Modifiers.HasFlag(Modifiers.Final)) &&
				
			    !(fcall.refersto.Owner is Struct))
			{
				addVirtCallStart(builder, fcall);
			}
			else if (!fcall.refersto.Modifiers.HasFlag(Modifiers.Static))
			{
				addInstanceCallStart(builder, fcall);
			}
			else
			{
				addStaticCallStart(builder, fcall);
			}

			int take = fcall.refersto.IsVariadic() ? fcall.refersto.Parameters.Count - 1 : fcall.parameters.Length;

			IEnumerator<Parameter> fparams = fcall.refersto.Parameters.Take(take).GetEnumerator();
			foreach (Expression param in fcall.parameters.Take(take))
			{
				if (fparams.MoveNext())
				{
					addExpressionImplCon(builder, param, fparams.Current.Type);
				}
				else
				{
					addExpression(builder, param);
				}
				builder.Append(", ");
			}

			if (fcall.refersto.IsVariadic())
			{
				Class icoll = (Class)fcall.refersto.Parameters.Last().Type;
				Class iter = (Class)icoll.Functions.Single(x => x.Identifier == "iterator").ReturnType;
				TypeSystem.Type type = iter.Functions.Single(x => x.Identifier == "get").ReturnType;
				addArrayInit(builder, fcall.parameters.Skip(take).ToArray(), type);
				builder.Append(", ");
			}

			if (!fcall.refersto.Modifiers.HasFlag(Modifiers.Native) || fcall.refersto.Parameters.Count > 0)
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
				Function objEq = StdType.Obj.Functions.Single(x => x.Identifier == "valEquals");
				builder.Append(mangler.getCFuncName(objEq));
				builder.Append("(NULL, ");
				addExpressionImplCon(builder, left, StdType.Obj);
				builder.Append(", ");
				addExpressionImplCon(builder, right, StdType.Obj);
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

				if (binop.operation == BinaryOperation.ASSIGN)
				{
					ExprVariable evar = binop.left as ExprVariable;
					if (evar != null)
					{
						evar.refersto.NullChecked = false;
					}
				}
			}
		}

		private void addTypeOf(StringBuilder builder, Bohc.TypeSystem.Type type)
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
				addExpression(builder, (unop.onwhat as ExprType).type.DefaultVal());
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
			if (f != null && f.Modifiers.HasFlag(Modifiers.Static | Modifiers.Public))
			{
				StringBuilder temp = new StringBuilder();
				temp.Append(mangler.getCFuncName(f.Owner.StaticConstr));
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

			foreach (Statement s in body.Statements)
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
			if (fore.expr.getType() is Bohc.TypeSystem.Class)
			{
				fcall = new FunctionCall(((Bohc.TypeSystem.Class)fore.expr.getType()).Functions.Single(x => x.Identifier == "iterator" && x.Parameters.Count() == 0), fore.expr, Enumerable.Empty<Expression>());
			}
			else
			{
				fcall = new FunctionCall(((Bohc.TypeSystem.Interface)fore.expr.getType()).GetFunctions("iterator", null).Single(x => x.Parameters.Count() == 0), fore.expr, Enumerable.Empty<Expression>());
			}
			string tmp = addTemp(fcall);
			builder.Append(tmp);
			builder.Append(" = ");
			addExpressionImplCon(builder, fcall, StdType.IIterator.GetTypeFor(new[] { fore.vardecl.refersto.Type }, null));
			builder.AppendLine(";");

			addIndent(builder);
			builder.Append("while (");
			builder.Append(tmp);
			builder.Append("->vtable->");
			builder.Append(mangler.getVFuncName(((Bohc.TypeSystem.Class)fcall.getType()).Functions.Single(x => x.Identifier == "next")));
			builder.Append("(");
			builder.Append(tmp);
			builder.AppendLine("))");

			addIndent(builder);
			builder.AppendLine("{");

			++indentation;
			addVarDec(builder, fore.vardecl);
			addIndent(builder);
			builder.Append(mangler.getVarUsageName(fore.vardecl.refersto, lambdaStack));
			builder.Append(" = ");
			builder.Append(tmp);
			builder.Append("->vtable->");
			builder.Append(mangler.getVFuncName(((Bohc.TypeSystem.Class)fcall.getType()).Functions.Single(x => x.Identifier == "get")));
			builder.Append("(");
			builder.Append(tmp);
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
			builder.Append(mangler.getCName(cstat.param.Type));
			builder.AppendLine("())");

			addIndent(builder);
			builder.AppendLine("{");
			++indentation;

			string typeusename = mangler.getCTypeName(cstat.param.Type);
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

			if (((Local)vdec.refersto).Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append("static ");
			}

			builder.Append(mangler.getCTypeName(vdec.refersto.Type));
			if (vdec.refersto.Enclosed && !((Local)vdec.refersto).Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append("*");
			}
			else if (vdec.refersto.Modifiers.HasFlag(Modifiers.Final) && vdec.refersto.Type is Primitive)
			{
				builder.Append(" const");
			}

			builder.Append(" ");
			builder.Append(mangler.getVarName(vdec.refersto));

			if (vdec.refersto.Enclosed && !((Local)vdec.refersto).Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append(" = boh_gc_alloc(sizeof(");
				builder.Append(mangler.getCTypeName(vdec.refersto.Type));
				builder.Append("))");
				if (vdec.initial != null)
				{
					builder.AppendLine(";");
					addIndent(builder);
					builder.Append(mangler.getVarUsageName(vdec.refersto, lambdaStack));
				}
			}

			if (vdec.initial != null && !((Local)vdec.refersto).Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append(" = ");
				addExpressionImplCon(builder, vdec.initial, vdec.refersto.Type);
			}

			builder.AppendLine(";");

			if (vdec.initial != null && ((Local)vdec.refersto).Modifiers.HasFlag(Modifiers.Static))
			{
				addIndent(builder);
				builder.Append(mangler.getVarUsageName(vdec.refersto, lambdaStack));
				builder.Append(" = ");
				addExpressionImplCon(builder, vdec.initial, vdec.refersto.Type);
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

		private string generateCode(Bohc.TypeSystem.Type type, IEnumerable<Bohc.TypeSystem.Type> others)
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

			Bohc.TypeSystem.Enum e = type as Bohc.TypeSystem.Enum;
			if (e != null)
			{
				return generateEnumCode(e, others);
			}

			throw new NotImplementedException();
		}

		private string generateEnumCode(Bohc.TypeSystem.Enum e, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(mangler.getHeaderName(e));
			builder.AppendLine("\"");

			addFunctionDef(builder, e.ToStringM);

			return builder.ToString();
		}

		private void addInterfaceConstructor(StringBuilder builder, Interface iface)
		{
			builder.Append(mangler.getCTypeName(iface));
			builder.Append(" new_");
			builder.Append(mangler.getCName(iface));
			builder.Append("(");
			builder.Append(mangler.getCTypeName(StdType.Obj));
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
			foreach (Interface impl in iface.Implements)
			{
				addInterfaceAssignments(builder, impl);
			}

			foreach (Function f in iface.Functions)
			{
				builder.Append("\tresult->");
				builder.Append(mangler.getVFuncName(f));
				builder.Append(" = ");
				builder.Append(mangler.getVFuncName(f));
				builder.AppendLine(";");
			}
		}

		private string generateInterfaceCode(Interface iface, IEnumerable<Bohc.TypeSystem.Type> others)
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

		private string generateClassCode(Class c, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("#include \"");
			builder.Append(mangler.getHeaderName(c));
			builder.AppendLine("\"");
			builder.AppendLine();

			addIncludes(builder, others);

			addStaticFieldProtos(builder, c.Fields, string.Empty);
			builder.AppendLine();

			addFunctionSigs(builder, c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Private) && !x.Modifiers.HasFlag(Modifiers.Abstract) && !x.Modifiers.HasFlag(Modifiers.Native)), "static ");
			addFunctionSigs(builder, c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Native)), string.Empty);
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

			if (c.OriginalGenType == StdType.Array)
			{
				addArrayInitDef(c, builder);
				builder.AppendLine();
			}
			
			addFunctionDefs(builder, c.Functions.Where(x => x.Body != null));

			return builder.ToString();
		}
	}
}
