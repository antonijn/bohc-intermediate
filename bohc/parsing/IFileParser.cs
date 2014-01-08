using System;
using bohc.general;

namespace bohc.parsing
{
	public interface IFileParser
	{
		File parseFileTS(ref string file);
		void parseFileTP(File f);
		void parseFileTCS(File f);
		void parseFileTCP(File f);
		void parseFileCP(File f);
		Project proj();
		// TODO: get rid of this functions
		void parseParam(File file, string x, out string id, out typesys.Modifiers mods, out typesys.Type type);
	}
}

