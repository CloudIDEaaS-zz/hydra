using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[Guid("40827089-2C93-4DD3-8E3E-EF2980132D05")]
	internal class FileTypesPane : VsPaneBase
	{
		private FileTypesView innerControl;

		public override IWin32Window Window
		{
			get
			{
				return this.innerControl;
			}
		}

		public FileTypesPane()
		{
			this.innerControl = new FileTypesView();
		}

		public override ViewControlBase GetInnerControl()
		{
			return this.innerControl;
		}

		public override string GetPaneName()
		{
			return "File Types";
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