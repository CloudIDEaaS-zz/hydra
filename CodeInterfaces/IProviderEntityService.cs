using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace CodeInterfaces
{
    public interface IProviderEntityService
    {
        ILog Log { get; }
        IProgrammingLanguage SelectedSourceLanguage { get; }
        IDatabaseProxy DatabaseProxy { get; }
        bool IncludePublicMemberVariablesAsProperties { get; }
    }
}
