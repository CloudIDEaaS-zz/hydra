using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IQueryBindingSource : IBindingSource
    {
        IQuery Query { get; set; }
    }
}
