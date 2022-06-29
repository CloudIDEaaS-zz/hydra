using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.IO;
using Microsoft.ServiceModel.DomainServices.Tools;
using ImpromptuInterface;
using System.Diagnostics;
using System.Collections;

namespace BuildTasks
{
    class TaskHost : ITaskHost, Microsoft.Build.Framework.ILogger, IBuildEngine
    {
        public void Initialize(IEventSource eventSource)
        {
            throw new NotImplementedException();
        }

        public string Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public LoggerVerbosity Verbosity
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            throw new NotImplementedException();
        }

        public int ColumnNumberOfTaskNode
        {
            get
            {
                return 0;
            }
        }

        public bool ContinueOnError
        {
            get { throw new NotImplementedException(); }
        }

        public int LineNumberOfTaskNode
        {
            get
            {
                return 5;
            }
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public string ProjectFileOfTaskNode
        {
            get
            {
                return @"C:\Projects\0024-0028 HydraShell\Hydra\Hydra\Hydra.SL.csproj";
            }
        }
    }

    public class CustomRIAServiceTask : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                var task = new CreateRiaClientFilesTask();
                var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                var solutionPath = projectFile.Directory.Parent.FullName;
                var serverDirectory = new DirectoryInfo(Path.Combine(solutionPath, @"Hydra.Web\bin"));

                task.HostObject = new TaskHost();
                task.BuildEngine = new TaskHost();

                task.ServerProjectPath = Path.Combine(solutionPath, @"Hydra.Web\Hydra.Web.csproj");
                task.OutputPath = @"bin\Debug";
                task.ClientProjectPath = BuildEngine.ProjectFileOfTaskNode;
                task.Language = "c#";
                task.CodeGeneratorName = "Microsoft.ServiceModel.DomainServices.Tools.CodeDomClientCodeGenerator";

                task.ServerAssemblies = serverDirectory.GetFiles("*.dll").Select(f =>
                {
                    return new
                    {
                        ItemSpec = f.FullName
                    }.ActLike<ITaskItem>();

                }).ToArray();

                try
                {
                    task.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                task.CodeGeneratorName = "DomainServices.Tools.CodeDomClientCodeGenerator";

                try
                {
                    task.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error applying generated code runtime changes: '{0}'", ex));

                BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", "", 0, 0, 0, 0, string.Format("Error applying generated code runtime changes: '{0}'", ex), "", ""));

                return false;
            }

            return true;
        }
    }
}
