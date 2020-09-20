using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public class AbstraxProviderAttribute : Attribute
    {
        public Guid Guid { get; }

        public AbstraxProviderAttribute(string guidValue)
        {
            this.Guid = Guid.Parse(guidValue);
        }
    }
}
    