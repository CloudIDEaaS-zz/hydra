// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.VisualStudio.Services.Agent.Worker.Build
{
    public abstract class TrackingConfigBase
    {
        protected static readonly string BuildSystem = "build";

        public string CollectionId { get; set; }

        public string DefinitionId { get; set; }

        public string HashKey { get; set; }

        public string RepositoryUrl { get; set; }

        public string System { get; set; }
    }
}