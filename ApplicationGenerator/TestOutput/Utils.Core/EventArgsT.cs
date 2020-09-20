using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Utils
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; set; }

        [DebuggerStepThrough]
        public EventArgs(T value)
        {
            this.Value = value;
        }
    }
}
