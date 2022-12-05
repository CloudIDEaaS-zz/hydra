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
    /// Models an array access.
    /// </summary>
	public class ArrayAccess : Expression
	{
		public ArrayAccess(DataType elementType, Expression array, Expression index) : base(elementType)
		{
            if (array == null) throw new ArgumentNullException("array");
            if (index == null) throw new ArgumentNullException("index");
			this.Array = array; this.Index = index;
		}

        public Expression Array { get; private set; }
        public Expression Index { get; private set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitArrayAccess(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitArrayAccess(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitArrayAccess(this);
		}

		public override Expression CloneExpression()
		{
			return new ArrayAccess(DataType, Array.CloneExpression(), Index.CloneExpression());
		}
	}
}
