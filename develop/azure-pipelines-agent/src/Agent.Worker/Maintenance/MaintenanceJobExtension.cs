// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Agent.Worker.Maintenance
{
    public interface IMaintenanceServiceProvider : IExtension
    {
        string MaintenanceDescription { get; }
        Task RunMaintenanceOperation(IExecutionContext context);
    }

    public sealed class MaintenanceJobExtension : JobExtension
    {
        public override Type ExtensionType => typeof(IJobExtension);
        public override HostTypes HostType => HostTypes.PoolMaintenance;
        public override IStep GetExtensionPreJobStep(IExecutionContext jobContext)
        {
            return new JobExtensionRunner(
                runAsync: MaintainAsync,
                condition: ExpressionManager.Succeeded,
                displayName: StringUtil.Loc("Maintenance"),
                data: null);
        }

        public override IStep GetExtensionPostJobStep(IExecutionContext jobContext)
        {
            return null;
        }

        public override string GetRootedPath(IExecutionContext context, string path)
        {
            return path;
        }

        public override void ConvertLocalPath(IExecutionContext context, string localPath, out string repoName, out string sourcePath)
        {
            sourcePath = localPath;
            repoName = string.Empty;
        }

        private async Task MaintainAsync(IExecutionContext executionContext, object data)
        {
            // Validate args.
            Trace.Entering();

            var extensionManager = HostContext.GetService<IExtensionManager>();
            var maintenanceServiceProviders = extensionManager.GetExtensions<IMaintenanceServiceProvider>();
            if (maintenanceServiceProviders != null && maintenanceServiceProviders.Count > 0)
            {
                foreach (var maintenanceProvider in maintenanceServiceProviders)
                {
                    // all maintenance operations should be best effort.
                    executionContext.Section(StringUtil.Loc("StartMaintenance", maintenanceProvider.MaintenanceDescription));
                    try
                    {
                        await maintenanceProvider.RunMaintenanceOperation(executionContext);
                    }
                    catch (Exception ex)
                    {
                        executionContext.Error(ex);
                    }

                    executionContext.Section(StringUtil.Loc("FinishMaintenance", maintenanceProvider.MaintenanceDescription));
                }
            }
        }

        public override void InitializeJobExtension(IExecutionContext context, IList<JobStep> steps, WorkspaceOptions workspace)
        {
            return;
        }
    }
}