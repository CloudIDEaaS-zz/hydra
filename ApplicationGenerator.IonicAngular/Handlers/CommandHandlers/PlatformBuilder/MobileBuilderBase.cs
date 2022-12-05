using AbstraX.Builds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.PlatformBuilder
{
    public class MobileBuilderBase
    {
        public string Platform { get; }
        private IAppTargetsBuilder appTargetsBuilder;

        public MobileBuilderBase(IAppTargetsBuilder builder, string platform)
        {
            appTargetsBuilder = builder;
            Platform = platform;
        }

        public bool AddPlatform(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
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

            logWriter.WriteLine("\r\n{0} add {1}", hydraCommandHandler.Command, this.Platform);
            appTargetsBuilder.ReportCommand(string.Format("{0} add {1}", hydraCommandHandler.Command, this.Platform));

            hydraCommandHandler.Add(appLayout.WebFrontEndRootPath, this.Platform);

            hydraCommandHandler.Wait();

            return hydraCommandHandler.Succeeded;
        }

        public bool BuildPlatform(string generatorHandler, IAppFolderStructureSurveyor appLayout, ILogWriter logWriter)
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

            logWriter.WriteLine("\r\n{0} build {1}", hydraCommandHandler.Command, this.Platform);

            hydraCommandHandler.Build(appLayout.WebFrontEndRootPath, this.Platform);

            hydraCommandHandler.Wait();

            return hydraCommandHandler.Succeeded;
        }
    }
}
