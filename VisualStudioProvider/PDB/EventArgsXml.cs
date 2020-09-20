using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace VisualStudioProvider.PDB
{
    [Serializable, XmlRoot("EventArgsXml")]
    public abstract class EventArgsXml : EventArgs
    {
    }
}
