namespace Microsoft.Build.Shared
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Reflection;

    internal static class AssemblyResources
    {
        private static readonly ResourceManager resources = new ResourceManager("VisualStudioProvider.MSBuild.Strings", Assembly.GetExecutingAssembly());
        private static readonly ResourceManager sharedResources = new ResourceManager("VisualStudioProvider.MSBuild.Strings.shared", Assembly.GetExecutingAssembly());

        internal static string GetString(string name)
        {
            string str = resources.GetString(name, CultureInfo.CurrentUICulture);
            if (str == null)
            {
                str = sharedResources.GetString(name, CultureInfo.CurrentUICulture);
            }
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(str != null, "Missing resource '{0}'", name);
            return str;
        }
    }
}

