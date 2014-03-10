using System;
using System.Collections.Generic;
using Bohc.General;

namespace Bohc.Parsing
{
	public class TokenRange
	{
		public readonly Token first;
		public readonly Token last;
		public readonly string str;

		public TokenRange(Token first, Token last, string str)
		{
			this.first = first;
			this.last = last;
			this.str = str;
		}

		public void error(ErrorManager e, string format, params object[] args)
		{
			++e.errors;
			display("ERROR", string.Format(format, args), ConsoleColor.Red);
		}

		public void warning(ErrorManager e, string format, params object[] args)
		{
			++e.warnings;
			display("warning", string.Format(format, args), ConsoleColor.DarkYellow);
		}

		public void hint(ErrorManager e, string format, params object[] args)
		{
			display("hint", string.Format(format, args), ConsoleColor.DarkCyan);
		}

		public void display(string msg, string msg2, ConsoleColor cc)
		{
			Stack<ConsoleColor> colors = new Stack<ConsoleColor>();
			colors.Push(Console.ForegroundColor);

			Console.Write("\x1b[1m{0}:{1}:{2}: ", first.filename, first.linenum, first.column);

			colors.Push(Console.ForegroundColor = cc);
			Console.Write("{0}:", msg);
			colors.Pop();
			Console.ForegroundColor = colors.Peek();

			Console.WriteLine(" {0}\x1b[0m", msg2);
			//Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(first.line);
			for (int i = 0; i < first.column - 1; ++i)
			{
				if (first.line[i] == '\t')
				{
					Console.Write("\t");
				}
				else
				{
					Console.Write(" ");
				}
			}
			//Console.Write("^");
			colors.Push(Console.ForegroundColor = cc);
			for (int i = 0; i < last.column + last.value.Length - first.column; ++i)
			{
				Console.Write("^");
			}
			colors.Pop();
			Console.ForegroundColor = colors.Peek();
			Console.WriteLine();
		}
	}
}

