namespace Microsoft.Build.Shared
{
    using System;

    internal static class XMakeAttributes
    {
        internal const string afterTargets = "AfterTargets";
        internal const string assemblyFile = "AssemblyFile";
        internal const string assemblyName = "AssemblyName";
        internal const string beforeTargets = "BeforeTargets";
        internal const string condition = "Condition";
        internal const string continueOnError = "ContinueOnError";
        internal const string defaultTargets = "DefaultTargets";
        internal const string defaultXmlNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
        internal const string dependsOnTargets = "DependsOnTargets";
        internal const string evaluate = "Evaluate";
        internal const string exclude = "Exclude";
        internal const string executeTargets = "ExecuteTargets";
        internal const string include = "Include";
        internal const string initialTargets = "InitialTargets";
        internal const string inputs = "Inputs";
        internal const string itemName = "ItemName";
        internal const string keepDuplicateOutputs = "KeepDuplicateOutputs";
        internal const string label = "Label";
        internal const string msbuildVersion = "MSBuildVersion";
        internal const string name = "Name";
        internal const string output = "Output";
        internal const string outputs = "Outputs";
        internal const string parameterType = "ParameterType";
        internal const string project = "Project";
        internal const string propertyName = "PropertyName";
        internal const string remove = "Remove";
        internal const string required = "Required";
        internal const string requiredPlatform = "RequiredPlatform";
        internal const string requiredRuntime = "RequiredRuntime";
        internal const string returns = "Returns";
        internal const string taskFactory = "TaskFactory";
        internal const string taskName = "TaskName";
        internal const string taskParameter = "TaskParameter";
        internal const string toolsVersion = "ToolsVersion";
        internal const string xmlns = "xmlns";

        internal static bool IsBadlyCasedSpecialTaskAttribute(string attribute)
        {
            if (IsSpecialTaskAttribute(attribute))
            {
                return false;
            }
            if (string.Compare(attribute, "Condition", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return (string.Compare(attribute, "ContinueOnError", StringComparison.OrdinalIgnoreCase) == 0);
            }
            return true;
        }

        internal static bool IsNonBatchingTargetAttribute(string attribute)
        {
            if ((!(attribute == "Name") && !(attribute == "Condition")) && (!(attribute == "DependsOnTargets") && !(attribute == "BeforeTargets")))
            {
                return (attribute == "AfterTargets");
            }
            return true;
        }

        internal static bool IsSpecialTaskAttribute(string attribute)
        {
            if (!(attribute == "Condition") && !(attribute == "ContinueOnError"))
            {
                return (attribute == "xmlns");
            }
            return true;
        }
    }
}

