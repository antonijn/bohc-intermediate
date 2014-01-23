using System;
using Bohc.General;

namespace Bohc.Parsing
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
		void parseParam(File file, string x, out string id, out Bohc.TypeSystem.Modifiers mods, out Bohc.TypeSystem.Type type);
	}
}

