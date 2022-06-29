using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace NetCoreReflectionShim.Test
{
    public class RuntimeProxy : IRuntimeProxy
    {
        public T CallMethod<T>(string identifier, string methodName, params object[] args)
        {
            return default(T);
        }

        public T PropertyGet<T>(string identifier, string propertyName)
        {
            return default(T);
        }

        public void PropertySet<T>(string identifier, string propertyName, T value)
        {
        }
    }
}
