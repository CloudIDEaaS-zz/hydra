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
using Decompiler.Core.Code;

namespace Decompiler.Core
{
    /// <summary>
    /// Statements constitute the contents basic blocks.
    /// </summary>
	public class Statement
	{
        public Statement(ulong linearAddress, Instruction instr, Block block)
        {
            this.LinearAddress = linearAddress;
            this.Instruction = instr;
            this.Block = block;
        }

        public Block Block { get; set; }
        public Instruction Instruction { get; set; }
        public ulong LinearAddress { get; private set; }

		public override string ToString()
		{
			return Instruction.ToString();
		}
	}
}
