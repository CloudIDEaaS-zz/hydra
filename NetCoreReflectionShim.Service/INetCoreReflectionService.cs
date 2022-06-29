using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetCoreReflectionShim.Service
{
    public interface INetCoreReflectionService
    {
        public Dictionary<int, object> CachedObjects { get; }
        public Dictionary<int, ICustomAttributeProvider> CachedTokenObjects { get; }
        public Dictionary<string, Assembly> CachedAssemblies { get; }
        public Type GetTypeProxy(Type type);
    }
}
