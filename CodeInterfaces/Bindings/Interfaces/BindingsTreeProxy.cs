using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CodeInterfaces.Bindings.Interfaces
{
    public class BindingsTreeProxy
    {
        private IBindingsTree bindingsTree;

        public BindingsTreeProxy(IBindingsTree bindingsTree)
        {
            this.bindingsTree = bindingsTree;
        }
    }
}
