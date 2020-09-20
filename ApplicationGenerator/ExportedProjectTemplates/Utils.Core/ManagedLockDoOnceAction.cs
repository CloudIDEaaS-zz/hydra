using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public class ManagedLockDoOnceAction
    {
        public IDisposable Lock { get; set; }
        public Delegate Action { get; set; }
        public DateTime TimeStarted { get; set; }
        public TimeSpan TimeOut { get; set; }
    }
}
