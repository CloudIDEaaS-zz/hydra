// file:	IDesktopForm.cs
//
// summary:	Declares the IDesktopForm interface

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbstraX
{
    /// <summary>   Interface for desktop form. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 7/3/2021. </remarks>

    public interface IDesktopForm
    {
        /// <summary>   Gets the form. </summary>
        ///
        /// <value> The form. </value>

        Form Form { get; }

        /// <summary>   Gets the progress bar. </summary>
        ///
        /// <value> The progress bar. </value>

        ToolStripProgressBar ProgressBar { get; }

        /// <summary>   Gets the primary status label. </summary>
        ///
        /// <value> The primary status label. </value>

        StatusStrip StatusStrip { get; }

        /// <summary>   Gets the progress status label. </summary>
        ///
        /// <value> The progress status label. </value>

        ToolStripStatusLabel ProgressStatusLabel { get; }

        /// <summary>   Gets or sets the text output. </summary>
        ///
        /// <value> The text output. </value>

        RichTextBox TextOutput { get; set; }

        /// <summary>   Gets or sets a value indicating whether the submit button is enabled. </summary>
        ///
        /// <value> True if submit button enabled, false if not. </value>

        bool UISubmitButtonEnabled { get; set; }

        /// <summary>
        /// Executes the on user interface thread on a different thread, and waits for the result.
        /// </summary>
        ///
        /// <param name="action">           The action. </param>
        /// <param name="throwException">   (Optional) True to throw exception. </param>

        void InvokeOnUIThread(Action action, bool throwException = false);
    }
}
