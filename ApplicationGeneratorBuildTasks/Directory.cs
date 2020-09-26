using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ApplicationGeneratorBuildTasks
{
    //
    // <Directory Id="ApplicationGenerator.Binaries" />
    // 
    [XmlRoot(Namespace = "http://schemas.microsoft.com/wix/2006/wi")]
    public class Directory
    {
        [XmlAttribute]
        public string Id { get; set; }

        public Directory()
        {
        }

        public Directory(string id)
        {
            this.Id = id;
        }

        public ComponentGroupRef CreateComponentGroupRef()
        {
            return new ComponentGroupRef(this);
        }

    }
}
