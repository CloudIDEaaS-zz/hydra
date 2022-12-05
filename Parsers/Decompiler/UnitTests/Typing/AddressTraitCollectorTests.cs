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
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class AddressTraitCollectorTests
	{
        private Program prog;
		private TypeFactory factory;
		private TypeStore store;
		private ProcedureBuilder m;
		private AddressTraitCollector atrco;
		private TestTraitHandler handler;
		private EquivalenceClassBuilder eqb;

		[Test]
		public void AtrcoTestIdPlusConst()
		{
			var r = m.Local32("r");
			var mem = m.Load(PrimitiveType.Word32, m.IAdd(r, 4));
			mem.Accept(eqb);
			atrco.Collect(null, 0, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoTestIdPlusConst.txt");
		}

		[Test]
		public void AtrcoTestId()
		{
			var r = m.Local32("r");
			var mem = m.Load(PrimitiveType.Byte, r);
			mem.Accept(eqb);
			atrco.Collect(null, 0, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoTestId.txt");
		}

		[Test]
		public void AtrcoTestidPlusId()
		{
			Identifier r = m.Local32("r");
			Identifier s = m.Local32("s");
			MemoryAccess mem = m.Load(PrimitiveType.Byte, m.IAdd(r, s));
			mem.Accept(eqb);
			atrco.Collect(null, 0, mem, mem.EffectiveAddress);
		}

		[Test]
		public void AtrcoIdMinusConst()
		{
			Identifier r = m.Local32("r");
			MemoryAccess mem = m.Load(PrimitiveType.Word32, m.ISub(r, 4));
			mem.Accept(eqb);
			atrco.Collect(null, 0, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoIdMinusConst.txt");

		}

		[Test]
		public void AtrcoMem()
		{
			Identifier pp = m.Local32("pp");
			MemoryAccess mem = m.Load(PrimitiveType.Byte, m.Load(PrimitiveType.Word32, pp));
			mem.Accept(eqb);
			atrco.Collect(null, 0, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoMem.txt");
		}

		[Test]
		public void AtrcoSegPtr()
		{
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			MemoryAccess mem = m.SegMem(PrimitiveType.Word16, ds, bx);
			mem.Accept(eqb);
			atrco.Collect(prog.Globals, 2, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoSegPtr.txt");
		}

		/// <summary>
		/// Tests the case when we have an induction variable where we only know that
		/// the increment.
		/// </summary>
		[Test]
		public void AtrcoInductionVariableIncr()
		{
			Identifier id = m.Local32("esi");
			LinearInductionVariable iv = Liv32(1);
            prog.InductionVariables.Add(id, iv);
            Constant zero = m.Word32(0);
			MemoryAccess mem = m.Load(PrimitiveType.Byte, m.IAdd(id, zero));
			mem.Accept(eqb);
            atrco.Collect(null, 0, mem, mem.EffectiveAddress);
			Verify(null, "Typing/AtrcoInductionVariableIncr.txt");
		}

        [Test]
        [Ignore("Infrastructure needs to be built to handle negative induction variables correctly.")]
        public void AtrcoInductionVariableDecrement()
        {
            Identifier id = m.Local32("ebx");
            LinearInductionVariable iv = Liv32(-1);
            MemoryAccess mem = m.Load(PrimitiveType.Byte, id);
            mem.Accept(eqb);
            atrco.VisitInductionVariable(id, iv, null);
            Verify(null, "Typing/AtrcoInductionVariableDecr.txt");
        }

        private static LinearInductionVariable Liv16(short stride)
        {
            return new LinearInductionVariable(null, Constant.Int16(stride), null, false);
        }

        private static LinearInductionVariable Liv16(short start, short stride, short end)
        {
            return new LinearInductionVariable(
                Constant.Word16((ushort)start),
                Constant.Int16(stride), 
                Constant.Word16((ushort) end),
                false);
        }

        private static LinearInductionVariable Liv32(short stride)
        {
            return new LinearInductionVariable(null, Constant.Int32(stride), null, false);
        }

		[SetUp]
		public void Setup()
		{
            prog = new Program();
            prog.Architecture = new FakeArchitecture();
            prog.Platform = new DefaultPlatform(null, prog.Architecture);
			factory = prog.TypeFactory;
			store = prog.TypeStore;
			handler = new TestTraitHandler(store);
			eqb = new EquivalenceClassBuilder(factory, store);
			store.EnsureExpressionTypeVariable(factory, prog.Globals);
			
			atrco = new AddressTraitCollector(factory, store, handler, prog);
			m = new ProcedureBuilder();
		}

		private void Verify(Program prog, string outputFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFilename))
			{
				if (prog != null)
				{
					foreach (Procedure proc in prog.Procedures.Values)
					{
						proc.Write(false, fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
