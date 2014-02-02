using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Boh;
using Bohc.Exceptions;
using Bohc.Parsing;
using Bohc.Parsing.Statements;
using Bohc.TypeSystem;
using Bohc.Generation.Mangling;

namespace Bohc.Generation.Llvm
{
	public class Llvm
	{
		public Llvm(LlvmFunction func)
		{
			this.func = func;
			tempvc.Push(new int[1]);
		}

		public static Stack<int[]> tempvc = new Stack<int[]>();

		public readonly LlvmFunction func;
		private StringBuilder builder = new StringBuilder();

		private StringBuilder AddIndent()
		{
			return builder.Append("\t");
		}

		public override string ToString()
		{
			return builder.ToString();
		}

		private LlvmValue AddBinOpRes(string op, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(left.Type());
			AddIndent().Append(result.ToString()).Append(" = ").Append(op).Append(" ")
				.Append(left.Type().ToString()).Append(" ")
					.Append(left.ToString()).Append(", ").AppendLine(right.ToString());
			return result;
		}

		public LlvmValue AddFadd(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("fadd", left, right);
		}

		public LlvmValue AddFsub(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("fsub", left, right);
		}

		public LlvmValue AddFmul(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("fmul", left, right);
		}

		public LlvmValue AddFdiv(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("fdiv", left, right);
		}

		public LlvmValue AddFrem(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("frem", left, right);
		}

		public LlvmValue AddAdd(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("add", left, right);
		}

		public LlvmValue AddSub(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("sub", left, right);
		}

		public LlvmValue AddMul(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("mul", left, right);
		}

		public LlvmValue AddSdiv(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("sdiv", left, right);
		}

		public LlvmValue AddUdiv(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("udiv", left, right);
		}

		public LlvmValue AddSrem(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("srem", left, right);
		}

		public LlvmValue AddUrem(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("urem", left, right);
		}

		public LlvmValue AddShl(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("shl", left, right);
		}

		public LlvmValue AddLshr(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("lshr", left, right);
		}

		public LlvmValue AddAshr(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("ashr", left, right);
		}

		public LlvmValue AddAnd(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("and", left, right);
		}

		public LlvmValue AddOr(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("or", left, right);
		}

		public LlvmValue AddXor(LlvmValue left, LlvmValue right)
		{
			return AddBinOpRes("xor", left, right);
		}

		public void AddAlloca(LlvmValue result, LlvmType type)
		{
			AddIndent().Append(result.ToString()).Append(" = alloca ").AppendLine(type.ToString());
		}

		public LlvmValue AddLoad(LlvmValue val)
		{
			LlvmPointer ptr = (LlvmPointer)val.Type();
			LlvmTemp result = new LlvmTemp(ptr.t);
			AddIndent().Append(result.ToString()).Append(" = load ").Append(ptr.ToString()).Append(" ").AppendLine(val.ToString());
			return result;
		}

		public void AddStore(LlvmValue ptr, LlvmValue rvalue)
		{
			AddIndent().Append("store ")
				.Append(rvalue.Type()).Append(" ").Append(rvalue.ToString()).Append(", ")
					.Append(ptr.Type().ToString()).Append(" ").AppendLine(ptr.ToString());
		}

		public LlvmValue AddCall(LlvmValue f, IEnumerable<LlvmValue> parameters)
		{
			LlvmFunctionPtrType function = (LlvmFunctionPtrType)f.Type();
			LlvmTemp result = function.ret.ToString() == "void" ? null : new LlvmTemp(function.ret);
			AddIndent();
			if (function.ret.ToString() != "void")
			{
				builder.Append(result.ToString()).Append(" = call ");
			}
			else
			{
				builder.Append("call ");
			}
			builder.Append(function.ret.ToString())
				.Append(" ").Append(f.ToString())
					.Append("(");
			foreach (LlvmValue val in parameters)
			{
				builder.Append(val.Type().ToString()).Append(" ").Append(val.ToString()).Append(", ");
			}
			if (parameters.Count() > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			builder.AppendLine(")");
			return result;
		}

		public LlvmValue AddCall(LlvmFunction function, IEnumerable<LlvmValue> parameters)
		{
			LlvmTemp result = function.ret.ToString() == "void" ? null : new LlvmTemp(function.ret);
			AddIndent();
			if (function.ret.ToString() != "void")
			{
				builder.Append(result.ToString()).Append(" = call ");
			}
			else
			{
				builder.Append("call ");
			}
			builder.Append(function.ret.ToString())
				.Append(" ").Append(function.ToString())
				.Append("(");
			foreach (LlvmValue val in parameters)
			{
				builder.Append(val.Type().ToString()).Append(" ").Append(val.ToString()).Append(", ");
			}
			if (parameters.Count() > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			builder.AppendLine(")");
			return result;
		}

		private LlvmLabel lbl;

		public LlvmLabel GetLabel()
		{
			return lbl;
		}

		public void AddLabel(LlvmLabel label)
		{
			lbl = label;
			builder.Append(label.id).AppendLine(":");
		}

		public void AddBranch(LlvmValue cond, LlvmLabel t, LlvmLabel f)
		{
			AddIndent().Append("br ").Append(cond.Type().ToString()).Append(" ").Append(cond.ToString())
				.Append(", label ").Append(t.ToString())
					.Append(", label ").AppendLine(f.ToString());
		}

		public void AddBranch(LlvmLabel label)
		{
			AddIndent().Append("br label ").AppendLine(label.ToString());
		}

		public LlvmValue AddIcmp(Icmp cmp, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(new LlvmPrimitive("i1"));
			AddIndent().Append(result.ToString()).Append(" = icmp ")
				.Append(cmp.ToString().ToLowerInvariant()).Append(" ").Append(left.Type().ToString()).Append(" ")
					.Append(left.ToString()).Append(", ").AppendLine(right.ToString());
			return result;
		}

		public LlvmValue AddFcmp(Fcmp cmp, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(new LlvmPrimitive("i1"));
			AddIndent().Append(result.ToString()).Append(" = fcmp ")
				.Append(cmp.ToString().ToLowerInvariant()).Append(" ").Append(left.Type().ToString()).Append(" ")
					.Append(left.ToString()).Append(", ").AppendLine(right.ToString());
			return result;
		}

		public LlvmValue InlinePtrToInt(LlvmValue ui, LlvmType to)
		{
			StringBuilder sb = new StringBuilder();
			return new LlvmInline(to, sb.Append("ptrtoint ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString())
			                      .Append(" to ").AppendLine(to.ToString()).ToString());
		}

		public LlvmValue InlineIntToPtr(LlvmValue ui, LlvmType to)
		{
			StringBuilder sb = new StringBuilder();
			return new LlvmInline(to, sb.Append("inttoptr (")
			                      .Append(ui.Type().ToString()).Append(" ").Append(ui.ToString())
			                      .Append(") to ").AppendLine(to.ToString()).ToString());
		}

		public LlvmValue InlineBitcast(LlvmValue ui, LlvmType to)
		{
			StringBuilder sb = new StringBuilder();
			return new LlvmInline(to, sb.Append("bitcast (")
			                      .Append(ui.Type().ToString()).Append(" ").Append(ui.ToString())
			                      .Append(") to ").Append(to.ToString()).ToString());
		}

		public LlvmValue AddPtrToInt(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = ptrtoint ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddIntToPtr(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = inttoptr ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddUiToFp(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = uitofp ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddSiToFp(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = sitofp ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddFpToUi(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = fptoui ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddFpToSi(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = fptosi ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddZext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = zext ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddSext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = sext ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddTrunc(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = trunc ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddFext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = fext ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddFtrunc(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = ftrunc ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddBitcast(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result.ToString()).Append(" = bitcast ")
				.Append(ui.Type().ToString()).Append(" ").Append(ui.ToString()).Append(" to ").AppendLine(to.ToString());
			return result;
		}

		public LlvmValue AddPhi(LlvmType ty, params Tuple<LlvmValue, LlvmLabel>[] pairs)
		{
			LlvmTemp result = new LlvmTemp(ty);
			AddIndent().Append(result.ToString()).Append(" = phi ")
				.Append(result.Type().ToString()).Append(" ");
			foreach (Tuple<LlvmValue, LlvmLabel> pair in pairs)
			{
				builder.Append("[ ").Append(pair.Item1.ToString()).Append(", ").Append(pair.Item2.ToString()).Append(" ], ");
			}
			if (pairs.Length > 0)
			{
				builder.Remove(builder.Length - 2, 2);
			}
			builder.AppendLine();
			return result;
		}

		public LlvmValue InlineGetElementPtr(LlvmValue value, params LlvmValue[] indices)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(", ");

			LlvmType t = value.Type();
			foreach (LlvmValue idx in indices)
			{
				if (t is LlvmParamType)
				{
					t = ((LlvmParamType)t).t;
				}

				if (t is LlvmPointer)
				{
					t = ((LlvmPointer)t).t;
				}
				else if (t is LlvmArrayType)
				{
					t = ((LlvmArrayType)t).type;
				}
				else
				{
					LlvmStruct str = (LlvmStruct)t;
					t = str.members.Values.ToArray()[int.Parse(((LlvmLiteral)idx).ToString())];
				}
				sb.Append(idx.Type().ToString()).Append(" ").Append(idx.ToString()).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			StringBuilder other = new StringBuilder();
			return new LlvmInline(new LlvmPointer(t), other.Append("getelementptr inbounds (")
				.Append(value.Type().ToString()).Append(" ")
					.Append(value.ToString()).Append(sb.ToString()).Append(")").ToString());
		}

		public LlvmValue AddGetElementPtr(LlvmValue value, params LlvmValue[] indices)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(", ");

			LlvmType t = value.Type();
			foreach (LlvmValue idx in indices)
			{
				while (t is LlvmParamType)
				{
					t = ((LlvmParamType)t).t;
				}

				if (t is LlvmPointer)
				{
					t = ((LlvmPointer)t).t;
				}
				else
				{
					LlvmStruct str = (LlvmStruct)t;
					t = str.members.Values.ToArray()[int.Parse(((LlvmLiteral)idx).ToString())];
				}
				sb.Append(idx.Type().ToString()).Append(" ").Append(idx.ToString()).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			LlvmTemp result = new LlvmTemp(new LlvmPointer(t));
			AddIndent().Append(result.ToString()).Append(" = getelementptr inbounds ")
				.Append(value.Type().ToString()).Append(" ").Append(value.ToString()).AppendLine(sb.ToString());
			return result;
		}

		public void AddSwitch(LlvmValue value, LlvmLabel def, params Tuple<LlvmValue, LlvmLabel>[] cases)
		{
			AddIndent().Append("switch ").Append(value.Type().ToString()).Append(" ").Append(value.ToString())
				.Append(", label ").Append(def.ToString()).Append(" [ ");
			foreach (var c in cases)
			{
				builder.Append(c.Item1.Type().ToString()).Append(" ").Append(c.Item1.ToString())
					.Append(", label ").Append(def.ToString()).Append(" ");
			}
			builder.AppendLine("]");
		}

		public void AddRet(LlvmValue val)
		{
			AddIndent().Append("ret ").Append(val.Type().ToString()).Append(" ").AppendLine(val.ToString());
		}

		public void AddRetVoid()
		{
			AddIndent().AppendLine("ret void");
		}

		public void AddLandingPad(LlvmValue result, bool cleanup)
		{
			AddIndent().Append(result.ToString()).Append(" = landingpad ")
				.Append(result.Type().ToString()).AppendLine(" personality i8* null");
			AddIndent();
			AddIndent().AppendLine("catch i8* null");
		}

		public void AddUnreachable()
		{
			AddIndent().AppendLine("unreachable");
		}
	}
}

