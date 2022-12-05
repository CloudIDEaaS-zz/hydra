using AbstraX.Builds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.PlatformBuilder
{
    [SupportsGeneratorHandler("Ionic/Angular")]
    public class WebBuilder : IPlatformBuilder
    {
        public float Priority => 0f;
        public bool AllBuildsSucceeded { get; private set; }
        public string Name => nameof(WebBuilder);
        public string[] Names => new[] { "SQLServer", "Solution", "Web", "macOS", "Windows" };

        public Dictionary<string, Image> IconImages { get; private set; }

        public Dictionary<string, bool> BuildSelections { get; private set; }

        public Dictionary<string, BuildStats> Builds { get; private set; }
        private List<Exception> exceptions;
        private IAppTargetsBuilder appTargetsBuilder;

        public WebBuilder(IAppTargetsBuilder builder)
        {
            exceptions = new List<Exception>();
            appTargetsBuilder = builder;

            this.BuildSelections = new Dictionary<string, bool>();

            foreach (var name in this.Names)
            {
                this.BuildSelections.Add(name, false);
            }
        }

        public bool Build(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
        {
            this.Builds = new Dictionary<string, BuildStats>();

            foreach (var name in this.Names)
            {
                this.Builds.Add(name, new BuildStats(false, null));
            }

            foreach (var selection in this.BuildSelections.Where(s => s.Value))
            {
                var succeeded = false;
                BuildStats buildResult;
                Exception[] exceptionsArray;

                switch (selection.Key)
                {
                    case "SQLServer":

                        var localDbBuilder = new SQLLocalDbBuilder(appTargetsBuilder);

                        succeeded = localDbBuilder.BuildDatabase(generatorHandler, appLayout, logWriter);

                        exceptionsArray = exceptions.ToArray();
                        exceptions.Clear();

                        buildResult = new BuildStats(succeeded, exceptionsArray);
                        this.Builds[selection.Key] = buildResult;

                        appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, "SQLServer", 20);

                        if (!succeeded)
                        {
                            this.AllBuildsSucceeded = false;
                            return false;
                        }

                        break;


                    case "Solution":

                        succeeded = BuildSolution(generatorHandler, appLayout, logWriter);

                        exceptionsArray = exceptions.ToArray();
                        exceptions.Clear();

                        buildResult = new BuildStats(succeeded, exceptionsArray);
                        this.Builds[selection.Key] = buildResult;

                        appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, "Solution", 40);

                        if (!succeeded)
                        {
                            this.AllBuildsSucceeded = false;
                            return false;
                        }

                        break;

                    case "Web":

                        succeeded = BuildFrontEnd(generatorHandler, appLayout, logWriter);

                        exceptionsArray = exceptions.ToArray();
                        exceptions.Clear();

                        buildResult = new BuildStats(succeeded, exceptionsArray);
                        this.Builds[selection.Key] = buildResult;

                        appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, 100, "Web", "Windows", "macOS");

                        if (!succeeded)
                        {
                            this.AllBuildsSucceeded = false;
                            return false;
                        }

                        break;
                }
            }

            this.AllBuildsSucceeded = true;
            return true;
        }

        private bool BuildFrontEnd(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
        {
            var hydraCommandHandler = new HydraCommandHandler();

            hydraCommandHandler.OutputWriteLine = (format, args) =>
            {
                logWriter.WriteLine(format, args);
            };

            hydraCommandHandler.ErrorWriteLine = (format, args) =>
            {
                using (logWriter.ErrorMode())
                {
                    logWriter.WriteLine(format, args);
                }
            };

            logWriter.WriteLine("\r\n{0} {1}", hydraCommandHandler.Command, "build --prod --release");
            appTargetsBuilder.ReportCommand(string.Format("{0} {1}", hydraCommandHandler.Command, "build --prod --release"));

            hydraCommandHandler.Build(appLayout.WebFrontEndRootPath, "--prod", "--release");

            hydraCommandHandler.Wait();

            return hydraCommandHandler.Succeeded;
        }

        private bool BuildSolution(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
        {
            var dotNetHandler = new DotNetCommandHandler();
            var directoryName = Path.GetDirectoryName(appLayout.WorkspaceFile.FullName);

            dotNetHandler.OutputWriteLine = (format, args) =>
            {
                logWriter.WriteLine(format, args);
            };

            dotNetHandler.ErrorWriteLine = (format, args) =>
            {
                using (logWriter.ErrorMode())
                {
                    logWriter.WriteLine(format, args);
                }
            };

            logWriter.WriteLine("\r\n{0} {1}", dotNetHandler.Command, "build --verbosity quiet");
            appTargetsBuilder.ReportCommand(string.Format("\r\n{0} {1}", dotNetHandler.Command, "build --verbosity quiet"));

            dotNetHandler.Build(directoryName, "quiet");

            dotNetHandler.Wait();

            return dotNetHandler.Succeeded;
        }
    }
}
