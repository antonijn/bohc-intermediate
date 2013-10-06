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
			parsing.ts.File f = Parser.parseFile(
				@"
package hey.hi;

import boh.lang;
import boh.lang.lala;

public interface Class {");
			Console.ReadKey();
		}
	}
}
