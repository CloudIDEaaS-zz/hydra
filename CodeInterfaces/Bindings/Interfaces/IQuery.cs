using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IQuery
    {
        string Name { get; set; }
        ISyntaxTree SyntaxTree { get; set; }
    }
}
