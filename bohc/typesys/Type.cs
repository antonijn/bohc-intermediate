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
using System.Xml.Linq;

using Bohc.Parsing;

namespace Bohc.TypeSystem
{
	public abstract class Type : IType
	{
		File IType.GetFile()
		{
			return File;
		}

		void IType.SetFile(File f)
		{
			File = f;
		}

		public GenericType OriginalGenType;

		/// <summary>
		/// gets a value indicating to which extent two types are related.
		/// zero means no relation, one means it's the type itself,
		/// two means it's the super class or an implemented interface,
		/// three means it's a super class of the super class, or an implemented interface of the super class
		/// and so on.
		/// </summary>
		public virtual int Extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			return 0;
		}

		public static bool IsValidIdentifier(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}

			char first = name.First();
			if (!char.IsLetter(first) && first != '_')
			{
				return false;
			}

			foreach (char ch in name)
			{
				if (!char.IsLetterOrDigit(ch) && ch != '_')
				{
					return false;
				}
			}

			return true;
		}

		public static bool isValidName(string name, bool canbeprimitive)
		{
			if (!canbeprimitive && Primitive.IsPrimitiveTypeName(name))
			{
				return false;
			}

			return IsValidIdentifier(name);
		}

		public readonly Package Package;
		public readonly Modifiers Modifiers;
		public readonly string Name;

		public Parsing.File File;

		public static readonly List<Type> Types = new List<Type>();

		public Type(Package package, Modifiers modifiers, string name)
		{
			Boh.Exception.require<Exceptions.ParserException>(
				this is NullType ||
				this is CompatibleWithAllType ||
				this is FunctionRefType ||
				isValidName(name, this is Primitive), name + " is not a valid typename");

			this.Package = package;
			this.Modifiers = modifiers;
			this.Name = name;

			if (!(this is NullType || this is CompatibleWithAllType || this is FunctionRefType || name.StartsWith("native.")))
			{
				lock (Types)
				{
					Boh.Exception.require<Exceptions.ParserException>(!Types.Any(x => x.Package == package && x.Name == name), "multiple definitions for " + package.ToString() + name);
					Types.Add(this);
				}
			}
		}

		public static Type GetExisting(string name, IFileParser parser)
		{
			if (name.TrimEnd().EndsWith(")"))
			{
				return GetFunctionRefType(new[] { Package.Global }, name, parser);
			}
			int idxLt = name.IndexOf('<');
			int lidxDot = idxLt == -1 ? name.LastIndexOf('.') : name.Substring(0, idxLt).LastIndexOf('.');
			string pkg = name.Substring(0, lidxDot != -1 ? lidxDot : 0);
			string cname = name.Substring(lidxDot != -1 ? lidxDot + 1 : 0);
			return Bohc.TypeSystem.Type.GetExisting(Package.GetFromStringExisting(pkg), cname, parser);
		}

		/// <returns>null on failure</returns>
		public static Type GetExisting(Package package, string name, IFileParser parser)
		{
			return GetExisting(package, new[] { package }, name, parser);
		}

		static Type GetFunctionRefType(IEnumerable<Package> packages, string name, IFileParser parser)
		{
			string nameTrimEnd = name.TrimEnd();
			int startParent = ParserTools.getMatchingBraceCharBackwards(nameTrimEnd, nameTrimEnd.Length - 1, '(');
			IEnumerable<Type> fParamTypes = ParserTools.split(nameTrimEnd, startParent, ')', ',').Select(x => GetExisting(packages, x.Trim(), parser));
			if (string.IsNullOrWhiteSpace(nameTrimEnd.Substring(startParent + 1, nameTrimEnd.Length - startParent - 2)))
			{
				fParamTypes = Enumerable.Empty<Type>();
			}
			string retTypeStr = nameTrimEnd.Substring(0, startParent);
			Type retType = GetExisting(packages, retTypeStr, parser);
			if (fParamTypes.Any(x => x == null) || retType == null)
			{
				return null;
			}
			return FunctionRefType.Get(retType, fParamTypes.ToArray());
		}

		public static Type GetExisting(Package package, IEnumerable<Package> packages, string name, IFileParser parser)
		{
			if (name.EndsWith("[]"))
			{
				return StdType.Array.GetTypeFor(new[] { GetExisting(packages, name.Substring(0, name.Length - 2).TrimEnd(), parser) }, parser);
			}

			/*if (name.StartsWith("native."))
			{
				if (isValidName(name.Replace(".", string.Empty).Replace("*", string.Empty), true))
				{
					return new NativeType(name.Substring("native.".Length));
				}
				return null;
			}*/

			lock (Types)
			{
				// function pointer type
				string nameTrimEnd = name.TrimEnd();
				if (nameTrimEnd.EndsWith(")"))
				{
					return GetFunctionRefType(packages, name, parser);
				}
				int idxL = name.IndexOf('<');
				if (idxL != -1)
				{
					string actName = name.Substring(0, idxL);
					string[] split = ParserTools.split(name, idxL, '>', ',').ToArray();
					Type[] genTypes = split.Select(x => GetExisting(packages, x, parser)).ToArray();
					IEnumerable<GenericType> options = GenericType.AllGenTypes.Where(x => x.File.package == package);
					if (options.Count() == 0)
					{
						return null;
					}

					GenericType gType = options.SingleOrDefault(x => x.Name == actName);
					if (gType == null)
					{
						return null;
					}
					return gType.GetTypeFor(genTypes, parser);
				}
				return Types.SingleOrDefault(x => x.Package == package && x.Name == name) ?? (name.Contains('.') ? GetExisting(name, parser) : null);
			}
		}

		public static Type GetExisting(IEnumerable<Package> packages, string name, IFileParser parser)
		{
			lock (Types)
			{
				foreach (Package p in packages.Concat(new[] { Package.Global }))
				{
					Type t = GetExisting(p, packages, name, parser);
					if (t != null)
					{
						return t;
					}
				}
			}

			// TODO: solve this
			//Boh.Exception._throw<Exceptions.ParserException>("type not found: " + name);
			return null;
		}

		public static bool Exists(IEnumerable<Package> packages, string name, IFileParser parser)
		{
			foreach (Package p in packages.Concat(new[] { Package.Global }))
			{
				try
				{
					Type t = GetExisting(p, name, parser);
					if (t != null)
					{
						return true;
					}
				}
				catch // if shit happens with generics (can't get hash code because null.GetHashCode() fails)
				{

				}
			}

			return false;
		}

		public override string ToString()
		{
			return Name;
		}

		public string GenName;

		public virtual string ExternName()
		{
			if (OriginalGenType != null)
			{
				string pckg = Package.ToString();
				return (string.IsNullOrEmpty(pckg) ? string.Empty : pckg + '.') + GenName.Replace("<", "&lt;").Replace(">", "&gt;");
			}
			return FullName();
		}

		public string FullName()
		{
			string pckg = Package.ToString();
			return (string.IsNullOrEmpty(pckg) ? string.Empty : pckg + '.') + Name;
		}

		public abstract Parsing.Expression DefaultVal();
		public abstract bool IsReferenceType();

		public XElement XElement;
		public bool IsExtern()
		{
			return XElement != null;
		}
	}
}
