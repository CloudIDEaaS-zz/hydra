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

namespace Decompiler.Core.Expressions
{
    /// <summary>
    /// Models an access to memory, using the effective address <paramref name="ea"/> and the datatype
    /// of the accessed memory.
    /// </summary>
    public class MemoryAccess : Expression
    {
        public MemoryAccess(Expression ea, DataType dt)
            : base(dt)
        {
            this.MemoryId = MemoryIdentifier.GlobalMemory;
            this.EffectiveAddress = ea;
        }

        public MemoryAccess(MemoryIdentifier id, Expression ea, DataType dt)
            : base(dt)
        {
            this.MemoryId = id;
            this.EffectiveAddress = ea;
        }

        public MemoryIdentifier MemoryId { get; set; }
        public Expression EffectiveAddress { get; set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMemoryAccess(this, context);
        }

        public override void Accept(IExpressionVisitor v)
        {
            v.VisitMemoryAccess(this);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMemoryAccess(this);
        }

        public override Expression CloneExpression()
        {
            return new MemoryAccess(EffectiveAddress.CloneExpression(), DataType);
        }

        public static MemoryAccess Create(Expression baseRegister, int offset, DataType dt)
        {
            return new MemoryAccess(MemoryIdentifier.GlobalMemory, CreateEffectiveAddress(baseRegister, offset), dt);
        }

        protected static Expression CreateEffectiveAddress(Expression baseRegister, int offset)
        {
            if (offset == 0)
                return baseRegister;
            else
                return new BinaryExpression(Operators.Operator.IAdd,
                    baseRegister.DataType,
                    baseRegister,
                    Constant.Create(PrimitiveType.Create(Domain.SignedInt, baseRegister.DataType.Size), offset));
        }
    }

	/// <summary>
	/// Segmented memory access that models x86 segmented memory addressing.
	/// </summary>
	public class SegmentedAccess : MemoryAccess
	{
		public SegmentedAccess(MemoryIdentifier id, Expression basePtr, Expression ea, DataType dt) : base(id, ea, dt)
		{
			this.BasePointer = basePtr;
		}

        public Expression BasePointer { get; set; }         // Segment selector

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitSegmentedAccess(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> visit)
        {
            return visit.VisitSegmentedAccess(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitSegmentedAccess(this);
		}

		public override Expression CloneExpression()
		{
			return new SegmentedAccess(MemoryId, BasePointer.CloneExpression(), EffectiveAddress.CloneExpression(), DataType);
		}

        public static SegmentedAccess Create(Expression segRegister, Expression baseRegister, int offset, DataType dt)
        {
            return new SegmentedAccess(MemoryIdentifier.GlobalMemory, segRegister, CreateEffectiveAddress(baseRegister, offset), dt);
        }
	}
}
