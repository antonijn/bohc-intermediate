using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc
{
	class Program
	{
		public static void Main(string[] args)
		{
			string file = @"
package hey.hi;

import boh.lang;
import boh.lang.lala;

public class Class extends Hi {";
			parsing.ts.File f0 = Parser.parseFileTS(file);

			parsing.ts.File f1 = Parser.parseFileTS(
				@"
package hey.hi;

public class Hi {");

			Parser.parseFileTP(f0, file);
			Console.ReadKey();
		}
	}
}
