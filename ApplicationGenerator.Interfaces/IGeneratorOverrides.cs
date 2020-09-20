using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System.Collections.Generic;

namespace AbstraX
{
    public interface IGeneratorOverrides
    {
        bool OverridesNamespace { get; }
        bool OverridesAppName { get; }
        string OriginalNamespace { get; set; }
        string GetNamespace(IGeneratorConfiguration generatorConfiguration, string argumentsKind);
        string GetAppName(IGeneratorConfiguration generatorConfiguration, string argumentsKind);
        void CopyFiles(IGeneratorConfiguration generatorConfiguration, string argumentsKind);
        Dictionary<string, object> GetHandlerArguments(string packageCachePath, string argumentsKind);
        bool SkipProcess(IFacetHandler facetHandler, IBase baseObject, Facet facet, IGeneratorConfiguration generatorConfiguration);
        string GetOverrideId(IBase baseObject, string predicate, string generatedId);
    }
}