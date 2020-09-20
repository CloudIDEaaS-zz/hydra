using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows.Threading;
#endif

#if SILVERLIGHT
namespace AbstraX.ClientInterfaces
#else
namespace AbstraX.Contracts
#endif
{
    public class EnumInvokeOperation<T>
    {
        private T enumValue;
#if SILVERLIGHT
		private DispatcherTimer timer;
#endif
        public event EventHandler OnEnumCompleted;
        public event ExceptionHandler ExceptionOccured;
        public bool IsComplete;
        public Exception Exception { get; set; }

        public EnumInvokeOperation(T enumValue)
        {
            DelaySetCompleted(enumValue);
        }

        public EnumInvokeOperation()
        {
            this.IsComplete = false;
        }

        public EnumInvokeOperation(Exception ex)
        {
            this.Exception = ex;
            this.IsComplete = true;
        }

        public T Value
        {
            get
            {
                return enumValue;
            }
        }

        public bool HasException
        {
            get
            {
                return this.Exception != null;
            }
        }

        public void SetException(Exception ex)
        {
            this.Exception = ex;
            ExceptionOccured(this, new ExceptionEventArgs(ex));
        }

        public void DelaySetCompleted(T enumValue)
        {
#if SILVERLIGHT

            timer = new DispatcherTimer();

            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                SetCompleted(enumValue);
            };

            timer.Interval = new TimeSpan(1);
            timer.Start(); 
#endif
        }

        public void SetCompleted(T enumValue)
        {
            this.enumValue = enumValue;

            this.IsComplete = true;
            OnEnumCompleted(this, EventArgs.Empty);
        }
    }

    [Flags]
    public enum CommittedState
    {
        NotSet,
        Committed = 1,
        Complete = 2,
        HasDocumentation = 4,
    }
}
