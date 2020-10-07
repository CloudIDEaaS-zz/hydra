using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    [DebuggerDisplay(" { Name } ")]
    public class Directory
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public List<Directory> Directories { get; set; }
        [XmlIgnore]
        public List<Component> Files { get; set; }

        public Directory()
        {
            this.Directories = new List<Directory>();
            this.Files = new List<Component>();
        }

        public Directory(string id) : this()
        {
            this.Id = id;
        }

        public Directory(string id, string name) : this()
        {
            this.Id = id;
            this.Name = name;
        }


        public Directory(DirectoryInfo directoryInfo) : this()
        {
            this.Id = "dir_" + Guid.NewGuid().ToString().Replace("-", "_");
            this.Name = directoryInfo.Name;
        }

        public ComponentGroupRef CreateComponentGroupRef()
        {
            return new ComponentGroupRef(this);
        }

    }
}
