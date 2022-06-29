using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace ApplicationGeneratorBuildTasks
{
    /// <summary>   A component. </summary>
    ///
    /// <remarks> 
    /// 
    ///       <Component Id="comp_31AD2AAC_DA22_431D_AE89_BA0AFB7148E4" Guid="D643E345-A6B8-4A5A-94D2-6C276D256EF0" Permanent="no" SharedDllRefCount="no" Transitive="no">
    ///        <File Id = "_6EA613CF_8B02_401D_80F1_165F056517CD" DiskId="1" Hidden="no" ReadOnly="no" TrueType="no" System="no" Vital="yes" Name="System.Net.Http.dll" Source="..\ApplicationGenerator\bin\Debug\System.Net.Http.dll" KeyPath="yes" />
    ///      </Component>
    ///
    ///  </remarks>
    [XmlRoot(Namespace="http://schemas.microsoft.com/wix/2006/wi")]
    [DebuggerDisplay(" { File.Source } ")]

    public class Component
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public Guid Guid { get; set; }
        [XmlAttribute]
        public string Permanent { get; set; } = "no";
        [XmlAttribute]
        public string SharedDllRefCount { get; set; } = "no";
        [XmlAttribute]
        public string Transitive { get; set; } = "no";
        public File File { get; }

        public Component()
        {
        }

        public Component(FileInfo fileBinary, string projectFolder, string productFilePath)
        {
            this.Id = "comp_" + Guid.NewGuid().ToString().Replace("-", "_");
            this.Guid = Guid.NewGuid();

            this.File = new File(fileBinary, projectFolder, productFilePath);
        }

        public ComponentRef CreateComponentRef()
        {
            return new ComponentRef(this);
        }
    }
}