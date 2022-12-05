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
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Structure
{
    public class AbsynStatementEmitter : InstructionVisitor, IAbsynVisitor
    {
        private List<AbsynStatement> stms;

        public AbsynStatementEmitter(List<AbsynStatement> stms)
        {
            this.stms = stms;
        }

        public void EmitStatement(Statement stm)
        {
            stm.Instruction.Accept(this);
        }

        public void EmitStatement(AbsynStatement stm)
        {
            stms.Add(stm);
        }

        public void EmitCaseLabel(StructureNode node, int i)
        {
            stms.Add(new AbsynCase(i));
        }

        public void EmitBreak()
        {
            stms.Add(new AbsynBreak());
        }

        public void EmitContinue()
        {
            stms.Add(new AbsynContinue());
        }

        public void EmitGoto(StructureNode dest)
        {
            stms.Add(new AbsynGoto(dest.Block.Name));
        }

        public void EmitReturn(Expression expr)
        {
            stms.Add(new AbsynReturn(expr));
        }

        public void EmitLabel(StructureNode node)
        {
            stms.Add(new AbsynLabel(node.Block.Name));
        }

        public void EmitForever(StructureNode node, List<AbsynStatement> body)
        {
            AbsynWhile whileStm = new AbsynWhile(Constant.True(), body);
            stms.Add(whileStm);
        }

        public void EmitDoWhile(List<AbsynStatement> body, Expression expr)
        {
            AbsynDoWhile doWhile = new AbsynDoWhile(body, expr);
            stms.Add(doWhile);
        }

        public void EmitWhile(StructureNode node, Expression expr, List<AbsynStatement> body)
        {
            if (node.Then == node.Loop.Follow)
                expr = expr.Invert();
            stms.Add(new AbsynWhile(expr, body));
        }

        public AbsynSwitch EmitSwitch(StructureNode node, Expression exp, List<AbsynStatement> stmts)
        {
            AbsynSwitch switchStm = new AbsynSwitch(exp, stmts);
            stms.Add(switchStm);
            return switchStm;
        }

        public bool StripDeclarations { get; set; }


        #region InstructionVisitor Members

        void InstructionVisitor.VisitAssignment(Assignment a)
        {
            stms.Add(new AbsynAssignment(a.Dst, a.Src));
        }

        void InstructionVisitor.VisitBranch(Branch b)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitCallInstruction(CallInstruction ci)
        {
            stms.Add(new AbsynSideEffect(new Application(ci.Callee, VoidType.Instance)));
        }

        void InstructionVisitor.VisitDeclaration(Declaration decl)
        {
            if (StripDeclarations)
            {
                if (decl.Expression != null)
                {
                    stms.Add(new AbsynAssignment(decl.Identifier, decl.Expression));
                }
            }
            else
            {
                stms.Add(new AbsynDeclaration(decl.Identifier, decl.Expression));
            }
        }

        void InstructionVisitor.VisitDefInstruction(DefInstruction def)
        {
            stms.Add(new AbsynLineComment(def.ToString()));
        }

        void InstructionVisitor.VisitGotoInstruction(GotoInstruction g)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitPhiAssignment(PhiAssignment phi)
        {
            stms.Add(new AbsynLineComment(phi.ToString()));
        }

        void InstructionVisitor.VisitReturnInstruction(ReturnInstruction ret)
        {
            stms.Add(new AbsynReturn(ret.Expression));
        }

        void InstructionVisitor.VisitSideEffect(SideEffect side)
        {
            stms.Add(new AbsynSideEffect(side.Expression));
        }

        void InstructionVisitor.VisitStore(Store store)
        {
            stms.Add(new AbsynAssignment(store.Dst, store.Src));
        }

        void InstructionVisitor.VisitSwitchInstruction(SwitchInstruction si)
        {
            throw new NotImplementedException();
        }

        void InstructionVisitor.VisitUseInstruction(UseInstruction u)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAbsynVisitor Members

        public void VisitAssignment(AbsynAssignment ass)
        {
            stms.Add(ass);
        }

        public void VisitBreak(AbsynBreak brk)
        {
            stms.Add(brk);
        }

        public void VisitCase(AbsynCase absynCase)
        {
            stms.Add(absynCase);
        }

        public void VisitContinue(AbsynContinue cont)
        {
            stms.Add(cont);
        }

        public void VisitDeclaration(AbsynDeclaration decl)
        {
            stms.Add(decl);
        }

        public void VisitDefault(AbsynDefault def)
        {
            stms.Add(def);
        }

        public void VisitDoWhile(AbsynDoWhile loop)
        {
            stms.Add(loop);
        }

        public void VisitGoto(AbsynGoto gotoStm)
        {
            stms.Add(gotoStm);
        }

        public void VisitIf(AbsynIf ifStm)
        {
            stms.Add(ifStm);
        }

        public void VisitLabel(AbsynLabel lbl)
        {
            stms.Add(lbl);
        }

        public void VisitLineComment(AbsynLineComment comment)
        {
            stms.Add(comment);
        }

        public void VisitReturn(AbsynReturn ret)
        {
            stms.Add(ret);
        }

        public void VisitSideEffect(AbsynSideEffect side)
        {
            stms.Add(side);
        }

        public void VisitSwitch(AbsynSwitch absynSwitch)
        {
            stms.Add(absynSwitch);
        }

        public void VisitWhile(AbsynWhile loop)
        {
            stms.Add(loop);
        }

        #endregion
    }
}
