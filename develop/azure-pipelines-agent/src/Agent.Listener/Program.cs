// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Agent.Sdk;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Agent.Listener
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            // We can't use the new SocketsHttpHandler for now for both Windows and Linux
            // On linux, Negotiate auth is not working if the TFS url is behind Https
            // On windows, Proxy is not working
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
            using (HostContext context = new HostContext("Agent"))
            {
                return MainAsync(context, args).GetAwaiter().GetResult();
            }
        }

        // Return code definition: (this will be used by service host to determine whether it will re-launch agent.listener)
        // 0: Agent exit
        // 1: Terminate failure
        // 2: Retriable failure
        // 3: Exit for self update
        public async static Task<int> MainAsync(IHostContext context, string[] args)
        {
            Tracing trace = context.GetTrace("AgentProcess");
            trace.Info($"Agent package {BuildConstants.AgentPackage.PackageName}.");
            trace.Info($"Running on {PlatformUtil.HostOS} ({PlatformUtil.HostArchitecture}).");
            trace.Info($"RuntimeInformation: {RuntimeInformation.OSDescription}.");
            context.WritePerfCounter("AgentProcessStarted");
            var terminal = context.GetService<ITerminal>();

            // TODO: check that the right supporting tools are available for this platform
            // (replaces the check for build platform vs runtime platform)

            try
            {
                trace.Info($"Version: {BuildConstants.AgentPackage.Version}");
                trace.Info($"Commit: {BuildConstants.Source.CommitHash}");
                trace.Info($"Culture: {CultureInfo.CurrentCulture.Name}");
                trace.Info($"UI Culture: {CultureInfo.CurrentUICulture.Name}");

                // Validate directory permissions.
                string agentDirectory = context.GetDirectory(WellKnownDirectory.Root);
                trace.Info($"Validating directory permissions for: '{agentDirectory}'");
                try
                {
                    IOUtil.ValidateExecutePermission(agentDirectory);
                }
                catch (Exception e)
                {
                    terminal.WriteError(StringUtil.Loc("ErrorOccurred", e.Message));
                    trace.Error(e);
                    return Constants.Agent.ReturnCode.TerminatedError;
                }

                if (PlatformUtil.RunningOnWindows)
                {
                    // Validate PowerShell 3.0 or higher is installed.
                    var powerShellExeUtil = context.GetService<IPowerShellExeUtil>();
                    try
                    {
                        powerShellExeUtil.GetPath();
                    }
                    catch (Exception e)
                    {
                        terminal.WriteError(StringUtil.Loc("ErrorOccurred", e.Message));
                        trace.Error(e);
                        return Constants.Agent.ReturnCode.TerminatedError;
                    }

                    // Validate .NET Framework 4.5 or higher is installed.
                    if (!NetFrameworkUtil.Test(new Version(4, 5), trace))
                    {
                        terminal.WriteError(StringUtil.Loc("MinimumNetFramework"));
                        // warn only, like configurationmanager.cs does. this enables windows edition with just .netcore to work
                    }
                }

                // Add environment variables from .env file
                string envFile = Path.Combine(context.GetDirectory(WellKnownDirectory.Root), ".env");
                if (File.Exists(envFile))
                {
                    var envContents = File.ReadAllLines(envFile);
                    foreach (var env in envContents)
                    {
                        if (!string.IsNullOrEmpty(env) && env.IndexOf('=') > 0)
                        {
                            string envKey = env.Substring(0, env.IndexOf('='));
                            string envValue = env.Substring(env.IndexOf('=') + 1);
                            Environment.SetEnvironmentVariable(envKey, envValue);
                        }
                    }
                }

                // Parse the command line args.
                var command = new CommandSettings(context, args, new SystemEnvironment());
                trace.Info("Arguments parsed");

                // Up front validation, warn for unrecognized commandline args.
                var unknownCommandlines = command.Validate();
                if (unknownCommandlines.Count > 0)
                {
                    terminal.WriteError(StringUtil.Loc("UnrecognizedCmdArgs", string.Join(", ", unknownCommandlines)));
                }

                // Defer to the Agent class to execute the command.
                IAgent agent = context.GetService<IAgent>();
                try
                {
                    return await agent.ExecuteCommand(command);
                }
                catch (OperationCanceledException) when (context.AgentShutdownToken.IsCancellationRequested)
                {
                    trace.Info("Agent execution been cancelled.");
                    return Constants.Agent.ReturnCode.Success;
                }
                catch (NonRetryableException e)
                {
                    terminal.WriteError(StringUtil.Loc("ErrorOccurred", e.Message));
                    trace.Error(e);
                    return Constants.Agent.ReturnCode.TerminatedError;
                }

            }
            catch (Exception e)
            {
                terminal.WriteError(StringUtil.Loc("ErrorOccurred", e.Message));
                trace.Error(e);
                return Constants.Agent.ReturnCode.RetryableError;
            }
        }
    }
}
