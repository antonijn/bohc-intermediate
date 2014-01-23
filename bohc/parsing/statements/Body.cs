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

namespace Bohc.Parsing.Statements
{
	public class Body
	{
		public readonly List<Statement> Statements = new List<Statement>();

		public bool hasSuperBeenCalled()
		{
			foreach (Statement s in Statements)
			{
				ExpressionStatement expr = s as ExpressionStatement;
				if (expr != null)
				{
					FunctionCall f = expr.expression as FunctionCall;
					if (f != null)
					{
						if (f.refersto.Identifier == "this")
						{
							return true;
						}
					}
				}

				IfStatement ifstat = s as IfStatement;
				if (ifstat != null)
				{
					if (ifstat.elsestat != null)
					{
						return ifstat.body.hasSuperBeenCalled() && ifstat.elsestat.body.hasSuperBeenCalled();
					}

					continue;
				}

				TryStatement trys = s as TryStatement;
				if (trys != null)
				{
					if (tryReturnsOrSuper(trys, x => x.hasSuperBeenCalled()))
					{
						return true;
					}

					continue;
				}

				if (s is BreakStatement || s is ContinueStatement)
				{
					return false;
				}
			}

			return false;
		}

		public bool hasReturned()
		{
			foreach (Statement s in Statements)
			{
				if (s is ReturnStatement || s is ThrowStatement)
				{
					return true;
				}

				IfStatement ifstat = s as IfStatement;
				if (ifstat != null)
				{
					if (ifstat.elsestat != null)
					{
						return ifstat.body.hasReturned() && ifstat.elsestat.body.hasReturned();
					}

					continue;
				}

				TryStatement trys = s as TryStatement;
				if (trys != null)
				{
					if (tryReturnsOrSuper(trys, x => x.hasReturned()))
					{
						return true;
					}

					continue;
				}

				if (s is BreakStatement || s is ContinueStatement)
				{
					return false;
				}
			}

			return false;
		}

		private bool tryReturnsOrSuper(TryStatement trys, Func<Body, bool> bodyAct)
		{
			if (!bodyAct(trys.body))
			{
				return false;
			}

			foreach (CatchStatement cs in trys.catches)
			{
				if (!bodyAct(cs.body))
				{
					return false;
				}
			}

			return true;
		}
	}
}
