using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Bohc.General
{
	public class Platform
	{
		static Platform()
		{
			FieldInfo[] fields = typeof(Platform).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fi in fields.Where(x => x.Name.StartsWith("Pf_")))
			{
				platforms[(TypeSystem.Modifiers)Enum.Parse(typeof(TypeSystem.Modifiers), fi.Name)] = (Platform)fi.GetValue(null);
			}
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.MacOSX:
					compiler = Environment.Is64BitOperatingSystem ? Pf_Osx64 : Pf_Osx32;
					return;
				case PlatformID.Unix:
					compiler = Environment.Is64BitOperatingSystem ? Pf_Linux64 : Pf_Linux32;
					return;
				case PlatformID.Win32NT:
					compiler = Environment.Is64BitOperatingSystem ? Pf_Windows64 : Pf_Windows32;
					return;
			}
			throw new NotImplementedException();
		}

		private bool isPlatform(Platform other)
		{
			return this == other || plats.Any(x => x.isPlatform(other));
		}

		private readonly Platform[] plats;

		private Platform(params Platform[] plats)
		{
			this.plats = plats;
		}

		public static readonly Platform compiler;

		public static readonly Platform Pf_Desktop = new Platform();

		public static readonly Platform Pf_Windows = new Platform(Pf_Desktop);
		public static readonly Platform Pf_Linux = new Platform(Pf_Desktop);
		public static readonly Platform Pf_Osx = new Platform(Pf_Desktop);

		public static readonly Platform Pf_Android = new Platform();
		public static readonly Platform Pf_Ios = new Platform();

		public static readonly Platform Pf_Web = new Platform();

		public static readonly Platform Pf_Desktop64 = new Platform(Pf_Desktop);
		public static readonly Platform Pf_Desktop32 = new Platform(Pf_Desktop);

		public static readonly Platform Pf_Windows64 = new Platform(Pf_Desktop64, Pf_Windows);
		public static readonly Platform Pf_Linux64 = new Platform(Pf_Desktop64, Pf_Linux);
		public static readonly Platform Pf_Osx64 = new Platform(Pf_Desktop64, Pf_Osx);

		public static readonly Platform Pf_Windows32 = new Platform(Pf_Desktop32, Pf_Windows);
		public static readonly Platform Pf_Linux32 = new Platform(Pf_Desktop32, Pf_Linux);
		public static readonly Platform Pf_Osx32 = new Platform(Pf_Desktop32, Pf_Osx);

		public static readonly Dictionary<TypeSystem.Modifiers, Platform> platforms = new Dictionary<Bohc.TypeSystem.Modifiers, Platform>();

		public override string ToString()
		{
			return GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Last(x => x.GetValue(null) == this).Name;
		}

		public string triple()
		{
			StringBuilder sb = new StringBuilder();
			if (isPlatform(Pf_Desktop64))
			{
				sb.Append("x86_64-");
			}
			else
			{
				sb.Append("i686-");
			}

			if (isPlatform(Pf_Linux))
			{
				sb.Append("unknown-linux-gnu");
			}
			else if (isPlatform(Pf_Osx))
			{
				sb.Append("apple-macosx");
			}
			else if (isPlatform(Pf_Windows))
			{
				sb.Append("microsoft-windows");
			}

			return sb.ToString();
		}

		public TypeSystem.Primitive longType()
		{
			if (isPlatform(Pf_Desktop32))
			{
				return TypeSystem.Primitive.Int;
			}
			return TypeSystem.Primitive.Long;
		}

		public static bool IsPlatform(TypeSystem.Modifiers mf, Platform other)
		{
			TypeSystem.Modifiers pfm = TypeSystem.ModifierHelper.GetPfMods(mf);
			if (pfm == TypeSystem.Modifiers.None)
			{
				return true;
			}
			return pfm.ToString()
					.Split(new [] { ", " }, StringSplitOptions.None)
					.Select(x => (TypeSystem.Modifiers)Enum.Parse(typeof(TypeSystem.Modifiers), x, true))
					.Any(x => other.isPlatform(platforms[x]));
		}
	}
}

