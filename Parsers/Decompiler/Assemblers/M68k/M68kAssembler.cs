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
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Assemblers.M68k
{
    /// <summary>
    /// Handy little assembler that lets you write C# code that assembles 
    /// directly to 68k opcodes.
    /// </summary>
    /// <remarks>
    /// //$TODO: many more instructions, symbols.
    /// </remarks>
    public class M68kAssembler
    {
        private M68kArchitecture arch;
        private List<EntryPoint> entryPoints;
        private List<ushort> constants;

        public M68kAssembler(M68kArchitecture arch, Address addrBase, List<EntryPoint> entryPoints)
        {
            this.arch = arch;
            this.BaseAddress = addrBase;
            this.entryPoints = new List<EntryPoint>();
            this.Emitter = new Emitter();
            this.constants = new List<ushort>();
            this.Symbols = new SymbolTable();
        }

        public Address BaseAddress { get; private set; }
        public SymbolTable Symbols { get; private set; }
        public IEmitter Emitter { get; private set; }

        public RegisterOperand d0 { get { return new RegisterOperand(Registers.d0); } }
        public RegisterOperand d1 { get { return new RegisterOperand(Registers.d1); } }
        public RegisterOperand d2 { get { return new RegisterOperand(Registers.d2); } }
        public RegisterOperand d3 { get { return new RegisterOperand(Registers.d3); } }
        public RegisterOperand d4 { get { return new RegisterOperand(Registers.d4); } }
        public RegisterOperand d5 { get { return new RegisterOperand(Registers.d5); } }
        public RegisterOperand d6 { get { return new RegisterOperand(Registers.d6); } }
        public RegisterOperand d7 { get { return new RegisterOperand(Registers.d7); } }

        public RegisterOperand a0 { get { return new RegisterOperand(Registers.a0); } }
        public RegisterOperand a1 { get { return new RegisterOperand(Registers.a1); } }
        public RegisterOperand a2 { get { return new RegisterOperand(Registers.a2); } }
        public RegisterOperand a3 { get { return new RegisterOperand(Registers.a3); } }
        public RegisterOperand a4 { get { return new RegisterOperand(Registers.a4); } }
        public RegisterOperand a5 { get { return new RegisterOperand(Registers.a5); } }
        public RegisterOperand a6 { get { return new RegisterOperand(Registers.a6); } }
        public RegisterOperand a7 { get { return new RegisterOperand(Registers.a7); } }

        public Program GetImage()
        {
            var image = new LoadedImage(BaseAddress, Emitter.GetBytes());
            return new Program(
                image,
                image.CreateImageMap(),
                arch, 
                new DefaultPlatform(null, arch));
        }

        internal void Cnop(int extra, int align)
        {
            Emitter.Align(extra, align);
        }

        public void Label(string label)
        {
            Symbols.DefineSymbol(label, Emitter.Position).ResolveBe(Emitter);
        }

        private int Ea(MachineOperand op, int shift)
        {
            return Ea(op) << shift;
        }

        private int Ea(MachineOperand op)
        {
            var rop = op as RegisterOperand;
            if (rop != null)
            {
                var dReg = rop.Register as DataRegister;
                if (dReg != null)
                    return (dReg.Number & 7);
                var aReg = rop.Register as AddressRegister;
                if (aReg != null)
                    return (aReg.Number & 7) | 8;
                throw new NotImplementedException(op.ToString());
            }
            var mop = op as MemoryOperand;
            if (mop != null)
            {
                var aReg = mop.Base;
                if (mop.Offset == null || mop.Offset.ToInt32() == 0)
                    return (aReg.Number & 7 | 0x10);
                constants.Add((ushort) mop.Offset.ToInt32());
                return (aReg.Number & 7 | 0x28);
            }

            var postOp = op as PostIncrementMemoryOperand;
            if (postOp != null)
            {
                return (postOp.Register.Number & 7 | 0x18);
            }
            var preOp = op as PredecrementMemoryOperand;
            if (preOp != null)
            {
                return (preOp.Register.Number & 7 | 0x20);
            }

            var addrOp = op as M68kAddressOperand;
            if (addrOp != null)
            {
                Imm(addrOp.Width.Size, addrOp.Address.ToUInt32());
                return 0x39;
            }

            var immOp = op as M68kImmediateOperand;
            if (immOp != null)
            {
                Imm(immOp.Width.Size, immOp.Constant.ToUInt32());
                return 0x3C;
            }
            throw new NotImplementedException(op.ToString());
        }

        private void Imm(int size, uint c)
        {
            switch (size)
            {
            case 1: constants.Add((ushort)(c & 0xFF)); break;
            case 2: constants.Add((ushort)(c & 0xFFFF)); break;
            case 4:
                constants.Add((ushort) (c >> 16));
                constants.Add((ushort) (c & 0xFFFF));
                break;
            default: throw new InvalidOperationException();
            }
        }

        public MachineOperand Mem(RegisterOperand rop)
        {
            var a = (AddressRegister) rop.Register;
            return new MemoryOperand(null, a);
        }

        public MemoryOperand Mem(int offset, RegisterOperand rop)
        {
            var a = (AddressRegister) rop.Register;
            return new MemoryOperand(null, a, Constant.Int16((short)offset));
            throw new NotImplementedException();
        }

        private int SmallQ(int c)
        {
            if (c <= 0 || c > 8)
                throw new ArgumentOutOfRangeException("c");
            if (c == 8)
                c = 0;
            return c;
        }

        private int SwapEa(MachineOperand op)
        {
            int ea = Ea(op) & 0x3F;
            return ((ea >> 3) & 7) | ((ea & 7) << 3);
        }

        private int AReg(MachineOperand op)
        {
            var rop = (RegisterOperand) op;
            var addr = (AddressRegister) rop.Register;
            return addr.Number & 7;
        }

        private int DReg(MachineOperand op)
        {
            var rop = (RegisterOperand) op;
            var data = (DataRegister) rop.Register;
            return data.Number & 7;
        }

        public PostIncrementMemoryOperand Post(RegisterOperand a)
        {
            return new PostIncrementMemoryOperand(null, (AddressRegister) a.Register);
        }

        public PredecrementMemoryOperand Pre(RegisterOperand a)
        {
            return new PredecrementMemoryOperand(null, (AddressRegister) a.Register);
        }

        private void EmitConstants()
        {
            foreach (ushort c in constants)
            {
                Emitter.EmitBeUInt16(c);
            }
            constants.Clear();
        }

        private void ReferToSymbol(Symbol psym, int off, DataType width)
        {
            if (psym.fResolved)
            {
                Emitter.PatchBe(off, psym.offset, width);
            }
            else
            {
                psym.AddForwardReference(off, width, 1);
            }
        }

        public void Add_b(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xD000 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }
        public void Add_w(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xD040 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }
        public void Add_l(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xD080 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }

        public void Add_b(RegisterOperand dSrc, MachineOperand eaDst)
        {
            Emit(0xD000 | Ea(eaDst, 0) | DReg(dSrc) << 9);
        }
        public void Add_w(RegisterOperand dSrc, MachineOperand eaDst)
        {
            Emit(0xD040 | Ea(eaDst, 0) | DReg(dSrc) << 9);
        }
        public void Add_l(RegisterOperand dSrc, MachineOperand eaDst)
        {
            Emit(0xD080 | Ea(eaDst, 0) | DReg(dSrc) << 9);
        }

        public void Adda_w(MachineOperand eaSrc, RegisterOperand aDst)
        {
            Emit(0xD0C0 | Ea(eaSrc, 0) | AReg(aDst) << 9);
        }
        public void Adda_l(MachineOperand eaSrc, RegisterOperand aDst)
        {
            Emit(0xD1C0 | Ea(eaSrc, 0) | AReg(aDst) << 9);
        }

        public void Addi_w(int c, MachineOperand eaDst)
        {
            constants.Add((ushort) c);
            Emit(0x0640 | Ea(eaDst));
        }

        public void Addq_b(int c, MachineOperand eaDst)
        {
            Emit(0x5000 | Ea(eaDst) & 0x3F | (SmallQ(c) << 9));
        }
        public void Addq_w(int c, MachineOperand eaDst)
        {
            Emit(0x5040 | Ea(eaDst) & 0x3F | (SmallQ(c) << 9));
        }
        public void Addq_l(int c, MachineOperand eaDst)
        {
            Emit(0x5080 | Ea(eaDst) & 0x3F | (SmallQ(c) << 9));
        }

        public void Asl_l(int c, RegisterOperand dDst)
        {
            Emit(0xE180 | SmallQ(c) << 9 | DReg(dDst));
        }

        public void Bchg(int c, MachineOperand eaDst)
        {
            Emit(0x0840 | Ea(eaDst));
            Emitter.EmitBeUInt16(c);
        }

        public void Bchg(RegisterOperand dSrc, MachineOperand eaDst)
        {
            Emit(0x0140 | DReg(dSrc) << 9 | Ea(eaDst));
        }

            //Opcode.bt, Opcode.bf, Opcode.bhi, Opcode.bls, Opcode.bcc, Opcode.bcs, Opcode.bne, Opcode.beq, 
            //Opcode.bvc, Opcode.bvs, Opcode.bpl, Opcode.bmi, Opcode.bge, Opcode.blt, Opcode.bgt, Opcode.ble };

        public void Beq(uint address)
        {
            Bcc(address, 7);
        }
        public void Bge(uint address)
        {
            Bcc(address, 0xC);
        }
        public void Bne(string target)
        {
            Bcc(target, 6);
        }
        public void Bra(string target)
        {
            Bcc(target, 0);
        }

        public void Bra(uint address)
        {
            Bcc(address, 0x0);
        }
        public void Bcc(string target, int flags)
        {
            constants.Add((ushort)-(Emitter.Position + 2));
            Emit(0x6000 | (flags << 8));
            ReferToSymbol(Symbols.CreateSymbol(target), Emitter.Position - 2, PrimitiveType.Word16);
        }

        private void Bcc(uint address, int flags)
        {
            Emit(0x6000 | (flags << 8) | (int)(address & 7)); //$BUG should be offset.
        }

        public void Clr_b(MachineOperand ea)
        {
            Emit(0x4200 | Ea(ea, 0));
        }
        public void Clr_w(MachineOperand ea)
        {
            Emit(0x4240 | Ea(ea, 0));
        }
        public void Clr_l(MachineOperand ea)
        {
            Emit(0x4280 | Ea(ea, 0));
        }

        public void Cmp_b(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xB000 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }
        public void Cmp_w(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xB040 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }
        public void Cmp_l(MachineOperand eaSrc, RegisterOperand dDst)
        {
            Emit(0xB080 | Ea(eaSrc, 0) | DReg(dDst) << 9);
        }

        public void Cmpa_w(MachineOperand eaSrc, RegisterOperand aDst)
        {
            Emit(0xB0C0 | Ea(eaSrc, 0) | AReg(aDst) << 9);
        }
        public void Cmpa_l(MachineOperand eaSrc, RegisterOperand aDst)
        {
            Emit(0xB1C0 | Ea(eaSrc, 0) | AReg(aDst) << 9);
        }

        public void Jsr(uint address)
        {
            Jsr(new M68kAddressOperand(address));
        }

        public void Jsr(MachineOperand op)
        {
            Emit(0x4E80 | Ea(op, 0));
        }

        public void Jsr(string target)
        {
            var linBase = BaseAddress.ToUInt32();
            Imm(4, linBase);
            Emit(0x4EB9);
            ReferToSymbol(Symbols.CreateSymbol(target), Emitter.Position - 4, PrimitiveType.Word32);
        }

        public void Lea(MachineOperand ea, RegisterOperand aReg)
        {
            Emit(0x41C0 | Ea(ea, 0) | AReg(aReg) << 9);
        }

        public void Lsl_l(int c, RegisterOperand dDst)
        {
            Emit(0xE188 | SmallQ(c) << 9 | DReg(dDst));
        }

        public void Move_b(MachineOperand src, MachineOperand dst)
        {
            var eaSrc = Ea(src);
            var eaDst = SwapEa(dst);
            Emit(0x1000 | eaSrc | (eaDst << 6));
        }
        public void Move_w(MachineOperand src, MachineOperand dst)
        {
            var eaSrc = Ea(src);
            var eaDst = SwapEa(dst);
            Emit(0x3000 | eaSrc | (eaDst << 6));
        }
        public void Move_l(MachineOperand src, MachineOperand dst)
        {
            var eaSrc = Ea(src);
            var eaDst = SwapEa(dst);
            Emit(0x2000 | eaSrc | (eaDst << 6));
        }

        public void Movem_w(RegisterSetOperand src, PredecrementMemoryOperand dst)
        {
            constants.Add((ushort)src.BitSet);
            Emit(0x48A0 | dst.Register.Number & 7);
        }
        public void Movem_l(RegisterSetOperand src, PredecrementMemoryOperand dst)
        {
            constants.Add((ushort) src.BitSet);
            Emit(0x48E0 | dst.Register.Number & 7);
        }

        public void Nop()
        {
            Emitter.EmitBeUInt16(0x4E71);
        }

        internal void Pea(MachineOperand ea)
        {
            //$BUG: check for valid ea's
            Emit(0x4840 | Ea(ea));
        }

        public void Rts()
        {
            Emit(0x4E75);
        }

        public void Subq_b(int q, MachineOperand eaDst)
        {
            Emit(0x5100 | SmallQ(q) << 9 | Ea(eaDst));
        }
        public void Subq_w(int q, MachineOperand eaDst)
        {
            Emit(0x5140 | SmallQ(q) << 9 | Ea(eaDst));
        }
        public void Subq_l(int q, MachineOperand eaDst)
        {
            Emit(0x5180 | SmallQ(q) << 9 | Ea(eaDst));
        }

        private void Emit(int opcode)
        {
            Emitter.EmitBeUInt16(opcode);
            EmitConstants();
        }

        internal void ReportUnresolvedSymbols()
        {
        }
    }
}
