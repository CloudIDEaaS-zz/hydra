using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Windows.Forms;
using System.Net;
using Utils.NamedPipes;
using Utils;
using System.Threading;
using HydraDebugAssistant;
using HydraDebugAssistant.Info;

namespace DynamicTemplateEngine
{
    public class DynamicTemplateEngineHost : ITextTemplatingEngineHost, ITemplateEngineHost
    {
        public event EventHandler OnDebugCallback;
        private static bool bSkipErrors;
        public IList<string> StandardAssemblyReferences { get; }
        public IList<string> StandardImports { get; }
        private ITemplateEngineHostManager templateEngineHostManager;
        private static NamedPipeClient client;
        private static bool connected;
        private PipeMessageEventHandler serverMessage;
        private PipeExceptionEventHandler pipeError;
        private static FileInfo userInfoFile;
        public string TemplateFile { get; set; }

        static DynamicTemplateEngineHost()
        {
            var programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var generatorApp = Path.Combine(programFilesPath, @"\CloudIDEaaS\Hydra\ApplicationGenerator.exe");

            if (!File.Exists(generatorApp))
            {
                programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                generatorApp = Path.Combine(programFilesPath, @"\CloudIDEaaS\Hydra\ApplicationGenerator.exe");
            }

            if (!File.Exists(generatorApp))
            {
                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var applicationGeneratorPath = Path.Combine(solutionPath, @"ApplicationGenerator\bin\Debug\ApplicationGenerator.exe");

                generatorApp = applicationGeneratorPath;
            }

            if (File.Exists(generatorApp))
            {
                var generatorsDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(generatorApp), "Generators"));

                if (generatorsDirectory.Exists)
                {
                    userInfoFile = new FileInfo(Path.Combine(generatorsDirectory.FullName, Environment.UserName + ".hui"));
                }
            }
        }

        public DynamicTemplateEngineHost(ITemplateEngineHostManager templateEngineHostManager)
        {
            this.StandardAssemblyReferences = new List<string>();
            this.StandardImports = new List<string>();

            this.templateEngineHostManager = templateEngineHostManager;
        }

        public object GetHostOption(string optionName)
        {
            throw new NotImplementedException();
        }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            throw new NotImplementedException();
        }

        public void LogErrors(CompilerErrorCollection errors)
        {
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            throw new NotImplementedException();
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            throw new NotImplementedException();
        }

        public Type ResolveDirectiveProcessor(string processorName)
        {
            throw new NotImplementedException();
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            throw new NotImplementedException();
        }

        public string ResolvePath(string path)
        {
            throw new NotImplementedException();
        }

        public void SetFileExtension(string extension)
        {
        }

        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
        }

        private void DebugCallback(object sender, EventArgs e)
        {
            OnDebugCallback(sender, e);
        }

        public string Generate<T>(Dictionary<string, object> sessionVariables, bool throwException = false)
        {
            string output;
            var generatorType = typeof(T);

            if (templateEngineHostManager.WriteFileMode)
            {
                var userInfo = UserInfo.Read(userInfoFile);
                var liveBreakPointsSet = userInfo.RootFolders.Any(f => Process.GetProcesses().Any(p => p.Id.ToHexString(true) == f.DebuggerAttachedProcess) && f.Breakpoints.Any(b => b.Enabled));

                if (liveBreakPointsSet && (client == null || !connected))
                {
                    var rootFolder = userInfo.RootFolders.Single(f => Process.GetProcesses().Any(p => p.Id.ToHexString(true) == f.DebuggerAttachedProcess) && f.Breakpoints.Any(b => b.Enabled));
                    var thisProcessId = Process.GetCurrentProcess().Id.ToHexString(true);

                    if (rootFolder.DebuggerRemoteProcess != thisProcessId)
                    {
                        rootFolder.DebuggerRemoteProcess = thisProcessId;
                        userInfo.Save(userInfoFile);
                    }

                    ConnectClient();
                }
            }

            output = Generate(generatorType, sessionVariables, throwException);

            return output;
        }

        public PostProcessResult PostProcess()
        {
            if (connected)
            {
                var continuePolling = true;
                var postProcessResult = PostProcessResult.None;
                var handlingEvents = false;
                var announcedPause = false;
                var resetEvent = new ManualResetEvent(false);

                while (continuePolling)
                {
                    resetEvent.Reset();

                    serverMessage = (sender, e) =>
                    {
                        var response = (string)e.Command;

                        switch (response)
                        {
                            case Commands.BreakpointSet:

                                if (!announcedPause)
                                {
                                    Console.WriteLine("HydraDebugAssistant paused on breakpoint");
                                    announcedPause = true;
                                }

                                continuePolling = true;
                                break;

                            case Commands.BreakpointNotSet:

                                continuePolling = false;
                                postProcessResult = PostProcessResult.Continue;
                                break;

                            case Commands.RetryAndBreak:

                                Console.WriteLine("HydraDebugAssistant retry and break");

                                continuePolling = false;
                                postProcessResult = PostProcessResult.RedoGenerate;
                                break;

                            case Commands.Continue:

                                Console.WriteLine("HydraDebugAssistant continue");

                                continuePolling = false;
                                postProcessResult = PostProcessResult.Continue;
                                break;
                        }

                        resetEvent.Set();
                    };

                    pipeError = (s, e) =>
                    {
                        Console.WriteLine("Disconnected from HydraDebugAssistant, Error: {0}", e.Message);

                        resetEvent.Set();
                    };

                    if (!handlingEvents)
                    {
                        client.ServerMessage += serverMessage;
                        client.PipeError += pipeError;

                        handlingEvents = true;
                    }

                    client.Send(new CommandPacket("IsBreakpointSet", new KeyValuePair<string, object>("TemplateFile", this.TemplateFile)));

                    while (!resetEvent.WaitOne(100))
                    {
                        Application.DoEvents();
                    }
                }

                client.ServerMessage -= serverMessage;
                client.PipeError -= pipeError;

                return postProcessResult;
            }

            return PostProcessResult.Continue;
        }

        private void ConnectClient()
        {
            var address = templateEngineHostManager.DebugAssistantAddress;

            try
            {
                client = new NamedPipeClient(address);
                client.Connect();

                connected = true;
            }
            catch (Exception ex)
            {
                connected = false;
            }
        }

        public string Generate(Type generatorType, Dictionary<string, object> sessionVariables, bool throwException = false)
        {
            try
            {
                string source = null;
                var engine = new Engine();
                var content = File.ReadAllText(this.TemplateFile);
                var references = new List<PortableExecutableReference>();
                string language;
                string[] referenceNames;
                SyntaxTree syntaxTree;
                CSharpCompilation compilation;
                MemoryStream memoryStream;
                EmitResult emitResult;
                Assembly templateAssembly;
                var requestingAssembly = generatorType.Assembly;
                var pdbLocation = Path.GetDirectoryName(this.TemplateFile);
                var pdbPath = Path.Combine(pdbLocation, Path.GetFileNameWithoutExtension(requestingAssembly.Location) + ".pdb");
                var pdbFile = new FileInfo(pdbPath);

                source = engine.PreprocessTemplate(content, this, generatorType.Name, generatorType.Namespace, out language, out referenceNames);

                syntaxTree = CSharpSyntaxTree.ParseText(source);

                references.Add(MetadataReference.CreateFromFile(requestingAssembly.Location));

                foreach (var assemblyName in requestingAssembly.GetReferencedAssemblies())
                {
                    var referenceAssembly = Assembly.Load(assemblyName);
                    var peReference = MetadataReference.CreateFromFile(referenceAssembly.Location);

                    references.Add(peReference);
                }

                compilation = CSharpCompilation.Create("TemplateCompilation+" + generatorType.Name + "_" + Guid.NewGuid().ToString(), new[] { syntaxTree }, references.ToArray(), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug));

                //Emit to stream
                // 
                memoryStream = new MemoryStream();

                emitResult = compilation.Emit(memoryStream);

                ////Load into currently running assembly. Normally we'd probably
                ////want to do this in an AppDomain

                if (emitResult.Success)
                {
                    var session = new TextTemplatingSession();
                    IDisposable generator;
                    string output;

                    session["DebugCallback"] = new EventHandler(DebugCallback);

                    foreach (var pair in sessionVariables)
                    {
                        session[pair.Key] = pair.Value;
                    }

                    templateAssembly = Assembly.Load(memoryStream.ToArray());
                    generatorType = templateAssembly.GetType(generatorType.Namespace + "." + generatorType.Name);

                    generator = (IDisposable) Activator.CreateInstance(generatorType);

                    generatorType.GetProperty("Session").SetValue(generator, session, null);
                    generatorType.GetMethod("Initialize").Invoke(generator, null);

                    output = (string)generatorType.GetMethod("TransformText").Invoke(generator, null);

                    generator.Dispose();

                    templateEngineHostManager.ReportGenerate(generatorType, this.TemplateFile);

                    return output;
                }
                else
                {
                    Console.WriteLine("Issues with generator type: {0}", generatorType.Name);

                    foreach (var result in emitResult.Diagnostics)
                    {
                        Console.WriteLine("\t" + result.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                if (bSkipErrors)
                {
                    return null;
                }
                else if (throwException)
                {
                    throw ex;
                }
                else if (MessageBox.Show(string.Format("Generator threw an error '{0}'. Would you like to debug?", ex.Message), "Generator error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bSkipErrors = false;
                    Debugger.Break();
                }
                else
                {
                    bSkipErrors = true;
                }
            }

            return null;
        }
    }
}
