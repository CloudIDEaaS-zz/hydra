using System;
using System.CodeDom;
using System.IO;
using System.Xml.Serialization;

namespace ApplicationGeneratorBuildTasks
{
    /// <summary>   A component. </summary>
    ///
    /// <remarks> 
    /// 
    /// 
    ///
    ///  </remarks>
    [XmlRoot(Namespace = "http://schemas.microsoft.com/wix/2006/wi")]
    public class ComponentRef
    {
        [XmlAttribute]
        public string Id { get; set; }

        public ComponentRef()
        {
        }

        public ComponentRef(string id)
        {
            this.Id = id;
        }

        public ComponentRef(Component component)
        {
            this.Id = component.Id;
        }
    }
}