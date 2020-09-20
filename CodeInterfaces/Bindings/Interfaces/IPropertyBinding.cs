using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IPropertyBinding
    {
        string PropertyBindingName { get; set; }
        IAttribute Property { get; set; }
        IBindingSource BindingSource { get; set; }
        BindingMode BindingMode { get; set; }
    }
}
