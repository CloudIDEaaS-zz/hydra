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

using Decompiler.Analysis;
using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class RegisterLivenessUnitTests
	{
		private Program prog;
		private Procedure proc;
		private Frame f;
		private ProgramDataFlow mpprocflow;
		private RegisterLiveness rl;
		private ProcedureBuilder m;
        private HashSet<Procedure> terminates;

		[SetUp]
		public void Setup()
		{
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Protected32);
            prog.Platform = new DefaultPlatform(null, prog.Architecture);
			m = new ProcedureBuilder();
			proc = m.Procedure;
			f = proc.Frame;
			mpprocflow = new ProgramDataFlow();
            terminates = new HashSet<Procedure>();
			rl = new RegisterLiveness(prog, mpprocflow, null);
			rl.Procedure = proc;
			rl.IdentifierLiveness.BitSet = prog.Architecture.CreateRegisterBitset();
		}

        private BlockFlow CreateBlockFlow(Block block, Frame frame)
        {
            return new BlockFlow(
                block,
                prog.Architecture.CreateRegisterBitset(),
                new SymbolicEvaluationContext(prog.Architecture, frame));
        }

		/// <summary>
		/// Tests using only part of a register.
		/// </summary>
		[Test]
		public void Rl_SliceRegister()
		{
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);

			m.Store(m.Int32(0x01F3004), ax).Instruction.Accept(rl);
			Assert.AreEqual(" ax", Dump(rl.IdentifierLiveness));
			m.Assign(eax, ecx).Accept(rl);
			Assert.AreEqual(16, rl.IdentifierLiveness.DefBitSize);
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		/// <summary>
		/// In this test, the assignment of eax = ecx is followed by uses of al, ah. This implies 
		/// that cx, not ecx should be live.
		/// </summary>
		[Test]
		public void Rl_AlAhUses()
		{
			Identifier al = f.EnsureRegister(Registers.al);
			Identifier ah = f.EnsureRegister(Registers.ah);
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);

			m.Store(m.Int32(0x01F3004), al).Instruction.Accept(rl);
			Assert.AreEqual(" al", Dump(rl.IdentifierLiveness));
			m.Store(m.Int32(0x01F3008), ah).Instruction.Accept(rl);	
			Assert.AreEqual(" al ah", Dump(rl.IdentifierLiveness));
			m.Assign(eax, ecx).Accept(rl);		
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		/// <summary>
		/// The assignment of ah = ah + 0x7 is followed by uses of al and ah. the result should still be
		/// that ah and al are live.
		/// </summary>
		[Test]
		public void Rl_AlAhUses2()
		{
			Identifier al = f.EnsureRegister(Registers.al);
			Identifier ah = f.EnsureRegister(Registers.ah);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int32(0x01F3004), ax).Instruction.Accept(rl);	// use al and ah
			Assert.AreEqual(" ax", Dump(rl.IdentifierLiveness));
			m.Assign(ah, m.IAdd(ah, 3)).Accept(rl);
			Assert.AreEqual(" al ah", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_MemLoad()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier eax = f.EnsureRegister(Registers.eax);

			m.Store(m.Int32(0x01F0300), ax).Instruction.Accept(rl);			// force ax to be live.
			m.Assign(ax, m.Load(ax.DataType, eax)).Accept(rl);	// eax should be live in here.
			Assert.AreEqual(" eax", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_Shift()
		{
			Identifier cl = f.EnsureRegister(Registers.cl);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int16(0x01F0300), ax).Instruction.Accept(rl);			// force ax to be live.
			m.Assign(ax, m.Shl(ax, cl)).Accept(rl);				// ax, cl should be live in.
			Assert.AreEqual(" ax cl", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_CopyDeadRegister()
		{
			Identifier bx = f.EnsureRegister(Registers.bx);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Assign(ax, bx).Accept(rl);
			Assert.AreEqual("", Dump(rl.IdentifierLiveness), "No identifiers should be live since ax is dead");
		}

		[Test]
		public void Rl_CopyLiveRegister()
		{
			Identifier bx = f.EnsureRegister(Registers.bx);
			Identifier ax = f.EnsureRegister(Registers.ax);

			m.Store(m.Int32(0x12341234), ax).Instruction.Accept(rl);
			m.Assign(ax, bx).Accept(rl);
			Assert.AreEqual(" bx", Dump(rl.IdentifierLiveness), "bx should be live since ax was stored");
		}

		[Test]
		public void Rl_Locals()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier ecx = f.EnsureRegister(Registers.ecx);
			Identifier loc = f.EnsureStackLocal(-8, PrimitiveType.Word32);

			m.Store(m.Int32(0x01DFDF), ax).Instruction.Accept(rl);
			m.Assign(ax, loc).Accept(rl);
			Assert.AreEqual(" Local -0008", Dump(rl.IdentifierLiveness));
			m.Assign(loc, ecx).Accept(rl);
			Assert.AreEqual(" cx", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_NarrowedStackParameters()
		{
			Identifier ax = f.EnsureRegister(Registers.ax);
			Identifier eax = f.EnsureRegister(Registers.eax);
			Identifier arg = f.EnsureStackArgument(4, PrimitiveType.Word32);

			m.Store(m.Int32(0x102343), ax).Instruction.Accept(rl);
			m.Assign(eax, arg).Accept(rl);
			Assert.AreEqual(16, rl.IdentifierLiveness.LiveStorages[arg.Storage]);
		}

		[Test]
		public void Rl_PushPop()
		{
			Identifier bp = f.EnsureRegister(Registers.bp);
			Identifier loc = f.EnsureStackLocal(-4, PrimitiveType.Word16);

			m.Assign(bp, loc).Accept(rl);
			m.Assign(loc, bp).Accept(rl);
			Assert.AreEqual("", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_PushPopLiveBp()
		{
			Identifier bp = f.EnsureRegister(Registers.bp);
			Identifier loc = f.EnsureStackLocal(-4, PrimitiveType.Word16);

			m.Assign(bp, loc).Accept(rl);
			m.Store(m.Int32(0x12345678), bp).Instruction.Accept(rl);
			m.Assign(loc, bp).Accept(rl);
			Assert.AreEqual(" bp", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_CallToProcedureWithValidSignature()
		{
			Procedure callee = new Procedure("callee", null);
			callee.Signature = new ProcedureSignature(
				f.EnsureRegister(Registers.eax),
				new Identifier[] {
					f.EnsureRegister(Registers.ebx),
					f.EnsureRegister(Registers.ecx),
					f.EnsureOutArgument(f.EnsureRegister(Registers.edi), PrimitiveType.Pointer32)
				});
			
			rl.IdentifierLiveness.BitSet[Registers.eax.Number] = true;
			rl.IdentifierLiveness.BitSet[Registers.esi.Number] = true;
			rl.IdentifierLiveness.BitSet[Registers.edi.Number] = true;
			CallInstruction ci = new CallInstruction(new ProcedureConstant(PrimitiveType.Pointer32, callee), new CallSite(4, 0));
			rl.VisitCallInstruction(ci);
			Assert.AreEqual(" ecx ebx esi", Dump(rl.IdentifierLiveness));
		}

		[Test]
        [Ignore("Won't be needed when class obsoleted")]
		public void Rl_CallToProcedureWithStackArgs()
		{
			Procedure callee = new Procedure("callee", null);
			BitSet trash = prog.Architecture.CreateRegisterBitset();
			callee.Signature = new ProcedureSignature(
				f.EnsureRegister(Registers.eax),
				new Identifier[] {
                    new Identifier("arg04", PrimitiveType.Word16, new StackArgumentStorage(4, PrimitiveType.Word16)),
					new Identifier("arg08", PrimitiveType.Byte, new StackArgumentStorage(8, PrimitiveType.Byte))
                });

			Identifier b04 = m.Frame.EnsureStackLocal(-4, PrimitiveType.Word32);
			Identifier w08 = m.Frame.EnsureStackLocal(-8, PrimitiveType.Word32);
			new CallInstruction(new ProcedureConstant(PrimitiveType.Pointer32, callee), new CallSite(4, 0)).Accept(rl);

			foreach (object o in rl.IdentifierLiveness.LiveStorages.Keys)
			{
				Console.WriteLine("{0} {1} {2}", o, Object.Equals(o, b04.Storage), Object.Equals(o, b04.Storage));
			}
            Assert.AreEqual(2, rl.IdentifierLiveness.LiveStorages.Count, "Should have two accesses");

			Assert.IsTrue(rl.IdentifierLiveness.LiveStorages.ContainsKey(b04.Storage), "Should have storage for b04");
			Assert.IsTrue(rl.IdentifierLiveness.LiveStorages.ContainsKey(w08.Storage), "Should have storage for w08");
		}

		[Test]
		public void Rl_MarkLiveStackParameters()
		{
            var callee = new Procedure("callee", prog.Architecture.CreateFrame());
			callee.Frame.ReturnAddressSize = 4;
            callee.Frame.ReturnAddressKnown = true;
			callee.Frame.EnsureStackArgument(0, PrimitiveType.Word32);
			callee.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
			Assert.AreEqual(8, callee.Frame.GetStackArgumentSpace());
			ProcedureFlow pf = new ProcedureFlow(callee, prog.Architecture);
			mpprocflow[callee] = pf;

			Identifier loc08 = m.Frame.EnsureStackLocal(-8, PrimitiveType.Word32);
			Identifier loc0C = m.Frame.EnsureStackLocal(-12, PrimitiveType.Word32);
			Identifier loc10 = m.Frame.EnsureStackLocal(-16, PrimitiveType.Word32);
			rl.CurrentState = new RegisterLiveness.ByPassState();
            var ci = new CallInstruction(
                new ProcedureConstant(PrimitiveType.Pointer32, callee),
                new CallSite(4, 0) { StackDepthOnEntry = 16 });
			rl.Procedure = m.Procedure;
			rl.MarkLiveStackParameters(ci);
			Assert.AreEqual(" Local -000C Local -0010", Dump(rl.IdentifierLiveness));
		}

		[Test]
		public void Rl_PredefinedSignature()
		{
			Procedure callee = new Procedure("callee", null);
			Identifier edx = new Identifier("edx", PrimitiveType.Word32, Registers.edx);
			callee.Signature = new ProcedureSignature(
				new Identifier("eax", PrimitiveType.Word32, Registers.eax),
				new Identifier[] { new Identifier("ecx",    PrimitiveType.Word32, Registers.ecx),
								   new Identifier("arg04",  PrimitiveType.Word16, new StackArgumentStorage(4, PrimitiveType.Word16)),
								   new Identifier("edxOut", PrimitiveType.Word32, new OutArgumentStorage(edx))});

			RegisterLiveness.State st = new RegisterLiveness.ByPassState();
			BlockFlow bf = CreateBlockFlow(callee.ExitBlock, null);
			mpprocflow[callee.ExitBlock] = bf;
			st.InitializeBlockFlow(callee.ExitBlock, mpprocflow, true);
			Assert.IsTrue(bf.DataOut[Registers.eax.Number],"eax is a return register");
			Assert.IsTrue(bf.DataOut[Registers.edx.Number],"edx is an out register");
			Assert.IsTrue(bf.DataOut[Registers.ax.Number], "ax is aliased by eax");
			Assert.IsFalse(bf.DataOut[Registers.ecx.Number], "ecx is an in register");
			Assert.IsFalse(bf.DataOut[Registers.esi.Number], "esi is not present in signature");
		}

		/// <summary>
		/// We assume preserved registers are _never_ live out.
		/// </summary>
		[Test]
		public void Rl_ProcedureWithTrashedAndPreservedRegisters()
		{
            Procedure proc = new Procedure("test", prog.Architecture.CreateFrame());
			ProcedureFlow pf = new ProcedureFlow(proc, prog.Architecture);
			mpprocflow[proc] = pf;
			pf.TrashedRegisters[Registers.eax.Number] = true;
			pf.TrashedRegisters[Registers.ebx.Number] = true;
			pf.PreservedRegisters[Registers.ebp.Number] = true;
			pf.PreservedRegisters[Registers.bp.Number] = true;

			RegisterLiveness.State st = new RegisterLiveness.ByPassState();
			BlockFlow bf = CreateBlockFlow(proc.ExitBlock, proc.Frame);
			mpprocflow[proc.ExitBlock] = bf;
			st.InitializeBlockFlow(proc.ExitBlock, mpprocflow, true);
			Assert.IsFalse(bf.DataOut[Registers.ebp.Number], "preserved registers cannot be live out");
			Assert.IsFalse(bf.DataOut[Registers.bp.Number], "preserved registers cannot be live out");
			Assert.IsTrue(bf.DataOut[Registers.eax.Number], "trashed registers may be live out");
			Assert.IsTrue(bf.DataOut[Registers.esi.Number], "Unmentioned registers may be live out");
		}

		private string Dump(IdentifierLiveness vl)
		{
			StringWriter sw = new StringWriter();
			vl.Write(sw, "");
            var sl = new SortedList<string, string>();
			foreach (Storage o in vl.LiveStorages.Keys)
			{
				string s = o.ToString();
				sl.Add(s, s);
			}
			foreach (string s in sl.Keys)
			{
				sw.Write(" ");
				sw.Write(s);
			}
			return sw.ToString();
		}	
	}
}
