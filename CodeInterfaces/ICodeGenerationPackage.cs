using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using CodeInterfaces.Packages;

namespace CodeInterfaces
{
    public interface ICodeGenerationPackage
    {
        XmlDocument WSDLDocument { get; set; }
        XmlDocument CodeToWSDL { get; set; }
        Dictionary<string, IElementBuild> ElementBuilds { get; set; }
        DirectoryInfo WorkspaceDirectory { get; set; }
        IPackageServiceType PortType { get; set; }
        object CurrentWhenFilter { get; set; }
    }
}
