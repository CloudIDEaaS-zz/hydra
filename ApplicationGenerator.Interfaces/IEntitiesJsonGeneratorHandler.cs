using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IEntitiesJsonGeneratorHandler : IHandler
    {
        bool Process(EntityDomainModel domainModel, Guid projectType, string projectFolderRoot, string templateFile, string outputFileName, IGeneratorConfiguration generatorConfiguration);
    }
}
