using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

using bohc.typesys;

namespace bohc.general
{
	public class DefaultParserStrategy : IParserStrategy
	{
		private ParserState pstate;
		private IFileParser fp;

		public DefaultParserStrategy(IFileParser fp)
		{
			this.fp = fp;
		}

		public ParserState getpstate()
		{
			return pstate;
		}

		public void parse(Project input)
		{
			List<parsing.File> files = new List<parsing.File>();

			Stopwatch sw = new Stopwatch();
			sw.Start();

			parseTS(input.input.ToArray(), files, fp);

			parseTP(files, fp);

			typesys.Primitive.figureOutFunctionsForAll();

			parseTCS(files, fp);

			foreach (typesys.Enum e in typesys.Enum.instances)
			{
				e.sortOutFunctions(fp);
			}

			parseTCP(files, fp);


			parseCP(files, fp);
		}

		private void parseTS(string[] filenames, List<File> files, IFileParser parser)
		{
			pstate = ParserState.TS;
			for (int i = 0; i < filenames.Length; ++i)
			{
				string file = System.IO.File.ReadAllText(filenames[i]);
				parsing.File f = parser.parseFileTS(ref file);
				files.Add(f);
			}
		}

		private void parseTP(List<File> files, IFileParser parser)
		{
			pstate = ParserState.TP;
			foreach (File f in files)
			{
				try
				{
					if (f.type is typesys.Type)
					{
						parser.parseFileTP(f);
						//Console.WriteLine("Type parsing for: {0}", ((typesys.Type)f.type).fullName());
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
		}

		private void parseTCS(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.TCS;
			for (int i = 0; i < GenericType.typeInstances.Count; ++i)
			{
				File f = GenericType.typeInstances[i].file;
				try
				{
					parser.parseFileTCS(f);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
			foreach (File f in filesassoc)
			{
				try
				{
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCS(f);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
		}

		private void parseTCP(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.TCP;
			for (int i = 0; i < GenericType.typeInstances.Count; ++i)
			{
				File f = GenericType.typeInstances[i].file;
				try
				{
					parser.parseFileTCP(f);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
			foreach (File f in filesassoc)
			{
				try
				{
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCP(f);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
		}

		private void parseCP(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.CP;
			for (int i = 0; i < GenericType.typeInstances.Count; ++i)
			{
				File f = GenericType.typeInstances[i].file;
				try
				{
					parser.parseFileCP(f);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
			foreach (File f in filesassoc)
			{
				try
				{
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Code parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileCP(f);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", f, e.ToString());
				}
			}
		}
	}
}

