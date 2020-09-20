using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ViewEngine.SemanticTree
{
    public static class Extensions
    {
        public static MethodDeclarationSyntax GetRenderMethod(this CompilationUnitSyntax root)
        {
            var _class = (ClassDeclarationSyntax)root.Members.OfType<ClassDeclarationSyntax>().Single();

            return _class.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Render");
        }
    }
}
