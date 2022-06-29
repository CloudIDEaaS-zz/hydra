using System;
using System.Diagnostics;
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

    [XmlRoot(Namespace = "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd")]
    [DebuggerDisplay(" src='{ src }' target='{ target }'")]
    public class file
    {
        [XmlAttribute]
        public string src { get; set; }
        [XmlAttribute]
        public string target { get; set; }

        public file()
        {
        }

        public file(FileInfo fileBinary, string rootPath)
        {
            var relativePath = fileBinary.GetRelativePath(rootPath);

            this.src = fileBinary.FullName;

            if (rootPath == Path.GetDirectoryName(fileBinary.FullName))
            {
                this.target = relativePath;
            }
            else
            {
                this.target = @"lib\" + relativePath;
            }
        }
    }
}