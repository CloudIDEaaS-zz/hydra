// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.VisualStudio.Services.Agent.Worker;
using Microsoft.VisualStudio.Services.Agent.Worker.Build;
using Microsoft.VisualStudio.Services.Agent.Worker.Release;
using Microsoft.VisualStudio.Services.Agent.Worker.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;

namespace Microsoft.VisualStudio.Services.Agent.Tests.L1.Worker
{
    public class TestResults
    {
        public int ReturnCode { get; internal set; }
        public TaskResult Result { get; internal set; }
        public bool TimedOut { get; internal set; }
    }

    public class L1TestBase
    {
        protected TimeSpan ChannelTimeout = TimeSpan.FromSeconds(100);
        protected TimeSpan JobTimeout = TimeSpan.FromSeconds(100);

        private List<IAgentService> _mockedServices = new List<IAgentService>();

        protected List<Timeline> GetTimelines()
        {
            return GetMockedService<FakeJobServer>().Timelines.Values.ToList();
        }

        protected IList<TimelineRecord> GetSteps()
        {
            var timeline = GetTimelines()[0];
            return timeline.Records.Where(x => x.RecordType == "Task").ToList();
        }

        protected T GetMockedService<T>()
        {
            return _mockedServices.Where(x => x is T).Cast<T>().Single();
        }

        protected IList<string> GetTimelineLogLines(TimelineRecord record)
        {
            var jobService = GetMockedService<FakeJobServer>();
            return jobService.LogLines.GetValueOrDefault(record.Log.Id);
        }

        protected void AssertJobCompleted()
        {
            Assert.Equal(1, GetMockedService<FakeJobServer>().RecordedEvents.Where(x => x is JobCompletedEvent).Count());
        }

        protected static Pipelines.AgentJobRequestMessage LoadTemplateMessage()
        {
            return LoadJobMessageFromJSON(JobMessageTemplate);
        }

        protected static Pipelines.AgentJobRequestMessage LoadJobMessageFromJSON(string message)
        {
            return JsonUtility.FromString<Pipelines.AgentJobRequestMessage>(message);
        }

        protected static TaskStep CreateScriptTask(string script)
        {
            var step = new TaskStep
            {
                Reference = new TaskStepDefinitionReference
                {
                    Id = Guid.Parse("d9bafed4-0b18-4f58-968d-86655b4d2ce9"),
                    Name = "CmdLine",
                    Version = "2.164.0"
                },
                Name = "CmdLine",
                DisplayName = "CmdLine",
                Id = Guid.NewGuid()
            };
            step.Inputs.Add("script", script);

            return step;
        }

        protected async Task<TestResults> RunWorker(Pipelines.AgentJobRequestMessage message,
            [CallerMemberName] string testName = "")
        {
            // Clear working directory
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/TestRuns/" + testName;
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // Fix localization
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var stringFile = Path.Combine(assemblyLocation, "en-US", "strings.json");
            StringUtil.LoadExternalLocalization(stringFile);

            using (L1HostContext context = new L1HostContext("Agent", GetLogFile(this, testName)))
            {
                SetupMocks(context);

                // Use different working directories for each test
                var config = GetMockedService<FakeConfigurationStore>();
                config.WorkingDirectoryName = testName;

                await SetupMessage(context, message);

                var cts = new CancellationTokenSource();
                cts.CancelAfter((int)JobTimeout.TotalMilliseconds);
                return await RunWorker(context, message, cts.Token);
            }
        }

        private void SetupMocks(L1HostContext context)
        {
            _mockedServices.Add(context.SetupService<IConfigurationStore>(typeof(FakeConfigurationStore)));
            _mockedServices.Add(context.SetupService<IJobServer>(typeof(FakeJobServer)));
            _mockedServices.Add(context.SetupService<ITaskServer>(typeof(FakeTaskServer)));
            _mockedServices.Add(context.SetupService<IBuildServer>(typeof(FakeBuildServer)));
            _mockedServices.Add(context.SetupService<IReleaseServer>(typeof(FakeReleaseServer)));
            _mockedServices.Add(context.SetupService<IAgentPluginManager>(typeof(FakeAgentPluginManager)));
            _mockedServices.Add(context.SetupService<ITaskManager>(typeof(FakeTaskManager)));
            _mockedServices.Add(context.SetupService<ICustomerIntelligenceServer>(typeof(FakeCustomerIntelligenceServer)));
            LoadTasks();
        }

        private void LoadTasks()
        {
            var baseDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var taskZipsPath = Path.Join(baseDirectory, "TaskZips");
            var tasksPath = Path.Join(baseDirectory, "L1", "Tasks");
            if (!Directory.Exists(tasksPath))
            {
                throw new Exception("No mock tasks provided");
            }
            if (!Directory.Exists(taskZipsPath))
            {
                Directory.CreateDirectory(taskZipsPath);
            }
            foreach (var d in Directory.GetDirectories(tasksPath))
            {
                var zip = Path.Join(taskZipsPath, Path.GetFileName(d) + ".zip");
                if (File.Exists(zip))
                {
                    File.Delete(zip);
                }
                ZipFile.CreateFromDirectory(d, zip);
            }
        }

        private string GetLogFile(object testClass, string testMethod)
        {
            // Trim the test assembly's root namespace from the test class's full name.
            var suiteName = testClass.GetType().FullName.Substring(
                startIndex: typeof(Tests.TestHostContext).FullName.LastIndexOf(nameof(TestHostContext)));
            var testName = testMethod.Replace(".", "_");

            return Path.Combine(
               Path.Combine(TestUtil.GetSrcPath(), "Test", "TestLogs"),
               $"trace_{suiteName}_{testName}.log");
        }

        private async Task SetupMessage(HostContext context, Pipelines.AgentJobRequestMessage message)
        {
            // The agent assumes the server creates this
            var jobServer = context.GetService<IJobServer>();
            await jobServer.CreateTimelineAsync(message.Plan.ScopeIdentifier, message.Plan.PlanType, message.Plan.PlanId, message.Timeline.Id, default(CancellationToken));
        }

        private async Task<TestResults> RunWorker(HostContext HostContext, Pipelines.AgentJobRequestMessage message, CancellationToken jobRequestCancellationToken)
        {
            var worker = HostContext.GetService<IWorker>();

            Task<int> workerTask = null;
            // Setup the anonymous pipes to use for communication with the worker.
            using (var processChannel = HostContext.CreateService<IProcessChannel>())
            {
                processChannel.StartServer(startProcess: (string pipeHandleOut, string pipeHandleIn) =>
                {
                    // Run the worker
                    // Note: this happens on the same process as the test
                    workerTask = worker.RunAsync(
                        pipeIn: pipeHandleOut,
                        pipeOut: pipeHandleIn);
                }, disposeClient: false);

                // Send the job request message to the worker
                var body = JsonUtility.ToString(message);
                using (var csSendJobRequest = new CancellationTokenSource(ChannelTimeout))
                {
                    await processChannel.SendAsync(
                        messageType: MessageType.NewJobRequest,
                        body: body,
                        cancellationToken: csSendJobRequest.Token);
                }

                // wait for worker process or cancellation token been fired.
                var completedTask = await Task.WhenAny(workerTask, Task.Delay(-1, jobRequestCancellationToken));
                if (completedTask == workerTask)
                {
                    int returnCode = await workerTask;

                    TaskResult result = TaskResultUtil.TranslateFromReturnCode(returnCode);
                    return new TestResults
                    {
                        ReturnCode = returnCode,
                        Result = result
                    };
                }
                else
                {
                    return new TestResults
                    {
                        TimedOut = true
                    };
                }
            }
        }

        protected static readonly String JobMessageTemplate = @"
 {
  'mask': [
    {
      'type': 'regex',
      'value': 'accessTokenSecret'
    },
    {
      'type': 'regex',
      'value': 'accessTokenSecret'
    }
  ],
  'steps': [
    {
      'inputs': {
        'repository': 'self'
      },
      'type': 'task',
      'reference': {
        'id': '6d15af64-176c-496d-b583-fd2ae21d4df4',
        'name': 'Checkout',
        'version': '1.0.0'
      },
      'condition': 'true',
      'id': 'af08acd5-c28a-5b03-f5a9-06f9a40627bb',
      'name': 'Checkout',
      'displayName': 'Checkout'
    },
    {
      'inputs': {
        'script': 'echo Hello World!'
      },
      'type': 'task',
      'reference': {
        'id': 'd9bafed4-0b18-4f58-968d-86655b4d2ce9',
        'name': 'CmdLine',
        'version': '2.164.0'
      },
      'id': '9c939e41-62c2-5605-5e05-fc3554afc9f5',
      'name': 'CmdLine',
      'displayName': 'CmdLine'
    }
  ],
  'variables': {
    'system': {
      'value': 'build',
      'isReadOnly': true
    },
    'system.hosttype': {
      'value': 'build',
      'isReadOnly': true
    },
    'system.servertype': {
      'value': 'Hosted',
      'isReadOnly': true
    },
    'system.culture': {
      'value': 'en-US',
      'isReadOnly': true
    },
    'system.collectionId': {
      'value': '297a3210-e711-4ddf-857a-1df14915bb29',
      'isReadOnly': true
    },
    'system.debug': {
      'value': 'true',
      'isReadOnly': true
    },
    'system.collectionUri': {
      'value': 'https://codedev.ms/alpeck/',
      'isReadOnly': true
    },
    'system.teamFoundationCollectionUri': {
      'value': 'https://codedev.ms/alpeck/',
      'isReadOnly': true
    },
    'system.taskDefinitionsUri': {
      'value': 'https://codedev.ms/alpeck/',
      'isReadOnly': true
    },
    'system.pipelineStartTime': {
      'value': '2020-02-10 13:29:58-05:00',
      'isReadOnly': true
    },
    'system.teamProject': {
      'value': 'MyFirstProject',
      'isReadOnly': true
    },
    'system.teamProjectId': {
      'value': '6302cb6f-c9d9-44c2-ae60-84eff8845059',
      'isReadOnly': true
    },
    'system.definitionId': {
      'value': '2',
      'isReadOnly': true
    },
    'build.definitionName': {
      'value': 'MyFirstProject (1)',
      'isReadOnly': true
    },
    'build.definitionVersion': {
      'value': '1',
      'isReadOnly': true
    },
    'build.queuedBy': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.queuedById': {
      'value': '00000002-0000-8888-8000-000000000000',
      'isReadOnly': true
    },
    'build.requestedFor': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.requestedForId': {
      'value': '8546ffd5-88f3-69c1-ad8f-30c41e8ce5ad',
      'isReadOnly': true
    },
    'build.requestedForEmail': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.sourceVersion': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.sourceBranch': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.sourceBranchName': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.reason': {
      'value': 'IndividualCI',
      'isReadOnly': true
    },
    'system.pullRequest.isFork': {
      'value': 'False',
      'isReadOnly': true
    },
    'system.jobParallelismTag': {
      'value': 'Private',
      'isReadOnly': true
    },
    'system.enableAccessToken': {
      'value': 'SecretVariable',
      'isReadOnly': true
    },
    'MSDEPLOY_HTTP_USER_AGENT': {
      'value': 'VSTS_297a3210-e711-4ddf-857a-1df14915bb29_build_2_0',
      'isReadOnly': true
    },
    'AZURE_HTTP_USER_AGENT': {
      'value': 'VSTS_297a3210-e711-4ddf-857a-1df14915bb29_build_2_0',
      'isReadOnly': true
    },
    'build.buildId': {
      'value': '5',
      'isReadOnly': true
    },
    'build.buildUri': {
      'value': 'vstfs:///Build/Build/5',
      'isReadOnly': true
    },
    'build.buildNumber': {
      'value': '20200210.2',
      'isReadOnly': true
    },
    'build.containerId': {
      'value': '12',
      'isReadOnly': true
    },
    'system.isScheduled': {
      'value': 'False',
      'isReadOnly': true
    },
    'system.definitionName': {
      'value': 'MyFirstProject (1)',
      'isReadOnly': true
    },
    'system.planId': {
      'value': 'c7a42561-d84c-4972-b78f-ec97a3b63d53',
      'isReadOnly': true
    },
    'system.timelineId': {
      'value': 'c7a42561-d84c-4972-b78f-ec97a3b63d53',
      'isReadOnly': true
    },
    'system.stageDisplayName': {
      'value': '__default',
      'isReadOnly': true
    },
    'system.stageId': {
      'value': '96ac2280-8cb4-5df5-99de-dd2da759617d',
      'isReadOnly': true
    },
    'system.stageName': {
      'value': '__default',
      'isReadOnly': true
    },
    'system.stageAttempt': {
      'value': '1',
      'isReadOnly': true
    },
    'system.phaseDisplayName': {
      'value': 'Job',
      'isReadOnly': true
    },
    'system.phaseId': {
      'value': '3a3a2a60-14c7-570b-14a4-fa42ad92f52a',
      'isReadOnly': true
    },
    'system.phaseName': {
      'value': 'Job',
      'isReadOnly': true
    },
    'system.phaseAttempt': {
      'value': '1',
      'isReadOnly': true
    },
    'system.jobIdentifier': {
      'value': 'Job.__default',
      'isReadOnly': true
    },
    'system.jobAttempt': {
      'value': '1',
      'isReadOnly': true
    },
    'System.JobPositionInPhase': {
      'value': '1',
      'isReadOnly': true
    },
    'System.TotalJobsInPhase': {
      'value': '1',
      'isReadOnly': true
    },
    'system.jobDisplayName': {
      'value': 'Job',
      'isReadOnly': true
    },
    'system.jobId': {
      'value': '12f1170f-54f2-53f3-20dd-22fc7dff55f9',
      'isReadOnly': true
    },
    'system.jobName': {
      'value': '__default',
      'isReadOnly': true
    },
    'system.accessToken': {
      'value': 'access',
      'isSecret': true
    },
    'agent.retainDefaultEncoding': {
      'value': 'false',
      'isReadOnly': true
    },
    'agent.readOnlyVariables': {
      'value': 'true',
      'isReadOnly': true
    },
    'agent.disablelogplugin.TestResultLogPlugin': {
      'value': 'true',
      'isReadOnly': true
    },
    'agent.disablelogplugin.TestFilePublisherPlugin': {
      'value': 'true',
      'isReadOnly': true
    },
    'build.repository.id': {
      'value': '05bbff1a-ac43-4a40-a1c1-99f4e17e61dd',
      'isReadOnly': true
    },
    'build.repository.name': {
      'value': 'MyFirstProject',
      'isReadOnly': true
    },
    'build.repository.uri': {
      'value': 'https://alpeck@codedev.ms/alpeck/MyFirstProject/_git/MyFirstProject',
      'isReadOnly': true
    },
    'build.sourceVersionAuthor': {
      'value': '[PII]',
      'isReadOnly': true
    },
    'build.sourceVersionMessage': {
      'value': 'Update azure-pipelines-1.yml for Azure Pipelines',
      'isReadOnly': true
    }
  },
  'messageType': 'PipelineAgentJobRequest',
  'plan': {
    'scopeIdentifier': '6302cb6f-c9d9-44c2-ae60-84eff8845059',
    'planType': 'Build',
    'version': 9,
    'planId': 'c7a42561-d84c-4972-b78f-ec97a3b63d53',
    'planGroup': 'Build:6302cb6f-c9d9-44c2-ae60-84eff8845059:5',
    'artifactUri': 'vstfs:///Build/Build/5',
    'artifactLocation': null,
    'definition': {
      '_links': {
        'web': {
          'href': 'https://codedev.ms/alpeck/6302cb6f-c9d9-44c2-ae60-84eff8845059/_build/definition?definitionId=2'
        },
        'self': {
          'href': 'https://codedev.ms/alpeck/6302cb6f-c9d9-44c2-ae60-84eff8845059/_apis/build/Definitions/2'
        }
      },
      'id': 2,
      'name': 'MyFirstProject (1)'
    },
    'owner': {
      '_links': {
        'web': {
          'href': 'https://codedev.ms/alpeck/6302cb6f-c9d9-44c2-ae60-84eff8845059/_build/results?buildId=5'
        },
        'self': {
          'href': 'https://codedev.ms/alpeck/6302cb6f-c9d9-44c2-ae60-84eff8845059/_apis/build/Builds/5'
        }
      },
      'id': 5,
      'name': '20200210.2'
    }
  },
  'timeline': {
    'id': 'c7a42561-d84c-4972-b78f-ec97a3b63d53',
    'changeId': 5,
    'location': null
  },
  'jobId': '12f1170f-54f2-53f3-20dd-22fc7dff55f9',
  'jobDisplayName': 'Job',
  'jobName': '__default',
  'jobContainer': null,
  'requestId': 0,
  'lockedUntil': '0001-01-01T00:00:00',
  'resources': {
    'endpoints': [
      {
        'data': {
          'ServerId': '297a3210-e711-4ddf-857a-1df14915bb29',
          'ServerName': 'alpeck'
        },
        'name': 'SystemVssConnection',
        'url': 'https://codedev.ms/alpeck/',
        'authorization': {
          'parameters': {
            'AccessToken': 'access'
          },
          'scheme': 'OAuth'
        },
        'isShared': false,
        'isReady': true
      }
    ],
    'repositories': [
      {
        'properties': {
          'id': '05bbff1a-ac43-4a40-a1c1-99f4e17e61dd',
          'type': 'Git',
          'version': 'cf64a69d29ae2e01a655956f67ee0332ffb730a3',
          'name': 'MyFirstProject',
          'project': '6302cb6f-c9d9-44c2-ae60-84eff8845059',
          'defaultBranch': 'refs/heads/master',
          'ref': 'refs/heads/master',
          'url': 'https://alpeck@codedev.ms/alpeck/MyFirstProject/_git/MyFirstProject',
          'versionInfo': {
            'author': '[PII]'
          },
          'checkoutOptions': {}
        },
        'alias': 'self',
        'endpoint': {
          'name': 'SystemVssConnection'
        }
      }
    ]
  },
  'workspace': {}
}
        ".Replace("'", "\"");
    }
}
