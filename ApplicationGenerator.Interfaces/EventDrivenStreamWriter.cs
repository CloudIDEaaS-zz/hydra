// file:	ChainedStreamWriter.cs
//
// summary:	Implements the chained stream writer class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Raises the write line handler event. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Event information to send to registered event handlers. </param>

    public delegate void OnWriteLineHandler(object sender, OnWriteEventArgs e);

    /// <summary>   Additional information for on write events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>

    public class OnWriteEventArgs : EventArgs
    {
        /// <summary>   Gets the output. </summary>
        ///
        /// <value> The output. </value>

        public string Output { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>
        ///
        /// <param name="output">   The output. </param>

        public OnWriteEventArgs(string output)
        {
            this.Output = output;
        }
    }

    /// <summary>   An event stream writer. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>

    public class EventDrivenStreamWriter : TextWriter
    {
        /// <summary>   Event queue for all listeners interested in OnWriteLine events. </summary>
        public event OnWriteLineHandler OnWriteLine;

        /// <summary>
        /// When overridden in a derived class, returns the character encoding in which the output is
        /// written.
        /// </summary>
        ///
        /// <value> The character encoding in which the output is written. </value>

        public override Encoding Encoding => Encoding.Default;

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>
        ///
        /// <param name="value">    The string to write. If <paramref name="value" /> is
        ///                         <see langword="null" />, only the line terminator is written. </param>

        public override void WriteLine(string value)
        {
            Thread.Sleep(1);

            OnWriteLine(this, new OnWriteEventArgs(value));
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as
        /// <see cref="M:System.String.Format(System.String,System.Object)" />.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/30/2022. </remarks>
        ///
        /// <param name="format">   A composite format string (see Remarks). </param>
        /// <param name="arg">      An object array that contains zero or more objects to format and
        ///                         write. </param>

        public override void WriteLine(string format, params object[] arg)
        {
            Thread.Sleep(1);

            OnWriteLine(this, new OnWriteEventArgs(string.Format(format, arg)));
        }
    }
}
