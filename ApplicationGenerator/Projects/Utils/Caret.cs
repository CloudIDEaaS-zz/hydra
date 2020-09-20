using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Utils
{
    public class Caret : IDisposable
    {
        private Control control;

        public Caret(Control control, int width, int height)
        {
            this.control = control;
            ControlExtensions.CreateCaret(control.Handle, IntPtr.Zero, width, height);
        }

        public System.Drawing.Point Position
        {
            set
            {
                var position = value;

                ControlExtensions.SetCaretPos((int)position.X, (int)position.Y);
            }
        }

        public void Show()
        {
            ControlExtensions.ShowCaret(control.Handle);
        }

        public void Dispose()
        {
            ControlExtensions.DestroyCaret();
        }

        internal void Hide()
        {
            ControlExtensions.HideCaret(control.Handle);
        }
    }
}
