using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("7ADBC362-BCD9-4A65-84AC-2EEAC245E531")]
	internal class UserInterfacePane : VsPaneBase
	{
		private UserInterfaceView innerControl;

		public override IWin32Window Window
		{
			get
			{
				return this.innerControl;
			}
		}

		public UserInterfacePane()
		{
			this.innerControl = new UserInterfaceView();
		}

		public override ViewControlBase GetInnerControl()
		{
			return this.innerControl;
		}

		public override string GetPaneName()
		{
			return "User Interface";
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