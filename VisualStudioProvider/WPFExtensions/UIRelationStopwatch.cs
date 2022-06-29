using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SLControlLibrary
{
    internal class UIRelationStopwatch : Utils.Stopwatch
    {
        public StackFrame StackFrame { get; private set; }
        public bool IsFromCache { get; set; }
        public int Hash { get; set; }
        public bool Reentrant { get; private set; }

        internal UIRelationStopwatch(object uiObject)
        {
            var stackTrace = new StackTrace();

            this.StackFrame = stackTrace.GetFrame(1);
            this.Hash = uiObject.GetHashCode();

            var diagnostic = StackFrame.GetCreateDiagnostic(this.Hash);

            if (diagnostic.CurrentStopWatch != null)
            {
                Reentrant = true;
            }

            startTime = DateTime.Now;
            diagnostic.CurrentStopWatch = this;
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (!Reentrant)
            {
                var diagnostic = StackFrame.GetCreateDiagnostic(this.Hash);
                var timingEntry = new UIRelationTimingEntry(this.Milliseconds);

                if (this.IsFromCache)
                {
                    diagnostic.CacheTimings.Enqueue(timingEntry);
                }
                else
                {
                    diagnostic.RawTimings.Enqueue(timingEntry);
                }
            }
        }
    }
}
