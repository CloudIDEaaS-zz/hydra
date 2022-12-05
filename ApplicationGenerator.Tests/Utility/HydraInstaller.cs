using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydra.ReleaseManagement
{
    public class HydraInstaller : IHydraInstaller
    {
        public string MsiPath { get; set; }

        public HydraInstaller()
        {   
        }

        public string GetProductVersion()
        {
            string version = null;

            using (var msiPackage = new InstallPackage(this.MsiPath, DatabaseOpenMode.Transact) { WorkingDirectory = @"d:\Users\Ken\Documents\HydraReleases\Export" })
            {
                using (var session = Installer.OpenPackage(msiPackage, false))
                {
                    var view = session.Database.OpenView("SELECT * FROM Property WHERE Property = 'ProductVersion'");
                    Record record;
                    string property;
                    string value;

                    view.Execute(null);
                    record = view.Fetch();

                    property = record.GetString("Property");
                    value = record.GetString("Value");

                    version = value;

                    view.Close();
                }
            }

            return version;
        }

        public string Extract(string workingDirectory)
        {
            string version = null;

            using (var msiPackage = new InstallPackage(this.MsiPath, DatabaseOpenMode.Transact) { WorkingDirectory = workingDirectory })
            {
                using (var session = Installer.OpenPackage(msiPackage, false))
                {
                    ExtractPackageFiles(session, msiPackage);
                }
            }

            return version;
        }

        private void ExtractPackageFiles(Session session, InstallPackage msiPackage)
        {
            var fileKeysToInstall = new HashSet<string>();

            foreach (var feature in session.Features)
            {
                var featureLevel = msiPackage.ExecuteIntegerQuery(string.Format("SELECT `Level` FROM `Feature` WHERE `Feature` = '{0}'", feature.Name)).First();

                if (featureLevel != 1)
                {
                    continue;
                }

                var featureComponents = msiPackage.ExecuteStringQuery(string.Format("SELECT `Component_` FROM `FeatureComponents` WHERE `Feature_` = '{0}'", feature.Name));

                foreach (var installableComponent in session.Components.Where(c => featureComponents.Contains(c.Name)))
                {
                    var componentFileKeys = msiPackage.ExecuteStringQuery(string.Format("SELECT `File` FROM `File` WHERE `Component_` = '{0}'",  installableComponent.Name));

                    foreach (var fileKey in componentFileKeys)
                    {
                        fileKeysToInstall.Add(fileKey);
                    }
                }

                msiPackage.ExtractFiles(fileKeysToInstall);
            }

            //var witempPath = Path.Combine(_targetFolder, "WITEMP");

            //if (Directory.Exists(witempPath))
            //{
            //    ClearReadOnly(new DirectoryInfo(witempPath));
            //    Directory.Delete(witempPath, true);
            //}
        }
    }
}
