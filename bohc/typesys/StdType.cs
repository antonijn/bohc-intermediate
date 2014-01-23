// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.TypeSystem
{
	public static class StdType
	{
		public static Package BohLang = Package.GetFromString("boh.std");

		public static readonly Class Obj = (Class)TypeSystem.Type.GetExisting(BohLang, "Object", null);
		public static readonly Class Str = (Class)TypeSystem.Type.GetExisting(BohLang, "String", null);
		public static readonly Class Type = (Class)TypeSystem.Type.GetExisting(BohLang, "Type", null);

		// TODO: check that package is Boh.std
		public static readonly GenericType Array = GenericType.AllGenTypes.Single(
			x => x.Name == "Array" && x.File.package == BohLang);
		public static readonly GenericType Box = GenericType.AllGenTypes.Single(
			x => x.Name == "Box" && x.File.package == BohLang);
		public static readonly GenericType ICollection = GenericType.AllGenTypes.Single(
			x => x.Name == "ICollection" && x.File.package == BohLang);
		public static readonly GenericType IIterator = GenericType.AllGenTypes.Single(
			x => x.Name == "IIterator" && x.File.package == BohLang);
	}
}
