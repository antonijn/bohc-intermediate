using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace bohc.parsing
{
	public static class ParserTools
	{
		public static int indexOf(string str, char[] begin, char[] end, char idxOf)
		{
			bool instring = false;

			int scope = 0;
			int i = 0;
			for (; true; ++i)
			{
				if (i >= str.Length)
				{
					return -1;
				}

				char ch = str[i];
				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

				if (begin.Contains(ch))
				{
					++scope;
				}
				else if (end.Contains(ch))
				{
					--scope;
				}

				if (ch == idxOf && scope <= 0)
				{
					break;
				}
			}

			return i;
		}

		public static int indexOf(string str, char begin, char end, char idxOf)
		{
			return indexOf(str, new[] { begin }, new[] { end }, idxOf);
		}

		private static int getMatchingBraceCh(string str, int first, char matches, int step)
		{
			bool instring = false;
			char startscope = str[first];
			int scope = 0;
			int i;
			for (i = first + step; scope != -1; i += step)
			{
				char ch = str[i];
				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

				if (ch == startscope)
				{
					++scope;
				}
				else if (ch == matches)
				{
					--scope;
				}
			}

			return i - step;
		}

		public static int getMatchingBraceChar(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, 1);
		}

		public static int getMatchingBraceCharBackwards(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, -1);
		}

		public static IEnumerable<string> split(string str, char[] begin, char[] end, char seperator)
		{
			bool instring = false;
			int scope = 0;

			int temp = 0;
			for (int i = 0; scope >= 0 && i < str.Length; ++i)
			{
				char ch = str[i];

				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

				if (begin.Contains(ch))
				{
					++scope;
				}
				else if (end.Contains(ch))
				{
					--scope;
				}
				else if (scope == 0 && ch == seperator)
				{
					yield return str.Substring(temp, i - temp);
					temp = i + 1;
				}
			}

			yield return str.Substring(temp, str.Length - temp);
		}

		public static IEnumerable<string> split(string str, int first, char matches, char seperator)
		{
			char begin = str[first];
			int stop = getMatchingBraceChar(str, first, matches);
			return split(str.Substring(first + 1, stop - first - 1), new[] { begin }, new[] { matches }, seperator);
		}

		/// <summary>
		/// Removes duplicate whitespace in string
		/// </summary>
		public static string remDupW(string str)
		{
			return Regex.Replace(str, "\\s+", " ");
		}

		public static string removeComments(string file)
		{
			List<char> charList = file.ToList();
			bool inStr = false;
			int numSlash = 0;
			bool prevBackSl = false;
			for (int i = 0; i < charList.Count; ++i)
			{
				char ch = charList[i];
				if (ch == '/')
				{
					if (!inStr)
					{
						++numSlash;
						if (numSlash == 2)
						{
							int idxnl = charList.IndexOf('\n', i);
							int idxsl = i - 1;
							int len = idxnl - idxsl;
							charList.RemoveRange(idxsl, len);
							numSlash = 0;
						}
					}
				}
				else
				{
					if (numSlash == 1 && ch == '*' && !inStr)
					{
						int idx = idxOfCloseCom(charList, i + 2);
						int len = idx - i + 3;
						charList.RemoveRange(i - 1, len);
						//i = idx;
					}
					else if (ch == '\\')
					{
						prevBackSl = !prevBackSl;
					}
					else
					{
						if (ch == '"' && !prevBackSl)
						{
							inStr = !inStr;
						}
						prevBackSl = false;
					}
					numSlash = 0;
				}
			}
			file = new string(charList.ToArray());
			return file;
		}

		private static int idxOfCloseCom(List<char> chars, int start)
		{
			for (; start < chars.Count; ++start)
			{
				if (chars[start - 1] == '*' && chars[start] == '/')
				{
					return start - 1;
				}
			}

			return -1;
		}

		/*public static string prepareStringLineNums(string str)
		{
			StringBuilder builder = new StringBuilder(str.Length);
			int i = 0;
			foreach (char ch in str)
			{
				builder.Append(ch);
				if (ch == '\n')
				{
					builder.Append("[\0");
					builder.Append((++i).ToString());
					builder.Append(']');
				}
			}
			return builder.ToString();
		}

		public static string removeLineNums(string str)
		{
			return Regex.Replace(str, "\\[\0.*\\]", "");
		}*/
	}
}
