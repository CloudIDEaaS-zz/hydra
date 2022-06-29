using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("CE83BA42-3DF9-4362-8D67-D4C3B397628E")]
	internal class FileSystemPane : VsPaneBase
	{
		private FileSystemView innerControl;

		public override IWin32Window Window
		{
			get
			{
				return this.innerControl;
			}
		}

		public FileSystemPane()
		{
			this.innerControl = new FileSystemView();
		}

		public override ViewControlBase GetInnerControl()
		{
			return this.innerControl;
		}

		public override string GetPaneName()
		{
			return "File System";
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