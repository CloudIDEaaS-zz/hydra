using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Function : Declaration
    {
        private unsafe CppSharp.Parser.AST.Function function;

        public unsafe Function(Declaration owningDeclaration, CppSharp.Parser.AST.Function function) : base(owningDeclaration, function)
        {
            this.function = function;
            this.function.AssertNotNull();
        }

        public unsafe Function(CppSharp.Parser.AST.Function function) : base(function)
        {
            this.function = function;
            this.function.AssertNotNull();
        }

        public string FunctionName
        {
            get
            {
                return function.Name;
            }
        }

        public IEnumerable<Parameter> Parameters
        {
            get
            {
                foreach (var parameter in function.GetParameters())
                {
                    yield return new Parameter(this, parameter);
                }
            }
        }

        public string Access
        {
            get
            {
                return function.Access.ToString();
            }
        }

        public string CallingConvention
        {
            get
            {
                return function.CallingConvention.ToString();
            }
        }

        public Comment Comment
        {
            get
            {
                if (function.Comment == null)
                {
                    return null;
                }
                else
                {
                    return new Comment(this, function.Comment);
                }
            }
        }

        public bool HasThisReturn
        {
            get
            {
                return function.HasThisReturn;
            }
        }

        public Function InstantiatedFrom
        {
            get
            {
                if (function.InstantiatedFrom == null)
                {
                    return null;
                }
                else if (function.InstantiatedFrom is CppSharp.Parser.AST.Method)
                {
                    return new Method((CppSharp.Parser.AST.Method)function);
                }
                else
                {
                    return new Function(function.InstantiatedFrom);
                }
            }
        }

        public bool IsDeleted
        {
            get
            {
                return function.IsDeleted;
            }
        }

        public bool IsInline
        {
            get
            {
                return function.IsInline;
            }
        }

        public bool IsPure
        {
            get
            {
                return function.IsPure;
            }
        }

        public bool IsReturnIndirect
        {
            get
            {
                return function.IsReturnIndirect;
            }
        }

        public bool IsVariadic
        {
            get
            {
                return function.IsVariadic;
            }
        }

        public string OperatorKind
        {
            get
            {
                return function.OperatorKind.ToString();
            }
        }

        public string Mangled
        {
            get
            {
                return function.Mangled;
            }
        }

        public QualifiedType ReturnType
        {
            get
            {
                if (function.ReturnType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(function.ReturnType);
                }
            }
        }

        public string Signature
        {
            get
            {
                return function.Signature;
            }
        }

        public string DebugText
        {
            get
            {
                return function.DebugText;
            }
        }
    }
}
