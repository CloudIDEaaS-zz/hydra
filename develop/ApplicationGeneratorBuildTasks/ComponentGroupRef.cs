using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ApplicationGeneratorBuildTasks
{
    [XmlRoot(Namespace = "http://schemas.microsoft.com/wix/2006/wi")]
    public class ComponentGroupRef
    {
        [XmlAttribute]
        public string Id { get; set; }

        public ComponentGroupRef()
        {
        }

        public ComponentGroupRef(Directory directory)
        {
            this.Id = directory.Id;
        }
    }
}
