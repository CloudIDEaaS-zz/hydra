using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("9CBAE5C3-39FD-4029-B0ED-550754763FE5")]
	internal class RegistryPane : VsPaneBase
	{
		private RegistryView innerControl;

		public override IWin32Window Window
		{
			get
			{
				return this.innerControl;
			}
		}

		public RegistryPane()
		{
			this.innerControl = new RegistryView();
		}

		public override ViewControlBase GetInnerControl()
		{
			return this.innerControl;
		}

		public override string GetPaneName()
		{
			return "Registry";
		}

		internal override void Initialize(VsWiXProject projectImpl, int id, bool solutionOpened, bool buildStarted)
		{
			base.Initialize(projectImpl, id, solutionOpened, buildStarted);
			this.innerControl.InitializeView(base.ProjectManager.WiXModel, this);
			base.ViewManager.OnToolWindowCreated(base.X, base.Y, base.Width, base.Height);
		}

		public override void OnToolWindowCreated()
		{
			base.OnToolWindowCreated();
		}
	}
}