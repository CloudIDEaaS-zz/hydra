using ApplicationGenerator.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WebApiProviderAttribute : AbstraXDataProviderAttribute
    {
        public WebApiProviderAttribute(Type interfaceType, Type contextType) : base(interfaceType, contextType)
        {
        }
    }
}
