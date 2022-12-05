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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcDisassemblerTests : DisassemblerTestBase<PowerPcInstruction>
    {
        private PowerPcArchitecture arch = new PowerPcArchitecture32();

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private PowerPcInstruction DisassembleX(uint op, uint rs, uint ra, uint rb, uint xo, uint rc)
        {
            uint w =
                (op << 26) |
                (rs << 21) |
                (ra << 16) |
                (rb << 11) |
                (xo << 1) |
                rc;
            LoadedImage img = new LoadedImage(Address.Ptr32(0x00100000), new byte[4]);
            img.WriteBeUInt32(0, w);
            return Disassemble(img);
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        private void RunTest(string expected, string bits)
        {
            var instr = DisassembleBits(bits);
            Assert.AreEqual(expected, instr.ToString());
        }

        [Test]
        public void PPCDis_IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Opcode.illegal, instr.Opcode);
        }

        [Test]
        public void PPCDis_Ori()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Opcode.ori, instr.Opcode);
            Assert.AreEqual(3, instr.Operands);
            Assert.AreEqual("ori\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Oris()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x64, 0x1F, 0x44, 0x44 });
            Assert.AreEqual("oris\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void PPCDis_Addi()
        {
            PowerPcInstruction instr = DisassembleBytes(new byte[] { 0x38, 0x1F, 0xFF, 0xFC });
            Assert.AreEqual("addi\tr0,r31,-0004", instr.ToString());
        }

        [Test]
        public void PPCDis_Or()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 0);
            Assert.AreEqual("or\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_Or_()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 1);
            Assert.AreEqual("or.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_b()
        {
            var instr = DisassembleWord(0x48000008);
            Assert.AreEqual("b\t$00100008", instr.ToString());
        }

        [Test]
        public void PPCDis_b_absolute()
        {
            var instr = DisassembleWord(0x4800000A);
            Assert.AreEqual("b\t$00000008", instr.ToString());
        }

        [Test]
        public void PPCDis_bl()
        {
            var instr = DisassembleWord(0x4BFFFFFD);
            Assert.AreEqual("bl\t$000FFFFC", instr.ToString());
        }

        [Test]
        public void PPCDis_bcxx()
        {
            var instr = DisassembleWord(0x4000FFF0);
            Assert.AreEqual("bdnzf\tlt,$000FFFF0", instr.ToString());
        }

        [Test]
        public void PPCDis_mtlr()
        {
            var instr = DisassembleWord(0x7C0803A6);
            Assert.AreEqual("mtlr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_blr()
        {
            var instr = DisassembleWord(0x4e800020);
            Assert.AreEqual("blr", instr.ToString());
        }

        [Test]
        public void PPCDis_lwz()
        {
            var instr = DisassembleWord(0x803F0005);
            Assert.AreEqual("lwz\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stw()
        {
            var instr = DisassembleWord(0x903FFFF8);
            Assert.AreEqual("stw\tr1,-8(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stwu()
        {
            var instr = DisassembleWord(0x943F0005);
            Assert.AreEqual("stwu\tr1,5(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_stb()
        {
            var instr = DisassembleWord(0x9A3FFE00);
            Assert.AreEqual("stb\tr17,-512(r31)", instr.ToString());
        }

        [Test]
        public void PPCDis_lhzx()
        {
            var instr = DisassembleWord(0x7F04022E);
            Assert.AreEqual("lhzx\tr24,r4,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzx()
        {
            var instr = DisassembleWord(0x7F0408AE);
            Assert.AreEqual("lbzx\tr24,r4,r1", instr.ToString());
        }

        [Test]
        public void PPCDis_mulli()
        {
            var instr = DisassembleWord(0x1F1F0003);
            Assert.AreEqual("mulli\tr24,r31,+0003", instr.ToString());
        }

        [Test]
        public void PPCDis_fadd()
        {
            var instr = DisassembleWord(0xFFF4402A);
            Assert.AreEqual("fadd\tf31,f20,f8", instr.ToString());
        }

        [Test]
        public void PPCDis_lbz()
        {
            var instr = DisassembleWord(0x88010203);
            Assert.AreEqual("lbz\tr0,515(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_stbux()
        {
            var instr = DisassembleWord(0x7CAA01EE);
            Assert.AreEqual("stbux\tr5,r10,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_stbu()
        {
            var instr = DisassembleWord(0x9C01FFEE);
            Assert.AreEqual("stbu\tr0,-18(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_lwarx()
        {
            var instr = DisassembleWord(0x7C720028);
            Assert.AreEqual("lwarx\tr3,r18,r0", instr.ToString());
        }

        [Test]
        public void PPCDis_lbzu()
        {
            var instr = DisassembleWord(0x8C0A0000);
            Assert.AreEqual("lbzu\tr0,0(r10)", instr.ToString());
        }

        [Test]
        public void PPCDis_xori()
        {
            var instr = DisassembleBits("011010 00001 00011 0101010101010101");
            Assert.AreEqual("xori\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xoris()
        {
            var instr = DisassembleBits("011011 00001 00011 0101010101010101");
            Assert.AreEqual("xoris\tr3,r1,5555", instr.ToString());
        }

        [Test]
        public void PPCDis_xor_()
        {
            var instr = DisassembleBits("011111 00010 00001 00011 0100111100 1");
            Assert.AreEqual("xor.\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void PPCDis_lhz()
        {
            var instr = DisassembleBits("101000 00010 00001 1111111111111000");
            Assert.AreEqual("lhz\tr2,-8(r1)", instr.ToString());
        }

        [Test]
        public void PPCDis_twi()
        {
            var instr = DisassembleBits("000011 00010 00001 1111111111111000");
            Assert.AreEqual("twi\t02,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_subfic()
        {
            var instr = DisassembleBits("001000 00010 00001 1111111111111000");
            Assert.AreEqual("subfic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_cmplwi()
        {
            var instr = DisassembleBits("001010 00010 00001 1111111111111000");
            Assert.AreEqual("cmplwi\tcr0,r1,FFF8", instr.ToString());
        }

        [Test]
        public void PPCDis_cmpi()
        {
            var instr = DisassembleBits("001011 00010 00001 1111111111111000");
            Assert.AreEqual("cmpwi\tcr0,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addic()
        {
            var instr = DisassembleBits("001100 00010 00001 1111111111111000");
            Assert.AreEqual("addic\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_addic_()
        {
            var instr = DisassembleBits("001101 00010 00001 1111111111111000");
            Assert.AreEqual("addic.\tr2,r1,-0008", instr.ToString());
        }

        [Test]
        public void PPCDis_sc()
        {
            var instr = DisassembleBits("010001 00010 00000 0000000000000010");
            Assert.AreEqual("sc", instr.ToString());
        }

        [Test]
        public void PPCDis_crnor()
        {
            var instr = DisassembleBits("010011 00001 00010 00011 00001000010");
            Assert.AreEqual("crnor\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_cror()
        {
            var instr = DisassembleBits("010011 00001 00010 00011 01110000010");
            Assert.AreEqual("cror\t01,02,03", instr.ToString());
        }

        [Test]
        public void PPCDis_rfi()
        {
            var instr = DisassembleBits("010011 00000 00000 00000 0 0001100100");
            Assert.AreEqual("rfi", instr.ToString());
        }

        [Test]
        public void PPCDis_andi_()
        {
            var instr = DisassembleBits("011100 00001 00011 1111110001100100");
            Assert.AreEqual("andi.\tr3,r1,FC64", instr.ToString());
        }

        [Test]
        public void PPCDis_andis_()
        {
            var instr = DisassembleBits("011101 00001 00011 1111110001100100");
            Assert.AreEqual("andis.\tr3,r1,FC64", instr.ToString());
        }

        [Test]
        public void PPCDis_cmp()
        {
            var instr = DisassembleBits("011111 01100 00001 00010 0000000000 0");
            Assert.AreEqual("cmp\tcr3,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_tw()
        {
            var instr = DisassembleBits("011111 01100 00001 00010 0000000100 0");
            Assert.AreEqual("tw\t0C,r1,r2", instr.ToString());
        }

        [Test]
        public void PPCDis_lmw()
        {
            var instr = DisassembleBits("101110 00001 00010 111111111110100 0");
            Assert.AreEqual("lmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_stmw()
        {
            var instr = DisassembleBits("101111 00001 00010 111111111110100 0");
            Assert.AreEqual("stmw\tr1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_lfs()
        {
            var instr = DisassembleBits("110000 00001 00010 111111111110100 0");
            Assert.AreEqual("lfs\tf1,-24(r2)", instr.ToString());
        }

        [Test]
        public void PPCDis_unknown()
        {
            var instr = base.DisassembleWord(0xEC6729D4);
            Assert.AreEqual("illegal", instr.ToString());
        }

        [Test]
        public void PPCDis_fpu_single_precision_instructions()
        {
            RunTest("fdivs.\tf1,f2,f3", "111011 00001 00010 00011 00000 10010 1");
            RunTest("fsubs.\tf1,f2,f3", "111011 00001 00010 00011 00000 10100 1");
            RunTest("fadds.\tf1,f2,f3", "111011 00001 00010 00011 00000 10101 1");
            RunTest("fsqrts.\tf1,f3", "111011 00001 00010 00011 00000 10110 1");
            RunTest("fres.\tf1,f3", "111011 00001 00010 00011 00000 11000 1");
            RunTest("fmuls.\tf1,f2,f4", "111011 00001 00010 00000 00100 11001 1");
            RunTest("fmsubs.\tf1,f2,f4,f3", "111011 00001 00010 00011 00100 11100 1");
            RunTest("fmadds.\tf1,f2,f4,f3", "111011 00001 00010 00011 00100 11101 1");
            RunTest("fnmsubs.\tf1,f2,f3,f4", "111011 00001 00010 00011 00100 11110 1");
            RunTest("fnmadds.\tf1,f2,f3,f4", "111011 00001 00010 00011 00100 11111 1");
        }

        [Test]
        public void PPCDis_mflr()
        {
            var instr = DisassembleWord(0x7C0802A6);
            Assert.AreEqual("mflr\tr0", instr.ToString());
        }

        [Test]
        public void PPCDis_add()
        {
            var instr = DisassembleWord(0x7c9a2214);
            Assert.AreEqual("add\tr4,r26,r4", instr.ToString());
        }

        [Test]
        public void PPCDis_mfcr()
        {
            var instr = DisassembleWord(0x7d800026);
            Assert.AreEqual("mfcr\tr12", instr.ToString());
        }

        private void AssertCode(uint instr, string sExp)
        {
            var i = DisassembleWord(instr);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void PPCDis_rlwinm()
        {
            AssertCode(0x5729103a, "rlwinm\tr9,r25,02,00,1D");
            AssertCode(0x57202036, "rlwinm\tr0,r25,04,00,1B");
        }

        [Test]
        public void PPCDis_lwzx()
        {
            AssertCode(0x7c9c002e, "lwzx\tr4,r28,r0");
        }

        [Test]
        public void PPCDis_stwx()
        {
            AssertCode(0x7c95012e, "stwx\tr4,r21,r0");
        }

        [Test]
        public void PPCDis_subf()
        {
            AssertCode(0x7c154850, "subf\tr0,r21,r9");
        }

        [Test]
        public void PPCDis_srawi()
        {
            AssertCode(0x7c002670, "srawi\tr0,r0,04");
        }

        [Test]
        public void PPCDis_bctr()
        {
            AssertCode(0x4e800420, "bcctr\t14,00");
        }

        [Test]
        public void PPCDis_stwux()
        {
            AssertCode(0x7d21016e, "stwux\tr9,r1,r0");
        }

        [Test]
        public void PPCDis_fmr()
        {
            AssertCode(0xFFE00890, "fmr\tf31,f1");
        }

        [Test]
        public void PPCDis_mtctr()
        {
            AssertCode(0x7d0903a6, "mtctr\tr8");
        }

        [Test]
        public void PPCDis_cmpl()
        {
            AssertCode(0x7f904840, "cmplw\tcr7,r16,r9");
        }

        [Test]
        public void PPCDis_neg()
        {
            AssertCode(0x7c0000d0, "neg\tr0,r0");
        }

        [Test]
        public void PPCDis_cntlzw()
        {
            AssertCode(0x7d4a0034, "cntlzw\tr10,r10");
        }

        [Test]
        public void PPCDis_fsub()
        {
            AssertCode(0xfc21f828, "fsub\tf1,f1,f31");
        }

        [Test]
        public void PPCDis_li()
        {
            AssertCode(0x38000000, "addi\tr0,r0,+0000");
        }

        [Test]
        public void PPCDis_addze()
        {
            AssertCode(0x7c000194, "addze\tr0,r0");
        }

        [Test]
        public void PPCDis_slw()
        {
            AssertCode(0x7d400030, "slw\tr0,r10,r0");
        }

        [Test]
        public void PPCDis_fctiwz()
        {
            AssertCode(0xfc00081e, "fctiwz\tf0,f1");
        }
        [Test]
        public void PPCDis_fmul()
        {
            AssertCode(0xfc010032, "fmul\tf0,f1,f0");
        }
        [Test]
        public void PPCDis_fcmpu()
        {
            AssertCode(0xff810000, "fcmpu\tcr7,f1,f0");
        }
        [Test]
        public void PPCDis_mtcrf()
        {
            AssertCode(0x7d808120, "mtcrf\t08,r12");
        }

        [Test]
        public void PPCDis_bctrl()
        {
            AssertCode(0x4e800421, "bctrl\t14,00");
        }

        [Test]
        public void PPCDis_rlwimi()
        {
            AssertCode(0x5120f042, "rlwimi\tr0,r9,1E,01,01");
        }

        [Test]
        public void PPCDis_cror_2()
        {
            AssertCode(0x4fddf382, "cror\t1E,1D,1E");
        }

        [Test]
        public void PPCDis_add_()
        {
            var instr = DisassembleWord(0x7c9a2215);
            Assert.AreEqual("add.\tr4,r26,r4", instr.ToString());
        }

        [Test]
        public void PPCDis_cmpwi()
        {
            AssertCode(0x2f830005, "cmpwi\tcr7,r3,+0005");
        }

        [Test]
        public void PPCDis_bcXX()
        {
            AssertCode(0x40bc011c, "bge\tcr7,$0010011C");
            AssertCode(0x40bd011c, "ble\tcr7,$0010011C");
            AssertCode(0x40be011c, "bne\tcr7,$0010011C");
            AssertCode(0x40bf011c, "bns\tcr7,$0010011C");
            AssertCode(0x41bc011c, "blt\tcr7,$0010011C");
            AssertCode(0x41bd011c, "bgt\tcr7,$0010011C");
            AssertCode(0x41be011c, "beq\tcr7,$0010011C");
            AssertCode(0x41bf011c, "bso\tcr7,$0010011C");
        }

        [Test]
        public void PPCDis_nor()
        {
            AssertCode(0x7c6318f8, "nor\tr3,r3,r3");
        }

        [Test]
        public void PPCDis_regression3()
        {
            AssertCode(0xfc000050, "fneg\tf0,f0");
            AssertCode(0xfc0062fa, "fmadd\tf0,f0,f11,f12");
            AssertCode(0x4cc63242, "creqv\t06,06,06");
            //AssertCode(0x4e080000, "mcrf\tcr4,cr2");
            AssertCode(0x7c684430, "srw\tr8,r3,r8");
            AssertCode(0x7cd9a810, "subfc\tr6,r25,r21");
            AssertCode(0x7c7ef038, "and\tr30,r3,r30");
            AssertCode(0x7ce03896, "mulhw\tr7,r0,r7");
            AssertCode(0x7d5be016, "mulhwu\tr10,r27,r28");
            AssertCode(0x7d3d03d6, "divw\tr9,r29,r0");
            AssertCode(0x7fda0016, "mulhwu\tr30,r26,r0");
            AssertCode(0x7c1ee8ee, "lbzux\tr0,r30,r29");
            AssertCode(0x7c0bd039, "and.\tr11,r0,r26");
            AssertCode(0x7fde0190, "subfze\tr30,r30");
            AssertCode(0x7c03fbd6, "divw\tr0,r3,r31");
            AssertCode(0x7c040096, "mulhw\tr0,r4,r0");
            AssertCode(0x7c000774, "extsb\tr0,r0");
            AssertCode(0x7c00252c, "stwbrx\tr0,r0,r4");
            AssertCode(0x7d080190, "subfze\tr8,r8");
            AssertCode(0x7d4a5110, "subfe\tr10,r10,r10");
            AssertCode(0x7c000775, "extsb.\tr0,r0");
            AssertCode(0x7c631910, "subfe\tr3,r3,r3");
            AssertCode(0x7c880039, "and.\tr8,r4,r0");
            AssertCode(0x7d605896, "mulhw\tr11,r0,r11");
            AssertCode(0x7e310038, "and\tr17,r17,r0");
            AssertCode(0x7e601c2c, "lwbrx\tr19,r0,r3");
            AssertCode(0xfdad02f2, "fmul\tf13,f13,f11");
        }

        [Test]
        public void PPCDis_sraw()
        {
            AssertCode(0x7c052e30, "sraw\tr5,r0,r5");
        }

        [Test]
        public void PPCDis_64bit()
        {
            AssertCode(0xf8410028, "std\tr2,40(r1)");
            AssertCode(0xebe10078, "ld\tr31,120(r1)");
            AssertCode(0x7fa307b4, "extsw\tr3,r29");
        }

        [Test]
        public void PPCDis_rldicl()
        {
            AssertCode(0x790407c0, "rldicl\tr4,r8,00,1F");
            AssertCode(0x790407E0, "rldicl\tr4,r8,00,3F");
            AssertCode(0x7863e102, "rldicl\tr3,r3,3C,04");
        }

        [Test]
        public void PPCDis_More64()
        {
            AssertCode(0xfd600018, "frsp\tf11,f0");
            AssertCode(0xec1f07ba, "fmadds\tf0,f31,f30,f0");
            AssertCode(0xec216824, "fdivs\tf1,f1,f13");
            AssertCode(0x7c4048ce, "lvx\tv2,r0,r9");
            AssertCode(0x4d9e0020, "beqlr\tcr7");
            AssertCode(0x10601a8c, "vspltw\tv3,v3,00");
            AssertCode(0x100004c4, "vxor\tv0,v0,v0");
            AssertCode(0x5c00c03e, "rlwnm\tr0,r0,r24,00,1F");
            AssertCode(0x4c9d0020, "blelr\tcr7");
            AssertCode(0x7c00222c, "dcbt\tr0,r4,00");
            AssertCode(0x7c0004ac, "sync");
            AssertCode(0x7c00f078, "andc\tr0,r0,r30");
            AssertCode(0x7c005836, "sld\tr0,r0,r11");
            AssertCode(0x7c0bfe76, "sradi\tr11,r0,3F");
            AssertCode(0x7c0a31d2, "mulld\tr0,r10,r6");
            AssertCode(0x7c07492a, "stdx\tr0,r7,r9");
        }

        [Test]
        public void PPCDis_lvlx()
        {
            AssertCode(0x7c6b040e, "lvlx\tr3,r11,r0");
        }

        [Test]
        public void PPCDis_bcctr()
        {
            AssertCode(0x4000fef8, "bdnzf\tlt,$000FFEF8");
            AssertCode(0x4040fef8, "bdzf\tlt,$000FFEF8");
            AssertCode(0x4080fef8, "bge\t$000FFEF8");
            AssertCode(0x4100fef8, "bdnzt\tlt,$000FFEF8");
            AssertCode(0x4180fef8, "blt\t$000FFEF8");

            AssertCode(0x4200fef8, "bdnz\t$000FFEF8");
            AssertCode(0x4220fef9, "bdnzl\t$000FFEF8");
            AssertCode(0x4240fef8, "bdz\t$000FFEF8");
            AssertCode(0x4260fef9, "bdzl\t$000FFEF8");
            //AssertCode(0x4280fef8, "bc+    20,lt,0xffffffffffffff24	 ");
            AssertCode(0x4300fef8, "bdnz\t$000FFEF8");
        }

        [Test]
        public void PPCDis_mftb()
        {
            AssertCode(0x7eac42e6, "mftb\tr21,0188");
        }

        [Test]
        public void PPCDis_stvx()
        {
            AssertCode(0x7c2019ce, "stvx\tv1,r0,r3");
        }

        [Test]
        public void PPCDis_stfiwx()
        {
            AssertCode(0x7c004fae, "stfiwx\tf0,r0,r9");
        }

        [Test]
        public void PPCDis_cntlzd()
        {
            AssertCode(0x7d600074, "cntlzd\tr0,r11");
        }

        [Test]
        public void PPCDis_vectorops()
        {
            AssertCode(0x10c6600a, "vaddfp\tv6,v6,v12");
            AssertCode(0x10000ac6, "vcmpgtfp\tv0,v0,v1");
            AssertCode(0x118108c6, "vcmpeqfp\tv12,v1,v1");
            AssertCode(0x10ed436e, "vmaddfp\tv7,v13,v13,v8");
            AssertCode(0x10a9426e, "vmaddfp\tv5,v9,v9,v8");
            AssertCode(0x10200a8c, "vspltw\tv1,v1,00");
            AssertCode(0x1160094a, "vrsqrtefp\tv11,v1");
            AssertCode(0x102b406e, "vmaddfp\tv1,v11,v1,v8");
            AssertCode(0x102bf06f, "vnmsubfp\tv1,v11,v1,v30");
            AssertCode(0x1020014a, "vrsqrtefp\tv1,v0");
            AssertCode(0x116b0b2a, "vsel\tv11,v11,v1,v12");
            AssertCode(0x1000012c, "vsldoi\tv0,v0,v0,04");
            AssertCode(0x101f038c, "vspltisw\tv0,-01");
            AssertCode(0x1000028c, "vspltw\tv0,v0,00");
            AssertCode(0x114948ab, "vperm\tv10,v9,v9,v2");
            AssertCode(0x112c484a, "vsubfp\tv9,v12,v9");
            AssertCode(0x118000c6, "vcmpeqfp\tv12,v0,v0");
            AssertCode(0x11ad498c, "vmrglw\tv13,v13,v9");
            AssertCode(0x118c088c, "vmrghw\tv12,v12,v1");
            AssertCode(0x125264c4, "vxor\tv18,v18,v12");
        }

        [Test]
        public void PPCDis_regression4()
        {
            //AssertCode(0x10000ac6, "vcmpgtfp\tv0,v0,v1");
            AssertCode(0xec0c5038, "fmsubs\tf0,f12,f0,f10");
            AssertCode(0x7c20480c, "lvsl\tv1,r0,r9");
            AssertCode(0x1000fcc6, "vcmpeqfp.\tv0,v0,v31");
            AssertCode(0x10c63184, "vslw\tv6,v6,v6");
            AssertCode(0x10e73984, "vslw\tv7,v7,v7");
            AssertCode(0x7c01008e, "lvewx\tv0,r1,r0");

            AssertCode(0x11a0010a, "vrefp\tv13,v0");
            AssertCode(0x10006e86, "vcmpgtuw.\tv0,v0,v13");
            AssertCode(0x7c00418e, "stvewx\tv0,r0,r8");
            AssertCode(0x118063ca, "vctsxs\tv12,v12,00");
            AssertCode(0x1020634a, "vcfsx\tv1,v12,00");
            AssertCode(0x118c0404, "vand\tv12,v12,v0");
            AssertCode(0x116c5080, "vadduwm\tv11,v12,v10");
            AssertCode(0x110c5404, "vand\tv8,v12,v10");
            AssertCode(0x1021ac44, "vandc\tv1,v1,v21");
            AssertCode(0x11083086, "vcmpequw\tv8,v8,v6");
        }

        [Test]
        public void PPCDis_lfsx()
        {
            AssertCode(0x7c01042e, "lfsx\tf0,r1,r0");
        }
        
        [Test]
        public void PPCDis_mffs()
        {
            AssertCode(0xfc00048e, "mffs\tf0");
        }

        [Test]
        public void PPCDis_mtfsf()
        {
            AssertCode(0xfdfe058e, "mtfsf\tFF,f0");
        }
    }
}
