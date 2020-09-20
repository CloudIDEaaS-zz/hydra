using AbstraX.ServerInterfaces;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public interface IBusinessModelGeneratorHandler : IHandler
    {
        bool Process(BusinessModel businessModel, Guid projectType, string projectFolderRoot, string templateFile, string outputFileName, IGeneratorConfiguration generatorConfiguration);
    }
}
