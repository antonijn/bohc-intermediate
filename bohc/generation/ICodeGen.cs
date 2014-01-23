using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc
{
	public interface ICodeGen
	{
		void generateGeneralBit(IEnumerable<Bohc.TypeSystem.Type> types);
		void generateFor(Bohc.TypeSystem.Type t, IEnumerable<Bohc.TypeSystem.Type> others);
		void finish(IEnumerable<Bohc.TypeSystem.Type> types);
	}
}
