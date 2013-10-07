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

public class Class
{
	public static Class function(){}

	private Class field;
}";
			parsing.ts.File f0 = Parser.parseFileTS(file);
			Parser.parseFileTP(f0, file);
			Parser.parseFileTCS(f0, file);

			Console.ReadKey();
		}
	}
}
