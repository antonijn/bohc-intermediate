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

namespace Bohc.Generation.Llvm
{
	public class LlvmCodeGen : ICodeGen
	{

		private IMangler mangler;

		private Dictionary<Function, LlvmFunction> functions = new Dictionary<Function, LlvmFunction>();
		private Dictionary<Constructor, LlvmFunction> newfuncs = new Dictionary<Constructor, LlvmFunction>();
		private Dictionary<TypeSystem.Type, LlvmFunction> fieldinits = new Dictionary<TypeSystem.Type, LlvmFunction>();
		private Dictionary<Variable, LlvmValue> locals = new Dictionary<Variable, LlvmValue>();
		private Dictionary<Variable, LlvmValue> svars = new Dictionary<Variable, LlvmValue>();
		private Stack<Dictionary<LlvmLabel, LlvmValue>> nullchecks = new Stack<Dictionary<LlvmLabel, LlvmValue>>();

		public readonly LlvmModule module = new LlvmModule();

		private Stack<Function> currentF = new Stack<Function>();
		private Stack<LlvmLabel> skiplabels = new Stack<LlvmLabel>();
		private Stack<LlvmLabel> restrtlabels = new Stack<LlvmLabel>();
		private Stack<LlvmTry> tries = new Stack<LlvmTry>();

		private Bohc.General.Project project;

		public LlvmCodeGen(IMangler mangler, Bohc.General.Project project)
		{
			this.mangler = mangler;
			this.project = project;
		}

		private LlvmFunction aquathrownullex = null;
		private LlvmFunction aqua_throw_null_ex()
		{
			if (aquathrownullex != null)
			{
				return aquathrownullex;
			}

			List<LlvmParam> parameters = new List<LlvmParam>();
			parameters.Add(new LlvmParam("%des", new LlvmPointer(new LlvmPrimitive("i8"))));
			aquathrownullex = new LlvmFunction(new LlvmPrimitive("void"),
			                             "@aqua_throw_null_ptr_ex",
			                             parameters,
			                             LlvmLinkage.None, false);
			module.AddDeclaration(aquathrownullex);
			return aquathrownullex;
		}

		private LlvmFunction aquaalloc = null;
		private LlvmFunction aqua_alloc()
		{
			if (aquaalloc != null)
			{
				return aquaalloc;
			}

			List<LlvmParam> parameters = new List<LlvmParam>();
			parameters.Add(new LlvmParam("%size", new LlvmPrimitive("i64")));
			aquaalloc = new LlvmFunction(new LlvmPointer(new LlvmPrimitive("i8")),
			                             "@aqua_alloc",
			                             parameters,
										LlvmLinkage.None, false);
			module.AddDeclaration(aquaalloc);
			return aquaalloc;
		}

		private LlvmFunction fieldinit(Class ty)
		{
			if (fieldinits.ContainsKey(ty))
			{
				return fieldinits[ty];
			}
			LlvmFunction func = new LlvmFunction(new LlvmPrimitive("void"), mangler.getFieldInitName(ty),
			                                   new List<LlvmParam> { new LlvmParam("%this", 
				                                    ty.IsReferenceType() ? type(ty) : (LlvmType)new LlvmPointer(type(ty))) },
			LlvmLinkage.None, true);
			fieldinits[ty] = func;
			if (!ty.IsExtern())
			{
				Llvm llvm = new Llvm(func);

				LlvmParam self = func.parameters.Single();
				if (ty.Super != null)
				{
					llvm.AddCall(fieldinit(ty.Super), new [] { llvm.AddBitcast(self, type(ty.Super)) });
				}
				foreach (Field f in ty.Fields.Where(x => !x.Modifiers.HasFlag(Modifiers.Static))
				         .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Desktop32)))
				{
					LlvmValue sptr = llvm.AddGetElementPtr(self, new LlvmValue[]
					                      {
						new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
						new LlvmLiteral(new LlvmPrimitive("i32"), 
						                (ty.IsReferenceType() ?
						                (LlvmStruct)((LlvmPointer)type(ty)).t : (LlvmStruct)type(ty))
						                .Offset(mangler.getVarName(f)).ToString())
					});
					llvm.AddStore(sptr, addExpression(llvm, f.Initial != null ? f.Initial : f.Type.DefaultVal()));
				}

				llvm.AddRetVoid();
				module.AddImplementation(llvm);
			}
			else
			{
				module.AddDeclaration(func);
			}
			return func;
		}

		private LlvmFunction newfunction(Constructor f)
		{
			if (newfuncs.ContainsKey(f))
			{
				return newfuncs[f];
			}
			LlvmFunction constrthis = function(f);
			List<LlvmParam> parameters = 
				f.Parameters.Select(x => new LlvmParam("%" + x.Identifier, type(x.Type))).ToList();
			LlvmFunction func = new LlvmFunction(type(f.Owner), mangler.getNewName(f),
			                                     parameters,
			                                     f.Modifiers.HasFlag(Modifiers.Private) &&
			                                     !f.Modifiers.HasFlag(Modifiers.Native) ?
			                                     LlvmLinkage.Internal : LlvmLinkage.None,
			                                     !f.Modifiers.HasFlag(Modifiers.Native));
			functions[f] = func;
			if (!f.Owner.IsExtern() && !f.Modifiers.HasFlag(Modifiers.Native))
			{
				Llvm llvm = new Llvm(func);
				LlvmValue mem = llvm.AddCall(aqua_alloc(), new [] { addSizeof(llvm, ((LlvmPointer)type(f.Owner)).t) });
				LlvmValue cst = llvm.AddBitcast(mem, type(f.Owner));
				LlvmValue vtablea = llvm.AddGetElementPtr(cst, new LlvmValue[]
				                                          {
					new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
					new LlvmLiteral(new LlvmPrimitive("i32"), "0")
				});
				llvm.AddStore(vtablea, vtableglobal((Class)f.Owner));

				llvm.AddCall(fieldinit((Class)f.Owner), new [] { cst });
				llvm.AddCall(constrthis, new [] { cst }.Concat(parameters));
				llvm.AddRet(cst);
				module.AddImplementation(llvm);
			}
			else
			{
				module.AddDeclaration(func);
			}
			return func;
		}

		void addFunction(Body b, IEnumerable<Variable> parameters, Llvm llvm)
		{
			nullchecks.Push(new Dictionary<LlvmLabel, LlvmValue>());
			nulllabelstack.Push(null);

			llvm.AddLabel(new LlvmLabel("entry."));
			foreach (Parameter p in parameters)
			{
				LlvmTemp tmp = new LlvmTemp(new LlvmPointer(type(p.Type)));
				llvm.AddAlloca(tmp, type(p.Type));
				llvm.AddStore(tmp,
				              llvm.func.parameters.Single(x => x.ToString() == "%" + p.Identifier));
				locals[p] = tmp;
			}
			addBody(llvm, b);

			if (!b.hasReturned())
			{
				llvm.AddRetVoid();
			}

			addNullChecks(llvm, nullchecks.Pop());
			nulllabelstack.Pop();
		}

		private LlvmFunction function(Function f)
		{
			if (functions.ContainsKey(f))
			{
				return functions[f];
			}
			List<LlvmParam> parameters = new List<LlvmParam>();
			if (!f.Modifiers.HasFlag(Modifiers.Static))
			{
				parameters.Add(new LlvmParam("%this", new LlvmParamType(
					f.Owner.IsReferenceType() ? type(f.Owner) : new LlvmPointer(type(f.Owner)))));
			}
			else if (!f.Modifiers.HasFlag(Modifiers.Native))
			{
				parameters.Add(new LlvmParam("%this", new LlvmPointer(new LlvmPrimitive("i8"))));
			}
			parameters = parameters.Concat(f.Parameters.Select(x => new LlvmParam("%" + x.Identifier, 
				                                        new LlvmParamType(type(x.Type)))))
				.ToList();
			LlvmFunction func = new LlvmFunction(type(f.ReturnType), mangler.getCFuncName(f),
			                                	parameters,
			                                     f.Modifiers.HasFlag(Modifiers.Private) &&
			                                     !f.Modifiers.HasFlag(Modifiers.Native) ?
			                                     LlvmLinkage.Internal : LlvmLinkage.None,
			                                     !f.Modifiers.HasFlag(Modifiers.Native));
			functions[f] = func;
			if (!f.Owner.IsExtern() && !f.Modifiers.HasFlag(Modifiers.Native))
			{
				Llvm llvm = new Llvm(func);
				currentF.Push(f);
				addFunction(f.Body, f.Parameters.Where(x => !x.Modifiers.HasFlag(Modifiers.Final) &&
				                                       !x.Modifiers.HasFlag(Modifiers.Ref)), llvm);
				currentF.Pop();
				module.AddImplementation(llvm);
			}
			else
			{
				module.AddDeclaration(func);
			}
			return func;
		}

		private Dictionary<TypeSystem.Type, LlvmType> types = new Dictionary<Bohc.TypeSystem.Type, LlvmType>();
		private int frts = 0;
		private LlvmType type(TypeSystem.Type t)
		{
			Primitive p = t as Primitive;
			if (p != null)
			{
				return new LlvmPrimitive(p.LlvmName);
			}

			if (types.ContainsKey(t))
			{
				return types[t];
			}

			Class c = t as Class;
			if (c != null)
			{
				Struct s = c as Struct;
				if (s != null && s.OriginalGenType == StdType.Ptr)
				{
					return new LlvmPointer(type(s.Functions.Single(x => x.Identifier == "deref").ReturnType));
				}

				return _class(c);
			}

			FunctionRefType frt = t as FunctionRefType;
			if (frt != null)
			{
				LlvmStruct str = new LlvmStruct("%function." + frts++.ToString());
				types[t] = str;
				str.members = new Dictionary<string, LlvmType>
				{
					{ "object", new LlvmPointer(new LlvmPrimitive("i8")) },
					{ "function", new LlvmFunctionPtrType(type(frt.RetType),
						                                      new[] { 
							new LlvmPointer(new LlvmPrimitive("i8")) }.Concat(frt.ParamTypes.Select(x => type(x))).ToArray()) }
				};
				module.AddStruct(str);
				return str;
			}

			throw new NotImplementedException();
		}

		private LlvmType vtablety(Class c)
		{
			LlvmStruct str = new LlvmStruct(mangler.getVtableName(c));
			str.members = vtableMembers(c).ToDictionary(x => x.Item1, x => x.Item2);
			module.AddStruct(str);
			return new LlvmPointer(str);
		}

		private Dictionary<Class, LlvmGlobal> vtableglobals = new Dictionary<Class, LlvmGlobal>();
		private LlvmGlobal vtableglobal(Class c)
		{
			if (vtableglobals.ContainsKey(c))
			{
				return vtableglobals[c];
			}
			LlvmType t = ((LlvmStruct)((LlvmPointer)type(c)).t).members["vtable"];
			LlvmStruct str = (LlvmStruct)((LlvmPointer)t).t;
			LlvmGlobal g;
			module.AddGlobal(g = new LlvmGlobal(
				c.IsExtern() ? LlvmLinkage.External : (c.Modifiers.HasFlag(Modifiers.Private) ?
			                                       LlvmLinkage.Internal : LlvmLinkage.None),
				LlvmGlobalFlags.Global, "@vtable." + mangler.getCName(c) + ".instance",
				str, c.IsExtern() ? null : new LlvmStructInit(str, getVtableInit(c).ToArray())));
			vtableglobals[c] = g;
			return g;
		}

		private IEnumerable<LlvmValue> getVtableInit(Class c)
		{
			IEnumerator<LlvmFunctionPtrType> fptrs = vtableMembers(c).Select(x => x.Item2).Cast<LlvmFunctionPtrType>().GetEnumerator();

			// skip getinterface for the moment
			fptrs.MoveNext();

			foreach (LlvmValue v in addVtableInit(c, c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Override))))
			{
				fptrs.MoveNext();

				LlvmFunctionPtrType fptr = fptrs.Current;
				if (!fptr.Equals(v.Type()))
				{
					yield return new Llvm(null).InlineBitcast(v, fptr);
				}
				else
				{
					yield return v;
				}
			}

			fptrs.Dispose();
		}

		private IEnumerable<Function> vtableFunctions(Class c)
		{
			Class super = c.Super;
			IEnumerable<Function> supers = 
				super == null ? Enumerable.Empty<Function>() : vtableFunctions(super);
			return supers.Concat(c.Functions
			                     .Where(x => x.Modifiers.HasFlag(Modifiers.Virtual) || x.Modifiers.HasFlag(Modifiers.Abstract)));
		}

		private IEnumerable<LlvmValue> addVtableInit(Class c, IEnumerable<Function> overriden)
		{
			Class super = c.Super;
			if (super != null)
			{
				foreach (LlvmValue v in addVtableInit(super, overriden.Union(
					c.Super.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Override)),
					new C.CCodeGen.FEqualComp())))
				{
					yield return v;
				}
			}

			foreach (Function f in c.Functions.Where(x => x.Modifiers.HasFlag(Modifiers.Abstract) || x.Modifiers.HasFlag(Modifiers.Virtual)))
			{
				// TODO: make sure this try-catch is appropriate
				Function overridenf = null;
				try
				{
					overridenf = overriden.Single(x => new C.CCodeGen.FEqualComp().Equals(x, f));
				}
				catch
				{
					//Console.WriteLine(f.Identifier);
					//throw;
				}
				if (overridenf != null)
				{
					LlvmFunction func = function(overridenf);
						yield return func;
				}
				else
				{
					if (f.Modifiers.HasFlag(Modifiers.Abstract))
					{
						yield return new LlvmLiteral(getfptype(f), "null");
					}
					else
					{
						yield return function(f);
					}
				}
			}
		}

		private IEnumerable<Tuple<string, LlvmType>> vtableMembers(Class c)
		{
			return new [] { new Tuple<string, LlvmType>("get_interface", new LlvmFunctionPtrType(
					new LlvmPointer(new LlvmPrimitive("i8")),
					new LlvmType[]
					{
					type(c),
					type(StdType.Type)
				}
					)) }.Concat(
				vtableFunctions(c).Select(x => new Tuple<string, LlvmType>(mangler.getCFuncName(x), getfptype(x))));
		}
	
		private LlvmType getfptype(Function f)
		{
			// this assumes f is an instance method
			// the method will (hopefully) not be called for
			// static functions anyway...
			return new LlvmFunctionPtrType(type(f.ReturnType),
			                               new LlvmType[] { type(f.Owner) }.Concat(
				f.Parameters.Select(x => type(x.Type))).ToArray());
		}

		private LlvmType _class(Class c)
		{
			LlvmStruct result = new LlvmStruct(mangler.getCTypeName(c));
			LlvmType res = (c is Struct) ? (LlvmType)result : (LlvmType)new LlvmPointer(result);
			types[c] = res;
			Dictionary<string, LlvmType> members = new Dictionary<string, LlvmType>();
			if (!(c is Struct))
			{
				members["vtable"] = vtablety(c);
			}
			members = members.Concat(classMembers(c).ToDictionary(x => x.Item1, x => x.Item2)).ToDictionary(x => x.Key, x => x.Value);
			result.members = members;
			module.AddStruct(result);
			return res;
		}

		private LlvmValue staticfield(Field sf)
		{
			if (svars.ContainsKey(sf))
			{
				return svars[sf];
			}
			LlvmGlobal v = new LlvmGlobal(sf.Modifiers.HasFlag(Modifiers.Private) ? 
			                              LlvmLinkage.Internal : (sf.Owner.IsExtern() ? LlvmLinkage.External : LlvmLinkage.None),
			                             LlvmGlobalFlags.Global, mangler.getVarName(sf), type(sf.Type), null);
			module.AddGlobal(v);
			svars[sf] = v;
			return v;
		}

		private IEnumerable<Tuple<string, LlvmType>> classMembers(Class c)
		{
			Class super = c.Super;
			IEnumerable<Tuple<string, LlvmType>> supers = 
				super == null ? Enumerable.Empty<Tuple<string, LlvmType>>() : classMembers(super);
			return supers.Concat(c.Fields
				.Where(x => !x.Modifiers.HasFlag(Modifiers.Static))
			    .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Desktop32))
			                     .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Windows))
			                     .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Android))
			                     .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Ios))
			                     .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Web))
			    .Where(x => !x.Modifiers.HasFlag(Modifiers.Pf_Osx))
			                     .Select(x => new Tuple<string, LlvmType>(mangler.getVarName(x), type(x.Type))));
		}

		public void generateGeneralBit(IEnumerable<Bohc.TypeSystem.Type> types)
		{
			foreach (TypeSystem.Type t in types)
			{
				Class c = t as Class;
				if (c != null)
				{
					TypeSystem.Function f = c.Functions.SingleOrDefault(x => x.Identifier == "main");
					if (f != null)
					{
						// do everything :)

						LlvmFunction main = new LlvmFunction(new LlvmPrimitive("i32"),
						                                     "@main",
						                                     new List<LlvmParam>
						                                     {
							new LlvmParam("%argc", new LlvmPrimitive("i32")),
							new LlvmParam("%argv", new LlvmPointer(new LlvmPointer(new LlvmPrimitive("i8"))))
						}, LlvmLinkage.None, false);
						Llvm ll = new Llvm(main);
						ll.AddCall(function(f), new LlvmValue[]
						           {
							new LlvmUndef(new LlvmPointer(new LlvmPrimitive("i8")))
						});
						ll.AddRet(new LlvmLiteral(new LlvmPrimitive("i32"), "0"));
						module.AddImplementation(ll);

						return;
					}
				}
			}
		}
		public void finish(IEnumerable<Bohc.TypeSystem.Type> types)
		{
			System.IO.File.WriteAllText("result.ll", module.ToString());
		}
		public void generateFor(Bohc.TypeSystem.Type t, IEnumerable<Bohc.TypeSystem.Type> others)
		{
			// not implemented, all done through generateGeneralBit :)
		}

		private void addBody(Llvm llvm, Body body)
		{
			foreach (Statement s in body.Statements)
			{
				addStatement(llvm, s);
			}
		}

		private void addStatement(Llvm llvm, Statement stat)
		{
			if (addSpecificStat<ExpressionStatement>(llvm, stat, (l, e) => addExpression(llvm, e.expression))) { return; }
			if (addSpecificStat<IfStatement>(llvm, stat, addIfStat)) { return; }
			if (addSpecificStat<WhileStatement>(llvm, stat, addWhileStat)) { return; }
			if (addSpecificStat<DoWhileStatement>(llvm, stat, addDoWhileStat)) { return; }
			if (addSpecificStat<ForStatement>(llvm, stat, addForStat)) { return; }
			if (addSpecificStat<ForeachStatement>(llvm, stat, addForeachStat)) { return; }
			if (addSpecificStat<SwitchStatement>(llvm, stat, addSwitch)) { return; }
			if (addSpecificStat<ReturnStatement>(llvm, stat, addReturn)) { return; }
			if (addSpecificStat<BreakStatement>(llvm, stat, addBreak)) { return; }
			if (addSpecificStat<ContinueStatement>(llvm, stat, addContinue)) { return; }
			if (addSpecificStat<VarDeclaration>(llvm, stat, addVarDec)) { return; }
		}

		private void addForeachStat(Llvm llvm, ForeachStatement fes)
		{
			//throw new NotImplementedException();
		}

		private void addIfStat(Llvm llvm, IfStatement ifs)
		{
			LlvmValue v = addExpression(llvm, ifs.condition);
			LlvmLabel ifl = new LlvmLabel("if.");
			LlvmLabel elsel = new LlvmLabel("else.");
			llvm.AddBranch(v, ifl, elsel);
			llvm.AddLabel(ifl);
			addBody(llvm, ifs.body);
			LlvmLabel skipelse = new LlvmLabel("if.skip.");
			llvm.AddBranch(skipelse);
			llvm.AddLabel(elsel);
			addBody(llvm, ifs.elsestat.body);
			llvm.AddBranch(skipelse);
			llvm.AddLabel(skipelse);
		}

		private void addWhileStat(Llvm llvm, WhileStatement ws)
		{
			LlvmLabel start = new LlvmLabel("while.");
			restrtlabels.Push(start);
			llvm.AddBranch(start);
			llvm.AddLabel(start);
			LlvmValue v = addExpression(llvm, ws.condition);
			LlvmLabel cont = new LlvmLabel();
			LlvmLabel skip = new LlvmLabel("while.skip.");
			skiplabels.Push(skip);
			llvm.AddBranch(v, cont, skip);
			llvm.AddLabel(cont);
			addBody(llvm, ws.body);
			llvm.AddBranch(start);
			llvm.AddLabel(skip);
			skiplabels.Pop();
			restrtlabels.Pop();
		}

		private void addDoWhileStat(Llvm llvm, DoWhileStatement stat)
		{
			LlvmLabel rep = new LlvmLabel("dowhile.");
			llvm.AddBranch(rep);
			llvm.AddLabel(rep);
			LlvmLabel cond = new LlvmLabel("dowhile.cond.");
			LlvmLabel skip = new LlvmLabel("dowhile.skip.");
			skiplabels.Push(skip);
			restrtlabels.Push(cond);
			addBody(llvm, stat.body);
			skiplabels.Pop();
			restrtlabels.Pop();
			llvm.AddBranch(cond);
			llvm.AddLabel(cond);
			LlvmValue v = addExpression(llvm, stat.condition);
			llvm.AddBranch(v, rep, skip);
			llvm.AddLabel(skip);
		}

		private void addForStat(Llvm llvm, ForStatement fors)
		{
			addStatement(llvm, fors.initial);
			LlvmLabel start = new LlvmLabel("for.start.");
			llvm.AddBranch(start);
			llvm.AddLabel(start);
			LlvmValue v = addExpression(llvm, fors.condition);
			LlvmLabel cont = new LlvmLabel("for.cont.");
			LlvmLabel skip = new LlvmLabel("for.skip.");
			restrtlabels.Push(start);
			skiplabels.Push(skip);
			llvm.AddBranch(v, cont, skip);
			llvm.AddLabel(cont);
			addBody(llvm, fors.body);
			skiplabels.Pop();
			restrtlabels.Pop();
			addStatement(llvm, fors.final);
			llvm.AddBranch(start);
			llvm.AddLabel(skip);
		}

		private void addSwitch(Llvm llvm, SwitchStatement ss)
		{
			LlvmLabel skip = new LlvmLabel("switch.skip.");
			LlvmLabel def = ss.labels.Any(x => x is DefaultLabel) ? new LlvmLabel("switch.default.") : skip;
			List<Tuple<LlvmValue, LlvmLabel>> cases = new List<Tuple<LlvmValue, LlvmLabel>>();
			skiplabels.Push(skip);
			foreach (SwitchLabel s in ss.labels)
			{
				CaseLabel cl = s as CaseLabel;
				if (cl != null)
				{
					LlvmLabel cll = new LlvmLabel("switch.case.");
					cases.Add(new Tuple<LlvmValue, LlvmLabel>(addExpression(llvm, cl.expression), cll));
					llvm.AddLabel(cll);
					addBody(llvm, s.body);
				}
				else
				{
					llvm.AddLabel(def);
					addBody(llvm, s.body);
				}
			}
			skiplabels.Pop();
			llvm.AddSwitch(addExpression(llvm, ss.expression), def, cases.ToArray());
			llvm.AddLabel(skip);
		}

		private void addVarDec(Llvm llvm, VarDeclaration vd)
		{
			LlvmValue v = null;
			if (!vd.refersto.Enclosed)
			{
				v = new LlvmTemp(new LlvmPointer(type(vd.refersto.Type)));
				llvm.AddAlloca(v, type(vd.refersto.Type));
			}
			else
			{
				v = llvm.AddCall(aqua_alloc(),
				                 new LlvmValue[] {
					addSizeof(llvm, type(vd.refersto.Type))
				});
				v = llvm.AddBitcast(v, new LlvmPointer(type(vd.refersto.Type)));
			}
			locals[vd.refersto] = v;

			if (vd.initial != null)
			{
				LlvmValue r = addConversion(llvm, vd.initial, vd.refersto.Type);
				llvm.AddStore(v, r);
			}
		}

		private void addReturn(Llvm llvm, ReturnStatement rets)
		{
			if (rets.returns == null)
			{
				llvm.AddRetVoid();
			}
			else
			{
				llvm.AddRet(addConversion(llvm, rets.returns, currentF.Peek().ReturnType));
			}
		}

		private void addBreak(Llvm llvm, BreakStatement brs)
		{
			llvm.AddBranch(skiplabels.Peek());
		}

		private void addContinue(Llvm llvm, ContinueStatement cs)
		{
			llvm.AddBranch(restrtlabels.Peek());
		}

		private void addThrow(Llvm llvm, ThrowStatement ts)
		{
			throw new NotImplementedException();
		}

		private void addTryStat(Llvm llvm, TryStatement trys)
		{
			LlvmLabel c = new LlvmLabel("try.catch.");
			LlvmTry t = new LlvmTry(c);
			tries.Push(t);
			addBody(llvm, trys.body);
			tries.Pop();
			llvm.AddBranch(c);
			llvm.AddLabel(c);
			LlvmTemp tmp = new LlvmTemp(new LlvmPointer(new LlvmPrimitive(Primitive.Byte.LlvmName)));
			llvm.AddLandingPad(tmp, trys.fin != null);
			foreach (CatchStatement ca in trys.catches)
			{
				addCatch(llvm, tmp, ca);
			}
		}

		private void addCatch(Llvm llvm, LlvmTemp tmp, CatchStatement c)
		{
			throw new NotImplementedException();
		}

		private bool addSpecificStat<T>(Llvm llvm, Statement stat, Action<Llvm, T> f)
			where T : Statement
		{
			T t = stat as T;
			if (t != null)
			{
				f(llvm, t);
				return true;
			}
			return false;
		}

		private LlvmValue addExpression(Llvm llvm, Expression expr, bool lvalue = false)
		{
			LlvmValue res;

			if (lvalue)
			{
				if (addSpecificExpr<ExprVariable>(llvm, expr, addExprVariableLvalue, out res))
				{
					return res;
				}
				else
				{
					res = addExpression(llvm, expr);
					LlvmValue tmp = new LlvmTemp(new LlvmPointer(res.Type()));
					llvm.AddAlloca(tmp, res.Type());
					llvm.AddStore(tmp, res);
					return tmp;
				}
			}
			else
			{
				if (addSpecificExpr<ExprVariable>(llvm, expr, addExprVariable, out res))
				{
					return res;
				}
			}

			if (addSpecificExpr<Literal>(llvm, expr, addLiteral, out res))
			{
				return res;
			}
			if (addSpecificExpr<BinaryOperation>(llvm, expr, addBinOp, out res))
			{
				return res;
			}
			if (addSpecificExpr<TypeCast>(llvm, expr, addTypeCast, out res))
			{
				return res;
			}
			if (addSpecificExpr<FunctionCall>(llvm, expr, addFunctionCall, out res))
			{
				return res;
			}
			if (addSpecificExpr<BinaryOperation>(llvm, expr, addBinOp, out res))
			{
				return res;
			}
			if (addSpecificExpr<UnaryOperation>(llvm, expr, addUnOp, out res))
			{
				return res;
			}
			if (addSpecificExpr<ConstructorCall>(llvm, expr, addConstructorCall, out res))
			{
				return res;
			}
			if (addSpecificExpr<Lambda>(llvm, expr, addLambda, out res))
			{
				return res;
			}
			if (addSpecificExpr<FunctionVarCall>(llvm, expr, addFunctionVarCall, out res))
			{
				return res;
			}

			return new LlvmTemp(new LlvmPrimitive("asdf"));
		}

		private bool addSpecificExpr<T>(Llvm llvm, Expression expr, Func<Llvm, T, LlvmValue> a, out LlvmValue res)
			where T : Expression
		{
			T t = expr as T;
			if (t != null)
			{
				res = a(llvm, t);
				return true;
			}
			res = null;
			return false;
		}

		private int lambdacnt = 0;
		private int lambdalevel = 0;
		private Stack<LlvmParam> lmbdctxts = new Stack<LlvmParam>();
		private LlvmValue addLambda(Llvm llvm, Lambda l)
		{
			Body b = l.body;
			if (b == null)
			{
				b = new Body();
				if (l.type.RetType.Name != "void")
				{
					b.Statements.Add(new ReturnStatement(l.expression));
				}
				else
				{
					b.Statements.Add(new ExpressionStatement(l.expression));
				}
			}

			LlvmStruct str = new LlvmStruct("%anon.ctx." + lambdacnt.ToString());
			str.members = l.enclosed.ToDictionary(x => "%" + x.Identifier, x => (LlvmType)new LlvmPointer(type(x.Type)));
			module.AddStruct(str);
			LlvmParam ctx = new LlvmParam("%this", new LlvmPointer(str));
			Llvm ll = new Llvm(new LlvmFunction(type(l.type.RetType), "@anon." + lambdacnt++.ToString(),
			                                    new[] { ctx }.Concat(l.lambdaParams.Select(x => new LlvmParam("%" + x.Identifier,
			                                         type(x.Type)))).ToList(), LlvmLinkage.Internal, true));
			lmbdctxts.Push(ctx);
			++lambdalevel;
			addFunction(b, l.lambdaParams, ll);
			--lambdalevel;
			lmbdctxts.Pop();
			module.AddImplementation(ll);

			LlvmValue ctxallocb = llvm.AddCall(aqua_alloc(), new [] {addSizeof(llvm, str)});
			LlvmValue ctxalloc = llvm.AddBitcast(ctxallocb, new LlvmPointer(str));
			for (int i = 0; i < l.enclosed.Count; ++i)
			{
				LlvmValue sto = llvm.AddGetElementPtr(
					ctxalloc,
					new LlvmValue[] {
					new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
					new LlvmLiteral(new LlvmPrimitive("i32"), i.ToString()),
				}
					);
				llvm.AddStore(sto, addExpression(llvm, new ExprVariable(l.enclosed[i], null), true));
			}

			LlvmTemp res = new LlvmTemp(new LlvmPointer(type(l.type)));
			llvm.AddAlloca(res, type(l.type));
			LlvmValue resobj = llvm.AddGetElementPtr(res, new LlvmValue[] {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
			});
			llvm.AddStore(resobj, ctxallocb);
			LlvmValue resfunc = llvm.AddGetElementPtr(res, new LlvmValue[] {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "1"),
			});
			llvm.AddStore(resfunc, llvm.AddBitcast(ll.func, ((LlvmStruct)type(l.type)).members["function"]));

			return llvm.AddLoad(res);
		}

		private LlvmValue addExprVariableLvalue(Llvm llvm, ExprVariable v)
		{
			if (v.refersto is Local)
			{
				if (((Local)v.refersto).LamdaLevel < lambdalevel)
				{
					LlvmValue th = lmbdctxts.Peek();
					LlvmValue tmp = llvm.AddGetElementPtr(th, new LlvmValue[]
					                      {
						new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
						new LlvmLiteral(new LlvmPrimitive("i32"), 
						                ((LlvmStruct)((LlvmPointer)((LlvmParamType)th.Type()).t).t)
						                .Offset("%" + v.refersto.Identifier).ToString())
					});
					return llvm.AddLoad(tmp);
				}
				return locals[v.refersto];
			}
			if (v.refersto is Parameter)
			{
				return locals[v.refersto];
			}

			if (v.refersto is Field)
			{
				Field f = (Field)v.refersto;
				if (f.Modifiers.HasFlag(Modifiers.Static))
				{
					return staticfield(f);
				}

				LlvmValue bt = addExpression(llvm, v.belongsto, !v.belongsto.getType().IsReferenceType());
				LlvmType t = bt.Type();
				while (t is LlvmParamType)
				{
					t = ((LlvmParamType)t).t;
				}
				LlvmStruct str = (LlvmStruct)((LlvmPointer)t).t;
				return llvm.AddGetElementPtr(bt, new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				                             new LlvmLiteral(new LlvmPrimitive("i32"),
				                str.members.Keys.ToList().IndexOf(mangler.getVarName(v.refersto)).ToString()));
			}

			throw new NotImplementedException();
		}

		private LlvmValue addExprVariable(Llvm llvm, ExprVariable v)
		{
			if (v.refersto.Identifier == "this")
			{
				return llvm.func.parameters.Single(x => x.ToString() == "%this");
			}
			if (v.refersto.Identifier == "super")
			{
				return llvm.func.parameters.Single(x => x.ToString() == "%this");
			}

			Parameter p = v.refersto as Parameter;
			if (p != null)
			{
				if (p.Modifiers.HasFlag(Modifiers.Final) ||
				    p.Modifiers.HasFlag(Modifiers.Ref))
				{
					return llvm.func.parameters.Single(x => x.ToString() == "%" + p.Identifier);
				}
			}
			return llvm.AddLoad(addExprVariableLvalue(llvm, v));
		}

		private void registerNullCheck(LlvmLabel origin, string str)
		{

		}

		private Stack<LlvmLabel> nulllabelstack = new Stack<LlvmLabel>();
		private LlvmLabel getnulllabel()
		{
			if (nulllabelstack.Peek() != null)
			{
				return nulllabelstack.Peek();
			}

			nulllabelstack.Pop();
			nulllabelstack.Push(new LlvmLabel("nullptr."));
			return nulllabelstack.Peek();
		}

		private void addNullChecks(Llvm llvm, Dictionary<LlvmLabel, LlvmValue> d)
		{
			if (nulllabelstack.Peek() == null)
			{
				return;
			}

			llvm.AddLabel(nulllabelstack.Peek());
			LlvmValue str = llvm.AddPhi(d.Last().Value.Type(), d.Select(x => new Tuple<LlvmValue, LlvmLabel>(x.Value, x.Key)).ToArray());
			llvm.AddCall(aqua_throw_null_ex(), new[] { str });
			llvm.AddUnreachable();
		}

		private LlvmValue addNullCheck(Llvm llvm, Expression e, TypeSystem.Type to)
		{
			LlvmValue expr = addConversion(llvm, e, to);
			if (project.unsafeNullPtr)
			{
				return expr;
			}
			LlvmValue cmpnull = llvm.AddIcmp(Icmp.Eq, expr, new LlvmLiteral(type(e.getType()), "null"));
			LlvmLabel clbl = new LlvmLabel("nullcheck.cont.");
			LlvmLabel njl = getnulllabel();
			llvm.AddBranch(cmpnull, njl, clbl);

			nullchecks.Peek()[llvm.GetLabel()] = inlineNullTermCharPtrLiteral(llvm, e.ToString());

			llvm.AddLabel(clbl);
			return expr;
		}

		private IEnumerable<LlvmValue> addParameters(Llvm llvm, IEnumerable<Expression> parameters, IEnumerable<TypeSystem.Type> types)
		{
			using (IEnumerator<Expression> param = parameters.GetEnumerator())
			{
				using (IEnumerator<TypeSystem.Type> type = types.GetEnumerator())
				{
					while (param.MoveNext() && type.MoveNext())
					{
						yield return this.addConversion(llvm, param.Current, type.Current);
					}
				}
			}
		}

		private LlvmValue addVirtCall(Llvm llvm, FunctionCall fcall)
		{
			List<LlvmValue> pars = new List<LlvmValue>();

			LlvmValue dis = addNullCheck(llvm, fcall.belongsto, fcall.refersto.Owner);
			pars.Add(dis);

			LlvmValue vt = llvm.AddGetElementPtr(dis, new LlvmValue[]
			                                   {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), 
					((LlvmStruct)((LlvmPointer)type(fcall.refersto.Owner)).t).Offset("vtable").ToString()),
			});
			LlvmValue vtld = llvm.AddLoad(vt);
			LlvmValue f = llvm.AddGetElementPtr(vtld, new LlvmValue[]
			                                    {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), 
				                ((LlvmStruct)((LlvmPointer)vtld.Type()).t).Offset(mangler.getCFuncName(fcall.refersto)).ToString()),
			});
			LlvmValue fld = llvm.AddLoad(f);

			pars.AddRange(addParameters(llvm, fcall.parameters, fcall.refersto.Parameters.Select(x => x.Type)));

			return llvm.AddCall(fld, pars);
		}

		private LlvmValue addInstanceCall(Llvm llvm, FunctionCall fcall)
		{
			List<LlvmValue> pars = new List<LlvmValue>();

			LlvmValue dis = addNullCheck(llvm, fcall.belongsto, fcall.refersto.Owner);
			pars.Add(dis);

			pars.AddRange(addParameters(llvm, fcall.parameters, fcall.refersto.Parameters.Select(x => x.Type)));

			// TODO: invoke
			return llvm.AddCall(function(fcall.refersto), pars);
		}

		private LlvmValue addStaticCall(Llvm llvm, FunctionCall fcall)
		{
			List<LlvmValue> pars = new List<LlvmValue>();

			if (!fcall.refersto.Modifiers.HasFlag(Modifiers.Native))
			{
				pars.Add(new LlvmUndef(new LlvmPointer(new LlvmPrimitive("i8"))));
			}

			pars.AddRange(addParameters(llvm, fcall.parameters, fcall.refersto.Parameters.Select(x => x.Type)));

			// TODO: invoke
			return llvm.AddCall(function(fcall.refersto), pars);
		}

		private LlvmValue addFunctionVarCall(Llvm llvm, FunctionVarCall fvc)
		{
			LlvmValue e = addExpression(llvm, fvc.belongsto, true);
			LlvmValue obj = llvm.AddGetElementPtr(e,
			                                      new LlvmValue[] {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0")
			});
			LlvmValue f = llvm.AddGetElementPtr(e,
			                                      new LlvmValue[] {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "1")
			});
			// TODO: invoke
			return llvm.AddCall(llvm.AddLoad(f), new[] {
				llvm.AddLoad(obj)
			}.Concat(fvc.parameters.Select(x => addExpression(llvm, x))));
		}

		private LlvmValue addConstructorCall(Llvm llvm, ConstructorCall ccall)
		{
			List<LlvmValue> pars = new List<LlvmValue>();

			foreach (Expression e in ccall.parameters)
			{
				pars.Add(addExpression(llvm, e));
			}

			// TODO: invoke
			return llvm.AddCall(newfunction(ccall.function), pars);
		}

		private LlvmValue addFunctionCall(Llvm llvm, FunctionCall fcall)
		{
			if (((fcall.refersto.Modifiers.HasFlag(Modifiers.Virtual) ||
			      fcall.refersto.Modifiers.HasFlag(Modifiers.Abstract) ||
			      (fcall.refersto.Modifiers.HasFlag(Modifiers.Override) &&
			 !fcall.refersto.Modifiers.HasFlag(Modifiers.Final))) &&

			     !fcall.belongsto.getType().Modifiers.HasFlag(Modifiers.Final)) &&

			    !(fcall.refersto.Owner is Struct))
			{
				return addVirtCall(llvm, fcall);
			}
			else if (!fcall.refersto.Modifiers.HasFlag(Modifiers.Static))
			{
				return addInstanceCall(llvm, fcall);
			}

			return addStaticCall(llvm, fcall);
		}

		private int strings = 0;
		private LlvmValue addCharPtrLiteral(Llvm llvm, string str)
		{
			LlvmValue initial = new LlvmStringLiteral(new UTF8Encoding().GetBytes(str));
			LlvmGlobal g = new LlvmGlobal(LlvmLinkage.Private, LlvmGlobalFlags.Unnamed_addr
			                              | LlvmGlobalFlags.Constant, "@.str" + strings++.ToString(),
			                              initial.Type(), initial);
			module.AddGlobal(g);

			return llvm.AddGetElementPtr(g, new LlvmValue[]
			                      {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0")
			});
		}

		private LlvmValue inlineCharPtrLiteral(Llvm llvm, string str)
		{
			LlvmValue initial = new LlvmStringLiteral(new UTF8Encoding().GetBytes(str));
			LlvmGlobal g = new LlvmGlobal(LlvmLinkage.Private, LlvmGlobalFlags.Unnamed_addr
			                              | LlvmGlobalFlags.Constant, "@.str" + strings++.ToString(),
			                              initial.Type(), initial);
			module.AddGlobal(g);

			return llvm.InlineGetElementPtr(g, new LlvmValue[]
			                             {
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0")
			});
		}

		private LlvmValue addNullTermCharPtrLiteral(Llvm llvm, string str)
		{
			return addCharPtrLiteral(llvm, str + "\0");
		}

		private LlvmValue inlineNullTermCharPtrLiteral(Llvm llvm, string str)
		{
			return inlineCharPtrLiteral(llvm, str + "\0");
		}

		private LlvmValue addStringLiteral(Llvm llvm, string str)
		{
			//throw new NotImplementedException();
			LlvmStruct stru = (LlvmStruct)((LlvmPointer)type(StdType.Str)).t;

			LlvmGlobal g = new LlvmGlobal(LlvmLinkage.Private, LlvmGlobalFlags.Unnamed_addr
			                              | LlvmGlobalFlags.Constant, "@.str" + strings++.ToString(),
			                                stru, new LlvmStructInit(stru, new LlvmValue[]
			                         {
				vtableglobal(StdType.Str),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				new LlvmLiteral(new LlvmPrimitive("i32"), "0"),
				inlineCharPtrLiteral(llvm, str)
			}));
			module.AddGlobal(g);
			return g;
		}

		private LlvmValue addLiteral(Llvm llvm, Literal literal)
		{
			if (literal.getType() == StdType.Str)
			{
				return addStringLiteral(llvm, literal.representation.Substring(1, literal.representation.Length - 2));
			}
			if (literal.getType() == Primitive.Char)
			{
				return new LlvmLiteral(type(literal.type),
				       new UTF8Encoding().GetBytes(literal.representation.Substring(1, literal.representation.Length - 2))
				                       .Single().ToString());
			}
			return new LlvmLiteral(type(literal.type), literal.representation.ToLower());
		}

		private LlvmValue addUnOp(Llvm llvm, UnaryOperation unop)
		{
			if (unop.operation == UnaryOperation.DECREMENT)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat, true);
				LlvmValue ld = llvm.AddLoad(v);
				LlvmValue dec = llvm.AddSub(ld, new LlvmLiteral(v.Type(), "1"));
				llvm.AddStore(v, dec);
				return dec;
			}
			else if (unop.operation == UnaryOperation.DECREMENT_POST)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat, true);
				LlvmValue ld = llvm.AddLoad(v);
				LlvmValue dec = llvm.AddSub(ld, new LlvmLiteral(v.Type(), "1"));
				llvm.AddStore(v, dec);
				return ld;
			}
			else if (unop.operation == UnaryOperation.DEFAULT)
			{
				return addExpression(llvm, ((ExprType)unop.onwhat).type.DefaultVal());
			}
			else if (unop.operation == UnaryOperation.INCREMENT)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat, true);
				LlvmValue ld = llvm.AddLoad(v);
				LlvmValue dec = llvm.AddAdd(ld, new LlvmLiteral(v.Type(), "1"));
				llvm.AddStore(v, dec);
				return dec;
			}
			else if (unop.operation == UnaryOperation.INCREMENT_POST)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat, true);
				LlvmValue ld = llvm.AddLoad(v);
				LlvmValue dec = llvm.AddAdd(ld, new LlvmLiteral(v.Type(), "1"));
				llvm.AddStore(v, dec);
				return ld;
			}
			else if (unop.operation == UnaryOperation.INVERT)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat);
				return llvm.AddXor(v, new LlvmLiteral(v.Type(), "-1"));
			}
			else if (unop.operation == UnaryOperation.MINUS)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat);
				return llvm.AddSub(new LlvmLiteral(v.Type(), "0"), v);
			}
			else if (unop.operation == UnaryOperation.NOT)
			{
				LlvmValue v = addExpression(llvm, unop.onwhat);
				return llvm.AddXor(v, new LlvmLiteral(v.Type(), "-1"));
			}
			else if (unop.operation == UnaryOperation.PLUS)
			{
				return addExpression(llvm, unop.onwhat);
			}
			else if (unop.operation == UnaryOperation.TYPEOF)
			{
				throw new NotImplementedException();
			}

			throw new NotImplementedException();
		}

		public LlvmValue addBinOp(Llvm llvm, BinaryOperation binop)
		{
			if (binop.operation == BinaryOperation.ADD)
			{
				return addFloatIntOp(llvm, binop, llvm.AddFadd, llvm.AddAdd, llvm.AddAdd);
			}
			else if (binop.operation == BinaryOperation.ASSIGN)
			{
				LlvmValue lvalue = addExpression(llvm, binop.left, true);
				LlvmValue rvalue = addConversion(llvm, binop.right, binop.left.getType());
				llvm.AddStore(lvalue, rvalue);
				return rvalue;
			}
			else if (binop.operation == BinaryOperation.CONDIT_AND)
			{
				LlvmValue lvalue = addExpression(llvm, binop.left);
				LlvmLabel cont = new LlvmLabel();
				LlvmLabel skip = new LlvmLabel();
				LlvmLabel origin = llvm.GetLabel();
				llvm.AddBranch(lvalue, cont, skip);
				llvm.AddLabel(cont);
				LlvmValue rvalue = addExpression(llvm, binop.right);
				llvm.AddBranch(skip);
				llvm.AddLabel(skip);

				return llvm.AddPhi(new LlvmPrimitive(Primitive.Boolean.LlvmName),
				           new Tuple<LlvmValue, LlvmLabel>(lvalue, origin),
				           new Tuple<LlvmValue, LlvmLabel>(rvalue, cont));
			}
			else if (binop.operation == BinaryOperation.CONDIT_OR)
			{
				LlvmValue lvalue = addExpression(llvm, binop.left);
				LlvmLabel cont = new LlvmLabel();
				LlvmLabel skip = new LlvmLabel();
				LlvmLabel origin = llvm.GetLabel();
				llvm.AddBranch(lvalue, skip, cont);
				llvm.AddLabel(cont);
				LlvmValue rvalue = addExpression(llvm, binop.right);
				llvm.AddBranch(skip);
				llvm.AddLabel(skip);

				return llvm.AddPhi(new LlvmPrimitive(Primitive.Boolean.LlvmName),
				           new Tuple<LlvmValue, LlvmLabel>(lvalue, origin),
				           new Tuple<LlvmValue, LlvmLabel>(rvalue, cont));
			}
			else if (binop.operation == BinaryOperation.DIV)
			{
				OverloadedOperator ol = binop.overloaded;

				if (ol == null)
				{
					return addFloatIntOp(llvm, binop, llvm.AddFdiv, llvm.AddSdiv, llvm.AddUdiv);
				}

				throw new NotImplementedException();
			}
			else if (binop.operation == BinaryOperation.EQUAL)
			{
				Primitive pl = binop.left.getType() as Primitive;

				if (pl != null)
				{
					// int, float, or bool cmp
					if (pl.IsInt() || pl == Primitive.Boolean)
					{
						return llvm.AddIcmp(Icmp.Eq, addExpression(llvm, binop.left), addExpression(llvm, binop.right));
					}
					return llvm.AddFcmp(Fcmp.Oeq, addExpression(llvm, binop.left), addExpression(llvm, binop.right));
				}

				throw new NotImplementedException();
			}
			else if (binop.operation == BinaryOperation.LOGIC_AND)
			{
				return llvm.AddAnd(addExpression(llvm, binop.left), addExpression(llvm, binop.right));
			}
			else if (binop.operation == BinaryOperation.LOGIC_OR)
			{
				return llvm.AddOr(addExpression(llvm, binop.left), addExpression(llvm, binop.right));
			}
			else if (binop.operation == BinaryOperation.LOGIC_XOR)
			{
				return llvm.AddXor(addExpression(llvm, binop.left), addExpression(llvm, binop.right));
			}
			else if (binop.operation == BinaryOperation.MUL)
			{
				OverloadedOperator ol = binop.overloaded;

				if (ol == null)
				{
					return addFloatIntOp(llvm, binop, llvm.AddFmul, llvm.AddMul, llvm.AddMul);
				}

				throw new NotImplementedException();
			}
			else if (binop.operation == BinaryOperation.NOT_EQUAL)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl != null)
				{
					// int, float, or bool cmp
					return addFloatIntOp(llvm, binop, 
					                    (_2, _3) => llvm.AddFcmp(Fcmp.One, _2, _3),
					                    (_2, _3) => llvm.AddIcmp(Icmp.Ne, _2, _3),
					                    (_2, _3) => llvm.AddIcmp(Icmp.Ne, _2, _3));
				}
			}
			else if (binop.operation == BinaryOperation.RELAT_G)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl == null)
				{
					throw new NotImplementedException();
				}
				return addFloatIntOp(llvm, binop, 
				                     (_2, _3) => llvm.AddFcmp(Fcmp.Ogt, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Sgt, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Ugt, _2, _3));
			}
			else if (binop.operation == BinaryOperation.RELAT_GE)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl == null)
				{
					throw new NotImplementedException();
				}
				return addFloatIntOp(llvm, binop, 
				                     (_2, _3) => llvm.AddFcmp(Fcmp.Oge, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Sge, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Uge, _2, _3));
			}
			else if (binop.operation == BinaryOperation.RELAT_L)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl == null)
				{
					throw new NotImplementedException();
				}
				return addFloatIntOp(llvm, binop, 
				                     (_2, _3) => llvm.AddFcmp(Fcmp.Olt, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Slt, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Ult, _2, _3));
			}
			else if (binop.operation == BinaryOperation.RELAT_LE)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl == null)
				{
					throw new NotImplementedException();
				}
				return addFloatIntOp(llvm, binop, 
				                     (_2, _3) => llvm.AddFcmp(Fcmp.Ole, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Sle, _2, _3),
				                     (_2, _3) => llvm.AddIcmp(Icmp.Ule, _2, _3));
			}
			else if (binop.operation == BinaryOperation.REM)
			{
				Primitive pl = binop.left.getType() as Primitive;
				if (pl == null)
				{
					throw new NotImplementedException();
				}
				return addFloatIntOp(llvm, binop, 
				                    llvm.AddFrem,
				                    llvm.AddSrem,
				                    llvm.AddUrem);
			}
			else if (binop.operation == BinaryOperation.R_EQ)
			{
				LlvmValue left = addConversion(llvm, binop.left);
				LlvmValue right = addConversion(llvm, binop.right);
				return llvm.AddIcmp(Icmp.Eq, left, right);
			}
			else if (binop.operation == BinaryOperation.SHL)
			{
				return addFloatIntOp(llvm, binop, null, llvm.AddShl, llvm.AddShl);
			}
			else if (binop.operation == BinaryOperation.SHR)
			{
				return addFloatIntOp(llvm, binop, null, llvm.AddLshr, llvm.AddLshr);
			}
			else if (binop.operation == BinaryOperation.SUB)
			{
				return addFloatIntOp(llvm, binop, llvm.AddFadd, llvm.AddSub, llvm.AddSub);
			}

			throw new NotImplementedException();
		}

		private LlvmValue addTypeCast(Llvm llvm, TypeCast tc)
		{
			return addConversion(llvm, tc.onwhat, tc.towhat);
		}

		private LlvmValue addSizeof(Llvm llvm, LlvmType ty)
		{
			return llvm.AddPtrToInt(llvm.AddGetElementPtr(new LlvmLiteral(new LlvmPointer(ty), "null"), 
			                                              new LlvmLiteral(new LlvmPrimitive("i32"), "1")),
			                        new LlvmPrimitive("i64"));
		}

		private LlvmValue addConversion(Llvm llvm, Expression expression)
		{
			return llvm.AddBitcast(addExpression(llvm, expression), new LlvmPointer(new LlvmPrimitive("i8")));
		}

		private LlvmValue addConversion(Llvm llvm, Expression expression, TypeSystem.Type to)
		{
			TypeSystem.Type fr = expression.getType();

			if (fr == to)
			{
				return addExpression(llvm, expression);
			}

			if (expression is Literal && (expression.getType() is Primitive || ((Literal)expression).representation == "NULL"))
			{
				// TODO: proper conversion
				return new LlvmLiteral(type(to), addExpression(llvm, expression).ToString());
			}

			Primitive frp = fr as Primitive;
			if (frp != null)
			{
				if (to == StdType.Obj)
				{
					// box
					throw new NotImplementedException();
				}

				Primitive top = (Primitive)to;
				if (frp.IsInt() && top.IsFloat())
				{
					return llvm.AddUiToFp(addExpression(llvm, expression), type(to));
				}
				else if (frp.IsFloat() && top.IsInt())
				{
					if (top == Primitive.Byte || top == Primitive.Char)
					{
						return llvm.AddFpToUi(addExpression(llvm, expression), type(to));
					}
					else
					{
						return llvm.AddFpToSi(addExpression(llvm, expression), type(to));
					}
				}
				else if (frp.IsInt() && top.IsInt())
				{
					if (top.Size > frp.Size)
					{
						// extension
						if (frp == Primitive.Byte || frp == Primitive.Char)
						{
							return llvm.AddZext(addExpression(llvm, expression), type(to));
						}
						else
						{
							return llvm.AddSext(addExpression(llvm, expression), type(to));
						}
					}
					else
					{
						// trunc
						return llvm.AddTrunc(addExpression(llvm, expression), type(to));
					}
				}
				else
				{
					if (top.Size > frp.Size)
					{
						return llvm.AddFext(addExpression(llvm, expression), type(to));
					}
					else
					{
						return llvm.AddFtrunc(addExpression(llvm, expression), type(to));
					}
				}
			}
			else
			{
				if (to is Class)
				{
					if (fr.Extends(to) != 0)
					{
						// i.e. (Object)string
						return llvm.AddBitcast(addExpression(llvm, expression), type(to));
					}
					else
					{
						// do type check first (unless disabled)
						throw new NotImplementedException();
					}
				}
			}
			throw new NotImplementedException();
		}

		private LlvmValue addFloatIntOp(Llvm llvm, BinaryOperation binop, 
		                                Func<LlvmValue, LlvmValue, LlvmValue> fop,
		                                Func<LlvmValue, LlvmValue, LlvmValue> siop,
		                                Func<LlvmValue, LlvmValue, LlvmValue> uiop)
		{
			Primitive lt = (Primitive)binop.left.getType();
			Primitive rt = (Primitive)binop.right.getType();
			Primitive rest = lt.Size > rt.Size ? lt : rt;

			LlvmValue l = addConversion(llvm, binop.left, rest);
			LlvmValue r = addConversion(llvm, binop.right, rest);

			if (rest.IsFloat())
			{
				return fop(l, r);
			}
			else if (rest == Primitive.Byte)
			{
				return uiop(l, r);
			}
			return siop(l, r);
		}
	}
}

