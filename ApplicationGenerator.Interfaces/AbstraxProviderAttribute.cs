using AbstraX.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public class AbstraXProviderAttribute : Attribute
    {
        public Guid Guid { get; }

        public AbstraXProviderAttribute(string guidValue)
        {
            this.Guid = Guid.Parse(guidValue);
        }
    }
}
    