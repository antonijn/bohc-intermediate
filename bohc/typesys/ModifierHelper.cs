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
	public static class ModifierHelper
	{
		public static bool isAccessLegal(Modifiers mod, bool classmember)
		{
			if (mod.HasFlag(Modifiers.CVISIBLE) && mod.HasFlag(Modifiers.PUBLIC))
			{
				return false;
			}

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

			return true;
		}

		public static bool areModifiersLegalForType(Modifiers mod)
		{
			if (!isAccessLegal(mod, true))
			{
				return false;
			}

			return !(mod.HasFlag(Modifiers.OVERRIDE) || mod.HasFlag(Modifiers.VIRTUAL) ||
				     mod.HasFlag(Modifiers.NOCONTEXT) || mod.HasFlag(Modifiers.PROTECTED));
		}

		public static bool areModifiersLegal(Modifiers mod, bool classmember)
		{
			if (!isAccessLegal(mod, classmember))
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

			if (mod.HasFlag(Modifiers.NATIVE) && !mod.HasFlag(Modifiers.STATIC))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.NATIVE) && !Program.Options.desktopMode)
			{
				return false;
			}

			return true;
		}

		public static Modifiers getPfMods(Modifiers mods)
		{
			mods &= (Modifiers)~0x000004FF;
			mods &= ~Modifiers.NATIVE;
			return mods;
		}

		public static Modifiers getModifierFromString(string str)
		{
			Modifiers result;
			bool success = System.Enum.TryParse<Modifiers>(str.ToUpperInvariant(), out result);
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
			IEnumerable<string> split = str.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x));

			return getModifiersFromStrings(split);
		}
	}
}
