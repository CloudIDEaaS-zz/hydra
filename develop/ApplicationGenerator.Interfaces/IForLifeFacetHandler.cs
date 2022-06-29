using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IForLifeFacetHandler : IFacetHandler
    {
        bool Terminate(IGeneratorConfiguration generatorConfiguration);
    }

    public interface ISingletonForLifeFacetHandler : IForLifeFacetHandler
    {
    }
}
