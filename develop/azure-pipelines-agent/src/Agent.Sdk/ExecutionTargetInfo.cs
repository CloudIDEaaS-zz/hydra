// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Agent.Sdk
{
    public interface ExecutionTargetInfo
    {
        PlatformUtil.OS ExecutionOS { get; }
        string CustomNodePath { get; set; }

        string TranslateContainerPathForImageOS(PlatformUtil.OS runningOs, string path);
        string TranslateToContainerPath(string path);
        string TranslateToHostPath(string path);
    }

    public class HostInfo : ExecutionTargetInfo
    {
        public PlatformUtil.OS ExecutionOS => PlatformUtil.HostOS;
        public string CustomNodePath { get; set; }

        public string TranslateToContainerPath(string path)
        {
            return path;
        }

        public string TranslateToHostPath(string path)
        {
            return path;
        }

        public string TranslateContainerPathForImageOS(PlatformUtil.OS runningOs, string path)
        {
            return path;
        }
    }
}