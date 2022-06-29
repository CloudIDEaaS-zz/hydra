// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.Agent.Listener.Configuration
{
    public interface IConfigurationProvider : IExtension, IAgentService
    {
        string ConfigurationProviderType { get; }

        void GetServerUrl(AgentSettings agentSettings, CommandSettings command);

        void GetCollectionName(AgentSettings agentSettings, CommandSettings command, bool isHosted);

        Task TestConnectionAsync(AgentSettings agentSettings, VssCredentials creds, bool isHosted);

        Task GetPoolIdAndName(AgentSettings agentSettings, CommandSettings command);

        string GetFailedToFindPoolErrorString();

        Task<TaskAgent> UpdateAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command);

        Task<TaskAgent> AddAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command);

        Task DeleteAgentAsync(AgentSettings agentSettings);

        Task<TaskAgent> GetAgentAsync(AgentSettings agentSettings);

        void ThrowTaskAgentExistException(AgentSettings agentSettings);
    }

    public class BuildReleasesAgentConfigProvider : AgentService, IConfigurationProvider
    {
        public Type ExtensionType => typeof(IConfigurationProvider);
        private ITerminal _term;
        protected IAgentServer _agentServer;

        public string ConfigurationProviderType
            => Constants.Agent.AgentConfigurationProvider.BuildReleasesAgentConfiguration;

        public override void Initialize(IHostContext hostContext)
        {
            base.Initialize(hostContext);
            _term = hostContext.GetService<ITerminal>();
            _agentServer = HostContext.GetService<IAgentServer>();
        }

        public void GetServerUrl(AgentSettings agentSettings, CommandSettings command)
        {
            agentSettings.ServerUrl = command.GetUrl();
        }

        public void GetCollectionName(AgentSettings agentSettings, CommandSettings command, bool isHosted)
        {
            // Collection name is not required for Build/Release agent
        }

        public virtual async Task GetPoolIdAndName(AgentSettings agentSettings, CommandSettings command)
        {
            string poolName = command.GetPool();

            TaskAgentPool agentPool = (await _agentServer.GetAgentPoolsAsync(poolName)).FirstOrDefault();
            if (agentPool == null)
            {
                throw new TaskAgentPoolNotFoundException(StringUtil.Loc("PoolNotFound", poolName));
            }
            else
            {
                Trace.Info("Found pool {0} with id {1} and name {2}", poolName, agentPool.Id, agentPool.Name);
                agentSettings.PoolId = agentPool.Id;
                agentSettings.PoolName = agentPool.Name;
            }
        }

        public string GetFailedToFindPoolErrorString() => StringUtil.Loc("FailedToFindPool");

        public void ThrowTaskAgentExistException(AgentSettings agentSettings)
        {
            throw new TaskAgentExistsException(StringUtil.Loc("AgentWithSameNameAlreadyExistInPool", agentSettings.PoolId, agentSettings.AgentName));
        }

        public Task<TaskAgent> UpdateAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            return _agentServer.UpdateAgentAsync(agentSettings.PoolId, agent);
        }

        public Task<TaskAgent> AddAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            return _agentServer.AddAgentAsync(agentSettings.PoolId, agent);
        }

        public Task DeleteAgentAsync(AgentSettings agentSettings)
        {
            return _agentServer.DeleteAgentAsync(agentSettings.PoolId, agentSettings.AgentId);
        }

        public async Task TestConnectionAsync(AgentSettings agentSettings, VssCredentials creds, bool isHosted)
        {
            _term.WriteLine(StringUtil.Loc("ConnectingToServer"));
            await _agentServer.ConnectAsync(new Uri(agentSettings.ServerUrl), creds);
        }

        public async Task<TaskAgent> GetAgentAsync(AgentSettings agentSettings)
        {
            var agents = await _agentServer.GetAgentsAsync(agentSettings.PoolId, agentSettings.AgentName);
            Trace.Verbose("Returns {0} agents", agents.Count);
            return agents.FirstOrDefault();
        }
    }

    public class DeploymentGroupAgentConfigProvider : AgentService, IConfigurationProvider
    {
        public Type ExtensionType => typeof(IConfigurationProvider);
        public string ConfigurationProviderType
            => Constants.Agent.AgentConfigurationProvider.DeploymentAgentConfiguration;
        protected ITerminal _term;
        protected string _projectName = string.Empty;
        private IDeploymentGroupServer _deploymentGroupServer = null;

        public override void Initialize(IHostContext hostContext)
        {
            base.Initialize(hostContext);
            _term = hostContext.GetService<ITerminal>();
            _deploymentGroupServer = HostContext.GetService<IDeploymentGroupServer>();
        }

        public void GetServerUrl(AgentSettings agentSettings, CommandSettings command)
        {
            agentSettings.ServerUrl = command.GetUrl();
            Trace.Info("url - {0}", agentSettings.ServerUrl);
        }

        public void GetCollectionName(AgentSettings agentSettings, CommandSettings command, bool isHosted)
        {
            // for onprem tfs, collection is required for deploymentGroup
            if (!isHosted)
            {
                Trace.Info("Provided url is for onprem tfs, need collection name");
                agentSettings.CollectionName = command.GetCollectionName();
            }
        }

        public virtual async Task GetPoolIdAndName(AgentSettings agentSettings, CommandSettings command)
        {
            _projectName = command.GetProjectName(_projectName);
            var deploymentGroupName = command.GetDeploymentGroupName();

            var deploymentGroup = await GetDeploymentGroupAsync(_projectName, deploymentGroupName);
            Trace.Info($"PoolId for deployment group '{deploymentGroupName}' is '{deploymentGroup.Pool.Id}'.");
            Trace.Info($"Project id for deployment group '{deploymentGroupName}' is '{deploymentGroup.Project.Id.ToString()}'.");

            agentSettings.PoolId = deploymentGroup.Pool.Id;
            agentSettings.PoolName = deploymentGroup.Pool.Name;
            agentSettings.DeploymentGroupId = deploymentGroup.Id;
            agentSettings.ProjectId = deploymentGroup.Project.Id.ToString();
        }

        public virtual string GetFailedToFindPoolErrorString() => StringUtil.Loc("FailedToFindDeploymentGroup");

        public virtual void ThrowTaskAgentExistException(AgentSettings agentSettings)
        {
            throw new TaskAgentExistsException(StringUtil.Loc("DeploymentMachineWithSameNameAlreadyExistInDeploymentGroup", agentSettings.DeploymentGroupId, agentSettings.AgentName));
        }

        public virtual async Task<TaskAgent> UpdateAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            var deploymentMachine = (await this.GetDeploymentTargetsAsync(agentSettings)).FirstOrDefault();

            deploymentMachine.Agent = agent;
            deploymentMachine = await _deploymentGroupServer.ReplaceDeploymentTargetAsync(new Guid(agentSettings.ProjectId), agentSettings.DeploymentGroupId, deploymentMachine.Id, deploymentMachine);

            await GetAndAddTags(deploymentMachine, agentSettings, command);
            return deploymentMachine.Agent;
        }

        public virtual async Task<TaskAgent> AddAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            var deploymentMachine = new DeploymentMachine() { Agent = agent };
            var azureSubscriptionId = await GetAzureSubscriptionIdAsync();
            if (!String.IsNullOrEmpty(azureSubscriptionId))
            {
                deploymentMachine.Properties.Add("AzureSubscriptionId", azureSubscriptionId);
            }
            deploymentMachine = await _deploymentGroupServer.AddDeploymentTargetAsync(new Guid(agentSettings.ProjectId), agentSettings.DeploymentGroupId, deploymentMachine);

            await GetAndAddTags(deploymentMachine, agentSettings, command);

            return deploymentMachine.Agent;
        }

        public virtual async Task DeleteAgentAsync(AgentSettings agentSettings)
        {
            var machines = await GetDeploymentTargetsAsync(agentSettings);
            Trace.Verbose("Returns {0} machines with name {1}", machines.Count, agentSettings.AgentName);
            var machine = machines.FirstOrDefault();
            if (machine != null)
            {
                if (!string.IsNullOrWhiteSpace(agentSettings.ProjectId))
                {
                    await _deploymentGroupServer.DeleteDeploymentTargetAsync(new Guid(agentSettings.ProjectId), agentSettings.DeploymentGroupId, machine.Id);
                }
                else
                {
                    await _deploymentGroupServer.DeleteDeploymentTargetAsync(agentSettings.ProjectName, agentSettings.DeploymentGroupId, machine.Id);
                }
            }
        }

        public virtual async Task TestConnectionAsync(AgentSettings agentSettings, VssCredentials creds, bool isHosted)
        {
            var url = agentSettings.ServerUrl;  // Ensure not to update back the url with agentSettings !!!
            _term.WriteLine(StringUtil.Loc("ConnectingToServer"));

            // Create the connection for deployment group
            Trace.Info("Test connection with deployment group");
            if (!isHosted && !string.IsNullOrWhiteSpace(agentSettings.CollectionName)) // For on-prm validate the collection by making the connection
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(url));
                uriBuilder.Path = uriBuilder.Path + "/" + agentSettings.CollectionName;
                Trace.Info("Tfs Collection level url to connect - {0}", uriBuilder.Uri.AbsoluteUri);
                url = uriBuilder.Uri.AbsoluteUri;
            }
            VssConnection deploymentGroupconnection = VssUtil.CreateConnection(new Uri(url), creds);

            await _deploymentGroupServer.ConnectAsync(deploymentGroupconnection);
            Trace.Info("Connect complete for deployment group");
        }

        public virtual async Task<TaskAgent> GetAgentAsync(AgentSettings agentSettings)
        {
            var machines = await GetDeploymentTargetsAsync(agentSettings);
            Trace.Verbose("Returns {0} machines", machines.Count);
            var machine = machines.FirstOrDefault();
            if (machine != null)
            {
                return machine.Agent;
            }

            return null;
        }

        private async Task GetAndAddTags(DeploymentMachine deploymentMachine, AgentSettings agentSettings, CommandSettings command)
        {
            // Get and apply Tags in case agent is configured against Deployment Group
            bool needToAddTags = command.GetDeploymentGroupTagsRequired();
            while (needToAddTags)
            {
                try
                {
                    string tagString = command.GetDeploymentGroupTags();
                    Trace.Info("Given tags - {0} will be processed and added", tagString);

                    if (!string.IsNullOrWhiteSpace(tagString))
                    {
                        var tagsList =
                            tagString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s.Trim())
                                .Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

                        if (tagsList.Any())
                        {
                            Trace.Info("Adding tags - {0}", string.Join(",", tagsList.ToArray()));

                            deploymentMachine.Tags = tagsList;
                            await _deploymentGroupServer.UpdateDeploymentTargetsAsync(new Guid(agentSettings.ProjectId), agentSettings.DeploymentGroupId, new List<DeploymentMachine>() { deploymentMachine });

                            _term.WriteLine(StringUtil.Loc("DeploymentGroupTagsAddedMsg"));
                        }
                    }
                    break;
                }
                catch (Exception e) when (!command.Unattended)
                {
                    _term.WriteError(e);
                    _term.WriteError(StringUtil.Loc("FailedToAddTags"));
                }
            }
        }

        private async Task<DeploymentGroup> GetDeploymentGroupAsync(string projectName, string deploymentGroupName)
        {
            ArgUtil.NotNull(_deploymentGroupServer, nameof(_deploymentGroupServer));

            var deploymentGroup = (await _deploymentGroupServer.GetDeploymentGroupsAsync(projectName, deploymentGroupName)).FirstOrDefault();

            if (deploymentGroup == null)
            {
                throw new DeploymentGroupNotFoundException(StringUtil.Loc("DeploymentGroupNotFound", deploymentGroupName));
            }

            Trace.Info("Found deployment group {0} with id {1}", deploymentGroupName, deploymentGroup.Id);
            return deploymentGroup;
        }

        private async Task<List<DeploymentMachine>> GetDeploymentTargetsAsync(AgentSettings agentSettings)
        {
            List<DeploymentMachine> machines;
            if (!string.IsNullOrWhiteSpace(agentSettings.ProjectId))
            {
                machines = await _deploymentGroupServer.GetDeploymentTargetsAsync(new Guid(agentSettings.ProjectId), agentSettings.DeploymentGroupId, agentSettings.AgentName);
            }
            else
            {
                machines = await _deploymentGroupServer.GetDeploymentTargetsAsync(agentSettings.ProjectName, agentSettings.DeploymentGroupId, agentSettings.AgentName);
            }

            return machines;
        }

        private async Task<string> GetAzureSubscriptionIdAsync()
        {
            // We will use the Azure Instance Metadata Service in order to fetch metadata ( in this case Subscription Id used to provision the VM) if the VM is an Azure VM
            // More on Instance Metadata Service can be found here: https://docs.microsoft.com/en-us/azure/virtual-machines/windows/instance-metadata-service
            string azureSubscriptionId = string.Empty;
            const string imdsUri = "http://169.254.169.254/metadata/instance/compute/subscriptionId?api-version=2017-08-01&format=text";
            using (var httpClient = new HttpClient(HostContext.CreateHttpClientHandler()))
            {
                httpClient.DefaultRequestHeaders.Add("Metadata", "True");
                httpClient.Timeout = TimeSpan.FromSeconds(5);
                try
                {
                    azureSubscriptionId = await httpClient.GetStringAsync(imdsUri);
                    if (!Guid.TryParse(azureSubscriptionId, out Guid result))
                    {
                        azureSubscriptionId = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    // An exception will be thrown if the Agent Machine is a non-Azure VM.
                    azureSubscriptionId = string.Empty;
                    Trace.Info($"GetAzureSubscriptionId ex: {ex.Message}");
                }
            }

            return azureSubscriptionId;
        }
    }

    public class SharedDeploymentAgentConfigProvider : BuildReleasesAgentConfigProvider, IConfigurationProvider
    {
        public new string ConfigurationProviderType
            => Constants.Agent.AgentConfigurationProvider.SharedDeploymentAgentConfiguration;

        public override async Task GetPoolIdAndName(AgentSettings agentSettings, CommandSettings command)
        {
            string poolName = command.GetDeploymentPoolName();

            TaskAgentPool agentPool = (await _agentServer.GetAgentPoolsAsync(poolName, TaskAgentPoolType.Deployment)).FirstOrDefault();
            if (agentPool == null)
            {
                throw new TaskAgentPoolNotFoundException(StringUtil.Loc("DeploymentPoolNotFound", poolName));
            }
            else
            {
                Trace.Info("Found deployment pool {0} with id {1} and name {2}", poolName, agentPool.Id, agentPool.Name);
                agentSettings.PoolId = agentPool.Id;
                agentSettings.PoolName = agentPool.Name;
            }
        }
    }

    public class EnvironmentVMResourceConfigProvider : DeploymentGroupAgentConfigProvider, IConfigurationProvider
    {
        public new string ConfigurationProviderType
            => Constants.Agent.AgentConfigurationProvider.EnvironmentVMResourceConfiguration;
        private IEnvironmentsServer _environmentsServer = null;

        public override void Initialize(IHostContext hostContext)
        {
            base.Initialize(hostContext);
            _environmentsServer = HostContext.GetService<IEnvironmentsServer>();
        }

        public override async Task TestConnectionAsync(AgentSettings agentSettings, VssCredentials creds, bool isHosted)
        {
            var url = agentSettings.ServerUrl;  // Ensure not to update back the url with agentSettings !!!
            _term.WriteLine(StringUtil.Loc("ConnectingToServer"));

            // Create the connection for environment virtual machine resource
            Trace.Info("Test connection with environment");
            if (!isHosted && !string.IsNullOrWhiteSpace(agentSettings.CollectionName)) // For on-prm validate the collection by making the connection
            {
                UriBuilder uriBuilder = new UriBuilder(new Uri(url));
                uriBuilder.Path = uriBuilder.Path + "/" + agentSettings.CollectionName;
                Trace.Info("Tfs Collection level url to connect - {0}", uriBuilder.Uri.AbsoluteUri);
                url = uriBuilder.Uri.AbsoluteUri;
            }
            VssConnection environmentConnection = VssUtil.CreateConnection(new Uri(url), creds);

            await _environmentsServer.ConnectAsync(environmentConnection);
            Trace.Info("Connection complete for environment");
        }

        public override async Task GetPoolIdAndName(AgentSettings agentSettings, CommandSettings command)
        {
            _projectName = command.GetProjectName(_projectName);
            var environmentName = command.GetEnvironmentName();
            Trace.Info("vm resource will be configured against the environment '{0}'", environmentName);

            var environmentInstance = await GetEnvironmentAsync(_projectName, environmentName);

            agentSettings.EnvironmentId = environmentInstance.Id;
            agentSettings.ProjectName = environmentInstance.Project.Name;
            agentSettings.ProjectId = environmentInstance.Project.Id.ToString();
            Trace.Info("vm resource configuration: environment id: '{0}', project name: '{1}', project id: '{2}'", agentSettings.EnvironmentId, agentSettings.ProjectName, agentSettings.ProjectId);
        }

        public override string GetFailedToFindPoolErrorString() => StringUtil.Loc("FailedToFindEnvironment");

        public override void ThrowTaskAgentExistException(AgentSettings agentSettings)
        {
            throw new TaskAgentExistsException(StringUtil.Loc("VMResourceWithSameNameAlreadyExistInEnvironment", agentSettings.EnvironmentId, agentSettings.AgentName));
        }

        public override async Task<TaskAgent> AddAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            var virtualMachine = new VirtualMachineResource() { Name = agent.Name, Agent = agent };
            var tags = GetVirtualMachineResourceTags(command);
            virtualMachine.Tags = tags;

            virtualMachine = await _environmentsServer.AddEnvironmentVMAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId, virtualMachine);
            Trace.Info("Environment virtual machine resource with name: '{0}', id: '{1}' has been added successfully.", virtualMachine.Name, virtualMachine.Id);

            var pool =  await _environmentsServer.GetEnvironmentPoolAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId);
            Trace.Info("environment pool id: '{0}'", pool.Id);
            agentSettings.PoolId = pool.Id;
            agentSettings.AgentName = virtualMachine.Name;
            agentSettings.EnvironmentVMResourceId = virtualMachine.Id;

            return virtualMachine.Agent;
        }

        private IList<String> GetVirtualMachineResourceTags(CommandSettings command)
        {
            // Get and apply Tags in case agent is configured against Deployment Group
            var result = new List<String>();
            bool needToAddTags = command.GetEnvironmentVirtualMachineResourceTagsRequired();
            if (needToAddTags)
            {
                string tagString = command.GetEnvironmentVirtualMachineResourceTags();
                Trace.Info("Given tags - '{0}' will be processed and added to environment vm resource", tagString);

                if (!string.IsNullOrWhiteSpace(tagString))
                {
                    var tagsList =
                        tagString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s.Trim())
                            .Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

                    result.AddRange(tagsList);
                }
            }
            return result;
        }

        public override async Task DeleteAgentAsync(AgentSettings agentSettings)
        {
            Trace.Info("Deleting environment virtual machine resource with id: '{0}'", agentSettings.EnvironmentVMResourceId);
            if (!string.IsNullOrWhiteSpace(agentSettings.ProjectId))
            {
                await _environmentsServer.DeleteEnvironmentVMAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId, agentSettings.EnvironmentVMResourceId);
            }
            else
            {
                await _environmentsServer.DeleteEnvironmentVMAsync(agentSettings.ProjectName, agentSettings.EnvironmentId, agentSettings.EnvironmentVMResourceId);
            }
            Trace.Info("Environment virtual machine resource with id: '{0}' has been successfully deleted.", agentSettings.EnvironmentVMResourceId);
        }

        public override async Task<TaskAgent> GetAgentAsync(AgentSettings agentSettings)
        {
            var vmResources = await GetEnvironmentVMsAsync(agentSettings);
            Trace.Verbose("Returns {0} virtual machine resources", vmResources.Count);
            var machine = vmResources.FirstOrDefault();
            if (machine != null)
            {
                return machine.Agent;
            }

            return null;
        }

        public override async Task<TaskAgent> UpdateAgentAsync(AgentSettings agentSettings, TaskAgent agent, CommandSettings command)
        {
            var tags = GetVirtualMachineResourceTags(command);

            var vmResource = (await GetEnvironmentVMsAsync(agentSettings)).FirstOrDefault();

            vmResource.Agent = agent;
            vmResource.Tags = tags;
            Trace.Info("Replacing environment virtual machine resource with id: '{0}'", vmResource.Id);
            vmResource = await _environmentsServer.ReplaceEnvironmentVMAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId, vmResource);
            Trace.Info("environment virtual machine resource with id: '{0}' has been replaced successfully", vmResource.Id);
            var pool =  await _environmentsServer.GetEnvironmentPoolAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId);

            agentSettings.AgentName = vmResource.Name;
            agentSettings.EnvironmentVMResourceId = vmResource.Id;
            agentSettings.PoolId = pool.Id;

            return vmResource.Agent;
        }

        private async Task<EnvironmentInstance> GetEnvironmentAsync(string projectName, string environmentName)
        {
            ArgUtil.NotNull(_environmentsServer, nameof(_environmentsServer));

            var environment = (await _environmentsServer.GetEnvironmentsAsync(projectName, environmentName)).FirstOrDefault();

            if (environment == null)
            {
                throw new EnvironmentNotFoundException(StringUtil.Loc("EnvironmentNotFound", environmentName));
            }

            Trace.Info("Found environment {0} with id {1}", environmentName, environment.Id);
            return environment;
        }

        private async Task<List<VirtualMachineResource>> GetEnvironmentVMsAsync(AgentSettings agentSettings)
        {
            List<VirtualMachineResource> machines;
            if (!string.IsNullOrWhiteSpace(agentSettings.ProjectId))
            {
                machines = await _environmentsServer.GetEnvironmentVMsAsync(new Guid(agentSettings.ProjectId), agentSettings.EnvironmentId, agentSettings.AgentName);
            }
            else
            {
                machines = await _environmentsServer.GetEnvironmentVMsAsync(agentSettings.ProjectName, agentSettings.EnvironmentId, agentSettings.AgentName);
            }

            return machines;
        }
    }
}
