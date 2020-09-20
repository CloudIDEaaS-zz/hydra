using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class ThreadDisposable : IDisposable
    {
        private IntPtr threadHandle;
        private uint threadId;

        public ThreadDisposable(IntPtr threadHandle, uint threadId)
        {
            this.threadHandle = threadHandle;
            this.threadId = threadId;
        }

        public void Dispose()
        {
            ThreadExtensions.CloseHandle(threadHandle);
        }
    }
}
