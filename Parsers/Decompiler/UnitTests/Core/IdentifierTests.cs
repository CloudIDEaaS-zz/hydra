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

using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class IdentifierTests
	{
		private Identifier reg;
		private Identifier outReg;

		public IdentifierTests()
		{
			reg = new Identifier("eax", PrimitiveType.Word32, Registers.eax);
			outReg = new Identifier("eaxOut", PrimitiveType.Pointer32, new OutArgumentStorage(reg));
		}

		[Test]
		public void RegToString()
		{
			Assert.AreEqual("eax", reg.ToString());
		}

        [Test]
        public void HashCode()
        {
            Assert.AreNotEqual(reg.GetHashCode(), outReg.GetHashCode());
            Assert.IsFalse(object.Equals(reg, outReg));
        }
	}
}