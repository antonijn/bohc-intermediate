using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing.ts;

namespace bohc.typesys
{
	public interface IType
	{
		File getFile();
		void setFile(File f);
	}
}
