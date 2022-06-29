using ApplicationGenerator.Data;
using System.IO;
using Utils;

namespace ApplicationGenerator.AST
{
    public class AbstraXProviderIdWriter : IAstVisitor
    {
        private TextWriter textWriter;
        private NamingConvention namingConvention;

        public AbstraXProviderIdWriter(TextWriter writer, NamingConvention namingConvention)
        {
            this.textWriter = writer;
            this.namingConvention = namingConvention;
        }

        public void BinaryOperatorExpression(BinaryOperatorExpressionNode binaryOperatorExpressionNode)
        {
            binaryOperatorExpressionNode.Left.AcceptVisitor(this);

            switch (binaryOperatorExpressionNode.Operator)
            {
                case Operator.Equality:
                    textWriter.Write("=");
                    break;
                default:
                    DebugUtils.Break();
                    break;
            }

            binaryOperatorExpressionNode.Right.AcceptVisitor(this);
        }

        public void VisitInvocationExpression(InvocationExpressionNode invocationExpression)
        {
            foreach (var argument in invocationExpression.Arguments)
            {
                argument.AcceptVisitor(this);
            }
        }

        public void VisitMemberExpression(MemberExpressionNode memberExpressionNode)
        {
            var expression = memberExpressionNode.Expression;
            string member;

            if (expression is ParameterExpressionNode)
            {
                switch (this.namingConvention)
                {
                    case NamingConvention.CamelCase:

                        member = memberExpressionNode.Name.ToCamelCase();
                        textWriter.Write(member);

                        break;
                }
            }
            else if (expression is MemberExpressionNode)
            {
                expression.AcceptVisitor(this);
            }
            else if (expression is ConstantExpressionNode)
            {
                if (memberExpressionNode.ParentNode is MemberExpressionNode)
                {
                    var propertyName = ((MemberExpressionNode)memberExpressionNode.ParentNode).Name;
                    var constantValue = ((ConstantExpressionNode)expression).Value;

                    // this doesn't feel right, but works

                    member = constantValue.GetPropertyValue<string>(propertyName);

                    textWriter.Write(member);
                }
                else
                { 
                    member = (string) ((ConstantExpressionNode)expression).Value;
                    textWriter.Write(member);
                }
            }
            else
            {
                DebugUtils.Break();
            }
        }

        public void VisitNewExpression(NewExpressionNode newExpressionNode)
        {
            DebugUtils.Break();
        }
    }
}
