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

using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Expressions;
using Decompiler.Core.Rtl;
using Decompiler.Core.Machine;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
    /// <summary>
    /// ProcessorState simulates the state of the processor and a part of the stack during scanning.
    /// </summary>
    public abstract class ProcessorState : EvaluationContext
    {
        private Dictionary<RegisterStorage, Expression> linearDerived;
        private SortedList<int, Expression> stackState;

        public ProcessorState()
        {
            this.linearDerived = new Dictionary<RegisterStorage, Expression>();
            this.stackState = new SortedList<int, Expression>();
        }

        public ProcessorState(ProcessorState orig)
        {
            this.linearDerived = new Dictionary<RegisterStorage, Expression>(orig.linearDerived);
            this.stackState = new SortedList<int, Expression>(orig.stackState);
            this.ErrorListener = this.ErrorListener;
        }

        /// <summary>
        /// Method to call if an error occurs within the processor state object (such as stack over/underflows).
        /// </summary>
        public Action<string> ErrorListener { get; set; }

        public abstract IProcessorArchitecture Architecture { get; }
        public abstract ProcessorState Clone();

        public abstract Constant GetRegister(RegisterStorage r);
        public abstract void SetRegister(RegisterStorage r, Constant v);
        public abstract void SetInstructionPointer(Address addr);

        public abstract void OnProcedureEntered();                 // Some registers need to be updated when a procedure is entered.
        public abstract void OnProcedureLeft(ProcedureSignature procedureSignature);

        /// <summary>
        /// Captures the the processor's state before calling a procedure.
        /// </summary>
        /// <param name="stackReg">An identifier for the stack register of the processor.</param>
        /// <param name="returnAddressSize">The size of the return address on stack.</param>
        /// <returns>A CallSite object that abstracts the processor state right before the call.</returns>
        public abstract CallSite OnBeforeCall(Identifier stackReg, int returnAddressSize);

        /// <summary>
        /// Perform any adjustments to the processor's state after returning from a procedure call with the
        /// specified signature.
        /// </summary>
        /// <param name="sigCallee">The signature of the called procedure.</param>
        public abstract void OnAfterCall(Identifier stackReg, ProcedureSignature sigCallee, ExpressionVisitor<Expression> eval);

        private bool IsStackRegister(Expression ea)
        {
            var id = ea as Identifier;
            if (id == null)
                return false;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return false;
            return (reg == Architecture.StackRegister);
        }

        private bool GetStackOffset(Expression ea, out int offset)
        {
            if (IsStackRegister(ea))
            {
                offset = 0;
                return true;
            }

            var bin = ea as BinaryExpression;
            if (bin != null && (bin.Operator == Operator.IAdd || bin.Operator == Operator.ISub) && IsStackRegister(bin.Left))
            {
                var cOffset = bin.Right as Constant;
                if (cOffset != null)
                {
                    offset = ((Constant)bin.Right).ToInt32();
                    if (bin.Operator == Operator.ISub)
                        offset = -offset;
                    return true;
                }
            }
            offset = 0;
            return false;
        }

        public Expression GetValue(Identifier id)
        {
            if (id.Storage is TemporaryStorage)
                return Constant.Invalid;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return Constant.Invalid;

            return GetValue(reg);
        }

        public Expression GetValue(RegisterStorage reg)
        {
            Expression exp = GetRegister(reg);
            if (exp != Constant.Invalid)
                return exp;
            if (linearDerived.TryGetValue(reg, out exp))
                return exp;
            return Constant.Invalid;
        }

        public Expression GetValue(MemoryAccess access)
        {
            int stackOffset;
            if (GetStackOffset(access.EffectiveAddress, out stackOffset))
            {
                Expression value;
                if (stackState.TryGetValue(stackOffset, out value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(SegmentedAccess access)
        {
            int stackOffset;
            if (GetStackOffset(access.EffectiveAddress, out stackOffset))
            {
                Expression value;
                if (stackState.TryGetValue(stackOffset, out value))
                    return value;
            }
            return Constant.Invalid;
        }

        public Expression GetValue(Application appl)
        {
            return Constant.Invalid;
        }

        public Expression GetDefiningExpression(Identifier id)
        {
            return null;
        }

        public void RemoveIdentifierUse(Identifier id)
        {
        }

        public void UseExpression(Expression expr)
        {
        }

        public void SetValue(Identifier id, Expression value)
        {
            if (id.Storage is TemporaryStorage)
                return;
            var reg = id.Storage as RegisterStorage;
            if (reg == null)
                return;
            SetValue(reg, value);
        }

        public Constant SetValue(RegisterStorage reg, Expression value)
        {
            var constVal = value as Constant;
            if (constVal != null)
            {
                SetRegister(reg, constVal);
                linearDerived.Remove(reg);
                return constVal;
            }
            var binVal = value as BinaryExpression;
            if (binVal != null)
            {
                if ((binVal.Operator == Operator.IAdd || binVal.Operator == Operator.ISub) &&
                    binVal.Left is Identifier &&
                    binVal.Right is Constant)
                {
                    linearDerived[reg] = binVal;
                    return Constant.Invalid;
                }
            }
            SetRegister(reg, Constant.Invalid);
            linearDerived.Remove(reg);
            return Constant.Invalid;
        }

        public void SetValueEa(Expression ea, Expression value)
        {
            int stackOffset;
            if (GetStackOffset(ea, out stackOffset))
                stackState[stackOffset] = value;
        }

        public void SetValueEa(Expression basePtr, Expression ea, Expression value)
        {
            int stackOffset;
            if (GetStackOffset(ea, out stackOffset))
                stackState[stackOffset] = value;
        }

        public bool IsUsedInPhi(Identifier id)
        {
            return false;
        }
    }
}
