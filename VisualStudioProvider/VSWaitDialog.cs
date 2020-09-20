using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace VisualStudioProvider
{
    public class VSWaitDialog : IDisposable
    {
        private IVsThreadedWaitDialog2 waitDialog;

        protected VSWaitDialog(string waitCaption, string waitMessage, bool showMarqueeProgress = false) : this(waitCaption, waitMessage, null, null, showMarqueeProgress)
        {
        }

        protected VSWaitDialog(string waitCaption, string waitMessage, string progressText, string statusBarText, bool showMarqueeProgress = false)
        {
            var dialogFactory = Package.GetGlobalService(typeof(SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;

            if (dialogFactory != null)
            {
                var hr = dialogFactory.CreateInstance(out waitDialog);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
                else
                {
                    hr = waitDialog.StartWaitDialog(waitCaption, waitMessage, progressText, null, statusBarText, 0, false, showMarqueeProgress);

                    if (ErrorHandler.Failed(hr))
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(4000);
                    }
                }
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            var message = string.Format(format, args);
            bool cancel;

            waitDialog.UpdateProgress(message, null, null, 0, 0, false, out cancel);
        }

        public void WriteProgress(float progress, string format, params object[] args)
        {
            var message = string.Format(format, args);
            bool cancel;

            waitDialog.UpdateProgress(message, null, null, (int)(progress * 100), 100, false, out cancel);
        }

        public void WriteProgress(int step, int totalSteps, string format, params object[] args)
        {
            var message = string.Format(format, args);
            bool cancel;

            waitDialog.UpdateProgress(message, null, null, step, totalSteps, false, out cancel);
        }

        public static VSWaitDialog Create(string waitCaption, string waitMessage, bool showMarqueeProgress = false)
        {
            var dialog = new VSWaitDialog(waitCaption, waitMessage, showMarqueeProgress);

            return dialog;
        }

        public static VSWaitDialog Create(string waitCaption, string waitMessage, string progressText, string statusBarText, bool showMarqueeProgress = false)
        {
            var dialog = new VSWaitDialog(waitCaption, waitMessage, progressText, statusBarText, showMarqueeProgress);

            return dialog;
        }

        public void Dispose()
        {
            int cancelled;

            waitDialog.EndWaitDialog(out cancelled);
        }
    }
}
