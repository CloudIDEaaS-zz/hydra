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
	/// Represents a C-style dereferenced pointer: *foo
	/// </summary>
	public class Dereference : Expression
	{
		private Expression exp;

		public Dereference(DataType ptrType, Expression exp) : base(ptrType)
		{
            this.exp = exp; if (exp == null) throw new ArgumentNullException();
		}

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitDereference(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitDereference(this);
        }

		public override void Accept(IExpressionVisitor visit)
		{
			visit.VisitDereference(this);
		}

		public override Expression CloneExpression()
		{
			return new Dereference(DataType, exp.CloneExpression());
		}

		public Expression Expression
		{
			get { return exp; }
            set { exp = value; if (exp == null) throw new ArgumentNullException(); }
		}

	}
}
