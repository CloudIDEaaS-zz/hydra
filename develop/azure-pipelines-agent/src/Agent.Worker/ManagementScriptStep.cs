// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.Agent.Worker.Handlers;
using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;

namespace Microsoft.VisualStudio.Services.Agent.Worker
{
    // This is for internal testing and is not publicly supported. This will be removed from the agent at a later time.
    public class ManagementScriptStep : AgentService, IStep
    {
        public ManagementScriptStep(
            string scriptPath,
            IExpressionNode condition,
            string displayName)
        {
            ScriptPath = scriptPath;
            Condition = condition;
            DisplayName = displayName;
        }

        public string ScriptPath { get; private set; }
        public IExpressionNode Condition { get; set; }
        public string DisplayName { get; private set; }
        public bool ContinueOnError => false;
        public bool Enabled => true;
        public TimeSpan? Timeout => null;
        public Pipelines.StepTarget Target => null;

        public string AccessToken { get; set; }
        public IExecutionContext ExecutionContext { get; set; }

        public async Task RunAsync()
        {
            // Validate script file.
            if (!File.Exists(ScriptPath))
            {
                throw new FileNotFoundException(StringUtil.Loc("FileNotFound", ScriptPath));
            }

            // Create the handler data.
            var scriptDirectory = Path.GetDirectoryName(ScriptPath);
            var handlerData = new PowerShellExeHandlerData()
            {
                Target = ScriptPath,
                WorkingDirectory = scriptDirectory,
                FailOnStandardError = "false"
            };

            // Create the handler invoker
            var stepHost = HostContext.CreateService<IDefaultStepHost>();

            // Create the handler.
            var handlerFactory = HostContext.GetService<IHandlerFactory>();
            var handler = (PowerShellExeHandler)handlerFactory.Create(
                executionContext: ExecutionContext,
                task: null,
                stepHost: stepHost,
                endpoints: ExecutionContext.Endpoints,
                secureFiles: new List<SecureFile>(0),
                data: handlerData,
                inputs: new Dictionary<string, string>(),
                environment: new Dictionary<string, string>(VarUtil.EnvironmentVariableKeyComparer),
                runtimeVariables: ExecutionContext.Variables,
                taskDirectory: scriptDirectory);

            // Add the access token to the handler.
            handler.AccessToken = AccessToken;

            // Run the task.
            await handler.RunAsync();
        }
    }
}