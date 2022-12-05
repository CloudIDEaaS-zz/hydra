using AbstraX.Angular;
using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.Generators;
using AbstraX.Generators.Client.AppDataProvider;
using AbstraX.Generators.Server.ConfigJson;
using AbstraX.Generators.Server.WebAPIModel;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.FacetHandlers
{
    [FacetHandler(typeof(UIAttribute), UIKindGuids.StaticContainer)]
    public class AppDataProviderFacetHandler : BaseNonPageFacetHandler
    {
        public override FacetHandlerLayer FacetHandlerLayer => FacetHandlerLayer.Client;

        public override bool Process(IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration)
        {
            var providersPath = generatorConfiguration.ApplicationFolderHierarchy[IonicFileSystemType.Providers];

            AppDataProviderGenerator.GenerateProvider(baseObject, providersPath, generatorConfiguration);

            return true;
        }
    }
}
