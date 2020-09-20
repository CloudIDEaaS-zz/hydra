using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public interface ISilverlightApp
    {
        Guid ProjectGuid { get; }
        IVSProject SilverlightProject { get; }
        string PathInWeb { get; }
        bool ConfigurationSpecificFolders { get; }
    }
}
