using AbstraX.Builds;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.PlatformBuilder
{
    [SupportsGeneratorHandler("Ionic/Angular")]
    public class AndroidBuilder : MobileBuilderBase, IPlatformBuilder
    {
        public float Priority => 2f;
        public bool AllBuildsSucceeded { get; private set; }
        public string Name => nameof(AndroidBuilder);
        public string[] Names => new[] { "AddPlatform", "Android", "Tablet" };
        public Dictionary<string, Image> IconImages { get; private set; }
        public Dictionary<string, bool> BuildSelections { get; private set; }
        public Dictionary<string, BuildStats> Builds { get; private set; }
        private IAppTargetsBuilder appTargetsBuilder;
        private List<Exception> exceptions;

        public AndroidBuilder(IAppTargetsBuilder builder) : base(builder, "android")
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
                BuildStats buildStats;
                Exception[] exceptionsArray;

                switch (selection.Key)
                {
                    case "AddPlatform":

                        succeeded = base.AddPlatform(generatorHandler, appLayout, logWriter);

                        exceptionsArray = exceptions.ToArray();
                        exceptions.Clear();

                        buildStats = new BuildStats(succeeded, exceptionsArray);
                        this.Builds[selection.Key] = buildStats;

                        if (this.BuildSelections["Android"])
                        {
                            appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, "AddPlatform", 30);
                        }
                        else
                        {
                            appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, 100, "Android", "Tablet");
                        }

                        if (!succeeded)
                        {
                            this.AllBuildsSucceeded = false;
                            return false;
                        }

                        break;

                    case "Android":

                        succeeded = base.BuildPlatform(generatorHandler, appLayout, logWriter);

                        exceptionsArray = exceptions.ToArray();
                        exceptions.Clear();

                        buildStats = new BuildStats(succeeded, exceptionsArray);
                        this.Builds[selection.Key] = buildStats;

                        appTargetsBuilder.ReportBuild(this, selection.Key, succeeded, exceptionsArray, 100, "Android", "Tablet");

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
    }
}
