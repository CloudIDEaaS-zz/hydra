// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Pipelines.Yaml.Contracts
{
    internal sealed class QueueTarget: IPhaseTarget
    {
        internal String ContinueOnError { get; set; }

        internal IList<String> Demands { get; set; }

        internal IDictionary<String, IDictionary<String, String>> Matrix { get; set; }

        internal String Name { get; set; }

        internal String Parallel { get; set; }

        internal String TimeoutInMinutes { get; set; }
    }
}
