using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XPathParser
{
    public enum Use
    {
        Optional,
        Required
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class UseAttribute : Attribute
    {
        public Use Use { get; private set; }

        public UseAttribute(Use use)
        {
            this.Use = use;
        }
    }
}
