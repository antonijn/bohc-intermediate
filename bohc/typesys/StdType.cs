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

namespace bohc.typesys
{
	public static class StdType
	{
		public static Package boh_lang = Package.getFromString("boh.std");

		public static readonly Class obj = (Class)Type.getExisting(boh_lang, "Object", null);
		public static readonly Class str = (Class)Type.getExisting(boh_lang, "String", null);
		public static readonly Class type = (Class)Type.getExisting(boh_lang, "Type", null);

		// TODO: check that package is boh.std
		public static readonly GenericType array = GenericType.allGenTypes.Single(
			x => x.name == "Array" && x.file.package == Package.getFromString("boh.std"));
		public static readonly GenericType box = GenericType.allGenTypes.Single(
			x => x.name == "Box" && x.file.package == Package.getFromString("boh.std"));
		public static readonly GenericType icollection = GenericType.allGenTypes.Single(
			x => x.name == "ICollection" && x.file.package == Package.getFromString("boh.std"));
		public static readonly GenericType iiterator = GenericType.allGenTypes.Single(
			x => x.name == "IIterator" && x.file.package == Package.getFromString("boh.std"));
	}
}
