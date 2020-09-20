using System;
using System.Net;
using System.Windows;
using System.Windows.Threading;
using System.Timers;
using System.Threading;

namespace Utils 
{
    public class OneTimeThreadTimer
    {
        private Thread internalThread;
        private bool elapsed;
        private double interval;
        private DateTime startTime;
        private event EventHandler Elapsed;

        public OneTimeThreadTimer(TimeSpan timeSpan)
        {
            internalThread = new Thread(ThreadProc);
            interval = timeSpan.TotalMilliseconds;
        }

        public OneTimeThreadTimer(int milliseconds)
        {
            internalThread = new Thread(ThreadProc);
            interval = milliseconds;
        }

        public void Continue()
        {
            elapsed = false;
            internalThread.Start();
        }

        public void Stop()
        {
            internalThread.Abort();
        }

        public void ThreadProc()
        {
            while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(interval))
            {
                Thread.Sleep(1);
            }

            Elapsed(this, EventArgs.Empty);
        }

        public void Start(Action action)
        {
            elapsed = false;
            startTime = DateTime.Now;

            Elapsed += (sender, e) =>
            {
                if (!elapsed)
                {
                    action();
                    elapsed = true;
                }
            };

            internalThread.Start();
        }
    }
}
