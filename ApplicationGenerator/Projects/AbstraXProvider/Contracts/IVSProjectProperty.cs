using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AbstraX.Contracts
{
    public interface IVSProjectProperty
    {
        string Name { get; }
        string Value { get; }
    }
}
