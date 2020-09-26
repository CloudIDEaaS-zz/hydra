using System.IO;
using System.Xml.Serialization;
using Utils;

namespace ApplicationGeneratorBuildTasks
{
    /// <summary>   A component. </summary>
    ///
    /// <remarks> 
    /// 
    ///   <File Id = "_6EA613CF_8B02_401D_80F1_165F056517CD" DiskId="1" Hidden="no" ReadOnly="no" TrueType="no" System="no" Vital="yes" Name="System.Net.Http.dll" Source="..\ApplicationGenerator\bin\PreRelease\System.Net.Http.dll" KeyPath="yes" />
    ///
    ///  </remarks>

    [XmlRoot(Namespace = "http://schemas.microsoft.com/wix/2006/wi")]
    public class File
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string DiskId { get; set; } = "1";
        [XmlAttribute]
        public string Hidden { get; set; } = "no";
        [XmlAttribute]
        public string ReadOnly { get; set; } = "no";
        [XmlAttribute]
        public string TrueType { get; set; } = "no";
        [XmlAttribute]
        public string System { get; set; } = "no";
        [XmlAttribute]
        public string Vital { get; set; } = "yes";
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Source { get; set; }
        [XmlAttribute]
        public string KeyPath { get; set; } = "yes";

        public File()
        {
        }

        public File(FileInfo fileBinary, string projectFolder, string productFilePath)
        {
            var productFile = new FileInfo(productFilePath);
            var relativePath = fileBinary.GetRelativePath(productFile.Directory.FullName);

            this.Source = relativePath;
        }
    }
}