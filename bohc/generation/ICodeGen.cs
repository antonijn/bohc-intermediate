using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.generation
{
	public interface ICodeGen
	{
		void generateGeneralBit(IEnumerable<typesys.Type> types);
		void generateFor(typesys.Type t, IEnumerable<typesys.Type> others);
		void finish(IEnumerable<typesys.Type> types);
	}
}
