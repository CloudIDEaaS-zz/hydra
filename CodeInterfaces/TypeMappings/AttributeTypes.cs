using System;
using System.Net;
#if SILVERLIGHT
using CodeInterfaces.ClientInterfaces;
#else
using CodeInterfaces;
#endif
using System.Collections.Generic;

namespace CodeInterfaces.TypeMappings
{
    public static class AttributeType
    {
        public static List<Type> Types
        {
            get
            {
                return new List<Type>()
                {
                    typeof(Guid),
                    typeof(DateTime),
                    typeof(string)
                };
            }
        }
    }
}
