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

using Decompiler.Core.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Rtl
{
    public class RtlBranch : RtlInstruction
    {
        public RtlBranch(Expression condition, Address target, RtlClass rtlClass) 
        {
            this.Condition = condition;
            this.Target = target;
            this.Class = rtlClass;
        }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitBranch(this);
        }

        public Expression Condition { get; private set; }

        public Address Target { get; private set; }

        protected override void WriteInner(TextWriter writer)
        {
            if (Condition != null)
            {
                writer.Write("if (");
                writer.Write(Condition);
                writer.Write(") ");
            }
            writer.Write("branch {0}", Target);
        }
    }
}
