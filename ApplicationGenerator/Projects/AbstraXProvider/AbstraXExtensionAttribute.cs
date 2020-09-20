using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX
{
    public class AbstraXExtensionAttribute : Attribute 
    {
        public String InterfaceName { get; set; }
        public String ImplementationName { get; set; }

        public AbstraXExtensionAttribute(String InterfaceName, String ImplementationName)
        {
            this.InterfaceName = InterfaceName;
            this.ImplementationName = ImplementationName;
        }
    }
}
