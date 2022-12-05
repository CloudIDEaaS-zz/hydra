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

using Decompiler.Arch.M68k;
using Decompiler.Assemblers.M68k;
using Decompiler.Core;
using Decompiler.Core.Rtl;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.M68k
{
    [TestFixture]
    public class RewriterTests : RewriterTestBase
    {
        private M68kArchitecture arch = new M68kArchitecture();
        private Address addrBase = Address.Ptr32(0x00010000);
        private LoadedImage image;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return addrBase; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(Frame frame, IRewriterHost host)
        {
            return arch.CreateRewriter(image.CreateLeReader(0), arch.CreateProcessorState(), arch.CreateFrame(), host);
        }

        private void Rewrite(params ushort[] opcodes)
        {
            byte[] bytes = new byte[opcodes.Length * 2];
            var writer = new BeImageWriter(bytes);
            foreach (ushort opcode in opcodes)
            {
                writer.WriteBeUInt16(opcode);
            }
            image = new LoadedImage(addrBase, bytes);
        }

        private void Rewrite(Action<M68kAssembler> build)
        {
            var asm = new M68kAssembler(arch, addrBase, new List<EntryPoint>());
            build(asm);
            image = asm.GetImage().Image;
        }

        [Test]
        public void M68krw_Movea_l()
        {
            Rewrite(0x2261);        // movea.l   (a1)-,a1
            AssertCode(
                "0|00010000(2): 2 instructions",
                "1|L--|a1 = a1 - 0x00000004",
                "2|L--|a1 = Mem0[a1:word32]");
        }

        [Test]
        public void M68krw_Eor_b()
        {
            Rewrite(0xB103);        // eorb %d0,%d3
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|v4 = (byte) d3 ^ (byte) d0",
                "2|L--|d3 = DPB(d3, v4, 0, 8)",
                "3|L--|ZN = cond(v4)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_Eor_l()
        {
            Rewrite(0xB183);        // eorl %d0,%d3
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|d3 = d3 ^ d0",
                "2|L--|ZN = cond(d3)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_Ext()
        {
            Rewrite(0x4884, 0x48C4, 0x49C4);
            AssertCode(
                "0|00010000(2): 2 instructions",
                "1|L--|d4 = (int16) (int8) d4",
                "2|L--|ZN = cond(d4)",
                "3|00010002(2): 2 instructions",
                "4|L--|d4 = (int32) (int16) d4",
                "5|L--|ZN = cond(d4)",
                "6|00010004(2): 2 instructions",
                "7|L--|d4 = (int32) (int8) d4",
                "8|L--|ZN = cond(d4)");
        }

        [Test]
        public void M68krw_adda_postinc() // addal (a4)+,%a5
        {
            Rewrite(0xDBDC);
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = Mem0[a4:word32]",
                "2|L--|a4 = a4 + 0x00000004",
                "3|L--|a5 = a5 + v3");
        }

        [Test]
        public void M68krw_or_imm()
        {
            Rewrite(0x867c, 0x1123);    // or.w #$1123,d3
            AssertCode(
                "0|00010000(4): 5 instructions",
                "1|L--|v3 = (word16) d3 | 0x1123",
                "2|L--|d3 = DPB(d3, v3, 0, 16)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_movew_indirect()
        {
            Rewrite(0x3410);    // move.w (A0),D2
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v4 = Mem0[a0:word16]",
                "2|L--|d2 = DPB(d2, v4, 0, 16)",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_move_pre_and_postdec()
        {
            Rewrite(0x36E3);    // move.w -(a3),(a3)+
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|a3 = a3 - 0x00000002",
                "2|L--|v3 = Mem0[a3:word16]",
                "3|L--|Mem0[a3:word16] = v3",
                "4|L--|a3 = a3 + 0x00000002",
                "5|L--|CVZN = cond(v3)");
        }

        [Test]
        public void M68krw_muls_w()
        {
            Rewrite(0xC1E3); // muls.w -(a3),r3
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|a3 = a3 - 0x00000002",
                "2|L--|d0 = d0 *s Mem0[a3:word16]",
                "3|L--|VZN = cond(d0)",
                "4|L--|C = false");
        }

        [Test]
        public void M68krw_mulu_l()
        {
            Rewrite(0x4c00, 0x7406); // mulu.l d0,d6,d7
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|d6_d7 = d7 *u d0",
                "2|L--|VZN = cond(d6_d7)",
                "3|L--|C = false");
        }

        [Test]
        public void M68krw_not_w()
        {
            Rewrite(0x4643); // not.w d3
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|v3 = ~(word16) d3",
                "2|L--|d3 = DPB(d3, v3, 0, 16)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        public void M68krw_not_l_reg()
        {
            Rewrite(0x4684);    // not.l d4
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|d4 = ~d4",
                "2|L--|ZN = cond(d4)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_not_l_pre()
        {
            Rewrite(0x46A4);    // not.l -(a4)
            AssertCode(
                "0|00010000(2): 6 instructions",
                "1|L--|a4 = a4 - 0x00000004",
                "2|L--|v3 = ~Mem0[a4:word32]",
                "3|L--|Mem0[a4:word32] = v3",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_and_re()
        {
            Rewrite(0xC363);    // and.w d1,-(a3)
            AssertCode(
                "0|00010000(2): 6 instructions",
                "1|L--|a3 = a3 - 0x00000002",
                "2|L--|v4 = Mem0[a3:word16] & (word16) d1",
                "3|L--|Mem0[a3:word16] = v4",
                "4|L--|ZN = cond(v4)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_andi_32()
        {
            Rewrite(0x029C, 0x0001, 0x0000);    // and.l #00010000,(a4)+
            AssertCode(
                "0|00010000(6): 6 instructions",
                "1|L--|v3 = Mem0[a4:word32] & 0x00010000",
                "2|L--|Mem0[a4:word32] = v3",
                "3|L--|a4 = a4 + 0x00000004",
                "4|L--|ZN = cond(v3)",
                "5|L--|C = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_andi_8()
        {
            Rewrite(0x0202, 0x00F0);     // and.l #F0,d2"
            AssertCode(
                "0|00010000(4): 5 instructions",
                "1|L--|v3 = (byte) d2 & 0xF0",
                "2|L--|d2 = DPB(d2, v3, 0, 8)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_asrb_qb()
        {
            Rewrite(0xEE00);        // asr.b\t#7,d0
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = (byte) d0 >> 0x07",
                "2|L--|d0 = DPB(d0, v3, 0, 8)",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_post()
        {
            Rewrite(0x445B);
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|v3 = -Mem0[a3:word16]",
                "2|L--|Mem0[a3:word16] = v3",
                "3|L--|a3 = a3 + 0x00000002",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_neg_w_mem()
        {
            Rewrite(0x4453);
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = -Mem0[a3:word16]",
                "2|L--|Mem0[a3:word16] = v3",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_negx_8()
        {
            Rewrite(0x4021);        // negx.b -(a1)

            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|a1 = a1 - 0x00000001",
                "2|L--|v3 = -Mem0[a1:byte] - X",
                "3|L--|Mem0[a1:byte] = v3",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_er_16()
        {
            Rewrite(0x9064);        // sub.w -(a4),d0
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|a4 = a4 - 0x00000002",
                "2|L--|v4 = (word16) d0 - Mem0[a4:word16]",
                "3|L--|d0 = DPB(d0, v4, 0, 16)",
                "4|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_suba_16()
        {
            Rewrite(0x90DC);      // suba.w (a4)+,a0
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|v3 = Mem0[a4:word16]",
                "2|L--|a4 = a4 + 0x00000002",
                "3|L--|v5 = (word16) a0 - v3",
                "4|L--|a0 = DPB(a0, v5, 0, 16)");
        }

        [Test]
        public void M68krw_clrw_ea_off()
        {
            Rewrite(0x4268, 0xFFF8);    // clr.w\t$0008(a0)
            AssertCode(
                "0|00010000(4): 5 instructions",
                "1|L--|Mem0[a0 + -8:word16] = 0x0000",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68k_clrw_reg()
        {
            Rewrite(0x4240);        // clr.w\td0
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|d0 = DPB(d0, 0x0000, 0, 16)",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_clrb_idx()
        {
            Rewrite(0x4230, 0x0800);
            AssertCode("0|00010000(4): 5 instructions",
                "1|L--|Mem0[a0 + d0:byte] = 0x00",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_clrl_postInc()
        {
            Rewrite(0x4298);
            AssertCode("0|00010000(2): 6 instructions",
                "1|L--|Mem0[a0:word32] = 0x00000000",
                "2|L--|a0 = a0 + 0x00000004",
                "3|L--|Z = true",
                "4|L--|C = false",
                "5|L--|N = false",
                "6|L--|V = false");
        }

        [Test]
        public void M68krw_cmpib_d()
        {
            Rewrite(0x0C18, 0x0042);    // cmpi.b #$42,(a0)+
            AssertCode(
                "0|00010000(4): 4 instructions",
                "1|L--|v3 = Mem0[a0:byte]",
                "2|L--|a0 = a0 + 0x00000001",
                "3|L--|v4 = v3 - 0x42",
                "4|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_d()
        {
            Rewrite(0xB041);        // cmp.w d1,d0
            AssertCode(
                "0|00010000(2): 2 instructions",
                "1|L--|v4 = (word16) d0 - (word16) d1",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpw_pre_pre()
        {
            Rewrite(0xB066);        // cmp.w -(a6),d0
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|a6 = a6 - 0x00000002",
                "2|L--|v4 = (word16) d0 - Mem0[a6:word16]",
                "3|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpaw()
        {
            Rewrite(0xB0EC, 0x0022);    // cmpa.w $22(a4),a0
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|v4 = (word16) a0 - Mem0[a4 + 34:word16]",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_cmpal()
        {
            Rewrite(0xB1EC, 0x0010);    // cmpa.l $10(a4),a0
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|v4 = a0 - Mem0[a4 + 16:word32]",
                "2|L--|CVZN = cond(v4)");
        }

        [Test]
        public void M68krw_jsr_mem()
        {
            Rewrite(0x4E90);    // jsr (a0)
            AssertCode(
                "0|00010000(2): 1 instructions",
                "1|T--|call a0 (4)");
        }

        [Test]
        public void M68krw_jsr()
        {
            Rewrite(
                0x4EB9, 0x0018, 0x5050, // jsr $00185050
                0x4EB8, 0xFFFA);        // jsr $FFFFFFFA
            AssertCode(
                "0|00010000(6): 1 instructions",
                "1|T--|call 00185050 (4)",
                "2|00010006(4): 1 instructions",
                "3|T--|call 0000FFFA (4)");
        }

        [Test]
        public void M68krw_or_rev()
        {
            Rewrite(0x81A8, 0xFFF8);
            AssertCode(
                "0|00010000(4): 5 instructions",
                "1|L--|v4 = Mem0[a0 + -8:word32] | d0",
                "2|L--|Mem0[a0 + -8:word32] = v4",
                "3|L--|ZN = cond(v4)");
        }

        [Test]
        public void M68krw_lsl_w()
        {
            Rewrite(0xE148);    // lsl.w #$01,d0"
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = (word16) d0 << 0x0008",
                "2|L--|d0 = DPB(d0, v3, 0, 16)",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subiw()
        {
            Rewrite(0x0440, 0x0140);    // subiw #320,%d0
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|v3 = (word16) d0 - 0x0140",
                "2|L--|d0 = DPB(d0, v3, 0, 16)",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_sub_re()
        {
            Rewrite(0x919F);    // sub.l\td0,(a7)+
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|v4 = Mem0[a7:word32] - d0",
                "2|L--|Mem0[a7:word32] = v4",
                "3|L--|a7 = a7 + 0x00000004",
                "4|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_subq_w()
        {
            Rewrite(0x5F66);    // subq.w\t#$07,-(a6)
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|a6 = a6 - 0x00000002",
                "2|L--|v3 = Mem0[a6:word16] - 0x0007",
                "3|L--|Mem0[a6:word16] = v3",
                "4|L--|CVZNX = cond(v3)");
            Rewrite(0x5370, 0x1034);    // subq.w\t#$01,(34,a0,d1)
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|v4 = Mem0[a0 + 52 + d1:word16] - 0x0001",
                "2|L--|Mem0[a0 + 52 + d1:word16] = v4",
                "3|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_rts()
        {
            Rewrite(0x4E75);    // rts
            AssertCode(
                "0|00010000(2): 1 instructions",
                "1|T--|return (4,0)");
        }

        [Test]
        public void M68krw_asr_ea()
        {
            Rewrite(0xE0E5);    // asr.w\t-(a5)
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|a5 = a5 - 0x00000002",
                "2|L--|v3 = Mem0[a5:word16] >> 1",
                "3|L--|Mem0[a5:word16] = v3",
                "4|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subx_mm()
        {
            Rewrite(0x9149);   // subx.w\t-(a1),-(a0)
            AssertCode(
                "0|00010000(2): 6 instructions",
                "1|L--|a1 = a1 - 0x00000002",
                "2|L--|v4 = Mem0[a1:word16]",
                "3|L--|a0 = a0 - 0x00000002",
                "4|L--|v5 = Mem0[a0:word16] - v4 - X",
                "5|L--|Mem0[a0:word16] = v5",
                "6|L--|CVZNX = cond(v5)");
        }

        [Test]
        public void M68krw_lsl_ea()
        {
            Rewrite(0xE3D1);    // lsl.w\t(a1)
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = Mem0[a1:word16] << 1",
                "2|L--|Mem0[a1:word16] = v3",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_lsl_r()
        {
            Rewrite(0xE36C);    // lsl.w\td1,d4
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v4 = (word16) d4 << (word16) d1",
                "2|L--|d4 = DPB(d4, v4, 0, 16)",
                "3|L--|CVZNX = cond(v4)");
        }

        [Test]
        public void M68krw_asl_w()
        {
            Rewrite((m) => { m.Asl_l(3, m.d1); });   // asl.l #$03,d0"
            AssertCode(
                "0|00010000(2): 2 instructions",
                "1|L--|d1 = d1 << 0x00000003",
                "2|L--|CVZNX = cond(d1)");
        }

        [Test]
        [Ignore("Hard to fit into the existing structure.")]
        public void M68krw_bchg_s()
        {
            Rewrite((m) => { m.Bchg(3, m.Mem(m.a0)); });    // bchg #3,(a0)
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|v2 = 0x00000001 << 0x00000003",
                "2|L--|v4 = Mem0[a0:word32]",
                "3|L--|Mem0[a0] = v4 ^ v5",
                "4|L--|Z = cond(v4 & v5)");
        }

        [Test]
        public void M68krw_dbra()
        {
            Rewrite(0x51CD, 0xFFFA);        // dbra -$6
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|d5 = d5 - 0x00000001",
                "2|T--|if (d5 != 0xFFFFFFFF) branch 0000FFFC");
        }

        [Test]
        public void M68krw_dble()
        {
            Rewrite(0x5FCF, 0xFFFA);
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|T--|if (Test(GT,VZN)) branch 00010004",
                "2|L--|d7 = d7 - 0x00000001",
                "3|T--|if (d7 != 0xFFFFFFFF) branch 0000FFFC");
        }

        [Test]
        public void M68krw_unlk()
        {
            Rewrite(0x4E5D);
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|a7 = a5",
                "2|L--|a5 = Mem0[a7:word32]",
                "3|L--|a7 = a7 + 0x00000004");
        }

        [Test]
        public void M68krw_link()
        {
            Rewrite(0x4E52, 0xFFF8);
            AssertCode(
                "0|00010000(4): 4 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a2",
                "3|L--|a2 = a7",
                "4|L--|a7 = a7 - 0x00000008");
        }

        [Test]
        public void M68krw_link_32()
        {
            Rewrite(0x480B, 0xFFFE, 0x0104);
            AssertCode(
                "0|00010000(6): 4 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a3",
                "3|L--|a3 = a7",
                "4|L--|a7 = a7 - 0x0001FEFC");
        }

        [Test]
        public void M68krw_movem_pop()
        {
            Rewrite(0x4CDF, 0x4C04);
            AssertCode(
                "0|00010000(4): 8 instructions",
                "1|L--|d2 = Mem0[a7:word32]",
                "2|L--|a7 = a7 + 0x00000004",
                "3|L--|a2 = Mem0[a7:word32]",
                "4|L--|a7 = a7 + 0x00000004",
                "5|L--|a3 = Mem0[a7:word32]",
                "6|L--|a7 = a7 + 0x00000004",
                "7|L--|a6 = Mem0[a7:word32]",
                "8|L--|a7 = a7 + 0x00000004");
        }

        [Test]
        public void M68krw_bra()
        {
            Rewrite(0x6008);
            AssertCode(
                "0|00010000(2): 1 instructions",
                "1|T--|goto 0001000A");
        }

        [Test]
        public void M68krw_lea_direct()
        {
            Rewrite(0x49f9, 0x0000, 0x7ffe);
            AssertCode(
                "0|00010000(6): 1 instructions",
                "1|L--|a4 = 00007FFE");
        }

        [Test]
        public void M68krw_lea_mem()
        {
            Rewrite(0x43EB, 0xFFFE);
            AssertCode(
                "0|00010000(4): 1 instructions",
                "1|L--|a1 = a3 + -2");
        }

        [Test]
        public void M68krw_bcc()
        {
            Rewrite(0x6438, 0x6636);
            AssertCode(
                "0|00010000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0001003A",
                "2|00010002(2): 1 instructions",
                "3|T--|if (Test(NE,Z)) branch 0001003A");
        }

        [Test]
        public void M68krw_addq_d()
        {
            Rewrite(0x5401);
            AssertCode(
                "0|00010000(2): 3 instructions",
                "1|L--|v3 = (byte) d1 + 0x02",
                "2|L--|d1 = DPB(d1, v3, 0, 8)",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_subq_a()
        {
            Rewrite(0x5549);
            AssertCode(
                "0|00010000(2): 1 instructions",
                "1|L--|a1 = a1 - 0x0002");
        }

        [Test]
        public void M68krw_moveq()
        {
            Rewrite(0x72FF);
            AssertCode(
                "0|00010000(2): 2 instructions",
                "1|L--|d1 = -1",
                "2|L--|CVZN = cond(d1)");
        }

        [Test]
        public void M68krw_lea_pc()
        {
            Rewrite(0x45FA, 0x0012);
            AssertCode(
                "0|00010000(4): 1 instructions",
                "1|L--|a2 = 00010014");
        }

        [Test]
        public void M68krw_tst()
        {
            Rewrite(0x4ABA, 0x0124);
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|ZN = cond(Mem0[0x00010126:word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_pea()
        {
            Rewrite(0x486A, 0x0004);
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|a7 = a7 - 0x00000004",
                "2|L--|Mem0[a7:word32] = a2 + 4");
        }

        [Test]
        public void M68krw_IndirectIndexed()
        {
            Rewrite(0x4AB3, 0x0000);
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|ZN = cond(Mem0[a3 + d0:word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_Swap()
        {
            Rewrite(0x4847);
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|d7 = __swap(d7)",
                "2|L--|ZN = cond(d7)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void M68krw_Clr_d1()
        {
            Rewrite(0x4241);
            AssertCode(
                "0|00010000(2): 5 instructions",
                "1|L--|d1 = DPB(d1, 0x0000, 0, 16)",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_ori()
        {
            Rewrite(0x0038, 0x584F, 0x4000);
            AssertCode(
                "0|00010000(6): 5 instructions",
                "1|L--|v2 = Mem0[0x00004000:byte] | 0x4F",
                "2|L--|Mem0[0x00004000:byte] = v2",
                "3|L--|ZN = cond(v2)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_addx()
        {
            Rewrite(0xD38D);
            AssertCode(
                "0|00010000(2): 6 instructions",
                "1|L--|a5 = a5 - 0x00000004",
                "2|L--|v5 = Mem0[a5:word32]",
                "3|L--|a1 = a1 - 0x00000004",
                "4|L--|v6 = v5 + Mem0[a1:word32] + X",
                "5|L--|Mem0[a1:word32] = v6",
                "6|L--|CVZNX = cond(v6)");
        }

        [Test]
        public void M68krw_movem_to_reg()
        {
            Rewrite(0x4cef, 0x0003, 0x0030);
            AssertCode(
                "0|00010000(6): 5 instructions",
                "1|L--|v3 = a7 + 48",
                "2|L--|d0 = Mem0[v3:word32]",
                "3|L--|v3 = v3 + 0x00000004",
                "4|L--|d1 = Mem0[v3:word32]");
        }

        [Test]
        [Obsolete("//$REVIEW: this is BROKEN")]
        public void M68krw_divu_w()
        {
            Rewrite(0x80C1);
            AssertCode(
                "0|00010000(2): 6 instructions",
                "1|L--|v3 = (uint16) (d0 % (uint16) d1)",
                "2|L--|v4 = (uint16) (d0 /u (uint16) d1)",
                "3|L--|d0 = DPB(d0, v3, 16, 16)",
                "4|L--|d0 = DPB(d0, v4, 0, 16)",
                "5|L--|VZN = cond(v4)",
                "6|L--|C = false");
        }

        [Test]
        public void M68krw_rol()
        {
            Rewrite(0xE199);
            AssertCode(
               "0|00010000(2): 3 instructions",
               "1|L--|d1 = __rol(d1, 0x00000008)",
               "2|L--|CZN = cond(d1)",
               "3|L--|V = false");
        }

        [Test]
        public void M68krw_roxl()
        {
            Rewrite(0xE391);
            AssertCode(
               "0|00010000(2): 3 instructions",
               "1|L--|d1 = __rcl(d1, 0x00000001, X)",
               "2|L--|CZNX = cond(d1)",
               "3|L--|V = false");
        }

        [Test]
        public void M68krw_st()
        {
            Rewrite(0x50EF, 0x0002);
            AssertCode(
               "0|00010000(4): 1 instructions",
               "1|L--|Mem0[a7 + 2:bool] = true");
        }

        [Test]
        public void M68krw_tst_mem()
        {
            Rewrite(0x4AB9, 0x0000, 0x13F8);
            AssertCode(
                "0|00010000(6): 3 instructions",
                "1|L--|ZN = cond(Mem0[0x000013F8:word32] - 0x00000000)",
                "2|L--|C = false",
                "3|L--|V = false");
        }

        [Test]
        public void M68krw_rorx()
        {
            Rewrite(0xE014);
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|v4 = __rcr((byte) d4, 0x08, X)",
                "2|L--|d4 = DPB(d4, v4, 0, 8)",
                "3|L--|CZNX = cond(v4)");
        }

        [Test]
        public void M68krw_ror_ea()
        {
            Rewrite(0xE6D4);
            AssertCode(
                "0|00010000(2): 4 instructions",
                "1|L--|v3 = __ror(Mem0[a4:word32], 0x01)",
                "2|L--|Mem0[a4:word32] = v3",
                "3|L--|CZN = cond(v3)");
        }

        [Test]
        public void M68krw_jsr_pc()
        {
            Rewrite(0x4EBA, 0x0030);
            AssertCode(
                "0|00010000(4): 1 instructions",
                "1|T--|call 00010032 (4)");
        }

        [Test]
        public void M68krw_clr_addr()
        {
            Rewrite(0x42B9, 0x0000, 0x15E8);
            AssertCode(
                "0|00010000(6): 5 instructions",
                "1|L--|Mem0[0x000015E8:word32] = 0x00000000",
                "2|L--|Z = true",
                "3|L--|C = false",
                "4|L--|N = false",
                "5|L--|V = false");
        }



        [Test]
        public void M68krw_bset_addr()
        {
            Rewrite(0x08e8, 0x0001, 0x0010); //                 bset #1,%a0@(1)
            AssertCode(
                "0|00010000(6): 1 instructions",
                "1|L--|Z = __bset(Mem0[a0 + 16:byte], 0x0001, out Mem0[a0 + 16:byte])");
            //.data:00000006 08 a8 00 00 00 10                bclr #0,%a0@(16)
        }

        [Test]
        public void M68krw_bclr_addr()
        {
            Rewrite(0x08A8, 0x0001, 0x0010); //                 bclr #1,%a0@(1)
            AssertCode(
                "0|00010000(6): 1 instructions",
                "1|L--|Z = __bclr(Mem0[a0 + 16:byte], 0x01, out Mem0[a0 + 16:byte])");
            //.data:00000006 08 a8 00 00 00 10                bclr #0,%a0@(16)
        }

        [Test]
        public void M68krw_addi()
        {
            Rewrite(0x0646, 0x000F);            // addiw #15,%d6
            AssertCode(
                "0|00010000(4): 3 instructions",
                "1|L--|v3 = (word16) d6 + 0x000F",
                "2|L--|d6 = DPB(d6, v3, 0, 16)",
                "3|L--|CVZNX = cond(v3)");
        }

        [Test]
        public void M68krw_eori()
        {
            Rewrite(0x0A40, 0x000F);     //                    eoriw #15,%d0    
            AssertCode(
                "0|00010000(4): 5 instructions",
                "1|L--|v3 = (word16) d0 ^ 0x000F",
                "2|L--|d0 = DPB(d0, v3, 0, 16)",
                "3|L--|ZN = cond(v3)",
                "4|L--|C = false",
                "5|L--|V = false");
        }

        [Test]
        public void M68krw_address_mode()
        {
            Rewrite(0x2432, 0x04fc);    // move.l\t(-04,a2,d0*2),d2",
            AssertCode(
                "0|00010000(4): 2 instructions",
                "1|L--|d2 = Mem0[a2 + -4 + d0 * 2:word32]"
                );

        }

        [Test]
        public void M68krw_movem()
        {
            Rewrite(0x4CB9, 0x0003, 0x0004, 0x000A); // , "movem.w\t$0004000A,d0-d1");
            AssertCode(
               "0|00010000(8): 5 instructions",
               "1|L--|v2 = 0004000A",
               "2|L--|d0 = Mem0[v2:word16]",
               "3|L--|v2 = v2 + 0x0002",
               "4|L--|d1 = Mem0[v2:word16]",
               "5|L--|v2 = v2 + 0x0002");
        }

        [Test]
        public void M68krw_indexedOperand()
        {
            Rewrite(0x2C70, 0xE9B5, 0x0001, 0x7FEC);
            AssertCode(
               "0|00010000(8): 1 instructions",
               "1|L--|a6 = Mem0[Mem0[0x00017FEC:word32] + a6:word32]");
        }
    }
}
