using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface ISilverlightApp
    {
        Guid ProjectGuid { get; }
        IVSProject SilverlightProject { get; }
        string PathInWeb { get; }
        bool ConfigurationSpecificFolders { get; }
    }
}
