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

using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core.Machine
{
    /// <summary>
    /// Abstract base class for low-level machine instructions.
    /// </summary>
    public abstract class MachineInstruction
    {
        /// <summary>
        /// The address at which the instruction begins.
        /// </summary>
        public Address Address;

        /// <summary>
        /// The length of the entire instruction. Some architectures, e.g. M68k, x86, and most
        /// 8-bit microprocessors, have variable length instructions.
        /// </summary>
        public int Length;

        public virtual void Render(MachineInstructionWriter writer)
        {
        }

        /// <summary>
        /// Each different supported opcode should have a different numerical value, exposed here.
        /// </summary>
        public abstract int OpcodeAsInteger { get; }

        public sealed override string ToString()
        {
            var renderer = new StringRenderer();
            this.Render(renderer);
            return renderer.ToString();
        }

        public string ToString(Platform platform)
        {
            var renderer = new StringRenderer(platform);
            this.Render(renderer);
            return renderer.ToString();

        }
    }
}
