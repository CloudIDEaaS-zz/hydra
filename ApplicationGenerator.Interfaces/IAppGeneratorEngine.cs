using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;

namespace AbstraX
{
    public interface IAppGeneratorEngine : IGeneratorEngine
    {
        void ProcessContainer(IEntityContainer container);
        void ProcessEntity(IEntityType entity, bool isIdentityEntity = false);
        void ProcessEntitySet(IEntitySet entitySet, bool isIdentitySet = false);
        void ProcessNavigationProperty(INavigationProperty property);
        void HandleModuleAssembly(IModuleAssembly moduleAssembly, Folder folder);
        HandlerStackItem ProcessProperty(IEntityProperty property);
        string CurrentUIHierarchyPath { get; }
    }
}