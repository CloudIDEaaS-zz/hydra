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

using Decompiler.Core.Types;
using System;

namespace Decompiler.Core
{
	public class EntryPoint
	{
		public EntryPoint(Address addr, ProcessorState state) : this(addr, null, state)
		{
		}

		public EntryPoint(Address addr, string name, ProcessorState state)
		{
            if (addr == null)
                throw new ArgumentNullException("addr");
            if (state == null)
				throw new ArgumentNullException("state");
			this.Address = addr;
			this.Name = name;
			this.ProcessorState = state;
		}

		public Address Address { get; private set; }
		public string Name { get; private set; }
		public ProcessorState ProcessorState { get; private set; }
	}
}