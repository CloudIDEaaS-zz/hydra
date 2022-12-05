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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.Arch.X86;
using Decompiler.Scanning;
using Decompiler.Typing;
using Decompiler.UnitTests.Mocks;
using Decompiler.UnitTests.Fragments;
using NUnit.Framework;
using System;
using System.IO;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TraitCollectorTests : TypingTestBase
	{
		private TraitCollector coll;
        private TestTraitHandler handler;
		private ExpressionNormalizer en;
		private EquivalenceClassBuilder eqb;
		private readonly string nl;

		public TraitCollectorTests()
		{
			nl = Environment.NewLine;
		}

		[Test]
		public void TrcoFactorial()
		{
			RunTest16("Fragments/factorial.asm", "Typing/TrcoFactorial.txt");
		}

		[Test]
		public void TrcoFactorialReg()
		{
			RunTest16("Fragments/factorial_reg.asm", "Typing/TrcoFactorialReg.txt");
		}

		[Test]
		public void TrcoFloatingPoint()
		{
			RunTest16("Fragments/fpuops.asm", "Typing/TrcoFloatingPoint.txt");
		}

		[Test]
		public void TrcoLength()
		{
			RunTest16("Fragments/type/listlength.asm", "Typing/TrcoLength.txt");
		}

		[Test]
		public void TrcoNestedStructs()
		{
			RunTest16("Fragments/type/nestedstructs.asm", "Typing/TrcoNestedStructs.txt");
		}

		[Test]
		public void TrcoSimpleLinearCode()
		{
			RunTest16("Fragments/simple_memoperations.asm", "Typing/TrcoSimpleLinearCode.txt");
		}

		[Test]
		public void TrcoUnknown()
		{
			RunTest16("Fragments/type/unknown.asm", "Typing/TrcoUnknown.txt");
		}

        [Test]
        public void TrcoReals()
        {
            RunTest16("Fragments/fpuops.asm", "Typing/TrcoReals.txt");
        }

		[Test]
		public void TrcoMemAccesses()
		{
			RunTest16("Fragments/multiple/memaccesses.asm", "Typing/TrcoMemAccesses.txt");
		}

		[Test]
		public void TrcoCmpMock()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new CmpMock());
			Program prog = mock.BuildProgram();
            coll = CreateCollector(prog);
            eqb.Build(prog);
			coll.CollectProgramTraits(prog);

			Verify(prog, "Typing/TrcoCmpMock.txt");
		}

		[Test]
		public void TrcoStaggeredArraysMock()
		{
			ProgramBuilder mock = new ProgramBuilder();
			mock.Add(new StaggeredArraysFragment());
			Program prog = mock.BuildProgram();
            coll = CreateCollector(prog);

			en.Transform(prog);
			eqb.Build(prog);
			coll.CollectProgramTraits(prog);

			Verify(prog, "Typing/TrcoStaggeredArraysMock.txt");
		}

		[Test]
		public void TrcoArrayExpression()
		{
			var b = new Identifier("base", PrimitiveType.Word32, null);
			var i = new Identifier("idx", PrimitiveType.Word32, null);
			var s = Constant.Word32(4);

			ProcedureBuilder m = new ProcedureBuilder();

			// e ::= Mem[(b+0x1003000)+(i*s):word16]
			Expression e = m.Load(
				PrimitiveType.Word16,
				m.IAdd(m.IAdd(b, Constant.Word32(0x10030000)),
				m.SMul(i, s)));
            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoArrayExpression.txt");
		}

		[Test]
		public void TrcoInductionVariable()
		{
			Identifier i = new Identifier("i", PrimitiveType.Word32, null);
			MemoryAccess load = new MemoryAccess(MemoryIdentifier.GlobalMemory, i, PrimitiveType.Int32);
			Identifier i2 = new Identifier("i2", PrimitiveType.Word32, null);
			MemoryAccess ld2 = new MemoryAccess(MemoryIdentifier.GlobalMemory, i2, PrimitiveType.Int32);

			LinearInductionVariable iv = new LinearInductionVariable(
				Constant.Word32(0), 
				Constant.Word32(1),
				Constant.Word32(10),
                false);
			LinearInductionVariable iv2 = new LinearInductionVariable(
				Constant.Word32(0x0010000),
				Constant.Word32(4),
				Constant.Word32(0x0010040),
                false);

            Program prog = CreateProgram();
			prog.InductionVariables.Add(i, iv);
			prog.InductionVariables.Add(i2, iv2);

            coll = CreateCollector(prog);
			prog.Globals.Accept(eqb);
			load.Accept(eqb);
			ld2.Accept(eqb);
			prog.Globals.Accept(coll);
			load.Accept(coll);
			ld2.Accept(coll);
			Verify(null, "Typing/TrcoInductionVariable.txt");
		}

		[Test]
		public void TrcoGlobalArray()
		{
            Program prog = CreateProgram();
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier i = m.Local32("i");
            Expression ea = m.IAdd(prog.Globals, m.IAdd(m.Shl(i, 2), 0x3000));
            Expression e = m.Load(PrimitiveType.Int32, ea);

            coll = CreateCollector(prog);
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoGlobalArray.txt");
		}

		[Test]
		public void TrcoMemberPointer()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Identifier ax = m.Local16("ax");
			MemberPointerSelector mps = m.MembPtrW(ds, m.IAdd(bx, 4));
			Expression e = m.Load(PrimitiveType.Byte, mps);

            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Assert.IsNotNull(mps.BasePointer.TypeVariable, "Base pointer should have type variable");
			Verify(null, "Typing/TrcoMemberPointer.txt");
		}

		[Test]
		public void TrcoSegmentedAccess()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Identifier bx = m.Local16("bx");
			Identifier ax = m.Local16("ax");
			Expression e = m.SegMem(PrimitiveType.Word16, ds, m.IAdd(bx, 4));

            coll = CreateCollector();
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedAccess.txt");
		}

		[Test]
		public void TrcoSegmentedDirectAddress()
		{
            Program prog = CreateProgram();
			prog.TypeStore.EnsureExpressionTypeVariable(prog.TypeFactory, prog.Globals);

            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local16("ds");
			Expression e = m.SegMem(PrimitiveType.Byte, ds, m.Int16(0x0200));

            coll = CreateCollector(prog);
			e = e.Accept(en);
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegmentedDirectAddress.txt");
		}

        [Test]
        public void TrcoMultiplication()
        {
            var prog = CreateProgram();
            var m = new ProcedureBuilder();
            var id = m.Local32("id");
            var e = m.IMul(id, id);

            coll = CreateCollector(prog);
            e = e.Accept(en);
            e.Accept(eqb);
            e.Accept(coll);
            Verify(null, "Typing/TrcoMultiplication.txt");
        }

        private static Program CreateProgram()
        {
            var arch = new FakeArchitecture();

            return new Program
            {
                Architecture = arch,
                Platform = new DefaultPlatform(null, arch),
            };
        }

		[Test]
		public void TrcoPtrPtrInt()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new PtrPtrIntMock());
			RunTest(p.BuildProgram(), "Typing/TrcoPtrPtrInt.txt");
		}

		[Test]
		public void TrcoFnPointerMock()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new FnPointerFragment());
			RunTest(p.BuildProgram(), "Typing/TrcoFnPointerMock.txt");
		}

		[Test]
		public void TrcoSegmentedMemoryPointer()
		{
			ProgramBuilder p = new ProgramBuilder();
			p.Add(new SegmentedMemoryPointerMock());
			RunTest(p.BuildProgram(), "Typing/TrcoSegmentedMemoryPointer.txt");
		}

		[Test]
		public void TrcoReg00008()
		{
			RunTest16("Fragments/regressions/r00008.asm", "Typing/TrcoReg00008.txt");
		}

        [Test]
        public void TrcoReg00011()
        {
            RunTest16("Fragments/regressions/r00011.asm", "Typing/TrcoReg00011.txt");
        }

        [Test]
        public void TrcoReg00012()
        {
            RunTest16("Fragments/regressions/r00012.asm", "Typing/TrcoReg00012.txt");
        }

        [Test]
        public void TrcoReg00014()
        {
            RunTest32("Fragments/regressions/r00014.asm", "Typing/TrcoReg00014.txt");
        }

        [Test]
		public void TrcoIntelIndexedAddressingMode()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new IntelIndexedAddressingMode());
			Program prog = m.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.AnalyzeProgram();
			RunTest(prog, "Typing/TrcoIntelIndexedAddressingMode.txt");
		}

		[Test]
		public void TrcoTreeFind()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new TreeFindMock());
			Program prog = m.BuildProgram();
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.AnalyzeProgram();
			RunTest(prog, "Typing/TrcoTreeFind.txt");
		}

		[Test]
		public void TrcoSegmentedDoubleReference()
		{
			ProgramBuilder m = new ProgramBuilder();
			m.Add(new SegmentedDoubleReferenceMock());
			RunTest(m.BuildProgram(), "Typing/TrcoSegmentedDoubleReference.txt");
		}

        [Test]
        public void TrcoSegmentedPointer()
        {
            var m = new ProgramBuilder();
            m.Add(new SegmentedPointerProc());
            RunTest(m.BuildProgram(), "Typing/TrcoSegmentedPointer.txt");
        }

        [Test]
        public void TrcoCallTable()
        {
            var pb = new ProgramBuilder();
            pb.Add(new IndirectCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TrcoCallTable.txt");
        }

        [Test]
        public void TrcoSegmentedCall()
        {
            var pb = new ProgramBuilder();
            pb.Add(new SegmentedCallFragment());
            RunTest(pb.BuildProgram(), "Typing/TrcoSegmentedCall.txt");
        }

		[Test]
		public void TrcoIcall()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier pfn = m.Local32("pfn");
			Expression l = m.Load(PrimitiveType.Word32, pfn);
			CallInstruction icall = new CallInstruction(l, new CallSite(0, 0));

            coll = CreateCollector();
			icall.Accept(eqb);
			icall.Accept(coll);
			StringWriter sw = new StringWriter();
			handler.Traits.Write(sw);
            string exp =
                "T_1 (in pfn : word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_mem(T_2, 0)" + nl +
                "T_2 (in Mem0[pfn:word32] : word32)" + nl +
                "\ttrait_primitive((ptr code))" + nl +
                "\ttrait_primitive(word32)" + nl;
            Console.WriteLine(sw.ToString());
			Assert.AreEqual(exp, sw.ToString());
		}

		[Test]
		public void TrcoSegMem()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Expression e = m.SegMemW(ds, m.Word16(0xC002U));

            coll = CreateCollector();
			e.Accept(eqb);
			e.Accept(coll);
			Verify(null, "Typing/TrcoSegMem.txt");
		}

		[Test]
		public void TrcoUnsignedCompare()
		{
			ProcedureBuilder m = new ProcedureBuilder();
			Identifier ds = m.Local16("ds");
			Expression e = m.Uge(ds, m.Word16(0x0800));

            coll = CreateCollector();
			e.Accept(eqb);
			e.Accept(coll);
			StringWriter sb = new StringWriter();
			handler.Traits.Write(sb);
			string exp = 
				"T_1 (in ds : word16)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_equal(T_2)" + nl +
				"\ttrait_primitive(cups16)" + nl +
				"T_2 (in 0x0800 : word16)" + nl +
				"\ttrait_primitive(word16)" + nl +
				"\ttrait_primitive(cups16)" + nl +
				"T_3 (in ds >=u 0x0800 : bool)" + nl +
				"\ttrait_primitive(bool)" + nl;
			Assert.AreEqual(exp, sb.ToString());
		}


        [Test]
        public void TrcoArrayAccess()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier ds = m.Local(PrimitiveType.SegmentSelector, "ds");
            Identifier bx = m.Local16("bx");
            Expression e = m.Array(PrimitiveType.Word32, m.Seq(ds, m.Word16(0x300)), m.IMul(bx, 8));
            coll = CreateCollector();
            e.Accept(eqb);
            e.Accept(coll);
            StringWriter sb = new StringWriter();
            handler.Traits.Write(sb);
            string sExp =
                "T_1 (in ds : selector)" + nl +
                "\ttrait_primitive(selector)" + nl +
                "\ttrait_mem_array(300, 8, 0, T_7)" + nl + 
                "T_2 (in 0x0300 : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "T_3 (in SEQ(ds, 0x0300) : ptr32)" + nl +
                "	trait_primitive(ptr32)" + nl +
                "T_4 (in bx : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_5 (in 0x0008 : word16)" + nl +
                "	trait_primitive(word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_6 (in bx * 0x0008 : word16)" + nl +
                "	trait_primitive(ui16)" + nl +
                "T_7 (in SEQ(ds, 0x0300)[bx * 0x0008] : word32)" + nl +
                "	trait_primitive(word32)" + nl;
            Assert.AreEqual(sExp, sb.ToString());
        }

        [Test]
        public void TrcoDbp()
        {
            ProcedureBuilder m = new ProcedureBuilder();
            Identifier a = m.Local32("a");
            Identifier b = m.LocalByte("b");
            var s = m.Assign(a, m.Dpb(a, b, 0, 8));
            coll = CreateCollector();
            s.Accept(eqb);
            s.Accept(coll);
            StringWriter sb = new StringWriter();
            handler.Traits.Write(sb);
            Console.WriteLine(sb);
            string exp =
                "T_1 (in a : word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_equal(T_3)" + nl +
                "T_2 (in b : byte)" + nl +
                "\ttrait_primitive(byte)" + nl +
                "T_3 (in DPB(a, b, 0, 8) : word32)" + nl +
                "\ttrait_primitive(word32)" + nl;
            Assert.AreEqual(exp, sb.ToString());
        }

        [Test]
        [Ignore("Complete the test by seeing the return type T_5 to be of type 'struct 3'")]
        public void TrcoCallFunctionWithArraySize()
        {
            var m = new ProcedureBuilder();
            var sig = new ProcedureSignature(null, 
                m.Frame.EnsureStackArgument(0, PrimitiveType.Word32));
            var ex = new ExternalProcedure("malloc", sig, new ProcedureCharacteristics
            {
                Allocator = true,
                ArraySize = new ArraySizeCharacteristic
                {
                    Argument = "r",
                    Factors = new ArraySizeFactor[] 
                    {
                        new ArraySizeFactor { Constant = "1" }
                    }
                }
            });

            Identifier eax = m.Local32("eax");
            var call = m.Assign(eax, m.Fn(new ProcedureConstant(PrimitiveType.Word32, ex), m.Word32(3)));

            coll = CreateCollector();
            call.Accept(eqb);
            call.Accept(coll);
            StringWriter sw = new StringWriter();
            handler.Traits.Write(sw);
            string sExp =
                "T_1 (in malloc : word32)" + nl +
                "\ttrait_func(T_4 -> T_5)" + nl +
                "T_3 (in dwArg00 : word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "T_4 (in 0x00000003 : word32)" + nl +
                "\ttrait_primitive(word32)" + nl +
                "\ttrait_equal(T_3)" + nl +
                "T_5 (in malloc(0x00000003) : word32)" + nl +
                "\ttrait_primitive(word32)"; 
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(sExp, sw.ToString());
        }

        private TraitCollector CreateCollector()
        {
            return CreateCollector(CreateProgram());
        }

        private TraitCollector CreateCollector(Program prog)
        {
            en = new ExpressionNormalizer(prog.Architecture.PointerType);
            eqb = new EquivalenceClassBuilder(prog.TypeFactory, prog.TypeStore);
            handler = new TestTraitHandler(prog.TypeStore);
            return new TraitCollector(prog.TypeFactory, prog.TypeStore, handler, prog);
        }

        private ITraitHandler CreateHandler(TypeStore store)
        {
            return new TestTraitHandler(store);
        }

        protected override void RunTest(Program prog, string outFile)
		{
            coll = CreateCollector(prog);
            en.Transform(prog);
            eqb.Build(prog);
			coll.CollectProgramTraits(prog);

			using (FileUnitTester fut = new FileUnitTester(outFile))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				handler.Traits.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
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

	public class IntelIndexedAddressingMode : ProcedureBuilder
	{
		protected override void BuildBody()
		{
            Identifier ds = Local16("ds");
			Identifier es = Local16("es");
			Identifier ax = Local16("ax");
			Identifier bx = Local16("bx");
			Identifier si = Local16("si");
			Assign(es, SegMem(PrimitiveType.Word16, ds, Int16(0x7070)));
			Assign(ax, 0x4A);
			Assign(si, SMul(ax, SegMem(PrimitiveType.Word16, ds, Int16(0x1C0A))));
            Assign(bx, SegMem(PrimitiveType.Word16, ds, Int16(0x0CA4)));
			SegStore(ds, IAdd(IAdd(bx, 10), si), Byte(0xF8));
			Return();
		}
	}

    public class TestTraitHandler : ITraitHandler
    {
        private TypeFactory factory = new TypeFactory();

        public TestTraitHandler(TypeStore store)
        {
            this.Traits = new TraitMapping(store);
        }

        public TraitMapping Traits { get; private set; }

        #region ITraitHandler Members

        public void ArrayTrait(TypeVariable tArray, int elementSize, int length)
        {
            Traits.AddTrait(tArray, new TraitArray(elementSize, length));
        }

        public void BuildEquivalenceClassDataTypes()
        {
        }

        public DataType DataTypeTrait(Expression exp, DataType p)
        {
            if (p == null)
                throw new ArgumentNullException("p");
            Traits.AddTrait(exp.TypeVariable, new TraitDataType(p));
            return p;
        }

        public DataType EqualTrait(Expression t1, Expression t2)
        {
            if (t1 != null && t2 != null)
                Traits.AddTrait(t1.TypeVariable, new TraitEqual(t2.TypeVariable));
            return null;
        }

        public DataType FunctionTrait(Expression function, int funcPtrSize, TypeVariable ret, params TypeVariable[] actuals)
        {
            return Traits.AddTrait(function.TypeVariable, new TraitFunc(function.TypeVariable, funcPtrSize, ret, actuals));
        }

        public DataType MemAccessArrayTrait(Expression tBase, Expression tStruct, int structPtrSize, int offset, int elementSize, int length, Expression tAccess)
        {
            return Traits.AddTrait(tStruct.TypeVariable, new TraitMemArray(tBase != null ? tBase.TypeVariable : null, structPtrSize, offset, elementSize, length, tAccess.TypeVariable));
        }

        public DataType MemAccessTrait(Expression tBase, Expression tStruct, int structPtrSize, Expression tField, int offset)
        {
            return Traits.AddTrait(tStruct.TypeVariable, new TraitMem(tBase != null ? tBase.TypeVariable: null, structPtrSize, tField.TypeVariable, offset));
        }

        public DataType MemFieldTrait(Expression tBase, Expression tStruct, Expression tField, int offset)
        {
            return Traits.AddTrait(tStruct.TypeVariable, new TraitMem(tBase != null ? tBase.TypeVariable : null, 0, tField.TypeVariable, offset));
        }

        public DataType MemSizeTrait(Expression tBase, Expression tStruct, int size)
        {
            return Traits.AddTrait(tStruct.TypeVariable, new TraitMemSize(size));
        }

        public DataType PointerTrait(Expression tPtr, int ptrSize, Expression tPointee)
        {
            return Traits.AddTrait(tPtr.TypeVariable, new TraitPointer(tPointee.TypeVariable));
        }

        #endregion
    }
}
