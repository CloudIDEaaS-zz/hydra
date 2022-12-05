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

using Decompiler.Core.Expressions;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Typing
{
	/// <summary>
	/// Matches and picks apart component of an array access expression.
	/// </summary>
	public class ArrayExpressionMatcher
	{
		private Constant elemSize;
        private PrimitiveType dtPointer;

		public ArrayExpressionMatcher(PrimitiveType dtPointer)
		{
            this.dtPointer = dtPointer;
		}

		public Expression ArrayPointer { get; set; }

        public Expression Index { get; set; }

			
		public Constant ElementSize
		{
			get { return elemSize; }
		}

		public bool MatchMul(BinaryExpression b)
		{
			if (b.Operator == Operator.SMul || b.Operator == Operator.UMul || b.Operator == Operator.IMul)
			{
				Constant c = b.Left as Constant;
				Expression e = b.Right;
				if (c == null)
				{
					c = b.Right as Constant;
					e = b.Left;
				}
				if (c != null)
				{
					elemSize = c;
					Index = e;
					return true;
				}
			}
			if (b.Operator == Operator.Shl)
			{
				Constant c = b.Right as Constant;
				if (c != null)
				{
					elemSize = b.Operator.ApplyConstants(Constant.Create(b.Left.DataType, 1), c);
					Index = b.Left;
					return true;
				}
			}
			return false;
		}

		public bool Match(Expression e)
		{
			elemSize = null;
			Index = null;
			ArrayPointer = null;

			BinaryExpression b = e as BinaryExpression;
			if (b == null)
				return false;
			if (MatchMul(b))
				return true;

			// (+ x y)
			if (b.Operator == Operator.IAdd)
			{
				BinaryExpression bInner = b.Left as BinaryExpression;
				if (bInner != null)
				{
					if (MatchMul(bInner))
					{
						// (+ (* i c) ptr)
						ArrayPointer = b.Right;
						return true;
					}
				}
				bInner = b.Right as BinaryExpression;
				if (bInner != null)
				{
					if (MatchMul(bInner))
					{
						// (+ ptr (* i c))
						ArrayPointer = b.Left;
						return true;
					}
					if (bInner.Operator == Operator.IAdd)
					{
						// (+ x (+ a b)) 
						var bbInner = bInner.Left as BinaryExpression;
                        if (bbInner != null && MatchMul(bbInner))
                        {
                            // (+ x (+ (* i c) y)) rearranges to become
                            // (+ (* i c) (+ x y))

                            this.ArrayPointer = new BinaryExpression(
                                Operator.IAdd,
                                PrimitiveType.Create(Domain.Pointer, b.Left.DataType.Size),
                                b.Left,
                                bInner.Right);
                            return true;
                        }
					}
				}
			}
			return false;
		}

		public Expression Transform(Expression baseptr, DataType dtAccess)
		{
            if (baseptr != null)
            {
                ArrayPointer = new MkSequence(dtPointer, baseptr, ArrayPointer);
            }
			return new ArrayAccess(
                dtAccess, 
                ArrayPointer,
                new BinaryExpression(
                    BinaryOperator.IMul, 
                    Index.DataType,
                    Index,
                    ElementSize));
		}
	}
}
