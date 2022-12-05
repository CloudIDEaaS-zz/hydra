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

using Decompiler.Arch.Z80;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using NUnit.Framework;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Arch.Z80
{
    [TestFixture]
    class RewriterTests : RewriterTestBase
    {
        private Z80ProcessorArchitecture arch = new Z80ProcessorArchitecture();
        private Address baseAddr = Address.Ptr16(0x0100);
        private Z80ProcessorState state;
        private LoadedImage image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return new Z80Rewriter(arch, new LeImageReader(image, 0), state, new Frame(arch.WordWidth), host);
        }

        public override Address LoadAddress
        {
            get { return baseAddr; }
        }

        [SetUp]
        public void Setup()
        {
            state = (Z80ProcessorState) arch.CreateProcessorState();
        }

        private void BuildTest(params byte[] bytes)
        {
            image = new LoadedImage(baseAddr, bytes);
        }

        [Test]
        public void Z80rw_lxi()
        {
            BuildTest(0x21, 0x34, 0x12);
            AssertCode("0|0100(3): 1 instructions",
                "1|L--|hl = 0x1234");
        }

        [Test]
        public void Z80rw_mov_a_hl()
        {
            BuildTest(0x7E);
            AssertCode("0|0100(1): 1 instructions",
                "1|L--|a = Mem0[hl:byte]");
        }

        [Test]
        public void Z80rw_mov_a_ix()
        {
            BuildTest(0xDD, 0x7E, 0x3);
            AssertCode("0|0100(3): 1 instructions",
                "1|L--|a = Mem0[ix + 0x0003:byte]");
        }

        [Test]
        public void Z80rw_jp()
        {
            BuildTest(0xC3, 0xAA, 0xBB);
            AssertCode("0|0100(3): 1 instructions",
                "1|T--|goto 0xBBAA");
        }

        [Test]
        public void Z80rw_jp_nz()
        {
            BuildTest(0xC2, 0xAA, 0xBB);
            AssertCode("0|0100(3): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch BBAA");
        }

        [Test]
        public void Z80rw_stx_b_d()
        {
            BuildTest(0xDD, 0x71, 0x80);
            AssertCode("0|0100(3): 1 instructions",
                "1|L--|Mem0[ix - 0x0080:byte] = c");
        }

        [Test]
        public void Z80rw_push_hl()
        {
            BuildTest(0xE5);
            AssertCode("0|0100(1): 2 instructions",
                "1|L--|sp = sp - 0x0002",
                "2|L--|Mem0[sp:word16] = hl");
        }

        [Test]
        public void Z80rw_add_a_R()
        {
            BuildTest(0x83);
            AssertCode("0|0100(1): 2 instructions",
                "1|L--|a = a + e",
                "2|L--|SZPC = cond(a)");
        }

        [Test]
        public void Z80rw_djnz()
        {
            BuildTest(0x10, 0xFE);
            AssertCode(
                "0|0100(2): 2 instructions",
                "1|L--|b = b - 0x01",
                "2|T--|if (b != 0x00) branch 0100");
        }

        [Test]
        public void Z80rw_jc()
        {
            BuildTest(0xC2, 0xFE, 0xCA);
            AssertCode(
                "0|0100(3): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch CAFE");
        }

        [Test]
        public void Z80rw_ldir()
        {
            BuildTest(0xED, 0xB0);
            AssertCode(
                "0|0100(2): 6 instructions",
                "1|L--|Mem0[de:byte] = Mem0[hl:byte]",
                "2|L--|hl = hl + 0x0001",
                "3|L--|de = de + 0x0001",
                "4|L--|bc = bc - 0x0001",
                "5|T--|if (bc != 0x0000) branch 0100",
                "6|L--|P = false");
        }

        [Test]
        public void Z80rw_call()
        {
            BuildTest(0xCD, 0xFE, 0xCA);
            AssertCode(
                "0|0100(3): 1 instructions",
                "1|T--|call 0xCAFE (2)");
        }

        [Test]
        public void Z80rw_call_Cond()
        {
            BuildTest(0xC4, 0xFE, 0xCA);
            AssertCode(
                "0|0100(3): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0103",
                "2|T--|call 0xCAFE (2)");
        }

        [Test]
        public void Z80rw_cp_ix_d()
        {
            BuildTest(0xDD, 0xBE, 0x08);
            AssertCode(
                "0|0100(3): 1 instructions",
                "1|L--|SZPC = a - Mem0[ix + 0x0008:byte]");
        }

        [Test]
        public void Z80rw_cpl()
        {
            BuildTest(0x2F);
            AssertCode(
                "0|0100(1): 1 instructions",
                "1|L--|a = ~a");
        }

        [Test]
        public void Z80rw_neg()
        {
            BuildTest(0xED, 0x44);
            AssertCode(
                "0|0100(2): 2 instructions",
                "1|L--|a = -a",
                "2|L--|SZPC = cond(a)");
        }

        [Test]
        public void Z80rw_jr()
        {
            BuildTest(0x18, 0x03);
            AssertCode(
                "0|0100(2): 1 instructions",
                "1|T--|goto 0105");
        }

        [Test]
        public void Z80rw_ret()
        {
            BuildTest(0xC9);
            AssertCode(
                "0|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }
    }
}
