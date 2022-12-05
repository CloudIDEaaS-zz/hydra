using AbstraX.Builds;
using SqlLocalDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.PlatformBuilder
{
    public class SQLLocalDbBuilder
    {
        private IAppTargetsBuilder appTargetsBuilder;

        public SQLLocalDbBuilder(IAppTargetsBuilder appTargetsBuilder)
        {
            this.appTargetsBuilder = appTargetsBuilder;
        }

        public bool BuildDatabase(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
        {
            var dotNetHandler = new DotNetCommandHandler();
            var directoryName = Path.GetDirectoryName(appLayout.WorkspaceFile.FullName);
            var appName = appLayout.AppName;
            var servicesProjectFile = new FileInfo(appLayout.ServicesProjectPath);
            var servicesFolder = servicesProjectFile.Directory;
            var appSettingsJsonFile = servicesFolder.GetFiles("appsettings.json", SearchOption.TopDirectoryOnly).Single();
            var migrationsFolder = servicesFolder.GetDirectories("Migrations", SearchOption.TopDirectoryOnly).SingleOrDefault();
            var handlerOutput = new StringBuilder();
            var handlerError = new StringBuilder();
            dynamic appSettings;
            FileInfo dbFile;
            FileInfo dbLogFile;
            var options = new SqlLocalDbOptions()
            {
                AutomaticallyDeleteInstanceFiles = true,
                StopOptions = StopInstanceOptions.NoWait,
            };

            logWriter.WriteLine("Creating database instance");

            using (var reader = new StreamReader(appSettingsJsonFile.FullName))
            {
                string connectionString;
                string dbFileName;

                appSettings = JsonExtensions.ReadJson<dynamic>(reader);
                connectionString = appSettings.Development.ConnectionStrings.DefaultConnection;
                dbFileName = ADOExtensions.GetAttachDbFilename(connectionString);

                dbFile = new FileInfo(dbFileName);
                dbLogFile = new FileInfo(Path.Combine(Path.GetDirectoryName(dbFileName), Path.GetFileNameWithoutExtension(dbFileName) + "_log.ldf"));
            }

            using (var localDB = new SqlLocalDbApi(options: options))
            {
                SqlLocalDbInstanceManager instanceManager;
                ISqlLocalDbInstanceInfo instance;
                var instanceName = appName;

                if (!localDB.IsLocalDBInstalled())
                {
                    return false;
                }

                instance = localDB.GetInstances().SingleOrDefault(i => i.Name == instanceName);

                if (instance != null)
                {
                    localDB.StopInstance(instanceName);
                    localDB.DeleteInstance(instanceName, true);
                }

                if (!dbFile.Directory.Exists)
                {
                    dbFile.Directory.Create();
                }
                else if (dbFile.Exists)
                {
                    dbFile.Delete();
                }

                if (dbLogFile.Exists)
                {
                    dbLogFile.Delete();
                }

                instance = localDB.CreateInstance(instanceName);

                instanceManager = new SqlLocalDbInstanceManager(instance, localDB);
                instanceManager.Start();
            }

            dotNetHandler.OutputWriteLine = (format, args) =>
            {
                logWriter.WriteLine(format, args);

                handlerOutput.AppendLineFormat(format, args);
            };

            dotNetHandler.ErrorWriteLine = (format, args) =>
            {
                using (logWriter.ErrorMode())
                {
                    logWriter.WriteLine(format, args);
                }

                handlerError.AppendLineFormat(format, args);
            };

            logWriter.WriteLine("\r\n{0} {1}", dotNetHandler.Command, "tool install dotnet-ef --global");
            appTargetsBuilder.ReportCommand(string.Format("{0} {1}", dotNetHandler.Command, "tool install dotnet-ef --global"));

            dotNetHandler.Tool.Install(servicesFolder.FullName, "dotnet-ef", true);

            dotNetHandler.Wait();

            if (!dotNetHandler.Succeeded)
            {
                if (!handlerError.ToString().StartsWith("Tool 'dotnet-ef' is already installed."))
                {
                    return false;
                }
            }

            ObjectExtensions.MultiAct(b => b.Clear(), handlerOutput, handlerError);

            logWriter.WriteLine("\r\n{0} {1}", dotNetHandler.Command, "tool update dotnet-ef --global");
            appTargetsBuilder.ReportCommand(string.Format("{0} {1}", dotNetHandler.Command, "tool update dotnet-ef --global"));

            dotNetHandler.Tool.Update(servicesFolder.FullName, "dotnet-ef", true);

            dotNetHandler.Wait();

            if (!dotNetHandler.Succeeded)
            {
                return false;
            }

            if (migrationsFolder != null)
            {
                migrationsFolder.ForceDeleteAllFilesAndSubFolders();
                migrationsFolder.Delete();
            }

            ObjectExtensions.MultiAct(b => b.Clear(), handlerOutput, handlerError);

            logWriter.WriteLine("\r\n{0} {1} {2}Context", dotNetHandler.Command, "ef migrations add InitialCreate --context", appName);
            appTargetsBuilder.ReportCommand(string.Format("{0} {1} {2}Context", dotNetHandler.Command, "ef migrations add InitialCreate --context", appName));

            dotNetHandler.EF.MigrationsAdd(servicesFolder.FullName, "InitialCreate", appName + "Context");

            dotNetHandler.Wait();

            if (!dotNetHandler.Succeeded)
            {
                return false;
            }

            ObjectExtensions.MultiAct(b => b.Clear(), handlerOutput, handlerError);

            logWriter.WriteLine("\r\n{0} {1} {2}Context", dotNetHandler.Command, "ef database update", appName);
            appTargetsBuilder.ReportCommand(string.Format("{0} {1} {2}Context", dotNetHandler.Command, "ef database update", appName));

            dotNetHandler.EF.DatabaseUpdate(servicesFolder.FullName);

            dotNetHandler.Wait();

            if (!dotNetHandler.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
