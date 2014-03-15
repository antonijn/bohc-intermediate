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
			tempvc.Push(new int[1] { 1 });
		}

		public static Stack<int[]> tempvc = new Stack<int[]>();

		public readonly LlvmFunction func;
		private LlvmBuilder builder = new LlvmBuilder();

		private LlvmBuilder AddIndent()
		{
			return builder.Append("\t");
		}

		public override string ToString()
		{
			return builder.ToString();
		}

		public void AddComment(string com)
		{
			builder.Append("; ").AppendLine(com);
		}

		private LlvmValue AddBinOpRes(string op, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(left.Type());
			AddIndent().Append(result).Append(" = ").Append(op).Append(" ")
				.Append(left.Type()).Append(" ")
					.Append(left).Append(", ").AppendLine(right);
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
			AddIndent().Append(result).Append(" = alloca ").AppendLine(type);
		}

		public LlvmValue AddLoad(LlvmValue val)
		{
			LlvmPointer ptr = (LlvmPointer)val.Type();
			LlvmTemp result = new LlvmTemp(ptr.t);
			AddIndent().Append(result).Append(" = load ").Append(ptr).Append(" ").AppendLine(val);
			return result;
		}

		public void AddStore(LlvmValue ptr, LlvmValue rvalue)
		{
			AddIndent().Append("store ")
				.Append(rvalue.Type()).Append(" ").Append(rvalue).Append(", ")
					.Append(ptr.Type()).Append(" ").AppendLine(ptr);
		}

		public LlvmValue AddCall(LlvmValue f, IEnumerable<LlvmValue> parameters)
		{
			LlvmFunctionPtrType function = (LlvmFunctionPtrType)f.Type();
			LlvmTemp result = function.ret.ToString() == "void" ? null : new LlvmTemp(function.ret);
			AddIndent();
			if (function.ret.ToString() != "void")
			{
				builder.Append(result).Append(" = call ");
			}
			else
			{
				builder.Append("call ");
			}
			builder.Append(function)
				.Append(" ").Append(f)
					.Append("(");
			foreach (LlvmValue val in parameters)
			{
				builder.Append(val.Type()).Append(" ").Append(val).Append(", ");
			}
			if (parameters.Count() > 0)
			{
				builder.RemoveLast();
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
				builder.Append(result).Append(" = call ");
			}
			else
			{
				builder.Append("call ");
			}
			builder.Append(function.ret)
				.Append(" ").Append(function)
				.Append("(");
			foreach (LlvmValue val in parameters)
			{
				builder.Append(val.Type()).Append(" ").Append(val).Append(", ");
			}
			if (parameters.Count() > 0)
			{
				builder.RemoveLast();
			}
			builder.AppendLine(")");
			return result;
		}

		private LlvmLabel lbl = new LlvmLabel();

		private void Terminator(LlvmLabel l)
		{
			l.id = tempvc.Peek()[0]++.ToString();
			lbl = l;

			builder.AppendLine();
			AddComment("<label>:" + l.id);
		}

		public LlvmLabel GetLabel()
		{
			return lbl;
		}

		public void AddBranch(LlvmValue cond, LlvmLabel after, LlvmLabel t, LlvmLabel f)
		{
			t.preds.Add(GetLabel());
			f.preds.Add(GetLabel());
			AddIndent().Append("br ").Append(cond.Type()).Append(" ").Append(cond)
				.Append(", label ").Append(t)
					.Append(", label ").AppendLine(f);
			Terminator(after);
		}

		public void AddBranch(LlvmLabel label, LlvmLabel after)
		{
			label.preds.Add(GetLabel());
			AddIndent().Append("br label ").AppendLine(label);
			Terminator(after);
		}

		public LlvmValue AddIcmp(Icmp cmp, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(new LlvmPrimitive("i1"));
			AddIndent().Append(result).Append(" = icmp ")
				.Append(cmp.ToString().ToLowerInvariant()).Append(" ").Append(left.Type()).Append(" ")
					.Append(left).Append(", ").AppendLine(right);
			return result;
		}

		public LlvmValue AddFcmp(Fcmp cmp, LlvmValue left, LlvmValue right)
		{
			LlvmTemp result = new LlvmTemp(new LlvmPrimitive("i1"));
			AddIndent().Append(result).Append(" = fcmp ")
				.Append(cmp.ToString().ToLowerInvariant()).Append(" ").Append(left.Type()).Append(" ")
					.Append(left).Append(", ").AppendLine(right);
			return result;
		}

		public LlvmValue InlinePtrToInt(LlvmValue ui, LlvmType to)
		{
			LlvmBuilder sb = new LlvmBuilder();
			return new LlvmInline(to, sb.Append("ptrtoint ")
				.Append(ui.Type()).Append(" ").Append(ui)
			                      .Append(" to ").AppendLine(to));
		}

		public LlvmValue InlineIntToPtr(LlvmValue ui, LlvmType to)
		{
			LlvmBuilder sb = new LlvmBuilder();
			return new LlvmInline(to, sb.Append("inttoptr (")
			                      .Append(ui.Type()).Append(" ").Append(ui)
			                      .Append(") to ").AppendLine(to));
		}

		public LlvmValue InlineBitcast(LlvmValue ui, LlvmType to)
		{
			LlvmBuilder sb = new LlvmBuilder();
			return new LlvmInline(to, sb.Append("bitcast (")
			                      .Append(ui.Type()).Append(" ").Append(ui)
			                      .Append(") to ").Append(to));
		}

		public LlvmValue AddPtrToInt(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = ptrtoint ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddIntToPtr(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = inttoptr ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddUiToFp(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = uitofp ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddSiToFp(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = sitofp ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddFpToUi(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = fptoui ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddFpToSi(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = fptosi ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddZext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = zext ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddSext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = sext ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddTrunc(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = trunc ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddFext(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = fext ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddFtrunc(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = ftrunc ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddBitcast(LlvmValue ui, LlvmType to)
		{
			LlvmTemp result = new LlvmTemp(to);
			AddIndent().Append(result).Append(" = bitcast ")
				.Append(ui.Type()).Append(" ").Append(ui).Append(" to ").AppendLine(to);
			return result;
		}

		public LlvmValue AddPhi(LlvmType ty, params Tuple<LlvmValue, LlvmLabel>[] pairs)
		{
			LlvmTemp result = new LlvmTemp(ty);
			AddIndent().Append(result).Append(" = phi ")
				.Append(result.Type()).Append(" ");
			foreach (Tuple<LlvmValue, LlvmLabel> pair in pairs)
			{
				builder.Append("[ ").Append(pair.Item1).Append(", ").Append(pair.Item2).Append(" ]").Append(", ");
			}
			if (pairs.Length > 0)
			{
				builder.RemoveLast();
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
				sb.Append(idx.Type()).Append(" ").Append(idx).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			LlvmBuilder other = new LlvmBuilder();
			return new LlvmInline(new LlvmPointer(t), other.Append("getelementptr inbounds (")
				.Append(value.Type()).Append(" ")
					.Append(value).Append(sb).Append(")"));
		}

		public LlvmValue AddGetElementPtr(LlvmValue value, params int[] indices)
		{
			return AddGetElementPtr(value, indices.Select(x => new LlvmLiteral(new LlvmPrimitive("i32"), x.ToString())).ToArray());
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
				else if (t is LlvmStruct)
				{
					LlvmStruct str = (LlvmStruct)t;
					t = str.members.Values.ToArray()[int.Parse(((LlvmLiteral)idx).ToString())];
				}
				else if (t is LlvmInlineStruct)
				{
					LlvmInlineStruct str = (LlvmInlineStruct)t;
					t = str.members.Values.ToArray()[int.Parse(((LlvmLiteral)idx).ToString())];
				}

				sb.Append(idx.Type()).Append(" ").Append(idx).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			LlvmTemp result = new LlvmTemp(new LlvmPointer(t));
			AddIndent().Append(result).Append(" = getelementptr inbounds ")
				.Append(value.Type()).Append(" ").Append(value).AppendLine(sb);
			return result;
		}

		public LlvmValue AddExtractValue(LlvmValue value, params int[] indices)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(", ");

			LlvmType t = value.Type();
			foreach (int idx in indices)
			{
				while (t is LlvmParamType)
				{
					t = ((LlvmParamType)t).t;
				}

				if (t is LlvmPointer)
				{
					t = ((LlvmPointer)t).t;
				}
				else if (t is LlvmStruct)
				{
					LlvmStruct str = (LlvmStruct)t;
					t = str.members.Values.ToArray()[idx];
				}
				else if (t is LlvmInlineStruct)
				{
					LlvmInlineStruct str = (LlvmInlineStruct)t;
					t = str.members.Values.ToArray()[idx];
				}

				sb.Append(idx).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			LlvmTemp result = new LlvmTemp(t);
			AddIndent().Append(result).Append(" = extractvalue ")
				.Append(value.Type()).Append(" ").Append(value).AppendLine(sb);
			return result;
		}

		public LlvmValue AddInsertValue(LlvmValue value, LlvmValue set, params int[] indices)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(", ");

			sb.Append(set.Type().ToString()).Append(" ").Append(set.ToString()).Append(", ");

			LlvmType t = value.Type();
			foreach (int idx in indices)
			{
				while (t is LlvmParamType)
				{
					t = ((LlvmParamType)t).t;
				}

				if (t is LlvmPointer)
				{
					t = ((LlvmPointer)t).t;
				}
				else if (t is LlvmStruct)
				{
					LlvmStruct str = (LlvmStruct)t;
					t = str.members.Values.ToArray()[idx];
				}
				else if (t is LlvmInlineStruct)
				{
					LlvmInlineStruct str = (LlvmInlineStruct)t;
					t = str.members.Values.ToArray()[idx];
				}

				sb.Append(idx).Append(", ");
			}
			sb.Remove(sb.Length - 2, 2);

			LlvmTemp result = new LlvmTemp(value.Type());
			AddIndent().Append(result).Append(" = insertvalue ")
				.Append(value.Type()).Append(" ").Append(value).AppendLine(sb);
			return result;
		}

		public void AddSwitch(LlvmValue value, LlvmLabel def, params Tuple<LlvmValue, LlvmLabel>[] cases)
		{
			AddIndent().Append("switch ").Append(value.Type()).Append(" ").Append(value)
				.Append(", label ").Append(def).Append(" [ ");
			foreach (var c in cases)
			{
				builder.Append(c.Item1.Type()).Append(" ").Append(c.Item1)
					.Append(", label ").Append(def).Append(" ");
			}
			builder.AppendLine("]");
			Terminator(new LlvmLabel());
		}

		public void AddRet(LlvmValue val)
		{
			AddIndent().Append("ret ").Append(val.Type()).Append(" ").AppendLine(val);
			Terminator(new LlvmLabel());
		}

		public void AddRetVoid()
		{
			AddRetVoid(new LlvmLabel());
		}

		public void AddRetVoid(LlvmLabel l)
		{
			AddIndent().AppendLine("ret void");
			Terminator(l);
		}

		public void AddLandingPad(LlvmValue result, bool cleanup)
		{
			AddIndent().Append(result).Append(" = landingpad ")
				.Append(result.Type()).AppendLine(" personality i8* null");
			AddIndent();
			AddIndent().AppendLine("catch i8* null");
		}

		public void AddUnreachable(LlvmLabel nxt)
		{
			AddIndent().AppendLine("unreachable");
			Terminator(nxt);
		}
	}
}

