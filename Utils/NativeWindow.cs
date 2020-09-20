using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utils
{
    public class NativeWindow : System.Windows.Forms.NativeWindow, IDisposable
    {
        private Func<Message, bool> msgProc;
        private Action<Message> msgPostProc;

        public NativeWindow(Func<Message, bool> msgProc)
        {
            this.msgProc = msgProc;
        }

        public NativeWindow(Func<Message, bool> msgProc, Action<Message> msgPostProc)
        {
            this.msgProc = msgProc;
            this.msgPostProc = msgPostProc;
        }

        public NativeWindow(IntPtr handle, Func<Message, bool> msgProc)
        {
            this.AssignHandle(handle);
            this.msgProc = msgProc;
        }

        public NativeWindow(IntPtr handle, Func<Message, bool> msgProc, Action<Message> msgPostProc)
        {
            this.AssignHandle(handle);
            this.msgProc = msgProc;
            this.msgPostProc = msgPostProc;
        }

        protected override void WndProc(ref Message m)
        {
            if (msgProc(m))
            {
                base.WndProc(ref m);
            }

            if (msgPostProc != null)
            {
                msgPostProc(m);
            }
        }

        public void Dispose()
        {
            this.ReleaseHandle();
        }
    }
}
