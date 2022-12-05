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

using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Expressions
{
	/// <summary>
	/// Represents an application of a procedure to a list of arguments.
	/// </summary>
	public class Application : Expression
	{
		public Application(Expression proc, DataType retVal, params Expression [] arguments) : base(retVal)
		{
			this.Procedure = proc;
			this.Arguments = arguments;
		}

        public Expression Procedure { get; set; }
        public Expression[] Arguments { get; set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitApplication(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitApplication(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitApplication(this);
		}

		public override Expression CloneExpression()
		{
			Expression [] p = new Expression[Arguments.Length];
			for (int i = 0; i < p.Length; ++i)
			{
				p[i] = Arguments[i].CloneExpression();
			}
			return new Application(this.Procedure, this.DataType, p);
		}

		public override Expression Invert()
		{
			return new UnaryExpression(Operator.Not, PrimitiveType.Bool, this);
		}
	}
}
