using System;
using Bohc.General;

namespace Bohc.Parsing
{
	public interface IFileParser
	{
		IParserStrategy getStrat();
		void regStrat(IParserStrategy p);
		File parseFileTS(ref string file, string filename);
		void parseFileTP(File f);
		void parseFileTCS(File f);
		void parseFileTCP(File f);
		void parseFileCP(File f);
		Project proj();
		ErrorManager getEM();
	}
}

