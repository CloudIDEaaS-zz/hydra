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
using Decompiler.Core.Assemblers;
using Decompiler.Core.Expressions;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using Decompiler.Arch.M68k;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.Assemblers.M68k
{
    public class M68kTextAssembler : Assembler
    {
        private M68kArchitecture arch;
        private Address addrBase;
        private IEmitter emitter;
        private List<EntryPoint> entryPoints;
        private Lexer lexer;
        private M68kAssembler asm ;
        private PrimitiveType dataWidth;

        public M68kTextAssembler()
        {
            this.entryPoints = new List<EntryPoint>();
            this.LineNumber = 1;
        }

        public int LineNumber { get; private set; }

        #region Assembler Members

        public Program Assemble(Address baseAddress, TextReader rdr)
        {
            this.addrBase = baseAddress;
            this.lexer = new Lexer(rdr);
            this.arch = new M68kArchitecture();
            asm = new M68kAssembler(arch, addrBase, entryPoints);
            this.emitter = asm.Emitter;

            // Assemblers are strongly line-oriented.

            while (lexer.PeekToken().Type != TokenType.EOFile)
            {
                ProcessLine();
            }

            asm.ReportUnresolvedSymbols();
            //addrStart = addrBase;
            return asm.GetImage();
        }

        private void ProcessLine()
        {
            var tok = lexer.GetToken();
            if (tok.Type == TokenType.NL)
                return;
            if (tok.Type == TokenType.ID)
            {
                asm.Label(tok.Text);
            }
            else if (tok.Type != TokenType.WS)
            {
                Error("Expected whitespace at the beginning of the line.");
            }
            tok = lexer.GetToken();
            switch (tok.Type)
            {
            default: Error("Unexpected token {0} ({1})", tok.Text, tok.Type); break;
            case TokenType.NL:
            case TokenType.EOFile:
                return;
            case TokenType.CNOP: ProcessCnop(); break;
            case TokenType.DC: ProcessDc(); break;
            case TokenType.EQU: ProcessEqu(); break;
            case TokenType.IDNT: ProcessIdnt(); break;
            case TokenType.OPT: ProcessOpt(); break;
            case TokenType.REG: ProcessReg(); break;
            case TokenType.PUBLIC: ProcessPublic(); break;
            case TokenType.SECTION: ProcessSection(); break;

            case TokenType.add: ProcessAdd(); break;
            case TokenType.addq: ProcessAddq(); break;
            case TokenType.clr: ProcessClr(); break;
            case TokenType.cmp: ProcessCmp(); break;
            case TokenType.beq: ProcessBeq(); break;
            case TokenType.bge: ProcessBge(); break;
            case TokenType.bne: ProcessBne(); break;
            case TokenType.bra: ProcessBra(); break;
            case TokenType.jsr: ProcessJsr(); break;
            case TokenType.lea: ProcessLea(); break;
            case TokenType.move: ProcessMove(); break;
            case TokenType.movem: ProcessMovem(); break;
            case TokenType.pea: ProcessPea(); break;
            case TokenType.rts: ProcessRts(); break;
            case TokenType.subq: ProcessSubq(); break;
            }

            Expect(TokenType.NL);
            ++LineNumber;
        }

        private void ProcessDc()
        {
            var code = ExpectDataWidth();
            var n = ExpectInteger();
            switch (code)
            {
            case 0: emitter.EmitByte(n); break;
            case 1: emitter.EmitBeUInt16(n); break;
            case 2: emitter.EmitBeUInt32(n); break;
            default: Debug.Assert(false); break;
            }
        }

        private void ProcessEqu()
        {
            ExpectInteger();
        }

        private void ProcessReg()
        {
        }

        private void ProcessAdd()
        {
            int width = ExpectDataWidth();
            var eaSrc = ExpectEffectiveAddress();
            Expect(TokenType.COMMA);
            var eaDst = ExpectEffectiveAddress();
            var rDst = eaDst as RegisterOperand;
            if (rDst != null)
            {
                if (rDst.Register is AddressRegister)
                {
                    Emit(eaSrc, rDst, width, ByteWidthIllegal, asm.Adda_w, asm.Adda_l);
                }
                else
                {
                    Emit(eaSrc, rDst, width, asm.Add_b, asm.Add_w, asm.Add_l);
                }
            }
            else
            {
                var dSrc = (RegisterOperand) eaSrc;
                Emit(dSrc, eaDst, width, asm.Add_b, asm.Add_w, asm.Add_l);
            }
        }

        private void ProcessAddq()
        {
            var code = ExpectDataWidth();
            Expect(TokenType.HASH);
            var q = ExpectInteger();
            Expect(TokenType.COMMA);
            var eaDst = ExpectEffectiveAddress();
            Emit(q, eaDst, code, asm.Addq_b, asm.Addq_w, asm.Addq_l);
        }

        private void ProcessClr()
        {
            int code = ExpectDataWidth();
            var ea = ExpectEffectiveAddress();
            Emit(ea, code, asm.Clr_b, asm.Clr_w, asm.Clr_l);
        }

        private void ProcessCmp()
        {
            int code = ExpectDataWidth();
            var eaSrc = ExpectEffectiveAddress();
            Expect(TokenType.COMMA);
            var eaDst = ExpectEffectiveAddress();
            var rDst = eaDst as RegisterOperand;
            if (rDst != null)
            {
                if (rDst.Register is AddressRegister)
                {
                    Emit(eaSrc, rDst, code, ByteWidthIllegal, asm.Cmpa_w, asm.Cmpa_l);
                }
                else
                {
                    Emit(eaSrc, rDst, code, asm.Cmp_b, asm.Cmp_w, asm.Cmp_l);
                }
                return;
            }
            Error("Invalid operands for cmp.");
        }

        private void ProcessBeq()
        {
            Expect(TokenType.ID);   //$BUG: create symbol.
            asm.Beq(0);     //$BUG
        }

        private void ProcessBge()
        {
            Expect(TokenType.ID);   //$BUG: create symbol.
            asm.Bge(0);     //$BUG
        }
        private void ProcessBne()
        {
            asm.Bne(Expect(TokenType.ID));
        }

        private void ProcessBra()
        {
            var name = Expect(TokenType.ID);   //$BUG: create symbol.
            asm.Bra(name);     //$BUG
        }

        private void ProcessJsr()
        {
            var name = Expect(TokenType.ID);
            asm.Jsr(name);     //$BUG.
        }

        private void ProcessLea()
        {
            var ea = ExpectEffectiveAddress();
            Expect(TokenType.COMMA);
            var aReg = ExpectAddressRegister();
            asm.Lea(ea, new RegisterOperand(aReg));
        }

        private void ProcessMove()
        {
            var width = ExpectDataWidth();
            var eaSrc = ExpectEffectiveAddress();
            Expect(TokenType.COMMA);
            var eaDst = ExpectEffectiveAddress();
            Emit(eaSrc, eaDst, width, asm.Move_b, asm.Move_w, asm.Move_l);
        }

        private void ProcessMovem()
        {
            int width = ExpectDataWidth();
            RegisterSetOperand mask = ExpectRegisterSet();
            Expect(TokenType.COMMA);
            var ea = ExpectEffectiveAddress();
            var pre = ea as PredecrementMemoryOperand;
            if (pre != null)
            {
                Emit(mask, pre, width, ByteWidthIllegal, asm.Movem_w, asm.Movem_l);
            }
            else
            {
                Error("Unpexected operand {0}.", ea);
            }
        }
        private void Emit<T>(T op, int width, Action<T> b, Action<T> w, Action<T> l)
        {
            switch (width)
            {
            case 0: b(op); return;
            case 1: w(op); return;
            case 2: l(op); return;
            }
            Error("Unknown width type {0}.", width);
        }

        private void Emit<TSrc, TDst>(TSrc src, TDst dst, int width, Action<TSrc, TDst> b, Action<TSrc, TDst> w, Action<TSrc, TDst> l)
        {
            switch (width)
            {
            case 0: b(src, dst); return;
            case 1: w(src, dst); return;
            case 2: l(src, dst); return;
            }
            Error("Unknown width type {0}.", width);
        }

        private void ByteWidthIllegal<TSrc, TDst>(TSrc s, TDst d)
        {
            Error("Byte sized operands are illegal with this instruction.");
        }

        private void ProcessPea()
        {
            var ea = ExpectEffectiveAddress();
            asm.Pea(ea);
        }

        private void ProcessRts()
        {
            asm.Rts();
        }

        private void ProcessSubq()
        {
            int mask = ExpectDataWidth();
            Expect(TokenType.HASH);
            var q = ExpectInteger();
            Expect(TokenType.COMMA);
            var d = ExpectEffectiveAddress();
            Emit(q, d, mask, asm.Subq_b, asm.Subq_w, asm.Subq_l);
        }

        private int ExpectDataWidth()
        {
            Expect(TokenType.DOT);
            var width = Expect(TokenType.ID);
            if (string.IsNullOrEmpty(width) || width.Length > 1)
                Error("Expected a data width (.b, .w, or .l) but got '{0}'", width);
            switch (width[0])
            { case 'b': dataWidth = PrimitiveType.Byte; return 0;
            case 'w': dataWidth = PrimitiveType.Word16; return 1;
            case 'l': dataWidth = PrimitiveType.Word32;  return 2;
            }
            throw new InvalidOperationException();
        }

        private int ExpectInteger()
        {
            return Convert.ToInt32(Expect(TokenType.INTEGER));
        }

        private RegisterSetOperand ExpectRegisterSet()
        {
            //$BUGBUG: lots o' work here :(
            Expect(TokenType.ID);
            return new RegisterSetOperand(0x42);
        }

        private MachineOperand ExpectEffectiveAddress()
        {
            AddressRegister aReg;
            int offset;
            var tok = lexer.GetToken();
            switch (tok.Type)
            {
            case TokenType.ID:
                var name = tok.Text;
                Debug.Assert(!string.IsNullOrEmpty(name));
                if (name.Length == 2 && "dD".IndexOf(name[0]) >= 0 && '0' <= name[1] && name[1] <= '7')
                {
                    return new RegisterOperand(Registers.DataRegister(name[1] - '0'));
                }
                if (name.Length == 2 && "aA".IndexOf(name[0]) >= 0 && '0' <= name[1] && name[1] <= '7')
                {
                    return new RegisterOperand(Registers.AddressRegister(name[1] - '0'));
                }
                // It's some identifier we haven't seen yet. Register our interest
                return ForwardEa(asm.Symbols.CreateSymbol(name));
            case TokenType.MINUS:
                tok = lexer.PeekToken();
                if (tok.Type == TokenType.INTEGER)
                {
                    offset = -ExpectInteger();
                    Expect(TokenType.LPAREN);
                    return ParseAddressExpression(offset);
                }
                Expect(TokenType.LPAREN);
                aReg = ExpectAddressRegister();
                Expect(TokenType.RPAREN);
                return new PredecrementMemoryOperand(null, aReg);
            case TokenType.INTEGER:
                offset = Convert.ToInt32(tok.Text);
                Expect(TokenType.LPAREN);
                return ParseAddressExpression(offset);
            case TokenType.LPAREN:
                return ParseAddressExpression(0);
            case TokenType.HASH:
                int n = ExpectInteger();
                return new M68kImmediateOperand(Constant.Create(this.dataWidth, n));
            }
            Error("Unexpected token '{0}' ('{1}').", tok.Text, tok.Type);
            return null;
        }

        private MachineOperand ParseAddressExpression(int offset)
        {
            AddressRegister aReg;
            var tok = lexer.PeekToken();
            switch (tok.Type)
            {
            case TokenType.INTEGER:
                int i = ExpectInteger();
                Expect(TokenType.PLUS); //$BUG support for expressions later.
                Expect(TokenType.ID);
                offset += i;
                Expect(TokenType.COMMA);
                aReg = ExpectAddressRegister();
                Expect(TokenType.RPAREN);
                return new MemoryOperand(null, aReg, Constant.Int32(offset));
            case TokenType.ID:
                var id = Expect(TokenType.ID);
                aReg = AReg(id);
                if (aReg != null)
                {
                    Expect(TokenType.RPAREN);
                    if (PeekAndDiscard(TokenType.PLUS))
                    {
                        return new PostIncrementMemoryOperand(null, aReg);
                    }
                    else
                    {
                        return new MemoryOperand(null, aReg, Constant.Int32(offset));
                    }
                }
                Error("oo");
                return null;
            default:
                Error("Unexpected address expression token '{0}'.", tok.Text);
                return null;
            }
        }

        private MachineOperand ForwardEa(Symbol symbol)
        {
            //$TODO: remember me so we can backpatch.
            return new MemoryOperand(null, Registers.a0);
        }

        private AddressRegister ExpectAddressRegister()
        {
            var name = Expect(TokenType.ID);
            Debug.Assert(!string.IsNullOrEmpty(name));
            var aReg = AReg(name);
            if (aReg != null)
                return aReg;
            Error("Expect address register but saw '{0}.", name);
            return null;
        }

        private AddressRegister AReg(string name)
        {
            if (IsAddressRegisterName(name))
            {
                return Registers.AddressRegister(name[1] - '0');
            }
            return null;
        }

        private static bool IsAddressRegisterName(string name)
        {
            return name.Length == 2 && "aA".IndexOf(name[0]) >= 0 && '0' <= name[1] && name[1] <= '7';
        }

        private void Emit_i_ea(int i, MachineOperand eaDst, int size, Action<int, MachineOperand> b, Action<int, MachineOperand> w, Action<int, MachineOperand> l)
        {
            switch (size)
            {
            case 0: b(i, eaDst); break;
            case 1: w(i, eaDst); break;
            case 2: l(i, eaDst); break;
            default: throw new InvalidOperationException();
            }
        }

        private void Emit_msk_a(RegisterSetOperand mask, AddressRegister aReg,
            int size,
            Action<RegisterSetOperand, AddressRegister> b,
            Action<RegisterSetOperand, AddressRegister> w,
            Action<RegisterSetOperand, AddressRegister> l)
        {
            switch (size)
            {
            case 0: b(mask, aReg); break;
            case 1: w(mask, aReg); break;
            case 2: l(mask, aReg); break;
            default: throw new InvalidOperationException();
            }
        }

        private void ProcessPublic()
        {
            Expect(TokenType.ID);
        }

        private void ProcessCnop()
        {
            int extra = ExpectInteger();
            Expect(TokenType.COMMA);
            int align = ExpectInteger();
            asm.Cnop(extra, align);
        }

        private void ProcessSection()
        {
            Expect(TokenType.STRING);
            Expect(TokenType.COMMA);
            Expect(TokenType.ID);
        }

        private void ProcessOpt()
        {
            var t = lexer.PeekToken();
            if (t.Type == TokenType.ID || t.Type == TokenType.INTEGER)
            {
                lexer.GetToken();
                return;
            }
            throw new NotSupportedException();
        }

        private void ProcessIdnt()
        {
            Expect(TokenType.STRING);
        }

        private string Expect(TokenType tokenType)
        {
            var t = lexer.GetToken();
            if (t.Type != tokenType)
                Error("Expected {0} but found {1}.", tokenType, t.Type);
            return t.Text;
        }

        private bool PeekAndDiscard(TokenType tokenType)
        {
            if (lexer.PeekToken().Type != tokenType)
                return false;

            lexer.GetToken();
            return true;
        }

        private void Error(string format, params object[] args)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Line {0}: ", LineNumber);
            sb.AppendFormat(format, args);
            throw new AssemblerException(sb.ToString());
        }

        public Program AssembleFragment(Address baseAddress, string fragment)
        {
            var rdr = new StringReader(fragment);
            return Assemble(baseAddress, rdr);
        }

        public LoadedImage GetImage()
        {
            return new LoadedImage(addrBase, emitter.GetBytes());
        }

        public LoadedImage Image
        {
            get { return GetImage(); }
        }

        public Address StartAddress
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<EntryPoint> EntryPoints
        {
            get { return entryPoints; }
        }

        public IProcessorArchitecture Architecture { get { return arch; } }

        public Platform Platform
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<Address, ImportReference> ImportReferences
        {
            get { return new Dictionary<Address,ImportReference>(); }
        }

        #endregion

        public void BraB(string target)
        {
            emitter.EmitByte(0x60);
            emitter.EmitByte(-(emitter.Length + 1));
            asm.Symbols.CreateSymbol(target).ReferToLe(emitter.Length - 1, PrimitiveType.Byte, emitter);
        }

        public void Move(DataRegister dSrc, DataRegister dDst)
        {
            int opcode = 0x2000 | dSrc.Number | (dSrc.Number << 9);
            emitter.EmitBeUInt16(opcode);
        }
   }
}
