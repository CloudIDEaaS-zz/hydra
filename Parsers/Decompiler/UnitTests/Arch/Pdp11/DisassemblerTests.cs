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

using Decompiler.Arch.Pdp11;
using Decompiler.Core;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Pdp11
{
    [TestFixture]   
    public class DisassemblerTests
    {
        private void RunTest(string expected, params ushort[] words)
        {
            var instr = RunTest(words);
            Assert.AreEqual(expected, instr.ToString());
        }

        private MachineInstruction RunTest(params ushort [] words)
        {
            var bytes = new byte[words.Length * 2];
            LeImageWriter writer = new LeImageWriter(bytes);
            foreach (ushort word in words)
            {
                writer.WriteLeUInt16(word);
            }
            var image = new LoadedImage(Address.Ptr16(0x200), bytes);
            var rdr = new LeImageReader(image, 0);
            var arch = new Pdp11Architecture();
            var dasm = new Pdp11Disassembler(rdr, arch);
            return dasm.First();
        }

        [Test]
        public void Pdp11dis_mul()
        {
            var instr = RunTest(0x7000);
            Assert.AreEqual("mul\tr0,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_div()
        {
            var instr = RunTest(0x7209);
            Assert.AreEqual("div\t@r1,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_xor()
        {
            var instr = RunTest(0x7811);
            Assert.AreEqual("xor\t(r1)+,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_jsr()
        {
            var instr = RunTest(0x0957, 0x1234);
            Assert.AreEqual("jsr\tr5,#1234", instr.ToString());
        }

        [Test]
        public void Pdp11dis_mov_sp_abs()
        {
            var instr = RunTest(0x119F, 0x1234);
            Assert.AreEqual("mov\tsp,@#1234", instr.ToString());
        }

        [Test]
        public void Pdp11dis_mov_ind_ind()
        {
            var instr = RunTest(0x6CB5, 0x0030, 0x0040);
            Assert.AreEqual("add\t0030(r5),0040(r2)", instr.ToString());
        }

        [Test]
        public void Pdp11dis_clr_abs()
        {
            var instr = RunTest(0x0A1F, 0x0030);
            Assert.AreEqual("clr\t@#0030", instr.ToString());
        }

        [Test]
        public void Pdp11dis_imm()
        {
             var instr = RunTest(0x15DF, 0x0030, 0x0040);
            Assert.AreEqual("mov\t#0030,@#0040", instr.ToString());
        }

        [Test]
        public void Pdp11dis_jsr_abs()
        {
            var instr = RunTest(0x09DF, 0x200);
            Assert.AreEqual("jsr\tpc,@#0200", instr.ToString());
        }

        [Test]
        public void Pdp11dis_sub_r_r()
        {
            var instr = RunTest(0xE083);
            Assert.AreEqual("sub\tr3,r2", instr.ToString());
        }

        [Test]
        public void Pdp11dis_asr()
        {
            var instr = RunTest(0x0C8A);
            Assert.AreEqual("asr\t@r2", instr.ToString());
        }

        [Test]
        public void Pdp11dis_movb()
        {
            var instr = RunTest(0x92A3);
            Assert.AreEqual("movb\t@r2,-(r3)", instr.ToString());
        }

        [Test]
        public void Pdp11dis_clr_post()
        {
            RunTest("clrb\t(r2)+", 0x8A12);
        }

        [Test]
        public void Pdp11dis_dec()
        {
            RunTest("dec\tr0", 0x0AC0);
        }

        [Test]
        public void Pdp11dis_beq()
        {
            RunTest("beq\t0200", 0x03FE);
        }
    }
}
