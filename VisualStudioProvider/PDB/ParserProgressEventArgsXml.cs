using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VisualStudioProvider.PDB
{
    [Serializable, XmlRoot("ParserProgressEventArgs")]
    public class ParserProgressEventArgsXml : EventArgsXml
    {
        [XmlIgnore, NonSerialized]
        private ParserProgressEventArgs internalEventArgs;
        private string message;
        private int progress;
        private int total;

        public ParserProgressEventArgsXml(ParserProgressEventArgs e)
        {
            this.internalEventArgs = e;

            this.message = e.Message;
            this.progress = e.Progress;
            this.total = e.Total;
        }

        [XmlElement]
        public string Message 
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Message;
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
        public int Progress 
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Progress;
                }
                else
                {
                    return progress;
                }
            }

            set
            {
                this.progress = value;
            }
        }

        [XmlElement]
        public int Total 
        {
            get
            {
                if (internalEventArgs != null)
                {
                    return internalEventArgs.Total;
                }
                else
                {
                    return total;
                }
            }

            set
            {
                this.total = value;
            }
        }
    }
}
