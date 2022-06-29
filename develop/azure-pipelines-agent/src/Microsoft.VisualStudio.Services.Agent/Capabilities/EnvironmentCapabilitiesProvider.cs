// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Agent.Sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Agent.Capabilities
{
    public sealed class EnvironmentCapabilitiesProvider : AgentService, ICapabilitiesProvider
    {
        // Ignore env vars specified in the 'VSO_AGENT_IGNORE' env var.
        private const string CustomIgnore = "VSO_AGENT_IGNORE";

        private const int IgnoreValueLength = 1024;

        private static readonly string[] s_wellKnownIgnored = new[]
        {
            "comp_wordbreaks",
            "ls_colors",
            "TERM",
            "TERM_PROGRAM",
            "TERM_PROGRAM_VERSION",
            "SHLVL",
            // the agent doesn't set this, but we have seen instances in the wild where
            // a customer has pre-configured this somehow. it's almost certain to contain
            // secrets that shouldn't be exposed as capabilities, so for defense in depth,
            // add it to the exclude list.
            "SYSTEM_ACCESSTOKEN",
        };

        public Type ExtensionType => typeof(ICapabilitiesProvider);

        public int Order => 1; // Process first so other providers can override.

        public Task<List<Capability>> GetCapabilitiesAsync(AgentSettings settings, CancellationToken cancellationToken)
        {
            Trace.Entering();
            var capabilities = new List<Capability>();

            // Initialize the ignored hash set.
            var comparer = StringComparer.Ordinal;
            if (PlatformUtil.RunningOnWindows)
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }
            var ignored = new HashSet<string>(s_wellKnownIgnored, comparer);

            // Also ignore env vars specified by the 'VSO_AGENT_IGNORE' env var.
            IDictionary variables = Environment.GetEnvironmentVariables();
            if (variables.Contains(CustomIgnore))
            {
                IEnumerable<string> additionalIgnored =
                    (variables[CustomIgnore] as string ?? string.Empty)
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));
                foreach (string ignore in additionalIgnored)
                {
                    Trace.Info($"Ignore: '{ignore}'");
                    ignored.Add(ignore); // Handles duplicates gracefully.
                }
            }

            // Get filtered env vars.
            IEnumerable<string> names =
                variables.Keys
                .Cast<string>()
                .Where(x => !string.IsNullOrEmpty(x))
                .OrderBy(x => x.ToUpperInvariant());
            foreach (string name in names)
            {
                string value = variables[name] as string ?? string.Empty;
                if (ignored.Contains(name) || value.Length >= IgnoreValueLength)
                {
                    Trace.Info($"Skipping: '{name}'");
                    continue;
                }

                Trace.Info($"Adding '{name}': '{value}'");
                capabilities.Add(new Capability(name, value));
            }

            return Task.FromResult(capabilities);
        }
    }
}
