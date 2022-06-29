using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface ISurrogateElement : IElement
    {
        IElement SurrogateSource { get; }
        IElement ReferencedFrom { get; }
    }
}
