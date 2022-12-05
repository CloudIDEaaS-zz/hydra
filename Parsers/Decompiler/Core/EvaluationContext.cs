﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Core
{
    /// <summary>
    /// An EvaluationContext is used by the SymbolicEvaluator to provide a statement context for the
    /// evaluation.
    /// </summary>
    /// <remarks>
    /// For instance, it might be interesting to find the expression currently bound to 
    /// an identifier, to see if a simplification could be made. The statements
    /// <code>
    ///     a = constant
    ///     b = a + 3
    /// </code>
    /// can be merged to
    /// <code>
    ///     b = (constant + 3)
    /// </code>
    /// if we know that a == constant.
    /// </remarks>
    public interface EvaluationContext
    {
        /// <summary>
        /// Gets the symbolic value of the identifier <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Expression GetValue(Identifier id);
        Expression GetValue(MemoryAccess access);
        Expression GetValue(SegmentedAccess access);
        Expression GetValue(Application appl);
        Expression GetDefiningExpression(Identifier id);
        void RemoveIdentifierUse(Identifier id);
        void UseExpression(Expression expr);
        void SetValue(Identifier id, Expression value);
        void SetValueEa(Expression ea, Expression value);
        void SetValueEa(Expression basePointer, Expression ea, Expression value);

        bool IsUsedInPhi(Identifier id);
    }
}
