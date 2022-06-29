using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeInterfaces
{
    public interface IGetSetProperty
    {
        MethodInfo Method { get; }
        string PropertyName { get; }
        Modifiers Modifiers { get; }
    }
}
