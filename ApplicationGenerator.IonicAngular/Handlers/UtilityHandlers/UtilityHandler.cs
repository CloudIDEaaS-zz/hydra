using AbstraX.Handlers.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.UtilityHandlers
{
    [UtilityHandler]
    public class UtilityHandler : IUtilityHandler
    {
        private HydraCommandHandler hydraCommandHandler;
        private Dictionary<string, object> arguments;
        public float Priority => 1.0f;

        public UtilityHandler()
        {
        }

        public UtilityHandler(Dictionary<string, object> arguments)
        {
            this.arguments = arguments;
        }

        public void GenerateStarterAppFrontend(string appName, string currentDirectory, bool debug)
        {
            hydraCommandHandler = new HydraCommandHandler(arguments);

            hydraCommandHandler.Start(appName, currentDirectory, debug);
        }

        public void GenerateCompleteAppFrontend(string appName, string currentDirectory, bool debug)
        {
            hydraCommandHandler = new HydraCommandHandler(arguments);

            hydraCommandHandler.GenerateApp(appName, currentDirectory, debug);
        }

        public bool DefaultIDEExists
        {
            get
            {
                var commandHandler = new VsCodeCommandHandler();

                return File.Exists(commandHandler.CommandExe);
            }
        }

        public void DefaultIDEOpenFile(string filePath)
        {
            var commandHandler = new VsCodeCommandHandler();

            Debug.Assert(File.Exists(filePath));

            commandHandler.OpenFile(filePath);
        }

        public void DefaultIDEOpenFolder(string folderPath)
        {
            var commandHandler = new VsCodeCommandHandler();

            Debug.Assert(Directory.Exists(folderPath));

            commandHandler.OpenFolder(folderPath);
        }

        public void Dispose()
        {
            hydraCommandHandler.Dispose();
        }
    }
}
