using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using Bohc.Generation;
using Bohc.Generation.Mangling;

using Bohc.Parsing;
using Bohc.Parsing.Statements;

using Bohc.TypeSystem;

namespace Bohc.General
{
	public class DefaultParserStrategy : IParserStrategy
	{
		private ParserState pstate;
		private IFileParser fp;

		public DefaultParserStrategy(IFileParser fp)
		{
			this.fp = fp;
			fp.regStrat(this);
		}

		public ParserState getpstate()
		{
			return pstate;
		}

		public void parse(Project input)
		{
			List<Bohc.Parsing.File> files = new List<Bohc.Parsing.File>();

			parseTS(input.input.ToArray(), files, fp);

			parseTP(files, fp);

			Bohc.TypeSystem.Primitive.FigureOutFunctionsForAll();

			parseTCS(files, fp);

			foreach (Bohc.TypeSystem.Enum e in Bohc.TypeSystem.Enum.Instances)
			{
				e.SortOutFunctions(fp);
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
				Bohc.Parsing.File f = parser.parseFileTS(ref file, filenames[i]);
				files.Add(f);
			}
		}

		private void parseTP(List<File> files, IFileParser parser)
		{
			pstate = ParserState.TP;
			foreach (File f in files)
			{
#if !DEBUG
				try
				{
#endif
				//if (f.type is Bohc.TypeSystem.Type)
				//	{
					parser.parseFileTP(f);
						//Console.WriteLine("Type Parsing for: {0}", ((typesys.Type)f.type).fullName());
				//	}
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
		}

		private void parseTCS(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.TCS;
			for (int i = 0; i < GenericType.TypeInstances.Count; ++i)
			{
				File f = GenericType.TypeInstances[i].File;
#if !DEBUG
				try
				{
#endif
					parser.parseFileTCS(f);
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
			foreach (File f in filesassoc)
			{
#if !DEBUG
				try
				{
#endif
					if (f.type is Bohc.TypeSystem.Type)
					{
						//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCS(f);
					}
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
		}

		private void parseTCP(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.TCP;
			for (int i = 0; i < GenericType.TypeInstances.Count; ++i)
			{
				File f = GenericType.TypeInstances[i].File;
#if !DEBUG
				try
				{
#endif
					parser.parseFileTCP(f);
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
			foreach (File f in filesassoc)
			{
#if !DEBUG
				try
				{
#endif
					if (f.type is Bohc.TypeSystem.Type)
					{
						//Console.WriteLine("Type Content Parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCP(f);
					}
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
		}

		private void parseCP(List<File> filesassoc, IFileParser parser)
		{
			pstate = ParserState.CP;
			for (int i = 0; i < GenericType.TypeInstances.Count; ++i)
			{
				File f = GenericType.TypeInstances[i].File;
#if !DEBUG
				try
				{
#endif
					parser.parseFileCP(f);
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
			foreach (File f in filesassoc)
			{
#if !DEBUG
				try
				{
#endif
					if (f.type is Bohc.TypeSystem.Type)
					{
						//Console.WriteLine("Code Parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileCP(f);
					}
#if !DEBUG
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: Parsing file {0} failed: {1}", f, e.ToString());
				}
#endif
			}
		}
	}
}

