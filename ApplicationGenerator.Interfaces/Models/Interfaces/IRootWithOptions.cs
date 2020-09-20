using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AbstraX.ServerInterfaces;

namespace AbstraX.Models.Interfaces
{
    public interface IRootWithOptions : IRoot
    {
        bool IsGeneratedModel { get; }
    }
}
