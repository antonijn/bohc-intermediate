﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public interface IMember
	{
		Modifiers getModifiers();
		string getName();
	}
}
