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
    /// Models a C-style cast. The specified expression is cast to the data type <code>dt</code>.
    /// </summary>
	public class Cast : Expression
	{
		public Cast(DataType dt, Expression expr) : base(dt)
		{
			this.Expression = expr;
		}

        public Expression Expression { get; set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitCast(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitCast(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitCast(this);
		}

		public override Expression CloneExpression()
		{
			return new Cast(DataType, Expression.CloneExpression());
		}
	}
}
