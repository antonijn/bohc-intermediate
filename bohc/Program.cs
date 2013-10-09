using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
	public static int thing()
{

}
}";
			Stopwatch total = new Stopwatch();
			total.Start();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			parsing.ts.File f0 = Parser.parseFileTS(file);
			sw.Stop();
			Console.WriteLine("Type Skimming step took:             {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			sw.Reset();
			sw.Start();
			Parser.parseFileTP(f0, file);
			sw.Stop();
			Console.WriteLine("Type Parsing step took:              {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			sw.Reset();
			sw.Start();
			Parser.parseFileTCS(f0, file);
			sw.Stop();
			Console.WriteLine("Type Content Skimming step took:     {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			total.Stop();
			Console.WriteLine("Parsing took:                        {0} milliseconds", total.Elapsed.TotalMilliseconds);

			parsing.Expression e = parsing.Expression.analyze("Class.thing() + 5", new List<typesys.Variable>(), f0);

			Console.ReadKey();
		}
	}
}
