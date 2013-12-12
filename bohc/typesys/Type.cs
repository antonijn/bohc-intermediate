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

using bohc.parsing.ts;

namespace bohc.typesys
{
	public abstract class Type : IType
	{
		File IType.getFile()
		{
			return file;
		}

		void IType.setFile(File f)
		{
			file = f;
		}

		/// <summary>
		/// gets a value indicating to which extent two types are related.
		/// zero means no relation, one means it's the type itself,
		/// two means it's the super class or an implemented interface,
		/// three means it's a super class of the super class, or an implemented interface of the super class
		/// and so on.
		/// </summary>
		public virtual int extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			return 0;
		}

		public static bool isValidIdentifier(string name)
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
			if (!canbeprimitive && Primitive.isPrimitiveTypeName(name))
			{
				return false;
			}

			return isValidIdentifier(name);
		}

		public readonly Package package;
		public readonly Modifiers modifiers;
		public readonly string name;

		public parsing.ts.File file;

		private static readonly List<Type> types = new List<Type>();

		public Type(Package package, Modifiers modifiers, string name)
		{
			boh.Exception.require<exceptions.ParserException>(
				this is NullType ||
				this is CompatibleWithAllType ||
				this is NativeType ||
				this is FunctionRefType ||
				isValidName(name, this is Primitive), name + " is not a valid typename");

			this.package = package;
			this.modifiers = modifiers;
			this.name = name;

			if (!(this is NullType || this is CompatibleWithAllType || this is FunctionRefType || name.StartsWith("native.")))
			{
				lock (types)
				{
					boh.Exception.require<exceptions.ParserException>(!types.Any(x => x.package == package && x.name == name), "multiple definitions for " + package.ToString() + name);
					types.Add(this);
				}
			}
		}

		public static Type getExisting(string name)
		{
			int lidxDot = name.LastIndexOf('.');
			string pkg = name.Substring(0, lidxDot != -1 ? lidxDot : 0);
			string cname = name.Substring(lidxDot != -1 ? lidxDot + 1 : 0);
			return typesys.Type.getExisting(Package.getFromStringExisting(pkg), cname);
		}

		/// <returns>null on failure</returns>
		public static Type getExisting(Package package, string name)
		{
			return getExisting(package, new[] { package }, name);
		}
		public static Type getExisting(Package package, IEnumerable<Package> packages, string name)
		{
			if (name.EndsWith("[]"))
			{
				return StdType.array.getTypeFor(new[] { getExisting(packages, name.Substring(0, name.Length - 2).TrimEnd()) });
			}

			if (name.StartsWith("native."))
			{
				if (isValidName(name.Replace(".", string.Empty).Replace("*", string.Empty), true))
				{
					return new NativeType(name.Substring("native.".Length));
				}
				return null;
			}

			lock (types)
			{
				// function pointer type
				string nameTrimEnd = name.TrimEnd();
				if (nameTrimEnd.EndsWith(")"))
				{
					int startParent = Parser.getMatchingBraceCharBackwards(nameTrimEnd, nameTrimEnd.Length - 1, '(');
					IEnumerable<Type> fParamTypes = Parser.split(nameTrimEnd, startParent, ')', ',').Select(x => getExisting(packages, x));
					if (string.IsNullOrWhiteSpace(nameTrimEnd.Substring(startParent + 1, nameTrimEnd.Length - startParent - 2)))
					{
						fParamTypes = Enumerable.Empty<Type>();
					}
					string retTypeStr = nameTrimEnd.Substring(0, startParent);
					Type retType = getExisting(packages, retTypeStr);
					if (fParamTypes.Any(x => x == null) || retType == null)
					{
						return null;
					}
					return FunctionRefType.get(retType, fParamTypes.ToArray());
				}
				int idxL = name.IndexOf('<');
				if (idxL != -1)
				{
					string actName = name.Substring(0, idxL);
					string[] split = Parser.split(name, idxL, '>', ',').ToArray();
					Type[] genTypes = split.Select(x => getExisting(packages, x)).ToArray();
					IEnumerable<GenericType> options = GenericType.allGenTypes.Where(x => x.file.package == package);
					if (options.Count() == 0)
					{
						return null;
					}

					GenericType gType = options.Single(x => x.name == actName);
					return gType.getTypeFor(genTypes);
				}
				return types.SingleOrDefault(x => x.package == package && x.name == name) ?? (name.Contains('.') ? getExisting(name) : null);
			}
		}

		public static Type getExisting(IEnumerable<Package> packages, string name)
		{
			lock (types)
			{
				foreach (Package p in packages.Concat(new[] { Package.GLOBAL }))
				{
					Type t = getExisting(p, packages, name);
					if (t != null)
					{
						return t;
					}
				}
			}

			// TODO: solve this
			//boh.Exception._throw<exceptions.ParserException>("type not found: " + name);
			return null;
		}

		public static bool exists(IEnumerable<Package> packages, string name)
		{
			foreach (Package p in packages.Concat(new[] { Package.GLOBAL }))
			{
				Type t = getExisting(p, name);
				if (t != null)
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			return name;
		}

		public string fullName()
		{
			string pckg = package.ToString();
			return (string.IsNullOrEmpty(pckg) ? string.Empty : pckg + '.') + name;
		}

		public abstract parsing.Expression defaultVal();
	}
}
