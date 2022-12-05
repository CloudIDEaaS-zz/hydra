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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using System;

namespace Decompiler.Core.Code
{
	/// <summary>
	/// Base class for rebuilding instructions -- and expressions therein.
	/// </summary>
    /// <remarks>Use this class if most of your transformations will be simple copies, but you need to make 
    /// exceptions for a few types of expressions. If your transformation will affect most or all of the instruction
    /// and/or expression types, use <code>ExpressionVisitor</code> directly instead.</remarks>
	public class InstructionTransformer : ExpressionVisitor<Expression>
	{
		public InstructionTransformer()
		{
		}

		public Instruction Transform(Instruction instr)
		{
			return instr.Accept(this);
		}

		#region InstructionTransformer Members

		public virtual Instruction TransformAssignment(Assignment a)
		{
			a.Src = a.Src.Accept(this);
			a.Dst = (Identifier) a.Dst.Accept(this);
			return a;
		}

		public virtual Instruction TransformBranch(Branch b)
		{
			b.Condition = b.Condition.Accept(this);
			return b;
		}

		public virtual Instruction TransformCallInstruction(CallInstruction ci)
		{
			ci.Callee = ci.Callee.Accept(this);
            return ci;
		}

		public virtual Instruction TransformDeclaration(Declaration decl)
		{
			if (decl.Expression != null)
				decl.Expression = decl.Expression.Accept(this);
			return decl;
		}

		public virtual Instruction TransformDefInstruction(DefInstruction def)
		{
			def.Expression = def.Expression.Accept(this);
			return def;
		}

        public virtual Instruction TransformGotoInstruction(GotoInstruction gotoInstruction)
        {
            gotoInstruction.Target = gotoInstruction.Target.Accept(this);
            return gotoInstruction;
        }

		public virtual Instruction TransformPhiAssignment(PhiAssignment phi)
		{
			for (int i = 0; i < phi.Src.Arguments.Length; ++i)
			{
				phi.Src.Arguments[i] = phi.Src.Arguments[i].Accept(this);
			}
			phi.Dst = (Identifier) phi.Dst.Accept(this);
			return phi;
		}

		public virtual Instruction TransformReturnInstruction(ReturnInstruction ret)
		{
			if (ret.Expression != null)
				ret.Expression = ret.Expression.Accept(this);
			return ret;
		}

		public virtual Instruction TransformSideEffect(SideEffect side)
		{
			side.Expression = side.Expression.Accept(this);
			return side;
		}

		public virtual Instruction TransformStore(Store store)
		{
			store.Src = store.Src.Accept(this);
			store.Dst = store.Dst.Accept(this);
			return store;
		}

		public virtual Instruction TransformSwitchInstruction(SwitchInstruction si)
		{
			si.Expression = si.Expression.Accept(this);
			return si;
		}

		public virtual Instruction TransformUseInstruction(UseInstruction u)
		{
			u.Expression = u.Expression.Accept(this);
			return u;
		}

		#endregion

		#region IExpressionTransformer Members

        public virtual Expression VisitAddress(Address addr)
        {
            return addr;
        }

		public virtual Expression VisitApplication(Application appl)
		{
			for (int i = 0; i < appl.Arguments.Length; ++i)
			{
				appl.Arguments[i] = appl.Arguments[i].Accept(this);
			}
			return appl;
		}

		public virtual Expression VisitArrayAccess(ArrayAccess acc)
		{
			var a = acc.Array.Accept(this);
			var i = acc.Index.Accept(this);
			return new ArrayAccess(acc.DataType, a, i);
		}

		public virtual Expression VisitBinaryExpression(BinaryExpression binExp)
		{
			var left = binExp.Left.Accept(this);
			var right = binExp.Right.Accept(this);
            return new BinaryExpression(binExp.Operator, binExp.DataType, left, right);
		}

		public virtual Expression VisitCast(Cast cast)
		{
			cast.Expression = cast.Expression.Accept(this);
			return cast;
		}

		public Expression VisitConditionOf(ConditionOf cof)
		{
			cof.Expression = cof.Expression.Accept(this);
			return cof;
		}

		public virtual Expression VisitConstant(Constant c)
		{
			return c;
		}

		public virtual Expression VisitDepositBits(DepositBits d)
		{
			d.Source = d.Source.Accept(this);
			d.InsertedBits = d.InsertedBits.Accept(this);
			return d;
		}

		public virtual Expression VisitDereference(Dereference deref)
		{
			deref.Expression = deref.Expression.Accept(this);
			return deref;
		}

		public virtual Expression VisitFieldAccess(FieldAccess acc)
		{
			acc.Structure = acc.Structure.Accept(this);
			return acc;
		}

		public virtual Expression VisitIdentifier(Identifier id)
		{
			return id;
		}

		public virtual Expression VisitMemberPointerSelector(MemberPointerSelector mps)
		{
			mps.BasePointer = mps.BasePointer.Accept(this);
			mps.MemberPointer = mps.MemberPointer.Accept(this);
			return mps;
		}
		
		public virtual Expression VisitMemoryAccess(MemoryAccess access)
		{
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			access.MemoryId = (MemoryIdentifier) access.MemoryId.Accept(this);
			return access;
		}

		public virtual Expression VisitMkSequence(MkSequence seq)
		{
			var head = seq.Head.Accept(this);
            var tail = seq.Tail.Accept(this);
			return new MkSequence(seq.DataType, head, tail);
		}

        public virtual Expression VisitOutArgument(OutArgument outArg)
        {
            return new OutArgument(outArg.DataType, outArg.Expression.Accept(this));
        }

		public virtual Expression VisitPhiFunction(PhiFunction phi)
		{
			for (int i = 0; i < phi.Arguments.Length; ++i)
			{
				phi.Arguments[i] = phi.Accept(this);
			}
			return phi;
		}

		public virtual Expression VisitPointerAddition(PointerAddition pa)
		{
			return new PointerAddition(pa.DataType, pa.Pointer.Accept(this), pa.Offset);
		}

		public virtual Expression VisitProcedureConstant(ProcedureConstant pc)
		{
			return pc;
		}

		public virtual Expression VisitSegmentedAccess(SegmentedAccess access)
		{
			access.BasePointer = access.BasePointer.Accept(this);
			access.EffectiveAddress = access.EffectiveAddress.Accept(this);
			access.MemoryId = (MemoryIdentifier) access.MemoryId.Accept(this);
			return access;
		}

		public virtual Expression VisitScopeResolution(ScopeResolution scope)
		{
			return scope;
		}

		public virtual Expression VisitSlice(Slice slice)
		{
			slice.Expression = slice.Expression.Accept(this);
			return slice;
		}

		public virtual Expression VisitTestCondition(TestCondition tc)
		{
			tc.Expression = tc.Expression.Accept(this);
			return tc;
		}

		public virtual Expression VisitUnaryExpression(UnaryExpression unary)
		{
			unary.Expression = unary.Expression.Accept(this);
			return unary;
		}

		#endregion
    }
}
