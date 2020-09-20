using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ImportHandlerAttribute : Attribute
    {
        public ulong HandlerId { get; }

        public ImportHandlerAttribute(ulong id)
        {
            this.HandlerId = id;
        }
    }
}
