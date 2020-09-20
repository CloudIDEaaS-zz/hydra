using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using AbstraX.Expressions;
using System.Linq.Expressions;

namespace AbstraX
{
    public class AbstraXVisitor : ExpressionVisitor
    {
        public BaseExpressionNode BaseNode { get; set; }
        private BaseExpressionNode CurrentNode { get; set; }

        public AbstraXVisitor(System.Linq.Expressions.Expression exp)
        {
            this.Visit(exp);
            this.CurrentNode = null;
        }

        protected override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression exp)
        {
            return base.Visit(exp);
        }

        protected override System.Linq.Expressions.Expression VisitBinary(System.Linq.Expressions.BinaryExpression b)
        {
            return base.VisitBinary(b);
        }

        protected override System.Linq.Expressions.Expression VisitConditional(System.Linq.Expressions.ConditionalExpression c)
        {
            Debugger.Break();
            return base.VisitConditional(c);
        }

        protected override System.Linq.Expressions.MemberBinding VisitBinding(System.Linq.Expressions.MemberBinding binding)
        {
            Debugger.Break();
            return base.VisitBinding(binding);
        }

        protected override IEnumerable<System.Linq.Expressions.MemberBinding> VisitBindingList(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.MemberBinding> original)
        {
            Debugger.Break();
            return base.VisitBindingList(original);
        }

        protected override System.Linq.Expressions.Expression VisitConstant(System.Linq.Expressions.ConstantExpression c)
        {
            IValueNode node = this.CurrentNode as IValueNode;

            if (node != null)
            {
                node.Value = new PrimitiveNode(c.Value, this.CurrentNode);
            }

            return base.VisitConstant(c);
        }

        protected override System.Linq.Expressions.ElementInit VisitElementInitializer(System.Linq.Expressions.ElementInit initializer)
        {
            Debugger.Break();
            return base.VisitElementInitializer(initializer);
        }

        protected override IEnumerable<System.Linq.Expressions.ElementInit> VisitElementInitializerList(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ElementInit> original)
        {
            Debugger.Break();
            return base.VisitElementInitializerList(original);
        }

        protected override System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> VisitExpressionList(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression> original)
        {
            return base.VisitExpressionList(original);
        }

        protected override System.Linq.Expressions.Expression VisitInvocation(System.Linq.Expressions.InvocationExpression iv)
        {
            Debugger.Break();
            return base.VisitInvocation(iv);
        }

        protected override System.Linq.Expressions.Expression VisitLambda(System.Linq.Expressions.LambdaExpression lambda)
        {
            return base.VisitLambda(lambda);
        }

        protected override System.Linq.Expressions.Expression VisitListInit(System.Linq.Expressions.ListInitExpression init)
        {
            Debugger.Break();
            return base.VisitListInit(init);
        }

        protected override System.Linq.Expressions.Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression m)
        {
            IValueNode node = this.CurrentNode as IValueNode;

            if (node != null)
            {
                node.Value = new MemberNode(m.Member, this.CurrentNode);
            }

            return base.VisitMemberAccess(m);
        }

        protected override System.Linq.Expressions.MemberAssignment VisitMemberAssignment(System.Linq.Expressions.MemberAssignment assignment)
        {
            Debugger.Break();
            return base.VisitMemberAssignment(assignment);
        }

        protected override System.Linq.Expressions.Expression VisitMemberInit(System.Linq.Expressions.MemberInitExpression init)
        {
            Debugger.Break();
            return base.VisitMemberInit(init);
        }

        protected override System.Linq.Expressions.MemberListBinding VisitMemberListBinding(System.Linq.Expressions.MemberListBinding binding)
        {
            Debugger.Break();
            return base.VisitMemberListBinding(binding);
        }

        protected override System.Linq.Expressions.MemberMemberBinding VisitMemberMemberBinding(System.Linq.Expressions.MemberMemberBinding binding)
        {
            Debugger.Break();
            return base.VisitMemberMemberBinding(binding);
        }

        protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression m)
        {
            var expression = base.VisitMethodCall(m);

            return expression;
        }

        protected override System.Linq.Expressions.NewExpression VisitNew(System.Linq.Expressions.NewExpression nex)
        {
            Debugger.Break();
            return base.VisitNew(nex);
        }

        protected override System.Linq.Expressions.Expression VisitParameter(System.Linq.Expressions.ParameterExpression p)
        {
            return base.VisitParameter(p);
        }

        protected override System.Linq.Expressions.Expression VisitNewArray(System.Linq.Expressions.NewArrayExpression na)
        {
            Debugger.Break();
            return base.VisitNewArray(na);
        }

        protected override System.Linq.Expressions.Expression VisitTypeIs(System.Linq.Expressions.TypeBinaryExpression b)
        {
            Debugger.Break();
            return base.VisitTypeIs(b);
        }

        protected override System.Linq.Expressions.Expression VisitUnary(System.Linq.Expressions.UnaryExpression u)
        {
            return base.VisitUnary(u);
        }

        protected override void StartMethod(MethodInfo m)
        {
            MethodNode methodNode;

            if (this.BaseNode == null)
            {
                methodNode = new MethodNode(m);
                this.BaseNode = methodNode;
                this.CurrentNode = methodNode;
            }
            else
            {
                methodNode = new MethodNode(m, this.CurrentNode);
                this.CurrentNode = methodNode;
            }

            foreach (ParameterInfo parm in m.GetParameters())
            {
                methodNode.Parameters.Add(parm.Name, new ListItemNode(methodNode));
            }
        }

        protected override void StartList()
        {
        }

        protected override void EndList()
        {
            this.CurrentNode = this.CurrentNode.ParentNode;
        }

        protected override void AddListItem(int index)
        {
            MethodNode methodNode;

            if (index == 0)
            {
                methodNode = this.CurrentNode as MethodNode;
            }
            else
            {
                methodNode = this.CurrentNode.ParentNode as MethodNode;
            }

            if (methodNode != null)
            {
                this.CurrentNode = methodNode.Parameters.ElementAt(index).Value;
            }
        }

        protected override void EndMethod()
        {
            this.CurrentNode = this.CurrentNode.ParentNode;
        }

        protected override void StartBinary(ExpressionType type)
        {
            IValueNode node = this.CurrentNode as IValueNode;

            if (node != null)
            {
                this.CurrentNode = new BinaryExpressionNode(type, this.CurrentNode);
                node.Value = this.CurrentNode;
            }
        }

        protected override void AddBinaryLeft()
        {
            BinaryExpressionNode node = (BinaryExpressionNode)this.CurrentNode;

            node.Left = new ListItemNode(node);

            this.CurrentNode = node.Left;
        }

        protected override void AddBinaryRight()
        {
            BinaryExpressionNode node = (BinaryExpressionNode)this.CurrentNode.ParentNode;

            node.Right = new ListItemNode(node);

            this.CurrentNode = node.Right;
        }

        protected override void EndBinary()
        {
            this.CurrentNode = this.CurrentNode.ParentNode;
        }
    }
}
