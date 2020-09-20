using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utils;

namespace VisualStudioProvider
{
    public class GlobalCommandTargetAction
    {
        public Action Action { get; private set; }
        public ManualResetEvent Event { get; private set; }
        public DateTime StartTime { get; private set; }
        public string CallStack { get; private set; }
        public int DelayCount { get; set; }
        public Func<bool> WhenCallback { get; set; }

        public GlobalCommandTargetAction(Action action, ManualResetEvent _event) : this(action)
        {
            this.Event = _event;
        }

        public GlobalCommandTargetAction(Action action)
        {
            this.Action = action;
            this.StartTime = DateTime.Now;
            this.CallStack = this.GetStackText(20, 5);
        }

        public GlobalCommandTargetAction(Action action, int delayCount)
        {
            this.Action = action;
            this.StartTime = DateTime.Now;
            this.DelayCount = delayCount;
            this.CallStack = this.GetStackText(20, 5);
        }

        public GlobalCommandTargetAction(Action action, Func<bool> whenCallback)
        {
            this.Action = action;
            this.StartTime = DateTime.Now;
            this.WhenCallback = whenCallback;
            this.CallStack = this.GetStackText(20, 5);
        }
    }
}
