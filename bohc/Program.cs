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
using System.Diagnostics;
using System.Xml.Linq;

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

namespace bohc
{
	public enum ParserState
	{
		TS,
		TP,
		TCS,
		TCP,
		CP,
	}

	public class Program
	{
		public static class Options
		{
			public static bool optimize;
			public static bool debugSymbols;

			public static bool desktopMode;
			public static bool webMode;

			public static bool library;

			public static bool noDelete;

			public static bool unsafeCasts;
			public static bool unsafeNullPtr;

			public static bool sizeOverSpeedHint;
			public static bool speedOverSizeHint;

			public static string output = "application";
			public static string outputDir = ".";

			public static bool noStd;

			public static bool thisMachine;

			public static bool gccOnly = true;

			public static List<string> cfiles = new List<string>();
			public static List<string> externals = new List<string>();
		}

		public static readonly Dictionary<string, parsing.File> genTypes = new Dictionary<string, parsing.File>();

		public static ParserState pstate;

		private static void displayHelp()
		{
			Console.WriteLine("The compiler for the Boh programming language. Intermediate version.");
			Console.WriteLine("Usage: mono bohc.exe [options...] [input files...]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("  --help");
		}

		private static IEnumerable<string> getInput(string[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				string s = args[i];
				switch (s)
				{
					case "--help":
						displayHelp();
						Environment.Exit(0);
						break;
					case "-c":
						Options.cfiles.Add(args[++i]);
						break;
					case "-C":
						Options.unsafeCasts = true;
						break;
					case "-d":
						Options.debugSymbols = true;
						break;
					case "-D":
						Options.desktopMode = true;
						break;
					case "-e":
						Options.externals.Add(args[++i]);
						break;
					case "-L":
						Options.library = true;
						break;
					case "-n":
						Options.noDelete = true;
						break;
					case "-N":
						Options.unsafeNullPtr = true;
						break;
					case "-o":
						Options.output = args[++i];
						break;
					case "-O":
						Options.outputDir = args[++i];
						break;
					case "-P":
						Console.Error.WriteLine("WARNING: Context preservation not implemented yet");
						break;
					case "-r":
						Console.Error.WriteLine("WARNING: Disabling of context deletion not implemented yet");
						break;
					case "-S":
						Options.noStd = true;
						break;
					case "-t":
						Options.thisMachine = true;
						break;
					case "-W":
						Options.webMode = true;
						break;
					default:
						yield return s;
						break;
				}
			}
		}

		public static void Main(string[] args)
		{
#if DEBUG
			string[] filenames = getInput(new string[]
			{
				"/home/antonijn/code/cs/bohp/bohp/bin/Release/hw/MainClass.boh",
				"-n",
			}
			).ToArray();
#else
			string[] filenames = getInput(args).ToArray();
#endif

			if (!Options.noStd)
			{
				Options.externals.Add("/usr/lib/libbohstd");
			}

			foreach (string e in Options.externals)
			{
				loadExtern(e);
			}

			if (filenames.Length == 0)
			{
				Console.Error.WriteLine("ERROR: No input files specified");
				Environment.Exit(1);
			}
			string[] files = new string[filenames.Length];

			for (int i = 0; i < filenames.Length; ++i)
			{
				files [i] = System.IO.File.ReadAllText(filenames [i]);
			}

			Dictionary<string, parsing.File> filesassoc = new Dictionary<string, parsing.File>();

			Stopwatch sw = new Stopwatch();
			sw.Start();

			Parser parser = new Parser(new DefaultStatementParser(new DefaultExpressionParser()));

			parseTS(files, filesassoc, parser);

			parseTP(files, filesassoc, parser);

			typesys.Primitive.figureOutFunctionsForAll();

			var vpairs = parseTCS(files, filesassoc, parser);

			foreach (typesys.Enum e in typesys.Enum.instances)
			{
				e.sortOutFunctions(parser);
			}

			parseTCP(files, filesassoc, parser, ref vpairs);


			parseCP(files, filesassoc, parser, ref vpairs);


			//typesys.GenericType arrayt = filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).Single();
			//arrayt.getTypeFor(new[] { typesys.Primitive.BOOLEAN });

			//Console.WriteLine("Generating code");

			//IMangler mangler = new CMangler();
			//ICodeGen codegen = new CCodeGen(mangler);

			IMangler mangler = new CMangler();
			ICodeGen codegen = new generation.c.CCodeGen(mangler);

			IEnumerable<typesys.Type> types = filesassoc.Values.Select(x => x.type as typesys.Type).Where(x => x != null).Concat(
				filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).SelectMany(x => x.types.Values));

			//Console.WriteLine("Generating lambdas");
			codegen.generateGeneralBit(typesys.Type.types.Where(x => !(x is typesys.Primitive)));

			foreach (typesys.Type type in typesys.Type.types.Where(x => !(x is typesys.Primitive)))
			{
				//Console.WriteLine("Generating code for: {0}", type.fullName());
				codegen.generateFor(type, typesys.Type.types.Where(x => !(x is typesys.Primitive)));
			}

			codegen.finish(typesys.Type.types.Where(x => !(x is typesys.Primitive)));

			sw.Stop();

			//Console.WriteLine("Done generating code");
			//Console.WriteLine("Compilation of {0} files took: {1} milliseconds", files.Length, sw.Elapsed.TotalMilliseconds);

			buildgcc(mangler, types);

			if (Options.library)
			{
				foreach (typesys.GenericType g in typesys.GenericType.allGenTypes)
				{
					string str = Options.outputDir + System.IO.Path.DirectorySeparatorChar + getGenTypePath(g);
					if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(str)))
					{
						System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(str));
					}
					System.IO.File.WriteAllText(str, g.file.content);
				}

				StringBuilder b = new StringBuilder();
				b.AppendLine("#!/bin/sh");
				b.AppendFormat("cp {0} {1}", 
				               Options.output + ".so",
				               "/usr/lib" + System.IO.Path.DirectorySeparatorChar + Options.output + ".so");
				b.AppendLine();
				b.AppendFormat("cp {0} {1}", 
				               Options.output + ".xml",
				               "/usr/lib" + System.IO.Path.DirectorySeparatorChar + Options.output + ".xml");
				b.AppendLine();
				b.AppendFormat("cp -ar {0} {1}", 
				               Options.output + ".gen",
				               "/usr/lib");
				b.AppendLine();
				b.AppendLine("ldconfig");

				System.IO.File.WriteAllText(Options.outputDir + System.IO.Path.DirectorySeparatorChar + "install",
				                            b.ToString());

				Process.Start("chmod", "+x \"" +
				              Options.outputDir + System.IO.Path.DirectorySeparatorChar + "install\"")
					.WaitForExit();
			}

			//Console.ReadKey();
		}

		static void parseTS(string[] files, Dictionary<string, File> filesassoc, Parser parser)
		{
			pstate = ParserState.TS;
			for (int i = 0; i < files.Length; ++i)
			{
				string file = files[i];
				parsing.File f = parser.parseFileTS(ref file);
				files[i] = file;
				filesassoc[file] = f;
				if (f.type is typesys.Type)
				{
					//Console.WriteLine("Type skimming for: {0}", ((typesys.Type)f.type).fullName());
				}
			}
		}

		static void parseTP(string[] files, Dictionary<string, File> filesassoc, Parser parser)
		{
			pstate = ParserState.TP;
			foreach (string file in files)
			{
				try
				{
					parsing.File f = filesassoc[file];
					if (f.type is typesys.Type)
					{
						parser.parseFileTP(f, file);
						//Console.WriteLine("Type parsing for: {0}", ((typesys.Type)f.type).fullName());
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", file, e.ToString());
				}
			}
		}

		static KeyValuePair<string, File>[] parseTCS(string[] files, Dictionary<string, File> filesassoc, Parser parser)
		{
			pstate = ParserState.TCS;
			KeyValuePair<string, parsing.File>[] vpairs = genTypes.ToArray();
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)v.Value.type).fullName());
				try
				{
					parser.parseFileTCS(v.Value, v.Key);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", v.Value, e.ToString());
				}
			}
			foreach (string file in files)
			{
				try
				{
					parsing.File f = filesassoc[file];
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCS(f, file);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", file, e.ToString());
				}
			}
			return vpairs;
		}

		static void parseTCP(string[] files, Dictionary<string, File> filesassoc, Parser parser, ref KeyValuePair<string, File>[] vpairs)
		{
			pstate = ParserState.TCP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				//Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				try
				{
					parser.parseFileTCP(v.Value, v.Key);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", v.Value, e.ToString());
				}
			}
			foreach (string file in files)
			{
				try
				{
					parsing.File f = filesassoc[file];
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileTCP(f, file);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", file, e.ToString());
				}
			}
		}

		static void parseCP(string[] files, Dictionary<string, File> filesassoc, Parser parser, ref KeyValuePair<string, File>[] vpairs)
		{
			pstate = ParserState.CP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				//Console.WriteLine("Code parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				try
				{
					parser.parseFileCP(v.Value, v.Key);
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", v.Value, e.ToString());
				}
			}
			foreach (string file in files)
			{
				try
				{
					parsing.File f = filesassoc[file];
					if (f.type is typesys.Type)
					{
						//Console.WriteLine("Code parsing for: {0}", ((typesys.Type)f.type).fullName());
						parser.parseFileCP(f, file);
					}
				}
				catch (Exception e)
				{
					Console.Error.WriteLine("ERROR: parsing file {0} failed: {1}", file, e.ToString());
				}
			}
		}

		private static string getGenTypePath(typesys.GenericType type)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Options.output);
			builder.Append(".gen");
			builder.Append(System.IO.Path.DirectorySeparatorChar);

			builder.Append(type.file.package.ToString().Replace('.', System.IO.Path.DirectorySeparatorChar));
			if (type.file.package != typesys.Package.GLOBAL)
			{
				builder.Append("/");
			}
			return builder.Append(type.name).Append(".boh").ToString();
		}

		private static void buildgcc(IMangler mangler, IEnumerable<bohc.typesys.Type> types)
		{
			if (!System.IO.Directory.Exists(Options.outputDir))
			{
				System.IO.Directory.CreateDirectory(Options.outputDir);
			}
			StringBuilder script = new StringBuilder();

            if (!Options.thisMachine || (Environment.Is64BitProcess && Options.thisMachine))
            {
				script.Append("-w -O3 -DPF_DESKTOP64 -m64 -DPF_LINUX -pthread -std=c11 -o ");
				script.Append(Options.outputDir + System.IO.Path.DirectorySeparatorChar + Options.output);
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
				if (Options.library)
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
				if (Options.library)
				{
					script.Append(" -fPIC -shared");
				}
				script.Append(" .c/boh_internal.c .c/function_types.c ");
				foreach (string s in Options.cfiles)
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
				foreach (string e in Options.externals)
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
            else if (!Options.thisMachine || (!Environment.Is64BitProcess && Options.thisMachine))
            {
				script.Append("-w -O3 -DPF_DESKTOP32 -m32 -DPF_LINUX -pthread -std=c11 -o ");
				script.Append(Options.outputDir + System.IO.Path.DirectorySeparatorChar + Options.output);
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
				if (Options.library)
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
				if (Options.library)
				{
					script.Append(" -fPIC -shared");
				}
				script.Append(" .c/boh_internal.c .c/function_types.c ");
                foreach (string s in Options.cfiles)
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
				foreach (string e in Options.externals)
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

			if (!Options.noDelete)
			{
				System.IO.Directory.Delete(".c", true);
			}

            if (Options.library)
            {
				new XDocument(new XElement("lib",
				                           types.Where(x => x.originalGenType == null).Select(getXElementForType)
				                           .Concat(typesys.GenericType.allGenTypes.Select(getXElementsForType))))
					.Save(Options.outputDir + System.IO.Path.DirectorySeparatorChar + Options.output + ".xml");
            }
		}

		private static XElement getXElementsForType(typesys.GenericType t)
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

		private static XElement getXElementForType(typesys.Type t)
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

		static IEnumerable<XElement> getFieldXElements(bohc.typesys.Class c)
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

		static IEnumerable<XElement> getFunctionsXElements(IEnumerable<typesys.Function> functions)
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

		static XElement[] getParamXElements(bohc.typesys.Function x)
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

		static string extname(string super)
		{
			return super.Replace("&lt;", "<").Replace("&gt;", ">");
		}

		private static void loadExtern(string ext)
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

			Parser parser = new Parser(new DefaultStatementParser(new DefaultExpressionParser()));

			var types = externTS(root, ext);
		
			externTcsTcp(parser, types);
		}

		static void externTcsTcp(Parser parser, List<bohc.typesys.Type> types)
		{
			foreach (typesys.Type t in types)
			{
				externTcsTcpClass(t, parser);
				externTcsTcpInterface(parser, t);
				externTcsTcpEnum(t);
			}
		}

		static void externTcsTcpClass(typesys.Type t, Parser parser)
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

		static void addFunctions(bohc.typesys.Type t, Parser parser, bohc.typesys.Class c)
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

		static void externTcsTcpEnum(bohc.typesys.Type t)
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

		static void externTcsTcpInterface(Parser parser, bohc.typesys.Type t)
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

		static List<typesys.Type> externTS(XElement root, string ext)
		{
			List<typesys.Type> types = new List<typesys.Type>();
			externClass(root, types);
			externStruct(root, types);
			externInterface(root, types);
			externEnum(root, types);
			externGenTypes(root, ext);
			return types;
		}

		static void externClass(XElement root, List<bohc.typesys.Type> types)
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

		static void externStruct(XElement root, List<bohc.typesys.Type> types)
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

		static void externInterface(XElement root, List<bohc.typesys.Type> types)
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

		static void externEnum(XElement root, List<bohc.typesys.Type> types)
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

		static void externGenTypes(XElement root, string ext)
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
