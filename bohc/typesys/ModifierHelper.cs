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
	public static class ModifierHelper
	{
		public static bool IsAccessLegal(Modifiers mod, bool classmember)
		{
			if (mod.HasFlag(Modifiers.CVisible) && mod.HasFlag(Modifiers.Public))
			{
				return false;
			}

			int numAccess = 0;
			if (mod.HasFlag(Modifiers.Public))
			{
				++numAccess;
			}
			if (mod.HasFlag(Modifiers.Protected))
			{
				++numAccess;
			}
			if (mod.HasFlag(Modifiers.Private))
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

		public static bool AreModifiersLegalForType(Modifiers mod)
		{
			if (!IsAccessLegal(mod, true))
			{
				return false;
			}

			return !(mod.HasFlag(Modifiers.Override) || mod.HasFlag(Modifiers.Virtual) ||
				     mod.HasFlag(Modifiers.NoContext) || mod.HasFlag(Modifiers.Protected));
		}

		public static bool AreModifiersLegal(Modifiers mod, bool classmember, Bohc.General.Project ctx)
		{
			if (!IsAccessLegal(mod, classmember))
			{
				return false;
			}

			if (mod.HasFlag(Modifiers.Abstract) && mod.HasFlag(Modifiers.Final))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.Virtual) && mod.HasFlag(Modifiers.Final))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.Abstract) && mod.HasFlag(Modifiers.Virtual))
			{
				return false;
			}

			if (mod.HasFlag(Modifiers.Override) && mod.HasFlag(Modifiers.Virtual))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.Abstract) && mod.HasFlag(Modifiers.Override))
			{
				return false;
			}

			if (mod.HasFlag(Modifiers.Native) && !mod.HasFlag(Modifiers.Static))
			{
				return false;
			}
			if (mod.HasFlag(Modifiers.Native) && !ctx.desktopMode)
			{
				return false;
			}

			return true;
		}

		public static Modifiers GetPfMods(Modifiers mods)
		{
			mods &= (Modifiers)~0x000004FF;
			mods &= ~Modifiers.Native;
			return mods;
		}

		public static Modifiers GetModifierFromString(string str)
		{
			Modifiers result;
			bool success = System.Enum.TryParse<Modifiers>(str, true, out result);
			Boh.Exception.require<Exceptions.ParserException>(success, str + ": invalid modifier");
			return result;
		}

		public static Modifiers GetModifiersFromStrings(IEnumerable<string> strings)
		{
			Modifiers mods = Modifiers.None;

			foreach (string s in strings)
			{
				Modifiers mod = GetModifierFromString(s);
				Boh.Exception.require<Exceptions.ParserException>(!mods.HasFlag(mod), s + ": duplicate modifier");
				mods |= mod;
			}

			return mods;
		}

		public static Modifiers GetModifiersFromString(string str)
		{
			if (str == null)
			{
				return Modifiers.None;
			}
			IEnumerable<string> split = str.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x) && !string.IsNullOrEmpty(x));

			return GetModifiersFromStrings(split);
		}
	}
}
