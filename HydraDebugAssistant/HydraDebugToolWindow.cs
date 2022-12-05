using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Utils;

namespace HydraDebugAssistant
{
    [Guid("135e71cb-e216-44c3-b5d2-57f165a8f081")]
    public class HydraDebugToolWindow : ToolWindowPane
    {
        private HydraDebugToolWindowControl contentWindow;

        public HydraDebugToolWindow() : base(null)
        {
            DTE dte;
            IVsStatusbar statusbar;
            IOleComponentManager oleComponentManager;
            IVsUIHierarchy vsUIHierarchy;
            IMenuCommandService menuCommandService;

            ThreadHelper.ThrowIfNotOnUIThread();
            dte = (DTE)ServiceProvider.GlobalProvider.GetService(typeof(EnvDTE.DTE));
            statusbar = (IVsStatusbar)ServiceProvider.GlobalProvider.GetService(typeof(IVsStatusbar));
            oleComponentManager = (IOleComponentManager)ServiceProvider.GlobalProvider.GetService(typeof(SOleComponentManager));
            vsUIHierarchy = (IVsUIHierarchy)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
            menuCommandService = (IMenuCommandService)ServiceProvider.GlobalProvider.GetService(typeof(IMenuCommandService));

            this.Caption = "Hydra Debug";
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
            {
                contentWindow = new HydraDebugToolWindowControl();

                contentWindow.DTE = dte;
                contentWindow.Statusbar = statusbar;
                contentWindow.ComponentManager = oleComponentManager;
                contentWindow.VSUIHierarchy = vsUIHierarchy;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            contentWindow.WindowFrame = (IVsWindowFrame)this.Frame;
            contentWindow.SetPane(this.GetPropertyValue<IntPtr>("ParentHandle"));
        }

        public override IWin32Window Window
        {
            get
            {
                return contentWindow;
            }
        }

        internal void Showing()
        {
            if (contentWindow != null)
            {
                contentWindow.Showing();

                contentWindow.WindowFrame = (IVsWindowFrame)this.Frame;
                contentWindow.SetPane(this.GetPropertyValue<IntPtr>("ParentHandle"));
            }
        }
    }
}
