using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmParamType : LlvmType
	{
		public readonly LlvmParamModifiers modifiers;
		public LlvmType t;

		private LlvmParamType(LlvmParamModifiers modifiers, LlvmType type)
		{
			this.modifiers = modifiers;
			this.t = type;
		}

		public LlvmParamType(LlvmType type)
		{
			this.t = type;
			this.modifiers = LlvmParamModifiers.None;
			string tstr = type.ToString();
			if (tstr == "i8")
			{
				this.modifiers = LlvmParamModifiers.ZeroExt;
			}
			else if (tstr == "i16")
			{
				this.modifiers = LlvmParamModifiers.SignExt;
			}
			else if (tstr == "i1")
			{
				this.modifiers = LlvmParamModifiers.ZeroExt;
			}
			else if (tstr == "i8")
			{
				this.modifiers = LlvmParamModifiers.ZeroExt;
			}
		}

		public override string ToString()
		{
			return t.ToString() + (modifiers == LlvmParamModifiers.None ? "" : 
				(" " + modifiers.ToString().ToLowerInvariant()));
		}
	}
}

