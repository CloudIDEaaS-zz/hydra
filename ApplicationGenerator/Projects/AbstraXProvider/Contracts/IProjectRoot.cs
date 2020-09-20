using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts
{
    public interface IProjectRoot : IProjectStructure
    {
        IProjectStructure FindLayer(string layerName);
    }
}
