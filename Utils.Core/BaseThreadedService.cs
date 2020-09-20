using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utils
{
    public abstract class BaseThreadedService
    {
        private bool startCalled;
        private Thread workerThread;
        private bool shutdownFlag;
        private ThreadPriority threadPriority;
        private TimeSpan iterationSleep;
        private DateTime firstCheck;
        private DateTime lastProcessing;
        private ManualResetEvent runningEvent;
        private TimeSpan iterationTimeOut;
        private TimeSpan startTimeOut;
        protected IManagedLockObject lockObject;
        public event EventHandler Started;
        public abstract void DoWork(bool stopping);
        private bool isRunning;
        private bool isPaused;

        public bool IsRunning
        {
            get
            {
                return this.LockGet(() => isRunning);
            }
        }

        public bool IsPaused
        {
            get
            {
                return this.LockGet(() => isPaused);
            }
        }

        public TimeSpan IterationSleep
        {
            get
            {
                return this.LockGet(() => this.iterationSleep);
            }
        }

        public BaseThreadedService(ThreadPriority threadPriority, TimeSpan iterationSleep, TimeSpan iterationTimeOut, TimeSpan startTimeOut, IManagedLockObject lockObject = null)
        {
            this.threadPriority = threadPriority;
            this.iterationSleep = iterationSleep;
            this.iterationTimeOut = iterationTimeOut;
            this.startTimeOut = startTimeOut;
            runningEvent = new ManualResetEvent(false);

            if (lockObject != null)
            {
                this.lockObject = lockObject;
            }
            else
            {
                this.lockObject = LockManager.CreateObject();
            }
        }
        
        public BaseThreadedService(ThreadPriority threadPriority, int iterationSleepMilliseconds, int iterationTimeOutMilliseconds, int startTimeOutMilliseconds, IManagedLockObject lockObject = null) : this(threadPriority, TimeSpan.FromMilliseconds(iterationSleepMilliseconds), TimeSpan.FromMilliseconds(iterationTimeOutMilliseconds), TimeSpan.FromMilliseconds(startTimeOutMilliseconds), lockObject)
        {
        }

        public BaseThreadedService(ThreadPriority threadPriority, int iterationSleepMilliseconds, IManagedLockObject lockObject = null) : this(threadPriority, TimeSpan.FromMilliseconds(iterationSleepMilliseconds), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), lockObject)
        {
        }

        public BaseThreadedService(ThreadPriority threadPriority, IManagedLockObject lockObject = null) : this(threadPriority, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15), lockObject)
        {
        }

        public BaseThreadedService(IManagedLockObject lockObject = null) : this(ThreadPriority.Lowest, lockObject)
        {
        }

        public void Pause()
        {
            this.LockSet(() => isPaused = true);
        }

        protected IDisposable Lock()
        {
            return lockObject.Lock();
        }

        protected T LockGet<T>(Func<T> getter)
        {
            T value;

            using (lockObject.Lock())
            {
                value = getter();
            }

            return value;
        }

        protected void LockSet(Action setter)
        {
            using (lockObject.Lock())
            {
                setter();
            }
        }

        private bool TryLock(out IDisposable disposable, int millisecondsTimeOut = 0)
        {
            return lockObject.TryLock(out disposable, millisecondsTimeOut);
        }

        public virtual void Start()
        {
            if (this.IsPaused)
            {
                using (this.Lock())
                {
                    isPaused = false;
                }
            }
            else
            {
                workerThread = new Thread(WorkerThreadProc);

                startCalled = true;
                workerThread.Priority = threadPriority;
                workerThread.Name = this.GetType().Name;

                workerThread.Start();
            }
        }

        protected virtual void OnStarted()
        {
        }

        public void WorkerThreadProc()
        {
            var running = true;
            var paused = false;

            isRunning = running;

            Started?.Invoke(this, EventArgs.Empty);
            OnStarted();

            while (true)
            {
                try
                {
                    var stopping = false;

                    using (this.Lock())
                    {
                        if (shutdownFlag)
                        {
                            stopping = true;
                            shutdownFlag = false;
                        }

                        running = isRunning;
                        paused = isPaused;

                        lastProcessing = DateTime.Now;
                    }

                    if (!paused)
                    {
                        try
                        {
                            DoWork(stopping);
                        }
                        catch
                        {
                        }
                    }

                    using (this.Lock())
                    {
                        if (stopping)
                        {
                            runningEvent.Set();
                            return;
                        }
                    }

                    Thread.Sleep(iterationSleep);
                }
                catch
                {
                    break;
                }
            }
        }

        public bool CheckThread(StringBuilder results, bool abortOnTimeout)
        {
            bool processing = false;
            IDisposable _lock;
            var message = string.Format(@"{0}: Checking thread state", this.GetType().Name);

            results.AppendLine(message);

            if (this.TryLock(out _lock, (int)TimeSpan.FromSeconds(iterationTimeOut.TotalSeconds).TotalMilliseconds))
            {
                if (lastProcessing == DateTime.MinValue)
                {
                    if (firstCheck == DateTime.MinValue)
                    {
                        results.AppendLine("First time check of thread. Not yet started.");

                        firstCheck = DateTime.Now;
                        processing = true;
                    }
                    else if (DateTime.Now - firstCheck > startTimeOut)
                    {
                        var error = string.Format(@"{0}: Thread appears to have not started within {1} seconds of initiation", this.GetType().Name, startTimeOut.TotalSeconds);

                        results.AppendLine(error);

                        if (abortOnTimeout)
                        {
                            workerThread.Interrupt();
                        }
                    }
                }
                else
                {
                    if (DateTime.Now - lastProcessing > iterationTimeOut)
                    {
                        var error = string.Format(@"{0}: Lost communication with thread.  Has not processed in {1} minutes", this.GetType().Name, iterationTimeOut.TotalSeconds);

                        results.AppendLine(error);

                        if (abortOnTimeout)
                        {
                            workerThread.Interrupt();
                        }
                    }
                    else
                    {
                        results.AppendLine("Thread running");
                        processing = true;
                    }
                }

                _lock.Dispose();
            }
            else
            {
                var error = string.Format(@"{0}: Lost communication with thread.  Thread locked out.", this.GetType().Name);

                results.AppendLine(error);

                if (abortOnTimeout)
                {
                    workerThread.Interrupt();
                }
            }

            if (results.ToString().EndsWith("\r\n"))
            {
                results.Remove(results.Length - 2, 2);
            }

            return processing;
        }

        public void Abort()
        {
            workerThread.Abort();

            using (this.Lock())
            {
                isRunning = false;
            }
        }

        public bool IsShuttingDown
        {
            get
            {
                bool shuttingDown;

                using (this.Lock())
                {
                    shuttingDown = shutdownFlag;
                }

                return shuttingDown;
            }
        }

        public virtual void ForceStop()
        {
            try
            {
                this.workerThread.Abort();
            }
            catch
            {
            }
        }

        public virtual void Wait(int millisecondsTimeout = Timeout.Infinite)
        {
            if (!runningEvent.WaitOne(millisecondsTimeout))
            {
                throw new Exception(string.Format(@"{0}: Service wait timeout", this.GetType().Name));
            }
        }

        public virtual void Stop()
        {
            var startTick = DateTime.Now.Ticks;
            var running = false;
            var name = this.GetType().Name;

            Debug.Assert(startCalled, $"Service { name } was never started.");
            Debug.Assert(isRunning, $"Service { name } was never running.");

            if (workerThread == null)
            {
                return;
            }

            using (this.Lock()) 
            {
                shutdownFlag = true;
                running = isRunning;
            }

            if (running)
            {
                using (this.Lock())
                {
                    isRunning = false;
                }

                if (!runningEvent.WaitOne(TimeSpan.FromSeconds(60)))
                {
                    throw new Exception(string.Format(@"{0}: Unable to stop service", this.GetType().Name));
                }
            }
        }
    }
}
