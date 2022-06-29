using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllExport
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [Serializable]
    public class ModuleInitializerAttribute : Attribute
    {
    }
}
