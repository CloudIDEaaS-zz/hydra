using ApplicationGenerator.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Utils;
using Expression = System.Linq.Expressions.Expression;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderQuery : ExpressionVisitor
    {
        public SyntaxTreeNode SyntaxTree { get; private set; }
        public AstNode PreviousNode { get; private set; }
        public AstNode CurrentNode { get; private set; }
        public Stack<AstNode> NodeStack { get; private set; }

        public void Parse(Expression expression)
        {
            this.SyntaxTree = new SyntaxTreeNode();
            this.CurrentNode = this.SyntaxTree;
            this.NodeStack = new Stack<AstNode>();

            this.Visit(expression);
        }

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }

        private void AddChild(AstNode astNode)
        {
            this.CurrentNode.AddChild(astNode);
            this.CurrentNode = astNode;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(Queryable))
            {
                var methodDeclaration = new InvocationExpressionNode(methodCallExpression.Method.Name);
                LambdaExpression lambda;

                AddChild(methodDeclaration);

                methodDeclaration.Target = (ExpressionNode) this.VisitNode(methodCallExpression.Arguments[0]);

                if (methodCallExpression.Arguments.Count > 1)
                {
                    lambda = (LambdaExpression)StripQuotes(methodCallExpression.Arguments[1]);

                    methodDeclaration.Arguments.Add((ExpressionNode)this.VisitNode(lambda.Body));
                }

                return methodCallExpression;
            }
            else
            {
                DebugUtils.Break();
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", methodCallExpression.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this.Visit(u.Operand);
                    break;

                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            var binaryOperatorExpression = new BinaryOperatorExpressionNode();

            switch (binaryExpression.NodeType)
            {
                case ExpressionType.And:

                    DebugUtils.Break();
                    break;

                case ExpressionType.Equal:

                    binaryOperatorExpression.Operator = Operator.Equality;
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", binaryExpression.NodeType));
            }

            AddChild(binaryOperatorExpression);

            binaryOperatorExpression.Left = (ExpressionNode) this.VisitNode(binaryExpression.Left);
            binaryOperatorExpression.Right = (ExpressionNode)this.VisitNode(binaryExpression.Right);

            return binaryExpression;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var memberExpression = new MemberExpressionNode(node.Member.Name);

            AddChild(memberExpression);

            memberExpression.Expression = (ExpressionNode) VisitNode(node.Expression);

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var queryable = node.Value as IQueryable;
            ConstantExpressionNode constantExpression;

            if (queryable != null)
            {
                constantExpression = new ConstantExpressionNode(ConstantKind.QuerySource, node.Type, node.Value);
            }
            else if (node.Value == null)
            {
                constantExpression = new ConstantExpressionNode(ConstantKind.Null);
            }
            else
            {
                var valueType = node.Value.GetType();
                var field = valueType.GetFields().Single();
                var value = field.GetValue(node.Value);

                constantExpression = new ConstantExpressionNode(ConstantKind.ClosureState, valueType, value);
            }

            AddChild(constantExpression);

            return node;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
        
        protected AstNode VisitNode(Expression node)
        {
            this.Visit(node);

            return this.PreviousNode;
        }

        public override Expression Visit(Expression node)
        {
            Expression expression;

            this.NodeStack.Push(this.CurrentNode);

            expression = base.Visit(node);

            this.PreviousNode = this.CurrentNode;
            this.CurrentNode = this.NodeStack.Pop();

            return expression;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return base.VisitLabelTarget(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var newExpression = new NewExpressionNode(node.Type, node.Constructor, node.Members.ToArray());

            AddChild(newExpression);

            foreach (var argument in node.Arguments)
            {
                newExpression.Arguments.Add((ExpressionNode) this.VisitNode(argument));
            }

            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var parameterExpression = new ParameterExpressionNode(node.Name, node.Type, node.IsByRef);

            AddChild(parameterExpression);

            return node;
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }
    }
}
