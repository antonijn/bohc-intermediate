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
		static StdType()
		{
			try
			{
				BohLang = Package.GetFromString("aqua.std");
				Obj = (Class)TypeSystem.Type.GetExisting(BohLang, "Object", null);
				Str = (Class)TypeSystem.Type.GetExisting(BohLang, "String", null);
				Type = (Class)TypeSystem.Type.GetExisting(BohLang, "Type", null);

				Ptr = GenericType.AllGenTypes.Single(
					x => x.Name == "Ptr" && x.File.package == Package.Get(BohLang, "interop"));
				Array = GenericType.AllGenTypes.Single(
					x => x.Name == "Array" && x.File.package == BohLang);
				Box = GenericType.AllGenTypes.Single(
					x => x.Name == "Box" && x.File.package == BohLang);
				ICollection = GenericType.AllGenTypes.Single(
					x => x.Name == "Collection" && x.File.package == BohLang);
				IIterator = GenericType.AllGenTypes.Single(
					x => x.Name == "Iterator" && x.File.package == BohLang);
			}
			catch
			{
			}
		}

		public static Package BohLang;

		public static readonly Class Obj;
		public static readonly Class Str = (Class)TypeSystem.Type.GetExisting(BohLang, "String", null);
		public static readonly Class Type = (Class)TypeSystem.Type.GetExisting(BohLang, "Type", null);

		// TODO: check that package is Boh.std
		public static readonly GenericType Ptr;
		public static readonly GenericType Array;
		public static readonly GenericType Box;
		public static readonly GenericType ICollection;
		public static readonly GenericType IIterator;
	}
}
