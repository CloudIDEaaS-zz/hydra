using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Hydra.Extension
{
    [HydraPackageRegistration()]
    [Guid(HydraExtensionPackage.PackageGuidString)]
    public sealed class HydraExtensionPackage : AsyncPackage
    {
        public const string PackageGuidString = "283dac65-5c15-4b1f-8310-c09a7847cfdb";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}

