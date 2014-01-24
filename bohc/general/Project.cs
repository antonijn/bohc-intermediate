using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using Bohc.Generation;
using Bohc.Generation.C;
using Bohc.Generation.Mangling;

using Bohc.Parsing;
using Bohc.Parsing.Statements;

using Bohc.TypeSystem;

namespace Bohc.General
{
	public class Project
	{
		public bool optimize;
		public bool debugSymbols;

		public bool desktopMode;
		public bool webMode;

		public bool library;

		public bool noDelete;

		public bool unsafeCasts;
		public bool unsafeNullPtr;

		public bool sizeOverSpeedHint;
		public bool speedOverSizeHint;

		public string output = "application";
		public string outputDir = ".";

		public bool noStd;

		public bool thisMachine;

		public bool gccOnly = true;

		public List<string> input = new List<string>();
		public List<string> cfiles = new List<string>();
		public List<string> externals = new List<string>();

		public readonly IParserStrategy pstrat;
		public readonly ICompilerStrategy cstrat;

		private readonly IMangler mangler;

		private void analyzeInput(string[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				string s = args[i];
				switch (s)
				{
					case "--help":
						//displayHelp();
						Environment.Exit(0);
						break;
					case "-c":
						cfiles.Add(args[++i]);
						break;
					case "-C":
						unsafeCasts = true;
						break;
					case "-d":
						debugSymbols = true;
						break;
					case "-D":
						desktopMode = true;
						break;
					case "-e":
						externals.Add(args[++i]);
						break;
					case "-L":
						library = true;
						break;
					case "-n":
						noDelete = true;
						break;
					case "-N":
						unsafeNullPtr = true;
						break;
					case "-o":
						output = args[++i];
						break;
					case "-O":
						outputDir = args[++i];
						break;
					case "-P":
						Console.Error.WriteLine("WARNING: Context preservation not implemented yet");
						break;
					case "-r":
						Console.Error.WriteLine("WARNING: Disabling of context deletion not implemented yet");
						break;
					case "-S":
						noStd = true;
						break;
					case "-t":
						thisMachine = true;
						break;
					case "-W":
						webMode = true;
						break;
					default:
						input.Add(s);
						break;
				}
			}
		}

		public Project(string[] args)
		{
			analyzeInput(args);

			if (input.Count == 0)
			{
				Console.Error.WriteLine("ERROR: No input files specified");
				Environment.Exit(1);
			}

			if (!noStd)
			{
				externals.Add("/usr/lib/libbohstd");
			}

			IFileParser fp = new FileParser(
				               new DefaultStatementParser(
					               new DefaultExpressionParser()),
				               this);
			this.pstrat = new DefaultParserStrategy(fp);

			mangler = new CMangler();
			this.cstrat = new DefaultCompilerStrategy(mangler, new CCodeGen(mangler, this));

			foreach (string e in externals)
			{
				loadExtern(e, fp);
			}
		}

		public void Parse()
		{
			pstrat.parse(this);
		}

		public void Build()
		{
			cstrat.compile(this);

			buildgcc(mangler, Bohc.TypeSystem.Type.Types.Where(x => !(x is Primitive) && !x.IsExtern()));

			if (library)
			{
				writeInstallScript();
			}
		}

		private void writeInstallScript()
		{
			foreach (Bohc.TypeSystem.GenericType g in Bohc.TypeSystem.GenericType.AllGenTypes)
			{
				string str = outputDir + System.IO.Path.DirectorySeparatorChar + getGenTypePath(g);
				if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(str)))
				{
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(str));
				}
				System.IO.File.WriteAllText(str, g.File.content);
			}

			StringBuilder b = new StringBuilder();
			b.AppendLine("#!/bin/sh");
			b.AppendFormat("cp {0} {1}", 
			               output + ".so",
			               "/usr/lib" + System.IO.Path.DirectorySeparatorChar + output + ".so");
			b.AppendLine();
			b.AppendFormat("cp {0} {1}", 
			               output + ".xml",
			               "/usr/lib" + System.IO.Path.DirectorySeparatorChar + output + ".xml");
			b.AppendLine();
			b.AppendFormat("cp -ar {0} {1}", 
			               output + ".gen",
			               "/usr/lib");
			b.AppendLine();
			b.AppendLine("ldconfig");

			System.IO.File.WriteAllText(outputDir + System.IO.Path.DirectorySeparatorChar + "install",
			                            b.ToString());

			Process.Start("chmod", "+x \"" +
			              outputDir + System.IO.Path.DirectorySeparatorChar + "install\"")
				.WaitForExit();
		}

		private string getGenTypePath(Bohc.TypeSystem.GenericType type)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(output);
			builder.Append(".gen");
			builder.Append(System.IO.Path.DirectorySeparatorChar);

			builder.Append(type.File.package.ToString().Replace('.', System.IO.Path.DirectorySeparatorChar));
			if (type.File.package != Bohc.TypeSystem.Package.Global)
			{
				builder.Append("/");
			}
			return builder.Append(type.Name).Append(".Boh").ToString();
		}

		private void buildgcc(IMangler mangler, IEnumerable<Bohc.TypeSystem.Type> types)
		{
			if (!System.IO.Directory.Exists(outputDir))
			{
				System.IO.Directory.CreateDirectory(outputDir);
			}
			StringBuilder script = new StringBuilder();

			if (!thisMachine || (Environment.Is64BitProcess && thisMachine))
			{
				script.Append("-w -O3 -DPF_DESKTOP64 -m64 -DPF_LINUX -pthread -std=c11 -o ");
				script.Append(outputDir + System.IO.Path.DirectorySeparatorChar + output);
				if (!Environment.Is64BitProcess)
				{
					script.Append("64");
				}
				if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				    Environment.OSVersion.Platform == PlatformID.Win32S ||
				    Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				    Environment.OSVersion.Platform == PlatformID.WinCE ||
				    Environment.OSVersion.Platform == PlatformID.Xbox)
				{
					script.Append(".exe");
				}
				if (library)
				{
					if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
					    Environment.OSVersion.Platform == PlatformID.Win32S ||
					    Environment.OSVersion.Platform == PlatformID.Win32Windows ||
					    Environment.OSVersion.Platform == PlatformID.WinCE ||
					    Environment.OSVersion.Platform == PlatformID.Xbox)
					{
						script.Append(".dll");
					}
					else
					{
						script.Append(".so");
					}
				}
				if (library)
				{
					script.Append(" -fPIC -shared");
				}
				if (noStd)
				{
					script.Append(" .c/boh_internal.c ");
				}
				script.Append(" .c/function_types.c ");
				foreach (string s in cfiles)
				{
					script.Append(s);
					script.Append(" ");
				}
				foreach (Bohc.TypeSystem.Type type in types)
				{
					script.Append(" .c/");
					script.Append(mangler.getCodeFileName(type));
					script.Append(" ");
				}
				script.Append("-lgc ");
				foreach (string e in externals)
				{
					script.Append("-L\"");
					script.Append(System.IO.Path.GetDirectoryName(e));
					script.Append("\" ");
					script.Append("-l\"");
					script.Append(System.IO.Path.GetFileName(e).Substring(3));
					script.Append("\" ");
				}
				Process.Start("gcc", script.ToString()).WaitForExit();
				script.Clear();
			}
			else if (!thisMachine || (!Environment.Is64BitProcess && thisMachine))
			{
				script.Append("-w -O3 -DPF_DESKTOP32 -m32 -DPF_LINUX -pthread -std=c11 -o ");
				script.Append(outputDir + System.IO.Path.DirectorySeparatorChar + output);
				if (Environment.Is64BitProcess)
				{
					script.Append("32");
				}
				if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
				    Environment.OSVersion.Platform == PlatformID.Win32S ||
				    Environment.OSVersion.Platform == PlatformID.Win32Windows ||
				    Environment.OSVersion.Platform == PlatformID.WinCE ||
				    Environment.OSVersion.Platform == PlatformID.Xbox)
				{
					script.Append(".exe");
				}
				if (library)
				{
					if (Environment.OSVersion.Platform == PlatformID.Win32NT ||
					    Environment.OSVersion.Platform == PlatformID.Win32S ||
					    Environment.OSVersion.Platform == PlatformID.Win32Windows ||
					    Environment.OSVersion.Platform == PlatformID.WinCE ||
					    Environment.OSVersion.Platform == PlatformID.Xbox)
					{
						script.Append(".dll");
					}
					else
					{
						script.Append(".so");
					}

					script.Append(" -fPIC -shared");
				}
				if (noStd)
				{
					script.Append(" .c/boh_internal.c ");
				}
				script.Append(" .c/function_types.c ");
				foreach (string s in cfiles)
				{
					script.Append(s);
					script.Append(" ");
				}
				foreach (Bohc.TypeSystem.Type type in types)
				{
					script.Append(" .c/");
					script.Append(mangler.getCodeFileName(type));
					script.Append(" ");
				}
				script.Append("-lgc ");
				foreach (string e in externals)
				{
					script.Append("-L\"");
					script.Append(System.IO.Path.GetDirectoryName(e));
					script.Append("\" ");
					script.Append("-l\"");
					script.Append(System.IO.Path.GetFileName(e).Substring(3));
					script.Append("\" ");
				}
				Process.Start("gcc", script.ToString()).WaitForExit();
			}

			if (!noDelete)
			{
				System.IO.Directory.Delete(".c", true);
			}

			if (library)
			{
				new XDocument(new XElement("lib",
				                           types.Where(x => x.OriginalGenType == null).Select(getXElementForType)
				                           .Concat(Bohc.TypeSystem.GenericType.AllGenTypes.Select(getXElementsForType))))
					.Save(outputDir + System.IO.Path.DirectorySeparatorChar + output + ".xml");
			}
		}

		private XElement getXElementsForType(Bohc.TypeSystem.GenericType t)
		{
			XElement x = new XElement("gentype", t.GenTypeNames.Select(z =>
			                                                           {
				XElement tmp = new XElement("param");
				tmp.SetAttributeValue("name", z);
				return tmp;
			}));
			x.SetAttributeValue("name", t.File.package.ToString() + ((t.File.package != Bohc.TypeSystem.Package.Global) ? "." + t.Name : t.Name));
			x.SetAttributeValue("file", getGenTypePath(t));
			return x;
		}

		private XElement getXElementForType(Bohc.TypeSystem.Type t)
		{
			Bohc.TypeSystem.Class c = t as Bohc.TypeSystem.Class;
			if (c != null)
			{
				XElement x = new XElement(c is Bohc.TypeSystem.Struct ? "struct" : "class", getFieldXElements(c)
				                          .Concat(getFunctionsXElements(c.Functions)));
				x.SetAttributeValue("name", c.ExternName());
				if (c.Super != null)
				{
					x.SetAttributeValue("super", c.Super.ExternName());
				}
				x.SetAttributeValue("modifiers", c.Modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			Bohc.TypeSystem.Interface i = t as Bohc.TypeSystem.Interface;
			if (i != null)
			{
				XElement x = new XElement("interface", getFunctionsXElements(c.Functions));
				x.SetAttributeValue("name", i.ExternName());
				x.SetAttributeValue("modifiers", i.Modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			Bohc.TypeSystem.Enum e = t as Bohc.TypeSystem.Enum;
			if (e != null)
			{
				XElement x = new XElement("enum", e.Enumerators.Select(y =>
				                                                       {
					XElement z = new XElement("enumerator");
					z.SetAttributeValue("name", y.Name);
					return z;
				}));
				x.SetAttributeValue("name", e.ExternName());
				x.SetAttributeValue("modifiers", e.Modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			throw new NotImplementedException();
		}

		private IEnumerable<XElement> getFieldXElements(Bohc.TypeSystem.Class c)
		{
			return c.Fields.Select(x =>
			                       {
				XElement xe = new XElement("field");
				xe.SetAttributeValue("id", x.Identifier);
				xe.SetAttributeValue("type", x.Type.ExternName());
				xe.SetAttributeValue("modifiers", x.Modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return xe;
			});
		}

		private IEnumerable<XElement> getFunctionsXElements(IEnumerable<Bohc.TypeSystem.Function> functions)
		{
			return functions.Select(x =>
			                        {
				XElement xe = new XElement("method", getParamXElements(x));
				xe.SetAttributeValue("id", x.Identifier);
				xe.SetAttributeValue("type", x.ReturnType.ExternName());

				Indexer idxer = x as Indexer;
				if (idxer != null && idxer.IsAssignment())
				{
					XElement xea = new XElement("assignment");
					xea.SetAttributeValue("id", idxer.Assignment.Identifier);
					xea.SetAttributeValue("type", idxer.Assignment.Type.ExternName());
					if (idxer.Assignment.Modifiers != Bohc.TypeSystem.Modifiers.None)
					{
						xea.SetAttributeValue("modifiers", idxer.Assignment.Modifiers.ToString().Replace('|', ' '));
					}
					xe.Add(xea);
				}

				if (x.Modifiers != Bohc.TypeSystem.Modifiers.None)
				{
					xe.SetAttributeValue("modifiers", x.Modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				}
				return xe;
			});
		}

		private XElement[] getParamXElements(Bohc.TypeSystem.Function x)
		{
			if (x.Parameters.Count == 0)
			{
				return Enumerable.Empty<XElement>().ToArray();
			}
			return x.Parameters.Select(z =>
			                           {
				XElement xe = new XElement("param");
				xe.SetAttributeValue("id", z.Identifier);
				xe.SetAttributeValue("type", z.Type.ExternName());
				if (z.Modifiers != Bohc.TypeSystem.Modifiers.None)
				{
					xe.SetAttributeValue("modifiers", z.Modifiers.ToString().Replace('|', ' '));
				}
				return xe;
			}).ToArray();
		}

		private string extname(string super)
		{
			return super.Replace("&lt;", "<").Replace("&gt;", ">");
		}

		private void loadExtern(string ext, IFileParser parser)
		{
			// call static constructors
			Bohc.TypeSystem.Primitive.Boolean.GetType();

			XDocument doc = XDocument.Load(ext + ".xml");
			XElement root = doc.Root;
			if (root.Name != "lib")
			{
				Console.Error.WriteLine("ERROR: Library definition file incompatible with current compiler version");
				Environment.Exit(1);
			}

			var types = externTS(root, ext);

			externTcsTcp(parser, types);
		}

		private void externTcsTcp(IFileParser parser, List<Bohc.TypeSystem.Type> types)
		{
			foreach (Bohc.TypeSystem.Type t in types)
			{
				externTcsTcpClass(t, parser);
				externTcsTcpInterface(parser, t);
				externTcsTcpEnum(t);
			}
		}

		private void externTcsTcpClass(Bohc.TypeSystem.Type t, IFileParser parser)
		{
			Bohc.TypeSystem.Class c = t as Bohc.TypeSystem.Class;
			if (c != null)
			{
				var super = t.XElement.Attribute("super");
				if (super != null)
				{
					c.Super = (Bohc.TypeSystem.Class)Bohc.TypeSystem.Class.GetExisting(extname(super.Value), parser);
				}
				foreach (XElement impl in t.XElement.Elements("implements"))
				{
					string implstr = impl.Value;
					c.Implement((Bohc.TypeSystem.Interface)Bohc.TypeSystem.Interface.GetExisting(extname(implstr), parser));
				}
				addFunctions(t, parser, c);
				foreach (XElement f in t.XElement.Elements("field"))
				{
					string name = f.Attribute("id").Value;
					string mods = f.Attribute("modifiers").Value;
					string type = extname(f.Attribute("type").Value);
					/*if (type == "void(int, double)")
					{
						System.Diagnostics.Debugger.Break();
					}*/
					Bohc.TypeSystem.Field field = new Bohc.TypeSystem.Field(Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(mods), name, Bohc.TypeSystem.Type.GetExisting(type, parser), c, null);
					c.AddMember(field);
				}
			}
		}

		private Parameter getParam(IFileParser parser, XElement p, Function f)
		{
			string pid = p.Attribute("id").Value;
			string pt = p.Attribute("type").Value;
			var pmodsattr = p.Attribute("modifiers");
			string pmods = "";
			if (pmodsattr != null)
			{
				pmods = pmodsattr.Value;
			}
			return new Bohc.TypeSystem.Parameter(f, Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(pmods), pid, Bohc.TypeSystem.Type.GetExisting(extname(pt), parser));
		}

		private void addFunctions(Bohc.TypeSystem.Type t, IFileParser parser, Bohc.TypeSystem.Class c)
		{
			foreach (XElement met in t.XElement.Elements("method"))
			{
				string name = met.Attribute("id").Value;
				string mods = met.Attribute("modifiers").Value;
				string rettype = extname(met.Attribute("type").Value);
				Bohc.TypeSystem.Function f = null;
				if (name == "this")
				{
					f = new Bohc.TypeSystem.Constructor(Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(mods), c, new List<Bohc.TypeSystem.Parameter>(), string.Empty);
				}
				else if (name == "indexer")
				{
					f = new TypeSystem.Indexer(c, TypeSystem.ModifierHelper.GetModifiersFromString(mods), Bohc.TypeSystem.Type.GetExisting(rettype, parser), new List<Parameter>(), string.Empty);
				}
				else if (name == "static")
				{
					f = new Bohc.TypeSystem.StaticConstructor(c, string.Empty);
				}
				else if (Bohc.Parsing.BinaryOperation.isOperator(name))
				{
					f = new Bohc.TypeSystem.OverloadedOperator(c, Bohc.Parsing.BinaryOperation.get(name),
					                                   Bohc.TypeSystem.Type.GetExisting(rettype, parser),
					                                   new List<Bohc.TypeSystem.Parameter>(),
					                                   string.Empty);
				}
				else
				{
					f = new Bohc.TypeSystem.Function(t, Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(mods), Bohc.TypeSystem.Type.GetExisting(rettype, parser), name, new List<Bohc.TypeSystem.Parameter>(), string.Empty);
				}
				foreach (XElement p in met.Elements("param"))
				{
					Parameter param = getParam(parser, p, f);
					f.Parameters.Add(param);
				}

				if (name == "indexer" && met.Elements("assignment").Count() == 1)
				{
					((TypeSystem.Indexer)f).Assignment = getParam(parser, met.Element("assignent"), f);
				}
				c.AddMember(f);
			}
		}

		private void externTcsTcpEnum(Bohc.TypeSystem.Type t)
		{
			Bohc.TypeSystem.Enum e = t as Bohc.TypeSystem.Enum;
			if (e != null)
			{
				foreach (XElement enumerator in e.XElement.Elements("enumerator"))
				{
					e.Enumerators.Add(new Bohc.TypeSystem.Enumerator(enumerator.Attribute("name").Value, e));
				}
			}
		}

		private void externTcsTcpInterface(IFileParser parser, Bohc.TypeSystem.Type t)
		{
			Bohc.TypeSystem.Interface i = t as Bohc.TypeSystem.Interface;
			if (i != null)
			{
				foreach (XElement impl in t.XElement.Elements("implements"))
				{
					string implstr = impl.Value;
					i.Implements.Add((Bohc.TypeSystem.Interface)Bohc.TypeSystem.Interface.GetExisting(extname(implstr), parser));
				}
				foreach (XElement met in t.XElement.Elements("method"))
				{
					string name = met.Attribute("id").Value;
					string mods = met.Attribute("modifiers").Value;
					string rettype = met.Attribute("type").Value;
					Bohc.TypeSystem.Function f = new Bohc.TypeSystem.Function(t, Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(mods), Bohc.TypeSystem.Type.GetExisting(extname(rettype), parser), name, new List<Bohc.TypeSystem.Parameter>(), string.Empty);
					foreach (XElement p in met.Elements("param"))
					{
						string pid = p.Attribute("id").Value;
						string pt = p.Attribute("type").Value;
						string modss = p.Attribute("modifiers").Value;
						Bohc.TypeSystem.Parameter param = new Bohc.TypeSystem.Parameter(f, Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(modss), pid, Bohc.TypeSystem.Type.GetExisting(extname(pt), parser));
						f.Parameters.Add(param);
					}
					i.Functions.Add(f);
				}
			}
		}

		private List<Bohc.TypeSystem.Type> externTS(XElement root, string ext)
		{
			List<Bohc.TypeSystem.Type> types = new List<Bohc.TypeSystem.Type>();
			externClass(root, types);
			externStruct(root, types);
			externInterface(root, types);
			externEnum(root, types);
			externGenTypes(root, ext);
			return types;
		}

		private void externClass(XElement root, List<Bohc.TypeSystem.Type> types)
		{
			foreach (XElement x in root.Elements("class"))
			{
				string name = (string)x.Attribute("name");
				string modifiers = (string)x.Attribute("modifiers");
				string pkg = name.LastIndexOf('.') != -1 ? name.Substring(0, name.LastIndexOf('.')) : "";
				if (name.LastIndexOf('.') != -1)
				{
					name = name.Substring(name.LastIndexOf('.') + 1);
				}
				types.Add(Bohc.TypeSystem.Class.Get<Bohc.TypeSystem.Class>(Bohc.TypeSystem.Package.GetFromString(pkg), Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(modifiers), name));
				types.Last().XElement = x;
			}
		}

		private void externStruct(XElement root, List<Bohc.TypeSystem.Type> types)
		{
			foreach (XElement x in root.Elements("struct"))
			{
				string name = (string)x.Attribute("name");
				string modifiers = (string)x.Attribute("modifiers");
				string pkg = name.LastIndexOf('.') != -1 ? name.Substring(0, name.LastIndexOf('.')) : "";
				if (name.LastIndexOf('.') != -1)
				{
					name = name.Substring(name.LastIndexOf('.') + 1);
				}
				types.Add(Bohc.TypeSystem.Class.Get<Bohc.TypeSystem.Struct>(Bohc.TypeSystem.Package.GetFromString(pkg), Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(modifiers), name));
				types.Last().XElement = x;
			}
		}

		private void externInterface(XElement root, List<Bohc.TypeSystem.Type> types)
		{
			foreach (XElement x in root.Elements("interface"))
			{
				string name = (string)x.Attribute("name");
				string modifiers = (string)x.Attribute("modifiers");
				string pkg = name.LastIndexOf('.') != -1 ? name.Substring(0, name.LastIndexOf('.')) : "";
				if (name.LastIndexOf('.') != -1)
				{
					name = name.Substring(name.LastIndexOf('.') + 1);
				}
				types.Add(Bohc.TypeSystem.Interface.Get(Bohc.TypeSystem.Package.GetFromString(pkg), Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(modifiers), name));
				types.Last().XElement = x;
			}
		}

		private void externEnum(XElement root, List<Bohc.TypeSystem.Type> types)
		{
			foreach (XElement x in root.Elements("enum"))
			{
				string name = (string)x.Attribute("name");
				string modifiers = (string)x.Attribute("modifiers");
				string pkg = name.LastIndexOf('.') != -1 ? name.Substring(0, name.LastIndexOf('.')) : "";
				if (name.LastIndexOf('.') != -1)
				{
					name = name.Substring(name.LastIndexOf('.') + 1);
				}
				types.Add(Bohc.TypeSystem.Enum.Get(Bohc.TypeSystem.Package.GetFromString(pkg), Bohc.TypeSystem.ModifierHelper.GetModifiersFromString(modifiers), name));
				types.Last().XElement = x;
			}
		}

		private void externGenTypes(XElement root, string ext)
		{
			foreach (XElement x in root.Elements("gentype"))
			{
				string name = (string)x.Attribute("name");
				string file = (string)x.Attribute("file");
				string pkg = name.LastIndexOf('.') != -1 ? name.Substring(0, name.LastIndexOf('.')) : "";
				if (name.LastIndexOf('.') != -1)
				{
					name = name.Substring(name.LastIndexOf('.') + 1);
				}
				Bohc.Parsing.File f = new File(null, Bohc.TypeSystem.Package.GetFromString(pkg), System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(ext) + System.IO.Path.DirectorySeparatorChar + file));
				Bohc.TypeSystem.GenericType gt = new Bohc.TypeSystem.GenericType(x.Elements("param").SelectMany(y => y.Attributes("name").Select(z => z.Value)).ToArray(), name);
				gt.File = f;
			}
		}
	}
}

