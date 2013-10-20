using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public static class StdType
	{
		public static Package boh_lang = Package.getFromString("boh.lang");

		public static readonly Class obj = (Class)Type.getExisting(boh_lang, "Object");
		public static readonly Class str = (Class)Type.getExisting(boh_lang, "String");
		public static readonly Class type = (Class)Type.getExisting(boh_lang, "Type");
	}
}
