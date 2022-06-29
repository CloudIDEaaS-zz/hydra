using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("FA672093-A6CA-4C19-A275-928E603407F6")]
	internal class CustomActionsPane : VsPaneBase
	{
		private CustomActionsView innerControl;

		public override IWin32Window Window
		{
			get
			{
				return this.innerControl;
			}
		}

		public CustomActionsPane()
		{
			this.innerControl = new CustomActionsView();
		}

		public override ViewControlBase GetInnerControl()
		{
			return this.innerControl;
		}

		public override string GetPaneName()
		{
			return "Custom Actions";
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