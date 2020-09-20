using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Angular;
using AbstraX.FolderStructure;
using Utils;

namespace AbstraX
{
    public class IonicFolderHierarchy : ApplicationFolderHierarchy
    {
        [IonicFileSystemType(IonicFileSystemType.App)]
        public string App { get; set; }
        [IonicFileSystemType(IonicFileSystemType.Pages)]
        public string Pages { get; set; }
        [IonicFileSystemType(IonicFileSystemType.Providers)]
        public string Providers { get; }
        [IonicFileSystemType(IonicFileSystemType.WebAPIControllers)]
        public string WebAPIControllers { get; }
        [IonicFileSystemType(IonicFileSystemType.WebAPIProviders)]
        public string WebAPIProviders { get; }
        [IonicFileSystemType(IonicFileSystemType.WebAPIModels)]
        public string WebAPIModels { get; }
        [IonicFileSystemType(IonicFileSystemType.WebAPIServicesRoot)]
        public string WebAPIServicesRoot { get; }
        [IonicFileSystemType(IonicFileSystemType.Models)]
        public string Models { get; }
        [IonicFileSystemType(IonicFileSystemType.i18n)]
        public string i18n { get; }

        [IonicFileSystemType(IonicFileSystemType.AppProjectRoot)]
        public string IonicProjectRoot
        {
            get
            {
                return this.ProjectRoot;
            }
        }

        public override string this[Enum type]
        {
            get
            {
                return this[(IonicFileSystemType)type];
            }
        }

        public override string this[string name]
        {
            get
            {
                return this[EnumUtils.GetValue<IonicFileSystemType>(name)];
            }
        }

        public string this[IonicFileSystemType type]
        {
            get
            {
                return base.GetPropertyValue(type);
            }
        }

        public IonicFolderHierarchy(string folderRoot, string projectFolderRoot, string servicesFolderRoot) : base(folderRoot, projectFolderRoot, servicesFolderRoot)
        {
            this.App = base.CreateProjectPath("src/app", true);
            this.Pages = base.CreateProjectPath("src/app/pages", true);
            this.Providers = base.CreateProjectPath("src/app/providers", true);
            this.Models = base.CreateProjectPath("src/app/models", true);
            this.i18n = base.CreateProjectPath("src/assets/i18n", true);
            this.WebAPIControllers = base.CreateServicesPath("Controllers", true);
            this.WebAPIProviders = base.CreateServicesPath("Providers", true);
            this.WebAPIModels = base.CreateServicesPath("Models", true);
            this.WebAPIServicesRoot = this.ServicesRoot;
        }
    }
}
