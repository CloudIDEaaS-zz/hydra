using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using AbstraX.TemplateObjects;
using CodeInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{ 
    public interface IAppSettingsKindHandler : IHandler
    {
        bool Process(EntityDomainModel entityDomainModel, BusinessModel businessModel, AppUIHierarchyNodeObject appHierarchyNodeObject, Dictionary<AppSettingsKind, BusinessModelObject> appSettingsObjects, Guid projectType, string projectFolderRoot, IGeneratorConfiguration generatorConfiguration);
    }
}
