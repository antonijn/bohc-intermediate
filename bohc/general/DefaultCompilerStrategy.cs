using System;
using System.Collections.Generic;
using System.Linq;

using Bohc.Generation;
using Bohc.Generation.Mangling;
using Bohc.Generation.C;

namespace Bohc.General
{
	public class DefaultCompilerStrategy : ICompilerStrategy
	{
		private readonly IMangler mangler;
		private readonly ICodeGen codegen;

		public DefaultCompilerStrategy(IMangler mangler, ICodeGen codegen)
		{
			this.mangler = mangler;
			this.codegen = codegen;
		}

		public IMangler getMangler()
		{
			return mangler;
		}

		public ICodeGen getCodeGen()
		{
			return codegen;
		}

		public void compile(Project p)
		{
			if (p.emanager.errors == 0)
			{
				IEnumerable<Bohc.TypeSystem.Type> types =
					Bohc.TypeSystem.Type.Types.Where(x => !(x is Bohc.TypeSystem.Primitive)).Where(x => x.File == null || !x.File.ignore);
				codegen.generateGeneralBit(types);

				foreach (Bohc.TypeSystem.Type type in types)
				{
					codegen.generateFor(type, types);
				}

				codegen.finish(types);

				if (p.emanager.warnings == 1)
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine("Project compiled with 1 warning");
				}
				else if (p.emanager.warnings > 1)
				{
					Console.Error.WriteLine();
					Console.Error.WriteLine("Project compiled with {0} warnings", p.emanager.warnings);
				}
			}
			else
			{
				Console.Error.WriteLine("Could not compile project, {0} errors were generated", p.emanager.errors);
			}
		}
	}
}

