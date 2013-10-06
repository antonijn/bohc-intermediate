using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public static class ModifierHelper
	{
		public static bool areModifiersLegal(Modifiers mod, bool classmember)
		{
			int numAccess = 0;
			if (mod.HasFlag(Modifiers.PUBLIC))
			{
				++numAccess;
			}
			if (mod.HasFlag(Modifiers.PROTECTED))
			{
				++numAccess;
			}
			if (mod.HasFlag(Modifiers.PRIVATE))
			{
				++numAccess;
			}

			if (numAccess > 1)
			{
				return false;
			}

			if (classmember && numAccess == 0)
			{
				return false;
			}

			if (mod.HasFlag(Modifiers.ABSTRACT) && mod.HasFlag(Modifiers.FINAL))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.VIRTUAL) && mod.HasFlag(Modifiers.FINAL))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.ABSTRACT) && mod.HasFlag(Modifiers.VIRTUAL))
			{
				return false;
			}

			if (mod.HasFlag(Modifiers.OVERRIDE) && mod.HasFlag(Modifiers.VIRTUAL))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.ABSTRACT) && mod.HasFlag(Modifiers.OVERRIDE))
			{
				return false;
			}

			return true;
		}

		public static Modifiers getModifierFromString(string str)
		{
			Modifiers result;
			bool success = Enum.TryParse<Modifiers>(str.ToUpperInvariant(), out result);
			boh.Exception.require<exceptions.ParserException>(success, str + ": invalid modifier");
			return result;
		}

		public static Modifiers getModifiersFromStrings(IEnumerable<string> strings)
		{
			Modifiers mods = Modifiers.NONE;

			foreach (string s in strings)
			{
				Modifiers mod = getModifierFromString(s);
				boh.Exception.require<exceptions.ParserException>(!mods.HasFlag(mod), s + ": duplicate modifier");
				mods |= mod;
			}

			return mods;
		}

		public static Modifiers getModifiersFromString(string str)
		{
			Modifiers mods = Modifiers.NONE;
			IEnumerable<string> split = str.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x));

			return getModifiersFromStrings(split);
		}
	}
}
