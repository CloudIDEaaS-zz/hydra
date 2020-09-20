using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VisualStudioProvider.PDB
{
    [Serializable, XmlRoot("ParserDiagnosticsEventArgs")]
    public class ParserDiagnosticsEventArgsXml : EventArgsXml
    {
        [XmlIgnore, NonSerialized]
        private unsafe Utils.EventArgs<CppSharp.Parser.ParserDiagnostic> internalEventArgs;
        private string message;
        private string level;
        private int columnNumber;
        private int lineNumber;

        public unsafe ParserDiagnosticsEventArgsXml(Utils.EventArgs<CppSharp.Parser.ParserDiagnostic> e)
        {
            this.message = e.Value.Message;
            this.level = e.Value.Level.ToString();
            this.columnNumber = e.Value.ColumnNumber;
            this.lineNumber = e.Value.LineNumber; 

            this.internalEventArgs = e;
        }

        [XmlElement]
        public string Message
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Value.Message;
                }
                else
                {
                    return message;
                }
            }

            set
            {
                this.message = value;
            }
        }

        [XmlElement]
        public string Level
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Value.Level.ToString();
                }
                else
                {
                    return level;
                }
            }

            set
            {
                this.level = value;
            }
        }

        [XmlElement]
        public int ColumnNumber
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Value.ColumnNumber;
                }
                else
                {
                    return columnNumber;
                }
            }

            set
            {
                this.columnNumber = value;
            }
        }

        [XmlElement]
        public int LineNumber
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Value.LineNumber;
                }
                else
                {
                    return lineNumber;
                }
            }

            set
            {
                this.lineNumber = value;
            }
        }
    }
}
