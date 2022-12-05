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
	/// Creates a sequence out of two components. Contrast with <see cref="Slice"/>.
	/// </summary>
    /// <remarks>Yeah, this is (cons) for you Lisp fans out there.</remarks>
	public class MkSequence : Expression
	{
		public MkSequence(DataType dt, Expression head, Expression tail) : base(dt)
		{
            if (head == Constant.Invalid || tail == Constant.Invalid)
                throw new ArgumentException();
			Head = head;
			Tail = tail;
		}

        public Expression Head { get; private set; }
        public Expression Tail { get; private set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitMkSequence(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitMkSequence(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitMkSequence(this);
		}

		public override Expression CloneExpression()
		{
			return new MkSequence(DataType, Head.CloneExpression(), Tail.CloneExpression());
		}
	}
}
