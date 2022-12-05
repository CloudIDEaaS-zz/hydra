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
using Decompiler.Core.Assemblers;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Decompiler.Assemblers.Pdp11
{
    public class Pdp11Assembler
    {
        private Pdp11Architecture arch;
        private IEmitter emitter;
        private SymbolTable symtab;

        public Pdp11Assembler(Pdp11Architecture arch, Address addrBase, IEmitter emitter)
        {
            this.arch = arch;
            this.BaseAddress = addrBase;
            this.emitter = emitter;
            this.Equates = new Dictionary<string, object>();
            this.symtab = new SymbolTable();
        }

        public Address BaseAddress { get; private set; }

        public Program GetImage()
        {
            var image = new LoadedImage(BaseAddress, emitter.GetBytes());
            return new Program(
                image,
                image.CreateImageMap(),
                arch,
                new DefaultPlatform(null, arch));
        }

        public Dictionary<string, object> Equates { get; private set; }
        public SymbolTable Symtab { get { return symtab; } }

        public void Equate(string id, object id2)
        {
            object value;
            var sId2 = id2 as string;
            if (sId2 != null && !Equates.TryGetValue(sId2, out value))
            {
                value = id2;
            }
            Equates[id] = id2;
            Debug.Print("{0} = {1}", id, id2);
        }

        public void Label(string label)
        {
            DefineSymbol(label);
        }

        private void DefineSymbol(string pstr)
        {
            var sym = symtab.DefineSymbol(pstr, emitter.Position);
            sym.ResolveLe(emitter);
        }

        public void Reset()
        {
            emitter.EmitLeUInt16(5);
        }

        public void Mov(ParsedOperand opSrc, ParsedOperand opDst)
        {
            EmitDoubleOperandInstruction(0x1000, opSrc, opDst);
        }
        public void Movb(ParsedOperand opSrc, ParsedOperand opDst)
        {
            EmitDoubleOperandInstruction(0x9000, opSrc, opDst);
        }

        internal void Sub(ParsedOperand opSrc, ParsedOperand opDst)
        {
            EmitDoubleOperandInstruction(0xE000, opSrc, opDst); 
        }


        private void EmitDoubleOperandInstruction(int opcode, ParsedOperand opSrc, ParsedOperand opDst)
        {
            opcode = opcode | (EncodeOperandField(opSrc) << 6) | EncodeOperandField(opDst);
            emitter.EmitLeUInt16(opcode);
            EmitOperand(opSrc);
            EmitOperand(opDst);
        }

        private void EmitSingleOperandInstruction(int opcode, ParsedOperand op)
        {
            opcode = opcode | EncodeOperandField(op);
            emitter.EmitLeUInt16(opcode);
            EmitOperand(op);
        }

        private void EmitOperand(ParsedOperand op)
        {
            switch (op.Type)
            {
            case AddressMode.Register:
            case AddressMode.AutoIncr:
            case AddressMode.AutoDecr:
            case AddressMode.RegDef:
                break;
            case AddressMode.Immediate:
                emitter.EmitLeUInt16((int) BaseAddress.ToLinear() + op.Offset);
                if (op.Symbol != null)
                {
                    ReferToSymbol(op.Symbol, emitter.Length - 2, PrimitiveType.Word16);
                }
                break;
            case AddressMode.Absolute:
                emitter.EmitLeUInt16((int) BaseAddress.ToLinear());
                if (op.Symbol != null)
                {
                    ReferToSymbol(op.Symbol, emitter.Length - 2, PrimitiveType.Word16);
                }
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private int EncodeOperandField(ParsedOperand op)
        {
            var enc = 0;
            switch (op.Type)
            {
            case AddressMode.Register:
                enc = op.Register.Number;
                break;
            case AddressMode.RegDef:
                enc = 0x08 | op.Register.Number;
                break;
            case AddressMode.AutoIncr:
                enc = 0x10 | op.Register.Number;
                break;
            case AddressMode.AutoDecr:
                enc = 0x20 | op.Register.Number;
                break;
            case AddressMode.Immediate:
                enc = 0x17;
                break;
            case AddressMode.Absolute:
                enc = 0x1F;
                break;
            default:
                throw new NotImplementedException("type " + op.Type);
            }
            return enc;
        }

        private void ReferToSymbol(Symbol psym, int off, DataType width)
        {
            if (psym.fResolved)
            {
                emitter.PatchLe(off, psym.offset, width);
            }
            else
            {
                psym.AddForwardReference(off, width, 1);
            }
        }

        internal void Jsr(ParsedOperand[] ops)
        {
            if (ops.Length != 2)
                throw new ArgumentException("ops");
            if (ops[0].Type != AddressMode.Register)
                throw new ArgumentException("First argument must be a register.");
            Debug.Print("ops(0): {0} {1}", ops[0].Register.Name, ops[0].Register.Number);
            EmitSingleOperandInstruction(0x0800 | (ops[0].Register.Number << 6), ops[1]);
        }

        internal void ProcessShortBranch(int opcode, string destination)
        {
            emitter.EmitLeUInt16(opcode | (byte)-(emitter.Length/2 + 1));
            
            var sym = symtab.CreateSymbol(destination);
            sym.ReferToLeWordCount(emitter.Length - 1, PrimitiveType.Byte, emitter);
        }

        internal void Asr(ParsedOperand op)
        {
            EmitSingleOperandInstruction(0x0C80, op);
        }

        public void Beq(string destination)
        {
            ProcessShortBranch(0x0300, destination);
        }

        internal void Clr(ParsedOperand op)
        {
            EmitSingleOperandInstruction(0x0A00, op);
        }

        internal void Clrb(ParsedOperand op)
        {
            EmitSingleOperandInstruction(0x8A00, op);
        }

        public void Inc(ParsedOperand op)
        {
            EmitSingleOperandInstruction(0x0A80, op);
        }

        public void Dec(ParsedOperand op)
        {
            EmitSingleOperandInstruction(0x0AC0, op);
        }
    }
}
