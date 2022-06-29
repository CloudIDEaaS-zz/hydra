using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSUserInterface : List<VSDialogBase>, IWin32Window
	{
		private WiXProjectParser _project;

		public List<VSDialogBase> AdministrativeInstall
		{
			get
			{
				return base.FindAll((VSDialogBase e) => e.DialogScope == DialogScope.AdministrativeInstall);
			}
		}

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		public List<VSDialogBase> UserInstall
		{
			get
			{
				return base.FindAll((VSDialogBase e) => e.DialogScope == DialogScope.UserInstall);
			}
		}

		public VSUserInterface(WiXProjectParser project)
		{
			this._project = project;
		}

		internal List<VSDialogBase> Add(DialogScope scope, DialogStage stage)
		{
			List<VSDialogBase> vSDialogBases = new List<VSDialogBase>();
			if (scope == DialogScope.UserInstall)
			{
				foreach (VSDialogBase userInstall in this.UserInstall)
				{
					vSDialogBases.Add(userInstall);
				}
			}
			else if (scope == DialogScope.AdministrativeInstall)
			{
				foreach (VSDialogBase administrativeInstall in this.AdministrativeInstall)
				{
					vSDialogBases.Add(administrativeInstall);
				}
			}
			FormSelectDialog formSelectDialog = new FormSelectDialog();
			formSelectDialog.Initialize(vSDialogBases, scope, stage, this.Project.IsWebSetup);
			if (formSelectDialog.ShowDialog(this) == DialogResult.OK)
			{
				if (formSelectDialog.SelectedDialogs.Count > 0)
				{
					return VSDialogBase.CreateDialog(formSelectDialog.SelectedDialogs, this, scope, stage);
				}
				if (formSelectDialog.SelectedDialog != DialogType.Undefined)
				{
					return VSDialogBase.CreateDialog(new List<DialogType>()
					{
						formSelectDialog.SelectedDialog
					}, this, scope, stage);
				}
			}
			return null;
		}

		public void Clean()
		{
			base.Clear();
		}

		internal bool Move(VSDialogBase sourceDialog, VSDialogBase prevDialog, VSDialogBase nextDialog)
		{
			bool flag;
			if (sourceDialog.DeleteFromUI())
			{
				base.Remove(sourceDialog);
				try
				{
					if (!sourceDialog.InsertToUI(prevDialog, nextDialog))
					{
						return false;
					}
					else
					{
						if (nextDialog != null)
						{
							base.Insert(base.IndexOf(nextDialog), sourceDialog);
						}
						else if (prevDialog == null)
						{
							base.Add(sourceDialog);
						}
						else
						{
							int num = base.IndexOf(prevDialog);
							if (num >= base.Count - 1)
							{
								base.Add(sourceDialog);
							}
							else
							{
								base.Insert(num + 1, sourceDialog);
							}
						}
						flag = true;
					}
				}
				catch (Exception exception)
				{
					base.Add(sourceDialog);
					return false;
				}
				return flag;
			}
			return false;
		}
	}
}