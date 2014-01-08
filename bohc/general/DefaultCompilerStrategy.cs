using System;
using System.Collections.Generic;
using System.Linq;

using bohc.generation;
using bohc.generation.mangling;
using bohc.generation.c;

namespace bohc.general
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
			IEnumerable<typesys.Type> types =
				typesys.Type.types.Where(x => !(x is typesys.Primitive));
			codegen.generateGeneralBit(types);

			foreach (typesys.Type type in types)
			{
				codegen.generateFor(type, types);
			}

			codegen.finish(types);
		}
	}
}

