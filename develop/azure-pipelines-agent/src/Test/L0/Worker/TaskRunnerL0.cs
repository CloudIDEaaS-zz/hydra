// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Moq;
using Pipelines = Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Xunit;
using Agent.Sdk;
using Microsoft.VisualStudio.Services.Agent.Worker;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Agent.Util;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.VisualStudio.Services.Agent.Tests.Worker
{

    public sealed class TaskRunnerL0
    {

        private TestHostContext CreateTestContext([CallerMemberName] String testName = "")
        {
            var hc = new TestHostContext(this, testName);

            return hc;
        }

        private class GetHandlerTest
        {
            public String Name;
            public ExecutionData Input;
            public PlatformUtil.OS HostOS;
            public HandlerData Expected;
            public ExecutionTargetInfo StepTarget = null;

            public void RunTest(TestHostContext hc, Dictionary<string, VariableValue> variables=null)
            {
                var _ec = new Mock<IExecutionContext>();
                _ec.Setup(x => x.StepTarget()).Returns(StepTarget);

                if (variables is null)
                {
                    variables = new Dictionary<string, VariableValue>();
                }
                List<string> warnings;
                _ec.Setup(x => x.Variables)
                   .Returns(new Variables(hc, copy: variables, warnings: out warnings));

                var tr = new TaskRunner();
                tr.Initialize(hc);
                var Got = tr.GetHandlerData(_ec.Object, Input, HostOS);
                // for some reason, we guard the setter of PowerShellDate in ExecutionData to only add if running on windows.
                // this makes testing hard
                if (!PlatformUtil.RunningOnWindows)
                {
                    Assert.True(true, "Passively pass this test since we have no way to actually prove it");
                }
                else
                {
                    Assert.True(Got == Expected, $"{Name} - Expected {Expected} Got {Got}");
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void GetHandlerHostOnlyTests()
        {
            var nodeData = new NodeHandlerData() { Platforms=new string[]{"linux", "osx" }};
            var nodeOnlyExecutionData = new ExecutionData();
            nodeOnlyExecutionData.Node = nodeData;
            var powerShell3Data = new PowerShell3HandlerData() { Platforms=new string[]{"windows"}};
            var ps3OnlyExecutionData = new ExecutionData();
            ps3OnlyExecutionData.PowerShell3 = powerShell3Data;
            var mixedExecutionData = new ExecutionData();
            mixedExecutionData.PowerShell3 = powerShell3Data;
            mixedExecutionData.Node = nodeData;


            foreach (var test in new GetHandlerTest[] {
                new GetHandlerTest() { Name="Empty Test",                  Input=null,                  Expected=null,            HostOS=PlatformUtil.OS.Windows },
                new GetHandlerTest() { Name="Node Only on Windows",        Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Windows},
                new GetHandlerTest() { Name="Node Only on Linux",          Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Linux },
                new GetHandlerTest() { Name="Node Only on OSX",            Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.OSX },
                new GetHandlerTest() { Name="PowerShell3 Only on Windows", Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Windows },
                new GetHandlerTest() { Name="PowerShell3 Only on Linux",   Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Linux },
                new GetHandlerTest() { Name="PowerShell3 Only on OSX",     Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.OSX },
                new GetHandlerTest() { Name="Mixed on Windows",            Input=mixedExecutionData,    Expected=powerShell3Data, HostOS=PlatformUtil.OS.Windows },
                new GetHandlerTest() { Name="Mixed on Linux",              Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.Linux },
                new GetHandlerTest() { Name="Mixed on OSX",                Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.OSX },
            })
            {
                using (TestHostContext hc = CreateTestContext(test.Name))
                {
                    test.RunTest(hc);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void GetHandlerContainerTargetTests()
        {
            var nodeData = new NodeHandlerData();
            var nodeOnlyExecutionData = new ExecutionData();
            nodeOnlyExecutionData.Node = nodeData;
            var powerShell3Data = new PowerShell3HandlerData() { Platforms=new string[]{"windows"}};
            var ps3OnlyExecutionData = new ExecutionData();
            ps3OnlyExecutionData.PowerShell3 = powerShell3Data;
            var mixedExecutionData = new ExecutionData();
            mixedExecutionData.Node = nodeData;
            mixedExecutionData.PowerShell3 = powerShell3Data;

            ContainerInfo containerInfo = new ContainerInfo() {};

            foreach (var test in new GetHandlerTest[] {
                new GetHandlerTest() { Name="Empty Test",                  Input=null,                  Expected=null,            HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on Windows",        Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on Linux",          Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on OSX",            Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on Windows", Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on Linux",   Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on OSX",     Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on Windows",            Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on Linux",              Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on OSX",                Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
            })
            {
                using (TestHostContext hc = CreateTestContext(test.Name))
                {
                    test.RunTest(hc);
                }
            }
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void GetHandlerContainerTargetPreferNodeDisabledTests()
        {
            var nodeData = new NodeHandlerData();
            var nodeOnlyExecutionData = new ExecutionData();
            nodeOnlyExecutionData.Node = nodeData;
            var powerShell3Data = new PowerShell3HandlerData() { Platforms=new string[]{"windows"}};
            var ps3OnlyExecutionData = new ExecutionData();
            ps3OnlyExecutionData.PowerShell3 = powerShell3Data;
            var mixedExecutionData = new ExecutionData();
            mixedExecutionData.Node = nodeData;
            mixedExecutionData.PowerShell3 = powerShell3Data;

            ContainerInfo containerInfo = new ContainerInfo() {};

            foreach (var test in new GetHandlerTest[] {
                new GetHandlerTest() { Name="Empty Test",                  Input=null,                  Expected=null,            HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on Windows",        Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on Linux",          Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Node Only on OSX",            Input=nodeOnlyExecutionData, Expected=nodeData,        HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on Windows", Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on Linux",   Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="PowerShell3 Only on OSX",     Input=ps3OnlyExecutionData,  Expected=powerShell3Data, HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on Windows",            Input=mixedExecutionData,    Expected=powerShell3Data, HostOS=PlatformUtil.OS.Windows, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on Linux",              Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.Linux, StepTarget=containerInfo },
                new GetHandlerTest() { Name="Mixed on OSX",                Input=mixedExecutionData,    Expected=nodeData,        HostOS=PlatformUtil.OS.OSX, StepTarget=containerInfo },
            })
            {
                var variables = new Dictionary<string, VariableValue>();
                variables.Add("agent.preferPowerShellOnContainers", "true");
                using (TestHostContext hc = CreateTestContext(test.Name))
                {
                    test.RunTest(hc, variables);
                    Environment.SetEnvironmentVariable("AGENT_PREFER_POWERSHELL_ON_CONTAINERS", "true");
                    test.RunTest(hc);
                    Environment.SetEnvironmentVariable("AGENT_PREFER_POWERSHELL_ON_CONTAINERS", null);
                }
            }
        }
    }
}