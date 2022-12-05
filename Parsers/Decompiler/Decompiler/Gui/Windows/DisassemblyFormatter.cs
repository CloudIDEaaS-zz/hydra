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
using Decompiler.Core.Machine;
using Decompiler.Gui.Windows.Controls;
using System.Collections.Generic;
using System;
using System.Text;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Used to render TextSpans for use in the disassembly viewer.
    /// </summary>
    public class DisassemblyFormatter : MachineInstructionWriter 
    {
        private Program program;
        private StringBuilder sb = new StringBuilder();
        private List<TextSpan> line;

        public DisassemblyFormatter(Program program, List<TextSpan> line)
        {
            this.program = program;
            this.line = line;
            this.Platform = program.Platform;
        }

        public Platform Platform { get; private set; }

        public void WriteOpcode(string opcode)
        {
            line.Add(new DisassemblyTextModel.InertTextSpan(opcode, "opcode"));
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            TerminateSpan();
            Procedure proc;
            TextSpan span;
            if (program.Procedures.TryGetValue(addr, out proc))
            {
                span = new DisassemblyTextModel.ProcedureTextSpan(proc, addr);
            }
            else
            {
                span = new DisassemblyTextModel.AddressTextSpan(addr, formattedAddress);
            }
            line.Add(span);
        }

        public void Tab()
        {
            TerminateSpan();
        }

        private void TerminateSpan()
        {
            if (sb.Length == 0)
                return;
            var span = new DasmTextSpan
            {
                Text = sb.ToString(),
                Tag = null,
                Style = "",
            };
            line.Add(span);
            sb = new StringBuilder();
        }

        public void Write(char c)
        {
            sb.Append(c);
        }

        public void Write(uint n)
        {
            sb.Append(n);
        }

        public void Write(string s)
        {
            sb.Append(s);
        }

        public void Write(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        public void NewLine()
        {
            TerminateSpan();
        }

        public class DasmTextSpan : TextSpan
        {
            public override string GetText()
            {
                return Text;
            }

            public string Text { get; set; }
        }
    }
}
