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
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Z80
{
    public class MemoryOperand : MachineOperand
    {
        public RegisterStorage Base;
        public Constant Offset;

        public MemoryOperand(RegisterStorage baseReg, PrimitiveType type): base(type)
        {
            this.Base = baseReg;
        }

        public MemoryOperand(Constant offset, PrimitiveType type) : base(type)
        {
            this.Offset = offset;
        }

        public MemoryOperand(RegisterStorage baseReg, sbyte offset, PrimitiveType type) : base(type)
        {
            this.Base = baseReg;
            this.Offset = Constant.SByte(offset);
        }

        public override void Write(bool fExplicit, MachineInstructionWriter writer)
        {
            if (Base != null)
            {
                if (Offset != null)
                {
                    int offset = Offset.ToInt32();
                    string fmt;
                    if (offset > 0)
                    {
                        fmt = "({0}+{1:X2})";
                    } 
                    else if (offset < 0)
                    {
                        offset = -offset;
                        fmt = "({0}-{1:X2})";
                    }
                    else 
                    {
                        fmt = "({0})";
                    }
                    writer.Write(string.Format(fmt, Base, offset));
                }
                else
                {
                    writer.Write(string.Format("({0})", Base));
                }
            }
            else
            {
                writer.Write("(");
                writer.WriteAddress(string.Format("{0:X4}", Offset.ToUInt16()), Address.Ptr16(Offset.ToUInt16()));
                writer.Write(")");
            }
        }
    }
}
