﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Evaluation;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Linq;

namespace Decompiler.UnitTests.Evaluation
{
    [TestFixture]
    public class SymbolicEvaluatorTests
    {
        private Identifier esp;
        private Identifier ebp;
        private SymbolicEvaluator se;
        private SymbolicEvaluationContext ctx;
        private IProcessorArchitecture arch;
        private Frame frame;

        [SetUp]
        public void Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            frame = new Frame(arch.FramePointerType);
        }

        private Expression GetRegisterState(SymbolicEvaluator se, Identifier id)
        {
            return ctx.RegisterState[(RegisterStorage)id.Storage];
        }

        private static Identifier Tmp32(string name)
        {
            return new Identifier(name, PrimitiveType.Word32, new TemporaryStorage(name, 1, PrimitiveType.Word32));
        }

        private static Identifier Tmp8(string name)
        {
            return new Identifier(name, PrimitiveType.Byte, new TemporaryStorage(name, 1, PrimitiveType.Byte));
        }

        private void CreateSymbolicEvaluator(Frame frame)
        {
            ctx = new SymbolicEvaluationContext(arch, frame);
            se = new SymbolicEvaluator(ctx);
            if (esp == null)
                esp = Tmp32("esp");
            ctx.SetValue(esp, frame.FramePointer);
        }

        private void RunBlockTest(Action<ProcedureBuilder> testBuilder)
        {
            var builder = new ProcedureBuilder(); 
            testBuilder(builder);
            var proc = builder.Procedure;
            CreateSymbolicEvaluator(proc.Frame);
            proc.ControlGraph.Successors(proc.EntryBlock).First().Statements.ForEach(x => se.Evaluate(x.Instruction));
        }

        [Test]
        public void EvaluateConstantAssignment()
        {
            var name = "edx";
            var edx = Tmp32(name);
            var ass = new Assignment(edx, Constant.Word32(3));
            CreateSymbolicEvaluator(frame);
            se.Evaluate(ass);
            Assert.IsNotNull(ctx.RegisterState);
            Assert.IsInstanceOf(typeof(Constant), ctx.TemporaryState[edx.Storage]);
        }


        [Test]
        public void IdentifierCopy()
        {
            esp = Tmp32("esp");
            var ebp = Tmp32("ebp");
            CreateSymbolicEvaluator(frame);
            var ass = new Assignment(ebp, esp);
            se.Evaluate(ass);
            Assert.AreSame(frame.FramePointer, ctx.TemporaryState[ebp.Storage], "Expected ebp to have the value of esp");
        }

        [Test]
        public void AdjustValue()
        {
            esp = Tmp32("esp");
            CreateSymbolicEvaluator(frame);
            var ass = new Assignment(esp, new BinaryExpression(BinaryOperator.IAdd, esp.DataType, esp, Constant.Word32(4)));
            se.Evaluate(ass);
            Assert.AreEqual("fp + 0x00000004", ctx.TemporaryState[esp.Storage].ToString());
        }

        [Test]
        public void LoadFromMemoryTrashes()
        {
            var ebx = Tmp32("ebx");
            var al = Tmp8("al");
            CreateSymbolicEvaluator(frame);
            var ass = new Assignment(al, new MemoryAccess(ebx, al.DataType));
            se.Evaluate(ass);
            Assert.AreEqual("<invalid>", ctx.TemporaryState[al.Storage].ToString());
        }

        [Test]
        public void PushConstant()
        {
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                ebp = m.Frame.EnsureRegister(Registers.ebp );
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
            });
            Assert.AreEqual("fp - 0x00000004", GetRegisterState(se, esp).ToString());
            Assert.AreEqual("fp - 0x00000004", GetRegisterState(se, ebp).ToString());
        }

        [Test]
        public void PushPop()
        {
            Identifier eax = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, eax);
                m.Assign(eax, m.Word32(1));
                m.Assign(eax, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
            });
            Assert.AreEqual("eax", GetRegisterState(se, eax).ToString());
        }

        [Test]
        public void FramePointer()
        {
            Identifier eax = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                esp = m.Frame.EnsureRegister(Registers.esp);
                ebp = m.Frame.EnsureRegister(Registers.ebp);
                eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(esp, m.ISub(esp, 20));

                m.Store(m.IAdd(ebp, 8), m.Word32(1));
                m.Assign(eax, m.LoadDw(m.IAdd(esp, 28)));

                m.Assign(esp, m.IAdd(esp, 20));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
            });
            Assert.AreEqual("ebp", GetRegisterState(se, ebp).ToString());
            Assert.AreEqual("0x00000001", GetRegisterState(se, eax).ToString());
        }

        [Test]
        public void ApplWithOutParameter()
        {
            Identifier r1 = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                r1 = m.Register(1);
                m.Assign(r1, 1);
                m.SideEffect(m.Fn("foo", m.Out(PrimitiveType.Pointer32, r1)));
            });
            Assert.AreEqual("<invalid>", GetRegisterState(se, r1).ToString());
        }

        [Test]
        public void Appl()
        {
            Identifier r1 = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                r1 = m.Register(1);
                m.Assign(r1, m.Fn("foo"));
            });
            Assert.AreEqual("<invalid>", GetRegisterState(se, r1).ToString());
        }

        [Test]
        public void RegisterPairStoreLoad()
        {
            Identifier ds = null;
            Identifier ax = null;
            Identifier eax = null;
            RunBlockTest(delegate (ProcedureBuilder m)
            {
                ds = m.Frame.EnsureRegister(Registers.ds);
                ax = m.Frame.EnsureRegister(Registers.ax);
                eax = m.Frame.EnsureRegister(Registers.eax);
                esp = m.Frame.EnsureRegister(Registers.esp);
                m.Store(m.ISub(esp, 4), ax);
                m.Store(m.ISub(esp, 2), ds);
                m.Assign(eax, m.LoadDw(m.ISub(esp, 4)));
            });
            Assert.AreEqual("SEQ(ds, ax)", GetRegisterState(se, eax).ToString());
        }

        [Test]
        public void Slice()
        {
            Identifier ax = null;
            Identifier cx = null;
            Identifier ebx = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                ax = m.Frame.EnsureRegister(Registers.ax);
                cx = m.Frame.EnsureRegister(Registers.cx);
                ebx = m.Frame.EnsureRegister(Registers.ebx);
                esp = m.Frame.EnsureRegister(Registers.esp);

                m.Store(m.ISub(esp, 4), ebx);
                m.Assign(ax, m.LoadW(m.ISub(esp, 4)));
                m.Assign(cx, m.LoadW(m.ISub(esp, 2)));
            });
            Assert.AreEqual("ebx", ctx.StackState[-4].ToString());
            Assert.AreEqual("(word16) ebx", GetRegisterState(se, ax).ToString());
            Assert.AreEqual("SLICE(ebx, word16, 16)", GetRegisterState(se, cx).ToString());
        }

        [Test]
        public void AssignToTemporary()
        {
            Identifier tmp = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                tmp = m.Frame.CreateTemporary(PrimitiveType.Word32);
                m.Assign(tmp, 3);
            });
            Assert.AreEqual("0x00000003", ctx.TemporaryState[tmp.Storage].ToString());
        }

        [Test]
        public void AssignToFlag()
        {
            Identifier flag = null;
            RunBlockTest(delegate(ProcedureBuilder m)
            {
                flag = m.Frame.EnsureFlagGroup(0x3, "SZ", PrimitiveType.Byte);
                m.Assign(flag, 0x03);
            });
            Assert.AreEqual(0x03, ctx.TrashedFlags);
        }

        [Test]
        public void OffsetToInvalidIsInvalid()
        {
            Identifier si = null;
            RunBlockTest(m =>
            {
                si = m.Frame.EnsureRegister(Registers.si);

                m.Assign(si,m.LoadW(m.Word16(0x0032)));
                m.Assign(si, m.IAdd(si,2));
            });
            Assert.AreEqual("<invalid>", ctx.RegisterState[(RegisterStorage)si.Storage].ToString());
        }
    }
}

