using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class BinaryOperator : Expression
    {
        private unsafe CppSharp.Parser.AST.BinaryOperator binaryOperator;

        public unsafe BinaryOperator(CppSharp.Parser.AST.BinaryOperator binaryOperator) : base(binaryOperator)
        {
            this.binaryOperator = binaryOperator;

            this.binaryOperator.AssertNotNullAndOfType<CppSharp.Parser.AST.BinaryOperator>();
        }

        public Expression LHS
        {
            get
            {
                return binaryOperator.LHS.GetRealExpressionInternal();
            }
        }

        public Expression RHS
        {
            get
            {
                return binaryOperator.RHS.GetRealExpressionInternal();
            }
        }

        public string OpCodeString
        {
            get
            {
                return binaryOperator.OpcodeStr;
            }
        }
    }
}