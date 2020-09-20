using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeInterfaces
{
    public interface IVSProjectElement
    {
        string Condition { get; }
        string Label { get; }
    }
}