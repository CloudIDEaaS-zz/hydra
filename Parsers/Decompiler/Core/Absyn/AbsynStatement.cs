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

using Decompiler.Core.Code;
using System;

namespace Decompiler.Core.Absyn
{
	/// <summary>
	/// Base class for all abstract syntax statements.
	/// </summary>
	public abstract class AbsynStatement : Instruction
	{
		public abstract void Accept(IAbsynVisitor visitor);

		public override sealed Instruction Accept(InstructionTransformer xform)
		{
			return this;
		}

		public override sealed void Accept(InstructionVisitor v)
		{
			Accept((IAbsynVisitor) v);
		}

        public sealed override T Accept<T>(InstructionVisitor<T> visitor)
        {
            Accept((IAbsynVisitor)visitor);
            return default(T);
        }

		public override bool IsControlFlow
		{
			get { return false; }
		}
    }
}
