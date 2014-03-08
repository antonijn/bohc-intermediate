using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Bohc.TypeSystem;
using Bohc.Parsing;

namespace Bohc.General
{
	public class ConcurrentParserStrategy : IParserStrategy
	{
		private ParserState pstate;
		private IFileParser fp;

		public ConcurrentParserStrategy(IFileParser fp)
		{
			this.fp = fp;
		}

		public void parse(Project input)
		{
			List<Bohc.Parsing.File> files = new List<Bohc.Parsing.File>();
		
			parseTS(input.input.ToArray(), files);
			parseTP(files);
			Primitive.FigureOutFunctionsForAll();
			parseTCS(files);

			foreach (TypeSystem.Enum e in TypeSystem.Enum.Instances)
			{
				e.SortOutFunctions(fp);
			}

			parseTCP(files);
			parseCP(files);
		}

		public ParserState getpstate()
		{
			return pstate;
		}

		private void parseFileTS(Object o)
		{
			Tuple<List<File>, string> t = (Tuple<List<File>, string>)o;
			string file = System.IO.File.ReadAllText(t.Item2);
			File f = fp.parseFileTS(ref file, t.Item2);
			lock (t.Item1)
			{
				t.Item1.Add(f);
			}
		}

		private void parseTS(string[] filenames, List<File> files)
		{
			List<ManualResetEvent> mre = new List<ManualResetEvent>();

			pstate = ParserState.TS;
			for (int i = 0; i < filenames.Length; ++i)
			{
				mre.Add(new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem(parseFileTS, new Tuple<List<File>, string>(files, filenames[i]));
			}

			WaitHandle.WaitAll(mre.ToArray());
		}

		private void parseTP(List<File> files)
		{
			List<ManualResetEvent> mre = new List<ManualResetEvent>();

			pstate = ParserState.TP;
			foreach (File f in files.Where(x => x.type is TypeSystem.Type))
			{
				mre.Add(new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem(o => fp.parseFileTP((File)o), f);
			}

			WaitHandle.WaitAll(mre.ToArray());
		}

		private void parseTCS(List<File> files)
		{
			List<ManualResetEvent> mre = new List<ManualResetEvent>();

			pstate = ParserState.TCS;
			foreach (File f in files.Where(x => x.type is TypeSystem.Type))
			{
				mre.Add(new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem(o => fp.parseFileTCS((File)o), f);
			}

			WaitHandle.WaitAll(mre.ToArray());
		}

		private void parseTCP(List<File> files)
		{
			List<ManualResetEvent> mre = new List<ManualResetEvent>();

			pstate = ParserState.TCP;
			foreach (File f in files.Where(x => x.type is TypeSystem.Type)
			         .Concat(GenericType.TypeInstances.Select(x => x.File)))
			{
				mre.Add(new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem(o => fp.parseFileTCP((File)o), f);
			}

			WaitHandle.WaitAll(mre.ToArray());
		}

		private void parseCP(List<File> files)
		{
			List<ManualResetEvent> mre = new List<ManualResetEvent>();

			pstate = ParserState.CP;
			foreach (File f in files.Where(x => x.type is TypeSystem.Type)
			         .Concat(GenericType.TypeInstances.Select(x => x.File)))
			{
				mre.Add(new ManualResetEvent(false));
				ThreadPool.QueueUserWorkItem(o => fp.parseFileCP((File)o), f);
			}

			WaitHandle.WaitAll(mre.ToArray());
		}
	}
}

