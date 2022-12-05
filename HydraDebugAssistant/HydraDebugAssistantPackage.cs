using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace HydraDebugAssistant
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(HydraDebugAssistantPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(HydraDebugToolWindow))]
    public sealed class HydraDebugAssistantPackage : AsyncPackage
    {
        public const string PackageGuidString = "1047e5f2-88eb-4933-9da5-906c1c15f516";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await HydraDebugToolWindowCommand.InitializeAsync(this);
            await HydraDebugAttachCommand.InitializeAsync(this);
        }
    }
}
