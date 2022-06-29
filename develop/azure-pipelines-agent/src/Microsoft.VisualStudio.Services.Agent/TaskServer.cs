// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;

namespace Microsoft.VisualStudio.Services.Agent
{
    [ServiceLocator(Default = typeof(TaskServer))]
    public interface ITaskServer : IAgentService
    {
        Task ConnectAsync(VssConnection jobConnection);

        // task download
        Task<Stream> GetTaskContentZipAsync(Guid taskId, TaskVersion taskVersion, CancellationToken token);

        Task<bool> TaskDefinitionEndpointExist();
    }

    public sealed class TaskServer : AgentService, ITaskServer
    {
        private bool _hasConnection;

        private VssConnection _connection;

        private TaskAgentHttpClient _taskAgentClient;

        public async Task ConnectAsync(VssConnection jobConnection)
        {
            _connection = jobConnection;
            int attemptCount = 5;
            while (!_connection.HasAuthenticated && attemptCount-- > 0)
            {
                try
                {
                    await _connection.ConnectAsync();
                    break;
                }
                catch (Exception ex) when (attemptCount > 0)
                {
                    Trace.Info($"Catch exception during connect. {attemptCount} attempt left.");
                    Trace.Error(ex);
                }

                await Task.Delay(100);
            }

            _taskAgentClient = _connection.GetClient<TaskAgentHttpClient>();
            _hasConnection = true;
        }

        private void CheckConnection()
        {
            if (!_hasConnection)
            {
                throw new InvalidOperationException("SetConnection");
            }
        }

        //-----------------------------------------------------------------
        // Task Manager: Query and Download Task
        //-----------------------------------------------------------------
        public Task<Stream> GetTaskContentZipAsync(Guid taskId, TaskVersion taskVersion, CancellationToken token)
        {
            CheckConnection();
            return _taskAgentClient.GetTaskContentZipAsync(taskId, taskVersion, cancellationToken: token);
        }

        public async Task<bool> TaskDefinitionEndpointExist()
        {
            CheckConnection();
            try
            {
                // D9BAFED4-0B18-4F58-968D-86655B4D2CE9 ->  CommandLine task
                var definitions = await _taskAgentClient.GetTaskDefinitionsAsync(new Guid("D9BAFED4-0B18-4F58-968D-86655B4D2CE9"));
            }
            catch (VssResourceNotFoundException)
            {
                return false;
            }
            catch (TaskDefinitionNotFoundException)
            {
                // ignore task not found exception
                // this exception means the task definition is not in the DB, but the rest endpoint exists.
            }

            return true;
        }
    }
}