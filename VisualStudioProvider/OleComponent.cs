using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using System.Threading;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Utils;

namespace VisualStudioProvider
{
    public abstract class OleComponent : IOleComponent
    {
        private uint componentId;
        private IOleComponentManager oleComponentManager;
        private IManagedLockObject lockObject;
        private Queue<GlobalCommandTargetAction> queuedActions;
        private List<GlobalCommandTargetAction> whenActions;
        private Thread uiThread;

        internal OleComponent()
        {
            lockObject = LockManager.CreateObject();
        }

        protected void Register()
        {
            Debug.Assert(!this.IsComponent);

            uiThread = Thread.CurrentThread;

            using (lockObject.Lock())
            {
                var crinfo = new OLECRINFO[1];

                oleComponentManager = (IOleComponentManager)Package.GetGlobalService(typeof(SOleComponentManager));

                queuedActions = new Queue<GlobalCommandTargetAction>();
                whenActions = new List<GlobalCommandTargetAction>();

                crinfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                crinfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime | (uint)_OLECRF.olecrfNeedPeriodicIdleTime;
                crinfo[0].grfcadvf = (uint)_OLECADVF.olecadvfModal | (uint)_OLECADVF.olecadvfRedrawOff | (uint)_OLECADVF.olecadvfWarningsOff;
                crinfo[0].uIdleTimeInterval = 100;

                var hr = oleComponentManager.FRegisterComponent(this, crinfo, out componentId);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        protected IDisposable Lock()
        {
            return lockObject.Lock();
        }

        protected bool IsComponent
        {
            get
            {
                bool isComponent;

                using (lockObject.Lock())
                {
                    if (oleComponentManager == null && componentId == 0)
                    {
                        isComponent = false;
                    }
                    else
                    {
                        isComponent = true;
                    }
                }

                return isComponent;
            }
        }    

        public void ClearRuns()
        {
            using (lockObject.Lock())
            {
                queuedActions.Clear();
            }
        }

        public void Run(Action action)
        {
            Debug.Assert(this.IsComponent);

            try
            {
                if (uiThread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                {
                    action();
                }
                else
                {
                    var resetEvent = new ManualResetEvent(false);

                    using (lockObject.Lock())
                    {
                        queuedActions.Enqueue(new GlobalCommandTargetAction(action, resetEvent));
                    }

                    resetEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void RunAsync(Action action)
        {
            Debug.Assert(this.IsComponent);

            try
            {
                using (lockObject.Lock())
                {
                    queuedActions.Enqueue(new GlobalCommandTargetAction(action));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void RunAsync(Action action, int delayCount)
        {
            Debug.Assert(this.IsComponent);

            try
            {
                using (lockObject.Lock())
                {
                    queuedActions.Enqueue(new GlobalCommandTargetAction(action, delayCount));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void RunAsync(Action action, Func<bool> whenCallback)
        {
            Debug.Assert(this.IsComponent);

            try
            {
                using (lockObject.Lock())
                {
                    whenActions.Add(new GlobalCommandTargetAction(action, whenCallback));
                }
            }
            catch (Exception ex)
            {
            }
        }

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked)
        {
            return 1;
        }

        public int FDoIdle(uint grfidlef)
        {
            GlobalCommandTargetAction queuedAction = null;
            List<GlobalCommandTargetAction> allQueuedActions = null;
            var idleFlags = (_OLEIDLEF)grfidlef;

            using (lockObject.Lock())
            {
                if (Thread.CurrentThread.ManagedThreadId != uiThread.ManagedThreadId)
                {
                    return VSConstants.S_OK;
                }

                if (queuedActions.Count > 0)
                {
                    if (idleFlags == _OLEIDLEF.oleidlefAll)
                    {
                        allQueuedActions = queuedActions.ToList();

                        while (queuedActions.Count > 0)
                        {
                            queuedActions.Dequeue();
                        }
                    }
                    else
                    {
                        queuedAction = queuedActions.Peek();

                        if (queuedAction.DelayCount > 0)
                        {
                            queuedAction.DelayCount--;
                            queuedAction = null;
                        }
                        else
                        {
                            queuedAction = queuedActions.Dequeue();
                        }
                    }
                }

                if (whenActions.Count > 0)
                {
                    foreach (var whenAction in whenActions.ToList())
                    {
                        if (whenAction.WhenCallback())
                        {
                            whenActions.Remove(whenAction);
                        }
                    }
                }
            }

            if (allQueuedActions != null)
            {
                foreach (var action in allQueuedActions)
                {
                    action.Action();

                    if (action.Event != null)
                    {
                        action.Event.Set();
                    }
                }
            }
            else if (queuedAction != null)
            {
                queuedAction.Action();

                if (queuedAction.Event != null)
                {
                    queuedAction.Event.Set();
                }
            }

            return VSConstants.S_OK;
        }

        public int FPreTranslateMessage(MSG[] pMsg)
        {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser)
        {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved)
        {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved)
        {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID)
        {
            if (fActive == 0)
            {
                int count = 0;

                using (lockObject.Lock())
                {
                    count = queuedActions.Count;

                    if (Thread.CurrentThread.ManagedThreadId != uiThread.ManagedThreadId)
                    {
                        return;
                    }
                }

                while (count > 0)
                {
                    GlobalCommandTargetAction queuedAction = null;

                    using (lockObject.Lock())
                    {
                        queuedAction = queuedActions.Peek();

                        if (queuedAction.DelayCount == 0)
                        {
                            queuedAction = queuedActions.Dequeue();
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (queuedAction != null)
                    {
                        queuedAction.Action();
                    }

                    using (lockObject.Lock())
                    {
                        count = queuedActions.Count;
                    }
                }
            }
        }

        public void OnEnterState(uint uStateID, int fEnter)
        {
            var oleState = (_OLECSTATE)uStateID;
        }

        public void OnLoseActivation()
        {
        }

        public void Terminate()
        {
        }

        internal void Unregister()
        {
            if (this.IsComponent)
            {
                using (lockObject.Lock())
                {
                    var hr = oleComponentManager.FRevokeComponent(componentId);

                    if (ErrorHandler.Failed(hr))
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                }
            }
        }
    }
}
