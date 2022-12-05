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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Linq;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class SegmentedAccessClassifierTests
	{
		private Procedure proc;
		private SsaIdentifierCollection ssaIds;

		[Test]
		public void Associate()
		{
			var foo = new Identifier("foo", PrimitiveType.SegmentSelector, null);
			var bar = new Identifier("bar", PrimitiveType.Word16, null);
			var mpc = new SegmentedAccessClassifier(null, null);
			mpc.Associate(foo, bar);
			Assert.IsNotNull(mpc.AssociatedIdentifier(foo), "Bar should be associated");
			mpc.Associate(foo, bar);
			Assert.IsNotNull(mpc.AssociatedIdentifier(foo), "Bar should still be associated");
		}

		[Test]
		public void Disassociate()
		{
			Identifier foo = new Identifier("foo", PrimitiveType.SegmentSelector, null);
			Identifier bar = new Identifier("bar", PrimitiveType.Word16, null);
			Identifier baz = new Identifier("baz", PrimitiveType.Word16, null);
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(null, null);
			mpc.Associate(foo, bar);
			mpc.Associate(foo, baz);
			Assert.IsNull(mpc.AssociatedIdentifier(foo), "Bar should no longer be associated");
		}

		[Test]
		public void AssociateConsts()
		{
			Identifier ptr = new Identifier("ptr", PrimitiveType.SegmentSelector, null);
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(null, null);
			mpc.Associate(ptr, Constant.Word32(3));
			mpc.Associate(ptr, Constant.Word32(4));
			Assert.IsTrue(mpc.IsOnlyAssociatedWithConstants(ptr), "Should only have been associated with constants");
		}

		[Test]
		public void DisassociateConsts()
		{
			Identifier ptr = new Identifier("ptr", PrimitiveType.SegmentSelector, null);
			Identifier mp =  new Identifier("mp",  PrimitiveType.SegmentSelector, null);
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(null, null);
			mpc.Associate(ptr, Constant.Word32(3));
			mpc.Associate(ptr, mp);
			mpc.Associate(ptr, Constant.Word32(4));
			Assert.IsFalse(mpc.IsOnlyAssociatedWithConstants(ptr), "Should have been disassociated");
		}



		[Test]
		public void Classify1()
		{
			Prepare(new Mp1());
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(proc, ssaIds);
			mpc.Classify();
            Identifier ds = ssaIds.Where(s => s.Identifier.Name == "ds").Single().Identifier;
			Identifier bx = ssaIds.Where(s => s.Identifier.Name == "bx").Single().Identifier;
			Identifier a = mpc.AssociatedIdentifier(ds);
			Assert.AreSame(a, bx);
		}

		[Test]
		public void Classify2()
		{
			Prepare(new Mp2());
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(proc, ssaIds);
			mpc.Classify();
            Identifier ds = ssaIds.Where(s => s.Identifier.Name == "ds").Single().Identifier;
			Assert.AreEqual("ds", ds.Name);
            Identifier bx = ssaIds.Where(s => s.Identifier.Name == "bx").Single().Identifier;
			Assert.AreEqual("bx", bx.Name);
			Identifier a = mpc.AssociatedIdentifier(ds);
			Assert.AreSame(a, bx);
		}

		[Test]
		public void Classify3()
		{
			Prepare(new Mp3());
			SegmentedAccessClassifier mpc = new SegmentedAccessClassifier(proc, ssaIds);
			mpc.Classify();
            Identifier ds = ssaIds.Where(s => s.Identifier.Name == "ds").Single().Identifier;
			Assert.AreEqual("ds", ds.Name);
            Identifier bx = ssaIds.Where(s => s.Identifier.Name == "bx").Single().Identifier;
			Assert.AreEqual("bx", bx.Name);
			Identifier a = mpc.AssociatedIdentifier(ds);
			Assert.IsNull(a, "ds is used both as ds:[bx+4] and ds:[0x3000], it should't be strongly associated with a register");
		}

		private void Prepare(ProcedureBuilder mock)
		{
			proc = mock.Procedure;
			SsaTransform tr = new SsaTransform(new ProgramDataFlow(), proc, proc.CreateBlockDominatorGraph());
			ssaIds = tr.SsaState.Identifiers;
		}

		public class Mp1 : ProcedureBuilder
		{
			protected override void BuildBody()
			{
				Identifier ds = this.Local16("ds");
				Identifier ax = this.Local16("ax");
				Identifier bx = this.Local16("bx");

				LoadId(ax, SegMem(PrimitiveType.Word16, ds, IAdd(bx, Int16(4))));
			}
		}

		public class Mp2 : ProcedureBuilder
		{
			// ds:bx is used in both fetches, so ds is strongly associated with bx.
			protected override void BuildBody()
			{
				Identifier ds = this.Local16("ds");
				Identifier ax = this.Local16("ax");
				Identifier bx = this.Local16("bx");

				LoadId(ax, SegMem(PrimitiveType.Word16, ds, IAdd(bx, Int16(4))));
				LoadId(ax, SegMem(PrimitiveType.Word16, ds, IAdd(bx, Int16(8))));
			}
		}

		public class Mp3 : ProcedureBuilder
		{
			// ds is used both as ds:[bx+4] and ds:[0x3000], which means
			// it isn't strongly associated with  any register.

			protected override void BuildBody()
			{
				Identifier ds = Local16("ds");
				Identifier ax = Local16("ax");
				Identifier bx = Local16("bx");
				LoadId(ax, MembPtrW(ds, IAdd(bx, Int16(4))));
				LoadId(ax, MembPtrW(ds, Int16(0x3000)));
			}
		}
	}
}
