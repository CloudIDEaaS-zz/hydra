using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbstraX.Contracts
{
    public interface IVSProjectElement
    {
        string Condition { get; }
        string Label { get; }
    }
}