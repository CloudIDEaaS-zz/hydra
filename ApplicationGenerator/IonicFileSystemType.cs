using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public class IonicFileSystemTypeAttribute : FileSystemTypeAttribute
    {
        public IonicFileSystemTypeAttribute(IonicFileSystemType type)
        {
            this.Type = type;
        }
    }

    public enum IonicFileSystemType
    {
        AppProjectRoot,
        App,
        i18n,
        Pages,
        Providers,
        Models,
        AppCore,
        AppCoreServices,
        Assets,
        AssetsImgs,
        Jasmine,
        Modules,
        ModulesComponents,
        WebAPIControllers,
        WebAPIModels,
        WebAPIProviders,
        WebAPIServicesRoot
    }
}
