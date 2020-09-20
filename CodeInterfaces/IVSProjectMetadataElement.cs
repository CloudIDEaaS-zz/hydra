using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeInterfaces
{
    public interface IVSProjectMetadataElement : IVSProjectElement
    {
        string Name { get; }
        string Value { get; }
    }
}