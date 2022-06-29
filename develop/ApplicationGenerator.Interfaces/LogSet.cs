using AbstraX.ServerInterfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A log set. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>

    public class LogSet
    {
        private string currentName;

        /// <summary>   Gets or sets the current stream writer. </summary>
        ///
        /// <value> The current stream writer. </value>

        public StreamWriter CurrentStreamWriter { get; set; }


        /// <summary>   Gets or sets the current HTML node. </summary>
        ///
        /// <value> The current HTML node. </value>

        public HtmlNode CurrentHtmlContentNode { get; set; }

        /// <summary>   Gets or sets the current base object. </summary>
        ///
        /// <value> The current base object. </value>

        public IBase CurrentBaseObject { get; set; }

        /// <summary>   Gets or sets the current name. </summary>
        ///
        /// <value> The name of the current. </value>

        public string CurrentName 
        {
            get
            {
                return currentName;
            }

            set
            {
                HtmlNode paraElement;

                currentName = value;

                if (this.CurrentHtmlContentNode != null)
                {
                    paraElement = this.CurrentHtmlContentNode.SelectSingleNode($"div/p[@name='{ currentName }']");

                    if (paraElement != null)
                    {
                        paraElement.RemoveAllChildren();
                    }
                }
            }
        }

        /// <summary>   Creates a writer. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="file"> The file. </param>
        ///
        /// <returns>   The new writer. </returns>

        public StreamWriter CreateWriter(string name, string file)
        {
            this.CurrentStreamWriter = new StreamWriter(file);
            this.CurrentName = name;

            return this.CurrentStreamWriter;
        }

        /// <summary>   Creates a writer. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="file"> The file. </param>
        ///
        /// <returns>   The new writer. </returns>

        public StreamWriter CreateWriter(string file)
        {
            this.CurrentStreamWriter = new StreamWriter(file);

            return this.CurrentStreamWriter;
        }

        /// <summary>   Logs a line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/25/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void LogLine(string format, params object[] args)
        {
            var output = string.Format(format + "\r\n", args);

            if (this.CurrentHtmlContentNode != null)
            {
                var paraElement = this.CurrentHtmlContentNode.SelectSingleNode($"div/p[@name='{ this.CurrentName }']");

                if (paraElement != null)
                {
                    paraElement.AppendChild(HtmlNode.CreateNode(output.HtmlEncodeWithWhitespace()));
                }
            }
            
            if (this.CurrentName != "Base Object" && this.CurrentStreamWriter != null)
            {
                this.CurrentStreamWriter.WriteLine(output);
            }
        }
    }
}
