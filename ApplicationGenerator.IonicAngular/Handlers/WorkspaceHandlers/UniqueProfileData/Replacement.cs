using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AbstraX.Handlers.WorkspaceHandlers.UniqueProfileData
{
    public interface IReplacement
    {
        string Path { get; set; }
    }

    public class Replacement<T> : IReplacement
    {
        public string Path { get; set; }
        public Action<T> Replace { get; set; }
    }

    public class XmlReplacment : Replacement<XElement>
    {
    }

    public class JsonReplacement : Replacement<JToken>
    {
    }
}
