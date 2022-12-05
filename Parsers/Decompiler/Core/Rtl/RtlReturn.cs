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

using Decompiler.Core.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Core.Rtl
{
    public class RtlReturn : RtlInstruction
    {
        public RtlReturn(int returnAddressBytes, int extraBytesPopped, RtlClass rtlClass)
        {
            this.ReturnAddressBytes = returnAddressBytes;
            this.ExtraBytesPopped = extraBytesPopped;
            this.Class = rtlClass;
        }

        /// <summary>
        /// The stack size of the return address in bytes.
        /// </summary>
        /// <remarks>
        /// Architectures where the return address is not passed on the stack should specify 0 as the
        /// size of this property.
        /// </remarks>
        public int ReturnAddressBytes { get; private set; }
        public int ExtraBytesPopped { get; private set; }

        public override T Accept<T>(RtlInstructionVisitor<T> visitor)
        {
            return visitor.VisitReturn(this);
        }

        protected override void WriteInner(TextWriter writer)
        {
            writer.Write("return ({0},{1})", ReturnAddressBytes, ExtraBytesPopped);
        }
    }
}

