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
			string[] filenames = new string[] { "src/Exception.boh", "src/Object.boh", "src/Type.boh", "src/Class.boh" };
			string[] files = new string[filenames.Length];

			for (int i = 0; i < filenames.Length; ++i)
			{
				files[i] = System.IO.File.ReadAllText(filenames[i]);
			}

			Dictionary<string, parsing.ts.File> filesassoc = new Dictionary<string, parsing.ts.File>();

			foreach (string file in files)
			{
				filesassoc[file] = Parser.parseFileTS(file);
			}

			foreach (string file in files)
			{
				Parser.parseFileTP(filesassoc[file], file);
			}

			foreach (string file in files)
			{
				Parser.parseFileTCS(filesassoc[file], file);
			}

			foreach (string file in files)
			{
				Parser.parseFileTCP(filesassoc[file], file);
			}

			foreach (string file in files)
			{
				Parser.parseFileCP(filesassoc[file], file);
			}

			foreach (typesys.Type type in filesassoc.Values.Select(x => (typesys.Type)x.type).Where(x => x != null))
			{
				CodeGen.generateFor(type, filesassoc.Values.Select(x => (typesys.Type)x.type).Where(x => x != null));
			}

			Console.ReadKey();
		}
	}
}
