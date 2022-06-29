// file:	ActionQueueService.cs
//
// summary:	Implements the action queue service class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>   A service for accessing action queues information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/1/2020. </remarks>

    public class ActionQueueService : BaseThreadedService, ILockable
    {
        /// <summary>   The lock object. </summary>
        private IManagedLockObject lockObject;
        /// <summary>   Queue of actions. </summary>
        private Queue<Action> actionQueue;
        private Queue<CanRunObject> canRunActionQueue;
        /// <summary>   The object to actions. </summary>
        private Dictionary<object, ActionStatus> objectToActions;

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/1/2020. </remarks>

        public ActionQueueService()
        {
            lockObject = LockManager.CreateObject();
            actionQueue = new Queue<Action>();
            canRunActionQueue = new Queue<CanRunObject>();
            objectToActions = new Dictionary<object, ActionStatus>();
        }

        /// <summary>   Runs. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/1/2020. </remarks>
        ///
        /// <param name="action">       The action. </param>
        /// <param name="objectKey">    (Optional) The object key. </param>

        public virtual void Run(Action action, object objectKey = null)
        {
            using (this.Lock())
            {
                if (objectKey != null)
                {
                    if (objectToActions.ContainsKey(objectKey))
                    {
                        var actionStatus = objectToActions[objectKey];

                        actionStatus.Cancelled = true;

                        return;
                    }

                    objectToActions.Add(objectKey, new ActionStatus(action));
                }

                actionQueue.Enqueue(action);
            }
        }

        public void Clear()
        {
            using (this.Lock())
            {
                actionQueue.Clear();
            }
        }

        public void Run(Action action, Func<bool> canRun)
        {
            using (this.Lock())
            {
                canRunActionQueue.Enqueue(new CanRunObject
                {
                    CanRun = canRun,
                    Run = action
                });
            }
        }

        /// <summary>   Gets the number of actions. </summary>
        ///
        /// <value> The number of actions. </value>

        public int ActionCount
        {
            get
            {
                int count = 0;

                using (this.Lock())
                {
                    count = actionQueue.Count;
                    count += canRunActionQueue.Count;
                }

                return count;
            }
        }

        /// <summary>   Executes the work operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/1/2020. </remarks>
        ///
        /// <param name="stopping"> True to stopping. </param>

        public override void DoWork(bool stopping)
        {
            Action action = null;
            CanRunObject canRunObject = null;

            using (this.Lock())
            {
                if (actionQueue.Count > 0)
                {
                    var cancelled = true;

                    while (cancelled)
                    {
                        action = actionQueue.Dequeue();

                        if (objectToActions.Any(a => a.Value.Action == action))
                        {
                            var actionStatus = objectToActions.Single(a => a.Value.Action == action);

                            cancelled = actionStatus.Value.Cancelled;

                            objectToActions.Remove(actionStatus.Key);
                        }
                        else
                        {
                            cancelled = false;
                        }

                        if (actionQueue.Count == 0)
                        {
                            break;
                        }
                    }
                }

                if (canRunActionQueue.Count > 0)
                {
                    canRunObject = canRunActionQueue.Peek();
                }
            }

            if (action != null)
            {
                action();
            }

            if (canRunObject != null)
            {
                if (canRunObject.CanRun())
                {
                    canRunActionQueue.Dequeue();

                    canRunObject.Run();
                }    
            }
        }

        private class CanRunObject
        {
            public Func<bool> CanRun { get; set; }
            public Action Run { get; set; }
        }
    }
}
