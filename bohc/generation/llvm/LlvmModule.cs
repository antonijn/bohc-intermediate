using System;
using System.Text;

using Bohc.Generation.Mangling;

namespace Bohc.Generation.Llvm
{
	public class LlvmModule
	{
		public LlvmModule()
		{
		}

		private StringBuilder types = new StringBuilder();
		private StringBuilder decls = new StringBuilder();
		private StringBuilder globvars = new StringBuilder();
		private StringBuilder impls = new StringBuilder();

		public override string ToString()
		{
			return types.ToString() + Environment.NewLine +
			decls.ToString() + Environment.NewLine +
			globvars.ToString() + Environment.NewLine +
			impls.ToString() + Environment.NewLine;
		}

		public void AddDeclaration(LlvmFunction f)
		{
			decls.Append("declare ").Append((f.linkage != LlvmLinkage.None) ? 
			                                f.linkage.ToString().ToLowerInvariant() + " " : "")
				.Append(f.ret.ToString())
				.Append(" ").Append(f.id).Append("(");
			foreach (LlvmParam t in f.parameters)
			{
				decls.Append(t.Type().ToString()).Append(", ");
			}
			if (f.parameters.Count > 0)
			{
				decls.Remove(decls.Length - 2, 2);
			}
			decls.AppendLine(")");
		}

		public void AddStruct(LlvmStruct str)
		{
			types.Append(str.ToString()).Append(" = type { ");
			foreach (LlvmType t in str.members.Values)
			{
				types.Append(t.ToString()).Append(", ");
			}
			if (str.members.Count > 0)
			{
				types.Remove(types.Length - 2, 2);
			}
			types.AppendLine(" }");
		}

		public void AddGlobal(LlvmGlobal g)
		{
			/*globvars.Append(g.linkage.ToString().ToLowerInvariant()).Append(" ").Append(g.Type().ToString())
				.Append(" ").AppendLine(g.id);*/

			globvars.Append(g.id).Append(" = ").Append(g.linkage.ToString().ToLowerInvariant()).Append(" ")
				.Append(g.flags.ToString().Replace(",", "").ToLowerInvariant()).Append(" ")
				.Append(((LlvmPointer)g.Type()).t.ToString());
			if (g.initial != null)
			{
				globvars.Append(" ").Append(g.initial.ToString());
			}
			globvars.AppendLine();
		}

		public void AddImplementation(Llvm llvm)
		{
			Llvm.tempvc.Pop();

			impls.Append("define ").Append((llvm.func.linkage != LlvmLinkage.None) ? 
			                               llvm.func.linkage.ToString().ToLowerInvariant() + " " : "")
				.Append(llvm.func.ret.ToString())
					.Append(" ").Append(llvm.func.id).Append("(");
			foreach (var p in llvm.func.parameters)
			{
				impls.Append(p.Type().ToString()).Append(" ").Append(p.ToString()).Append(", ");
			}
			if (llvm.func.parameters.Count > 0)
			{
				impls.Remove(impls.Length - 2, 2);
			}
			impls.AppendLine(") {");
			impls.Append(llvm.ToString());
			impls.AppendLine("}");
		}
	}
}

