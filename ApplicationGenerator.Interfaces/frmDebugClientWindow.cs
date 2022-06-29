// file:	frmClientWindow.cs
//
// summary:	Implements the form client Windows Form

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using static Utils.ControlExtensions;

namespace AbstraX
{
    /// <summary>   Form for viewing the form client. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/17/2021. </remarks>

    public partial class frmDebugClientWindow : Form
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/17/2021. </remarks>

        public ManualResetEvent ManualResetAttached { get; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/23/2021. </remarks>

        public frmDebugClientWindow()
        {
            ManualResetAttached = new ManualResetEvent(false);

            InitializeComponent();
        }

        /// <summary>   Sets the control to the specified visible state. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/17/2021. </remarks>
        ///
        /// <param name="value">    true to make the control visible; otherwise, false. </param>

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        /// <summary>   Processes Windows messages. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/17/2021. </remarks>
        ///
        /// <param name="m">    [in,out] The Windows <see cref="T:System.Windows.Forms.Message" /> to
        ///                     process. </param>

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)WindowsMessage.COPYDATA)
            {
                IntPtr messagePtr;
                ControlExtensions.COPYDATASTRUCT copyDataStruct;
                string json;
                CommandPacket commandPacket;

                copyDataStruct = Marshal.PtrToStructure<ControlExtensions.COPYDATASTRUCT>(m.LParam);
                messagePtr = copyDataStruct.lpData;

                json = Marshal.PtrToStringAnsi(messagePtr);
                commandPacket = JsonExtensions.ReadJson<CommandPacket>(json);

                switch (commandPacket.Command)
                {
                    case "ProcessAttached":
                        this.ManualResetAttached.Set();
                        break;
                }

                m.Result = (IntPtr)1;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
