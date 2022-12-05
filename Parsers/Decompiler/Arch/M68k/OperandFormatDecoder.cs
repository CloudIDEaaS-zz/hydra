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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    /// <summary>
    /// Decodes M86k operands using a simple format language.
    /// </summary>
    public class OperandFormatDecoder
    {
        private M68kDisassembler dasm;
        ushort opcode;
        int i;
        private ushort bitSet;          // MoveM

        public OperandFormatDecoder(M68kDisassembler dasm, int i)
        {
            this.dasm = dasm;
            this.opcode = dasm.instruction;
            this.i = i;
        }

        public MachineOperand GetOperand(ImageReader rdr, string args, PrimitiveType dataWidth)
        {
            if (i >= args.Length)
                return null;
            for (; ; )
            {
                if (args[i] == ',')
                    ++i;
                Address addr;
                switch (args[i++])
                {
                case 'A':   // Address register A0-A7 encoded in in instrution
                    return new RegisterOperand(AddressRegister(opcode, GetOpcodeOffset(args[i++])));
                case 'c':   // CCR register 
                    return new RegisterOperand(Registers.ccr);
                case 'D':   // Data register D0-D7 encoded in instruction
                    return DataRegisterOperand(opcode, GetOpcodeOffset(args[i++]));
                case 'E':   // Effective address (EA) 
                    return ParseOperand(opcode, GetOpcodeOffset(args[i++]), dataWidth, rdr);
                case 'e':   // Effective address with 3-bit halves swapped
                    return ParseSwappedOperand(opcode, GetOpcodeOffset(args[i++]), dataWidth, rdr);
                case 'I':   // Immediate operand
                    return GetImmediate(rdr, GetSizeType(0, args[i++], dataWidth));
                case 'J':   // PC Relative jump 
                    addr = rdr.Address;
                    int offset = opcode & 0xFF;
                    if (offset == 0xFF)
                        offset = rdr.ReadBeInt32();
                    else if (offset == 0x00)
                        offset = rdr.ReadBeInt16();
                    else
                        offset = (sbyte) offset;
                    return new M68kAddressOperand(addr + offset);
                case 'M':   // Register bitset
                    return new RegisterSetOperand(rdr.ReadBeUInt16());
                case 'n':   // cache bitset
                    bitSet = rdr.ReadBeUInt16();
                    break;
                case 'm':   // Register bitset reversed
                    return RegisterSetOperand.CreateReversed(bitSet);
                case 'q':   // "Small" quick constant (3-bit part of the opcode)
                    return GetQuickImmediate(GetOpcodeOffset(args[i++]), 0x07, 8, PrimitiveType.Byte);
                case 'Q':   // "Large" quick constant (8-bit part of the opcode)
                    return GetQuickImmediate(GetOpcodeOffset(args[i++]), 0xFF, 0, PrimitiveType.SByte);
                case 'R': // relative
                    addr = rdr.Address;
                    int relative = 0;
                    switch (args[i++])
                    {
                    case 'w': relative = rdr.ReadBeInt16(); break;
                    case 'l': relative = rdr.ReadBeInt32(); break;
                    default: throw new NotImplementedException();
                    }
                    return new M68kAddressOperand(addr + relative);
                case 's':   // SR register
                    return new RegisterOperand(Registers.sr);
                case '+':   // Postincrement operator; following character specifies bit offset of the address register code.
                    return new PostIncrementMemoryOperand(dataWidth, AddressRegister(opcode, GetOpcodeOffset(args[i++])));
                case '-':   // Predecrement operator; following character specifies bit offset of the address register code.
                    return new PredecrementMemoryOperand(dataWidth, AddressRegister(opcode, GetOpcodeOffset(args[i++])));
                default: throw new FormatException(string.Format("Unknown argument type {0}.", args[--i]));
                }
            }
        }

        private MachineOperand GetQuickImmediate(int offset, int mask, int zeroValue, PrimitiveType dataWidth)
        {
            int v = ((int) opcode >> offset) & mask;
            if (v == 0)
                v = zeroValue;
            return new M68kImmediateOperand(Constant.Create(dataWidth, v));
        }

        private static PrimitiveType SizeField(ushort opcode, int bitOffset)
        {
            switch ((opcode >> bitOffset) & 3)
            {
            case 0: return PrimitiveType.Byte;
            case 1: return PrimitiveType.Word16;
            case 2: return PrimitiveType.Word32;
            default: throw new InvalidOperationException(string.Format("Illegal size field in opcode {0:X4}.", opcode));
            }
        }

        private static M68kImmediateOperand GetImmediate(ImageReader rdr, PrimitiveType type)
        {
            if (type.Size == 1)
            {
                rdr.ReadByte();     // skip a byte so we get the appropriate lsb byte and align the word stream.
            }
            return new M68kImmediateOperand(rdr.ReadBe(type));
        }

        public MachineOperand ParseOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            opcode >>= bitOffset;
            byte operandBits = (byte) (opcode & 7);
            byte addressMode = (byte) ((opcode >> 3) & 7);
            return ParseOperandInner(addressMode, operandBits, dataWidth, rdr);
        }

        private MachineOperand ParseSwappedOperand(ushort opcode, int bitOffset, PrimitiveType dataWidth, ImageReader rdr)
        {
            opcode >>= bitOffset;
            byte addressMode = (byte) (opcode & 7);
            byte operandBits = (byte) ((opcode >> 3) & 7);
            return ParseOperandInner(addressMode, operandBits, dataWidth, rdr);
        }

        private static int GetOpcodeOffset(char c)
        {
            int offset = c - '0';
            if (offset < 0)
                throw new FormatException("Invalid offset specification.");
            if (offset < 10)
                return offset;
            return offset - 6;
        }

        public static PrimitiveType GetSizeType(ushort opcode, char c, PrimitiveType dataWidth)
        {
            switch (c)
            {
            case 'b': return PrimitiveType.Byte;
            case 'v': return dataWidth;
            case 'u': return PrimitiveType.UInt16;
            case 'w': return PrimitiveType.Word16;
            case 'l': return PrimitiveType.Word32;
            default: return SizeField(opcode, GetOpcodeOffset(c)); ;
                throw new NotImplementedException();
            }
        }

        private MachineOperand ParseOperandInner(byte addressMode, byte operandBits, PrimitiveType dataWidth, ImageReader rdr)
        {
            Constant offset;
            switch (addressMode)
            {
            case 0: // Data register direct.
                return DataRegisterOperand(operandBits, 0);
            case 1: // Address register direct
                return new RegisterOperand(AddressRegister(operandBits, 0));
            case 2:  // Address register indirect
                return MemoryOperand.Indirect(dataWidth, AddressRegister(operandBits, 0));
            case 3:  // Address register indirect with postincrement.
                return MemoryOperand.PostIncrement(dataWidth, AddressRegister(operandBits, 0));
            case 4:  // Address register indirect with predecrement.
                return MemoryOperand.PreDecrement(dataWidth, AddressRegister(operandBits, 0));
            case 5: // Address register indirect with displacement.
                offset = Constant.Int16(rdr.ReadBeInt16());
                return MemoryOperand.Indirect(dataWidth, AddressRegister(operandBits, 0), offset);
            case 6: // Address register indirect with index
                return AddressRegisterIndirectWithIndex(dataWidth, rdr);
            case 7:
                switch (operandBits)
                {
                case 0: // Absolute short address
                    return new M68kAddressOperand(rdr.ReadBeUInt16());
                case 1: // Absolute long address
                    return new M68kAddressOperand(rdr.ReadBeUInt32());
                case 2: // Program counter with displacement
                    var off = rdr.Address - dasm.instr.Address;
                    off += rdr.ReadBeInt16();
                    return new MemoryOperand(dataWidth, Registers.pc, Constant.Int16((short) off));
                case 3:
                    // Program counter with index
                    var addrExt = rdr.Address;
                    ushort extension = rdr.ReadBeUInt16();

                    if (EXT_FULL(extension))
                    {
                        if (EXT_EFFECTIVE_ZERO(extension))
                        {
                            return new M68kImmediateOperand(Constant.Word32(0));
                        }
                        Constant @base = null;
                        Constant outer = null;
                        if (EXT_BASE_DISPLACEMENT_PRESENT(extension)) 
                            @base = EXT_BASE_DISPLACEMENT_LONG(extension) 
                                ? rdr.ReadBe(PrimitiveType.Word32)
                                : rdr.ReadBe(PrimitiveType.Int16);
                        if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                            outer = EXT_OUTER_DISPLACEMENT_LONG(extension)
                                ? rdr.ReadBe(PrimitiveType.Word32)
                                : rdr.ReadBe(PrimitiveType.Int16);
                        RegisterStorage base_reg = EXT_BASE_REGISTER_PRESENT(extension)
                            ? Registers.pc
                            : null;
                        RegisterStorage index_reg = null;
                        PrimitiveType index_width = null;
                        int index_scale = 0;
                        if (EXT_INDEX_REGISTER_PRESENT(extension))
                        {
                            index_reg = EXT_INDEX_AR(extension)
                                ? Registers.AddressRegister((int)EXT_INDEX_REGISTER(extension))
                                : Registers.DataRegister((int)EXT_INDEX_REGISTER(extension));
                            index_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16;
                            index_scale = (EXT_INDEX_SCALE(extension) != 0)
                                ? 1 << EXT_INDEX_SCALE(extension)
                                : 0;
                        }
                       return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_width, index_scale, 
                           (extension & 7) > 0 && (extension & 7) < 4,
                           (extension & 7) > 4);
                    }
                    return new IndirectIndexedOperand(
                        EXT_8BIT_DISPLACEMENT(extension), 
                        Registers.pc,
                        EXT_INDEX_AR(extension) ? Registers.AddressRegister((int)EXT_INDEX_REGISTER(extension)) : Registers.DataRegister((int)EXT_INDEX_REGISTER(extension)),
                        EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16,
                        1 << EXT_INDEX_SCALE(extension));

                case 4:
                    //  Immediate
                    if (dataWidth.Size == 1)        // don't want the instruction stream to get misaligned!
                        rdr.ReadByte();
                    return new M68kImmediateOperand(rdr.ReadBe(dataWidth));
                default:
                    throw new NotImplementedException(string.Format("Address mode {0}:{1} not implemented.", addressMode, operandBits));
                }
            default: 
                throw new NotImplementedException(string.Format("Address mode {0:X1} not implemented.", addressMode));
            }
        }

        private MachineOperand AddressRegisterIndirectWithIndex(PrimitiveType dataWidth, ImageReader rdr)
        {
            ushort extension = rdr.ReadBeUInt16();
            if (EXT_FULL(extension))
            {
                if (M68kDisassembler.EXT_EFFECTIVE_ZERO(extension))
                {
                    return new M68kImmediateOperand(Constant.Zero(dataWidth));
                }

                RegisterStorage base_reg = null;
                RegisterStorage index_reg = null;
                PrimitiveType index_reg_width = null;
                int index_scale = 1;
                Constant @base = null;
                if (EXT_BASE_DISPLACEMENT_PRESENT(extension))
                {
                    @base = rdr.ReadBe(EXT_BASE_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32: PrimitiveType.Int16);
                }

                Constant outer = null;
                if (EXT_OUTER_DISPLACEMENT_PRESENT(extension))
                {
                    outer = rdr.ReadBe(EXT_OUTER_DISPLACEMENT_LONG(extension) ? PrimitiveType.Word32: PrimitiveType.Int16);
                }
                if (EXT_BASE_REGISTER_PRESENT(extension))
                    base_reg = Registers.AddressRegister(opcode & 7);
                if (EXT_INDEX_REGISTER_PRESENT(extension))
                {
                    index_reg = EXT_INDEX_AR(extension)
                        ? Registers.AddressRegister((int) EXT_INDEX_REGISTER(extension))
                        : Registers.DataRegister((int) EXT_INDEX_REGISTER(extension));
                    index_reg_width = EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Word16;
                    if (EXT_INDEX_SCALE(extension) != 0)
                        index_scale = 1 << EXT_INDEX_SCALE(extension);
                }
                bool preindex = (extension & 7) > 0 && (extension & 7) < 4;
                bool postindex = (extension & 7) > 4;
                return new IndexedOperand(dataWidth, @base, outer, base_reg, index_reg, index_reg_width, index_scale, preindex, postindex);
            }
            else
            {
                return new IndirectIndexedOperand(
                    EXT_8BIT_DISPLACEMENT(extension),
                    Registers.AddressRegister(opcode & 7),
                    EXT_INDEX_AR(extension)
                        ? Registers.AddressRegister((int)EXT_INDEX_REGISTER(extension))
                        : Registers.DataRegister((int)EXT_INDEX_REGISTER(extension)),
                    EXT_INDEX_LONG(extension) ? PrimitiveType.Word32 : PrimitiveType.Int16,
                    EXT_INDEX_SCALE(extension));
            }
        }

        // Extension word formats
        private static sbyte EXT_8BIT_DISPLACEMENT(uint A) { return (sbyte) ((A) & 0xff); }
        internal static bool EXT_FULL(uint A) { return M68kDisassembler.BIT_8(A); }
        internal static bool EXT_EFFECTIVE_ZERO(uint A) { return (((A) & 0xe4) == 0xc4 || ((A) & 0xe2) == 0xc0); }
        private static bool EXT_BASE_REGISTER_PRESENT(uint A) { return !(M68kDisassembler.BIT_7(A)); }
        private static bool EXT_INDEX_REGISTER_PRESENT(uint A) { return !(M68kDisassembler.BIT_6(A)); }
        private static uint EXT_INDEX_REGISTER(uint A) { return (((A) >> 12) & 7); }
        private static bool EXT_INDEX_PRE_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && (A & 3) != 0); }
        private static bool EXT_INDEX_PRE(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) < 4 && ((A) & 7) != 0); }
        private static bool EXT_INDEX_POST(uint A) { return (EXT_INDEX_REGISTER_PRESENT(A) && ((A) & 7) > 4); }
        internal static int EXT_INDEX_SCALE(uint A) { return (int) (((A) >> 9) & 3); }
        private static bool EXT_INDEX_LONG(uint A) { return M68kDisassembler.BIT_B(A); }
        private static bool EXT_INDEX_AR(uint A) { return M68kDisassembler.BIT_F(A); }
        private static bool EXT_BASE_DISPLACEMENT_PRESENT(uint A) { return (((A) & 0x30) > 0x10); }
        private static bool EXT_BASE_DISPLACEMENT_WORD(uint A) { return (((A) & 0x30) == 0x20); }
        private static bool EXT_BASE_DISPLACEMENT_LONG(uint A) { return (((A) & 0x30) == 0x30); }
        private static bool EXT_OUTER_DISPLACEMENT_PRESENT(uint A) { return (((A) & 3) > 1 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_WORD(uint A) { return (((A) & 3) == 2 && ((A) & 0x47) < 0x44); }
        private static bool EXT_OUTER_DISPLACEMENT_LONG(uint A) { return (((A) & 3) == 3 && ((A) & 0x47) < 0x44); }

        private static AddressRegister AddressRegister(ushort opcode, int bitOffset)
        {
            return (AddressRegister)Registers.GetRegister(8 + ((opcode >> bitOffset) & 0x7));
        }

        private static RegisterOperand DataRegisterOperand(ushort opcode, int bitOffset)
        {
            return new RegisterOperand(Registers.GetRegister((opcode >> bitOffset) & 0x7));
        }

        private static M68kImmediateOperand SignedImmediateByte(ushort opcode, int bitOffset, int mask)
        {
            return new M68kImmediateOperand(Constant.Create(PrimitiveType.SByte, (opcode >> bitOffset) & mask));
        }
    }
}
