using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ViewEngine
{
    public delegate void PartialViewEventHandler(object target, PartialViewEventArgs e);

    public class PartialViewEventArgs : EventArgs
    {
        public string Method { get; }
        public SeparatedSyntaxList<ArgumentSyntax> Arguments { get;  }
        public View View { get; set; }

        public PartialViewEventArgs(string method, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            this.Method = method;
            this.Arguments = arguments;
        }
    }
}
