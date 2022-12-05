#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;
using BitSet = Decompiler.Core.Lib.BitSet;
using CallRewriter = Decompiler.Core.CallRewriter;
using FpuStackStorage = Decompiler.Core.FpuStackStorage;
using Frame = Decompiler.Core.Frame;
using Identifier = Decompiler.Core.Expressions.Identifier;
using OutArgumentStorage = Decompiler.Core.OutArgumentStorage;
using PrimtiveType = Decompiler.Core.Types.PrimitiveType;
using Procedure = Decompiler.Core.Procedure;
using Program = Decompiler.Core.Program;
using RegisterStorage = Decompiler.Core.RegisterStorage;
using ReturnInstruction = Decompiler.Core.Code.ReturnInstruction;
using SignatureBuilder = Decompiler.Core.SignatureBuilder;
using StackArgumentStorage= Decompiler.Core.StackArgumentStorage;
using Statement = Decompiler.Core.Statement;
using UseInstruction = Decompiler.Core.Code.UseInstruction;

namespace Decompiler.Analysis
{
	/// <summary>
	/// Rewrites a program, based on summary live-in and live-out information, so that all
	/// CALL codes are converted into procedure calls, with the appropriate parameter lists.
	/// </summary>
	/// <remarks>
	/// Call Rewriting should take place before SSA conversion and dead code removal.
	/// </remarks>
	public class GlobalCallRewriter : CallRewriter
	{
		private ProgramDataFlow mpprocflow;

		public GlobalCallRewriter(Program prog, ProgramDataFlow mpprocflow) : base(prog)
		{
			this.mpprocflow = mpprocflow;
		}

		public void AddStackArgument(int x, Identifier id, ProcedureFlow flow, SignatureBuilder sb)
		{
			object o = flow.StackArguments[id];
			if (o != null)
			{
				int bitWidth = (int) o;
				if (bitWidth < id.DataType.BitSize)
				{
					PrimtiveType pt = id.DataType as PrimtiveType;
					if (pt != null)
					{
						id.DataType = PrimtiveType.Create(pt.Domain, bitWidth/8);
					}
				}
			}
			sb.AddStackArgument(x, id);
		}

		public void AddUseInstructionsForOutArguments(Procedure proc)
		{
			foreach (Identifier id in proc.Signature.Parameters)
			{
				var os = id.Storage as OutArgumentStorage;
				if (os == null)
					continue;
				var r = os.OriginalIdentifier.Storage as RegisterStorage;
				if (r == null)
					continue;

				proc.ExitBlock.Statements.Add(0, new UseInstruction(os.OriginalIdentifier, id));
			}
		}

		/// <summary>
		/// Adjusts LiveOut values for use as out registers.
		/// </summary>
		/// <remarks>
		/// LiveOut sets contain registers that aren't modified by the procedure. When determining
		/// the returned registers, those unmodified registers must be filtered away.
		/// </remarks>
		/// <param name="flow"></param>
		private void AdjustLiveOut(ProcedureFlow flow)
		{
			flow.grfLiveOut &= flow.grfTrashed;
			flow.LiveOut &= flow.TrashedRegisters;
		}

		public static void Rewrite(Program prog, ProgramDataFlow summaries)
		{
			GlobalCallRewriter crw = new GlobalCallRewriter(prog, summaries);
			foreach (Procedure proc in prog.Procedures.Values)
			{
				ProcedureFlow flow = (ProcedureFlow) crw.mpprocflow[proc];
                flow.Dump(prog.Architecture);
				crw.AdjustLiveOut(flow);
				crw.EnsureSignature(proc, flow);
				crw.AddUseInstructionsForOutArguments(proc);
			}

			foreach (Procedure proc in prog.Procedures.Values)
			{
				crw.RewriteCalls(proc, prog.Architecture);
				crw.RewriteReturns(proc);
			}
		}


		/// <summary>
		/// Having identified the return variable -- if any, rewrite all 
		/// return statements to return that variable.
		/// </summary>
		/// <param name="proc"></param>
        private void RewriteReturns(Procedure proc)
        {
            Identifier idRet = proc.Signature.ReturnValue;
            if (idRet == null)
                return;
            foreach (Statement stm in proc.Statements)
            {
                var ret = stm.Instruction as ReturnInstruction;
                if (ret != null)
                {
                    ret.Expression = idRet;
                }
            }
        }

		/// <summary>
		/// Creates a signature for this procedure, and ensures that all registers accessed by the procedure are in the procedure
		/// Frame.
		/// </summary>
		public void EnsureSignature(Procedure proc, ProcedureFlow flow)
		{
			if (proc.Signature != null && proc.Signature.ParametersValid)
				return;

			SignatureBuilder sb = new SignatureBuilder(proc, Program.Architecture);
			Frame frame = proc.Frame;
			if (flow.grfLiveOut != 0)
			{
				sb.AddFlagGroupReturnValue(flow.grfLiveOut, frame);
			}

            var implicitRegs = Program.Platform.CreateImplicitArgumentRegisters();
            BitSet mayUse = flow.MayUse - implicitRegs;
			foreach (int r in mayUse)
			{
				if (!IsSubRegisterOfRegisters(r, mayUse))
				{
					sb.AddRegisterArgument(r);
				}
			}

			foreach (KeyValuePair<int,Identifier> de in GetSortedStackArguments(proc.Frame))
			{
				AddStackArgument(de.Key, de.Value, flow, sb);
			}

            foreach (KeyValuePair<int, Identifier> de in GetSortedFpuStackArguments(proc.Frame, 0))
			{
				sb.AddFpuStackArgument(de.Key, de.Value);
			}

            BitSet liveOut = flow.LiveOut - implicitRegs;
			foreach (int r in liveOut)
			{
				if (!IsSubRegisterOfRegisters(r, liveOut))
				{
					sb.AddArgument(frame.EnsureRegister(Program.Architecture.GetRegister(r)), true);
				}
			}

            foreach (KeyValuePair<int, Identifier> de in GetSortedFpuStackArguments(proc.Frame, -proc.Signature.FpuStackDelta))
			{
				int i = de.Key;
				if (i <= proc.Signature.FpuStackOutArgumentMax)
				{
					sb.AddArgument(frame.EnsureFpuStackVariable(i, de.Value.DataType), true);
				}
			}

            var sig = sb.BuildSignature();
            flow.Signature = sig;
			proc.Signature = sig;
		}

		public SortedList<int, Identifier> GetSortedArguments(Frame f, Type type, int startOffset)
		{
			SortedList<int, Identifier> arguments = new SortedList<int,Identifier>();
			foreach (Identifier id in f.Identifiers)
			{
				if (id.Storage.GetType() == type)
				{
					int externalOffset = f.ExternalOffset(id);		//$REFACTOR: do this with BindToExternalFrame.
					if (externalOffset >= startOffset)
					{
						Identifier vOld;
                        if (!arguments.TryGetValue(externalOffset, out vOld) ||
                            vOld.DataType.Size < id.DataType.Size)
                        {
                            arguments[externalOffset] = id;
                        }
					}
				}
			}
			return arguments;
		}

		/// <summary>
		/// Returns a list of all stack arguments accessed, indexed by their offsets
		/// as seen by a caller. I.e. the first argument is at offset 0, &c.
		/// </summary>
        public SortedList<int, Identifier> GetSortedStackArguments(Frame frame)
		{
			return GetSortedArguments(frame, typeof (StackArgumentStorage), 0);
		}

        public SortedList<int, Identifier> GetSortedFpuStackArguments(Frame frame, int d)
		{
			return GetSortedArguments(frame, typeof (FpuStackStorage), d);
		}

		/// <summary>
		/// Returns true if the register is a strict subregister of one of the registers in the bitset.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="regs"></param>
		/// <returns></returns>
		private bool IsSubRegisterOfRegisters(int r, BitSet regs)
		{
            var rr = Program.Architecture.GetRegister(r);
            if (rr == null)
                return false;
			foreach (int r2 in regs)
			{
				if (rr.IsSubRegisterOf(Program.Architecture.GetRegister(r2)))
					return true;
			}
			return false;
		}
	}
}
