using AbstraX.Builds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    [SupportsGeneratorHandler("Ionic/Angular")]
    public class AppTargetsBuilder : IAppTargetsBuilder
    {
        public event BuildResultHandler OnBuildResults;
        public event EventHandlerT<string> OnCommand;

        public IPlatformBuilder[] GetSupportedPlatforms()
        {
            var supportsAttribute = this.GetType().GetCustomAttribute<SupportsGeneratorHandlerAttribute>();
            var builders = this.GetPlaformBuilders(supportsAttribute.GeneratorHandlerType).OrderBy(b => b.Priority);

            return builders.ToArray();
        }   

        public void Build(string generatorHandlerType, IAppFolderStructureSurveyor appLayout, IPlatformBuilder[] platforms, ILogWriter logWriter)
        {
            foreach (var platform in platforms)
            {
                platform.Build(generatorHandlerType, appLayout, logWriter);
            }
        }

        public void ReportBuild(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, string percentageOfName = null, int percentage = 0)
        {
            OnBuildResults(this, new BuildResultEventArgs(builder, name, succeeded, exceptions, percentageOfName, percentage));
        }

        public void ReportBuild(IPlatformBuilder builder, string name, bool succeeded, Exception[] exceptions, int percentage, params string[] percentageOfNames)
        {
            OnBuildResults(this, new BuildResultEventArgs(builder, name, succeeded, exceptions, percentageOfNames, percentage));
        }

        public void ReportCommand(string command)
        {
            OnCommand.Raise(this, command);
        }
    }
}
