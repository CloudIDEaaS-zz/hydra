using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.Data.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, Inherited=true)]
    public abstract class AbstraXDataProviderAttribute : Attribute
    {
        public Type ContextType { get; }
        public Type InterfaceType { get; }

        public AbstraXDataProviderAttribute(Type interfaceType, Type contextType)
        {
            this.InterfaceType = interfaceType;
            this.ContextType = contextType;
        }
    }
}
