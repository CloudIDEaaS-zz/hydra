using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeInterfaces.Bindings.Interfaces
{
    public interface IProviderService
    {
        void LogGenerateByID(string id, MethodInfo method);
        void PostLogGenerateByID();
    }
}
