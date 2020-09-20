using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using AbstraX.Bindings;
using AbstraX.Contracts.Packages;

namespace AbstraX.Contracts
{
    public interface ICodeGenerationPackage
    {
        XmlDocument WSDLDocument { get; set; }
        XmlDocument AbstraXToWSDL { get; set; }
        Dictionary<string, IElementBuild> ElementBuilds { get; set; }
        DirectoryInfo WorkspaceDirectory { get; set; }
        IPackageServiceType PortType { get; set; }
        object CurrentWhenFilter { get; set; }
    }
}
