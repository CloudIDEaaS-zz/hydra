using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ActionQueueService : BaseThreadedService
    {
        private IManagedLockObject lockObject;
        private Queue<Action> actionQueue;

        public ActionQueueService()
        {
            lockObject = LockManager.CreateObject();
            actionQueue = new Queue<Action>();
        }

        private IDisposable Lock()
        {
            return lockObject.Lock();
        }

        private bool TryLock(out IDisposable disposable, int millisecondsTimeOut = 0)
        {
            return lockObject.TryLock(out disposable, millisecondsTimeOut);
        }

        public void Run(Action action)
        {
            using (this.Lock())
            {
                actionQueue.Enqueue(action);
            }
        }

        public int ActionCount
        {
            get
            {
                int count = 0;

                using (this.Lock())
                {
                    count = actionQueue.Count;
                }

                return count;
            }
        }

        public override void DoWork(bool stopping)
        {
            Action action = null;

            using (this.Lock())
            {
                if (actionQueue.Count > 0)
                {
                    action = actionQueue.Dequeue();
                }
            }

            if (action != null)
            {
                action();
            }
        }
    }
}
