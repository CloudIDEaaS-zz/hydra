using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IBindingSource
    {
        IAttribute BindingAttribute { get; set; }
        bool IsSearchable { get; set; }
    }
}
