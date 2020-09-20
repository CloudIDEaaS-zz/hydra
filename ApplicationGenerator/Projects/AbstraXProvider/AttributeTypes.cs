using System;
using System.Net;
#if SILVERLIGHT
using AbstraX.ClientInterfaces;
#else
using AbstraX.ServerInterfaces;
#endif
using System.Collections.Generic;

namespace Hydra
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
