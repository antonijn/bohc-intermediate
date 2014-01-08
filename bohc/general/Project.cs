using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

using bohc.typesys;

namespace bohc.general
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

		public void parse()
		{
			pstrat.parse(this);
		}

		public void build()
		{
			cstrat.compile(this);

			buildgcc(mangler, typesys.Type.types.Where(x => !(x is Primitive)));

			if (library)
			{
				writeInstallScript();
			}
		}

		private void writeInstallScript()
		{
			foreach (typesys.GenericType g in typesys.GenericType.allGenTypes)
			{
				string str = outputDir + System.IO.Path.DirectorySeparatorChar + getGenTypePath(g);
				if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(str)))
				{
					System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(str));
				}
				System.IO.File.WriteAllText(str, g.file.content);
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

		private string getGenTypePath(typesys.GenericType type)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(output);
			builder.Append(".gen");
			builder.Append(System.IO.Path.DirectorySeparatorChar);

			builder.Append(type.file.package.ToString().Replace('.', System.IO.Path.DirectorySeparatorChar));
			if (type.file.package != typesys.Package.GLOBAL)
			{
				builder.Append("/");
			}
			return builder.Append(type.name).Append(".boh").ToString();
		}

		private void buildgcc(IMangler mangler, IEnumerable<bohc.typesys.Type> types)
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
				script.Append(" .c/boh_internal.c .c/function_types.c ");
				foreach (string s in cfiles)
				{
					script.Append(s);
					script.Append(" ");
				}
				foreach (typesys.Type type in types)
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
				}
				if (library)
				{
					script.Append(" -fPIC -shared");
				}
				script.Append(" .c/boh_internal.c .c/function_types.c ");
				foreach (string s in cfiles)
				{
					script.Append(s);
					script.Append(" ");
				}
				foreach (typesys.Type type in types)
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
				                           types.Where(x => x.originalGenType == null).Select(getXElementForType)
				                           .Concat(typesys.GenericType.allGenTypes.Select(getXElementsForType))))
					.Save(outputDir + System.IO.Path.DirectorySeparatorChar + output + ".xml");
			}
		}

		private XElement getXElementsForType(typesys.GenericType t)
		{
			XElement x = new XElement("gentype", t.genTypeNames.Select(z =>
			                                                           {
				XElement tmp = new XElement("param");
				tmp.SetAttributeValue("name", z);
				return tmp;
			}));
			x.SetAttributeValue("name", t.file.package.ToString() + ((t.file.package != typesys.Package.GLOBAL) ? "." + t.name : t.name));
			x.SetAttributeValue("file", getGenTypePath(t));
			return x;
		}

		private XElement getXElementForType(typesys.Type t)
		{
			typesys.Class c = t as typesys.Class;
			if (c != null)
			{
				XElement x = new XElement(c is typesys.Struct ? "struct" : "class", getFieldXElements(c)
				                          .Concat(getFunctionsXElements(c.functions)));
				x.SetAttributeValue("name", c.externName());
				if (c.super != null)
				{
					x.SetAttributeValue("super", c.super.externName());
				}
				x.SetAttributeValue("modifiers", c.modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			typesys.Interface i = t as typesys.Interface;
			if (i != null)
			{
				XElement x = new XElement("interface", getFunctionsXElements(c.functions));
				x.SetAttributeValue("name", i.externName());
				x.SetAttributeValue("modifiers", i.modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			typesys.Enum e = t as typesys.Enum;
			if (e != null)
			{
				XElement x = new XElement("enum", e.enumerators.Select(y =>
				                                                       {
					XElement z = new XElement("enumerator");
					z.SetAttributeValue("name", y.name);
					return z;
				}));
				x.SetAttributeValue("name", e.externName());
				x.SetAttributeValue("modifiers", e.modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return x;
			}

			throw new NotImplementedException();
		}

		private IEnumerable<XElement> getFieldXElements(bohc.typesys.Class c)
		{
			return c.fields.Select(x =>
			                       {
				XElement xe = new XElement("field");
				xe.SetAttributeValue("id", x.identifier);
				xe.SetAttributeValue("type", x.type.externName());
				xe.SetAttributeValue("modifiers", x.modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				return xe;
			});
		}

		private IEnumerable<XElement> getFunctionsXElements(IEnumerable<typesys.Function> functions)
		{
			return functions.Select(x =>
			                        {
				XElement xe = new XElement("method", getParamXElements(x));
				xe.SetAttributeValue("id", x.identifier);
				xe.SetAttributeValue("type", x.returnType.externName());
				if (x.modifiers != bohc.typesys.Modifiers.NONE)
				{
					xe.SetAttributeValue("modifiers", x.modifiers.ToString().ToLowerInvariant().Replace(",", ""));
				}
				return xe;
			});
		}

		private XElement[] getParamXElements(bohc.typesys.Function x)
		{
			if (x.parameters.Count == 0)
			{
				return Enumerable.Empty<XElement>().ToArray();
			}
			return x.parameters.Select(z =>
			                           {
				XElement xe = new XElement("param");
				xe.SetAttributeValue("id", z.identifier);
				xe.SetAttributeValue("type", z.type.externName());
				if (z.modifiers != bohc.typesys.Modifiers.NONE)
				{
					xe.SetAttributeValue("modifiers", z.modifiers.ToString().Replace('|', ' '));
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
			typesys.Primitive.BOOLEAN.GetType();

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

		private void externTcsTcp(IFileParser parser, List<bohc.typesys.Type> types)
		{
			foreach (typesys.Type t in types)
			{
				externTcsTcpClass(t, parser);
				externTcsTcpInterface(parser, t);
				externTcsTcpEnum(t);
			}
		}

		private void externTcsTcpClass(typesys.Type t, IFileParser parser)
		{
			typesys.Class c = t as typesys.Class;
			if (c != null)
			{
				var super = t.xelement.Attribute("super");
				if (super != null)
				{
					c.super = (typesys.Class)typesys.Class.getExisting(extname(super.Value), parser);
				}
				foreach (XElement impl in t.xelement.Elements("implements"))
				{
					string implstr = impl.Value;
					c.implement((typesys.Interface)typesys.Interface.getExisting(extname(implstr), parser));
				}
				addFunctions(t, parser, c);
				foreach (XElement f in t.xelement.Elements("field"))
				{
					string name = f.Attribute("id").Value;
					string mods = f.Attribute("modifiers").Value;
					string type = extname(f.Attribute("type").Value);
					/*if (type == "void(int, double)")
					{
						System.Diagnostics.Debugger.Break();
					}*/
					typesys.Field field = new bohc.typesys.Field(typesys.ModifierHelper.getModifiersFromString(mods), name, typesys.Type.getExisting(type, parser), c, null);
					c.addMember(field);
				}
			}
		}

		private void addFunctions(bohc.typesys.Type t, IFileParser parser, bohc.typesys.Class c)
		{
			foreach (XElement met in t.xelement.Elements("method"))
			{
				string name = met.Attribute("id").Value;
				string mods = met.Attribute("modifiers").Value;
				string rettype = extname(met.Attribute("type").Value);
				typesys.Function f = null;
				if (name == "this")
				{
					f = new bohc.typesys.Constructor(typesys.ModifierHelper.getModifiersFromString(mods), c, new List<bohc.typesys.Parameter>(), string.Empty);
				}
				else if (name == "static")
				{
					f = new bohc.typesys.StaticConstructor(c, string.Empty);
				}
				else if (parsing.BinaryOperation.isOperator(name))
				{
					f = new typesys.OverloadedOperator(c, parsing.BinaryOperation.get(name),
					                                   typesys.Type.getExisting(rettype, parser),
					                                   new List<bohc.typesys.Parameter>(),
					                                   string.Empty);
				}
				else
				{
					f = new bohc.typesys.Function(t, typesys.ModifierHelper.getModifiersFromString(mods), typesys.Type.getExisting(rettype, parser), name, new List<bohc.typesys.Parameter>(), string.Empty);
				}
				foreach (XElement p in met.Elements("param"))
				{
					string pid = p.Attribute("id").Value;
					string pt = p.Attribute("type").Value;
					var pmodsattr = p.Attribute("modifiers");
					string pmods = "";
					if (pmodsattr != null)
					{
						pmods = pmodsattr.Value;
					}
					typesys.Parameter param = new bohc.typesys.Parameter(f, typesys.ModifierHelper.getModifiersFromString(pmods), pid, typesys.Type.getExisting(extname(pt), parser));
					f.parameters.Add(param);
				}
				c.addMember(f);
			}
		}

		private void externTcsTcpEnum(bohc.typesys.Type t)
		{
			typesys.Enum e = t as typesys.Enum;
			if (e != null)
			{
				foreach (XElement enumerator in e.xelement.Elements("enumerator"))
				{
					e.enumerators.Add(new bohc.typesys.Enumerator(enumerator.Attribute("name").Value, e));
				}
			}
		}

		private void externTcsTcpInterface(IFileParser parser, bohc.typesys.Type t)
		{
			typesys.Interface i = t as typesys.Interface;
			if (i != null)
			{
				foreach (XElement impl in t.xelement.Elements("implements"))
				{
					string implstr = impl.Value;
					i.implements.Add((typesys.Interface)typesys.Interface.getExisting(extname(implstr), parser));
				}
				foreach (XElement met in t.xelement.Elements("method"))
				{
					string name = met.Attribute("id").Value;
					string mods = met.Attribute("modifiers").Value;
					string rettype = met.Attribute("type").Value;
					typesys.Function f = new bohc.typesys.Function(t, typesys.ModifierHelper.getModifiersFromString(mods), typesys.Type.getExisting(extname(rettype), parser), name, new List<bohc.typesys.Parameter>(), string.Empty);
					foreach (XElement p in met.Elements("param"))
					{
						string pid = p.Attribute("id").Value;
						string pt = p.Attribute("type").Value;
						string modss = p.Attribute("modifiers").Value;
						typesys.Parameter param = new bohc.typesys.Parameter(f, typesys.ModifierHelper.getModifiersFromString(modss), pid, typesys.Type.getExisting(extname(pt), parser));
						f.parameters.Add(param);
					}
					i.functions.Add(f);
				}
			}
		}

		private List<typesys.Type> externTS(XElement root, string ext)
		{
			List<typesys.Type> types = new List<typesys.Type>();
			externClass(root, types);
			externStruct(root, types);
			externInterface(root, types);
			externEnum(root, types);
			externGenTypes(root, ext);
			return types;
		}

		private void externClass(XElement root, List<bohc.typesys.Type> types)
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
				types.Add(typesys.Class.get<typesys.Class>(typesys.Package.getFromString(pkg), typesys.ModifierHelper.getModifiersFromString(modifiers), name));
				types.Last().xelement = x;
			}
		}

		private void externStruct(XElement root, List<bohc.typesys.Type> types)
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
				types.Add(typesys.Class.get<typesys.Struct>(typesys.Package.getFromString(pkg), typesys.ModifierHelper.getModifiersFromString(modifiers), name));
				types.Last().xelement = x;
			}
		}

		private void externInterface(XElement root, List<bohc.typesys.Type> types)
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
				types.Add(typesys.Interface.get(typesys.Package.getFromString(pkg), typesys.ModifierHelper.getModifiersFromString(modifiers), name));
				types.Last().xelement = x;
			}
		}

		private void externEnum(XElement root, List<bohc.typesys.Type> types)
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
				types.Add(typesys.Enum.get(typesys.Package.getFromString(pkg), typesys.ModifierHelper.getModifiersFromString(modifiers), name));
				types.Last().xelement = x;
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
				parsing.File f = new File(null, typesys.Package.getFromString(pkg), System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(ext) + System.IO.Path.DirectorySeparatorChar + file));
				typesys.GenericType gt = new bohc.typesys.GenericType(x.Elements("param").SelectMany(y => y.Attributes("name").Select(z => z.Value)).ToArray(), name);
				gt.file = f;
			}
		}
	}
}

