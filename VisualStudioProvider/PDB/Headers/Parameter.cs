using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Parameter : Declaration
    {
        private unsafe CppSharp.Parser.AST.Parameter parameter;
        public FunctionType OwningFunctionType { get; set; }
        public Function OwningFunction { get; set; }

        public unsafe Parameter(FunctionType owningFunctionType, CppSharp.Parser.AST.Parameter parameter) : base(parameter)
        {
            this.parameter = parameter;
            this.OwningFunctionType = owningFunctionType;

            this.OwningFunctionType.AssertNotNull();
            this.parameter.AssertNotNullAndOfType<CppSharp.Parser.AST.Parameter>();
        }

        public unsafe Parameter(Function owningFunction, CppSharp.Parser.AST.Parameter parameter) : base(parameter)
        {
            this.parameter = parameter;
            this.OwningFunction = owningFunction;

            this.OwningFunction.AssertNotNull();
            this.parameter.AssertNotNullAndOfType<CppSharp.Parser.AST.Parameter>();
        }

        public unsafe Parameter(CppSharp.Parser.AST.Parameter parameter) : base(parameter)
        {
            this.parameter = parameter;
            this.parameter.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return parameter.Location.ID;
            }
        }

        public string DebugText
        {
            get
            {
                return parameter.DebugText;
            }
        }

        public Comment Comment 
        {
            get
            {
                if (parameter.Comment == null)
                {
                    return null;
                }
                else
                {
                    return new Comment(this, parameter.Comment);
                }
            }
        }

        public string Access
        {
            get
            {
                return parameter.Access.ToString();
            }
        }

        public string DefaultArgument
        {
            get
            {
                if (parameter.DefaultArgument == null)
                {
                    return null;
                }
                else
                {
                    return parameter.DefaultArgument.ToString();
                }
            }
        }

        public bool IsIndirect
        {
            get
            {
                return parameter.IsIndirect;
            }
        }

        public QualifiedType QualifiedType
        {
            get
            {
                if (parameter.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(parameter.QualifiedType);
                }
            }
        }
    }
}
