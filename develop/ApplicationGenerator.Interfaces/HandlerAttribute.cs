using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public abstract class HandlerAttribute : Attribute
    {
        public string[] Imports { get; protected set; }
        public Type[] PackageTypes { get; protected set; }
    }
}
