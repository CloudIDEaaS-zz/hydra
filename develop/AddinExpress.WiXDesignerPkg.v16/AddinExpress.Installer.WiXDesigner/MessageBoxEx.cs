using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class MessageBoxEx : IDisposable
	{
		private MessageBoxExForm _msgBox = new MessageBoxExForm();

		private string _name;

		private bool disposed;

		private bool _useSavedResponse = true;

		private IntPtr mainWindowHandle = IntPtr.Zero;

		public bool AllowSaveResponse
		{
			get
			{
				return this._msgBox.AllowSaveResponse;
			}
			set
			{
				this._msgBox.AllowSaveResponse = value;
			}
		}

		public System.Drawing.Icon AppIcon
		{
			set
			{
				this._msgBox.Icon = value;
			}
		}

		public string Caption
		{
			set
			{
				this._msgBox.Caption = value;
			}
		}

		public System.Drawing.Icon CustomIcon
		{
			set
			{
				this._msgBox.CustomIcon = value;
			}
		}

		public System.Drawing.Font Font
		{
			set
			{
				this._msgBox.Font = value;
			}
		}

		public MessageBoxExIcon Icon
		{
			set
			{
				this._msgBox.StandardIcon = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), value.ToString());
			}
		}

		public IntPtr MainWindowHandle
		{
			get
			{
				return this.mainWindowHandle;
			}
			set
			{
				this.mainWindowHandle = value;
			}
		}

		internal string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		internal MessageBoxExForm NativeForm
		{
			get
			{
				return this._msgBox;
			}
		}

		public bool PlayAlsertSound
		{
			get
			{
				return this._msgBox.PlayAlertSound;
			}
			set
			{
				this._msgBox.PlayAlertSound = value;
			}
		}

		public bool SaveResponse
		{
			get
			{
				return this._msgBox.SaveResponse;
			}
		}

		public string SaveResponseText
		{
			set
			{
				this._msgBox.SaveResponseText = value;
			}
		}

		public bool ShowAppIcon
		{
			set
			{
				this._msgBox.ShowIcon = value;
			}
		}

		public FormStartPosition StartPosition
		{
			get
			{
				return this._msgBox.StartPosition;
			}
			set
			{
				this._msgBox.StartPosition = value;
			}
		}

		public string Text
		{
			set
			{
				this._msgBox.Message = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this._msgBox.Timeout;
			}
			set
			{
				this._msgBox.Timeout = value;
			}
		}

		public AddinExpress.Installer.WiXDesigner.TimeoutResult TimeoutResult
		{
			get
			{
				return this._msgBox.TimeoutResult;
			}
			set
			{
				this._msgBox.TimeoutResult = value;
			}
		}

		public bool UseSavedResponse
		{
			get
			{
				return this._useSavedResponse;
			}
			set
			{
				this._useSavedResponse = value;
			}
		}

		internal MessageBoxEx()
		{
		}

		public void AddButton(MessageBoxExButton button)
		{
			if (button == null)
			{
				throw new ArgumentNullException("button", "A null button cannot be added");
			}
			this._msgBox.Buttons.Add(button);
			if (button.IsCancelButton)
			{
				this._msgBox.CustomCancelButton = button;
			}
		}

		public MessageBoxExButton AddButton(string text, string val, bool isDefault)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text", "Text of a button cannot be null");
			}
			if (val == null)
			{
				throw new ArgumentNullException("val", "Value of a button cannot be null");
			}
			MessageBoxExButton messageBoxExButton = new MessageBoxExButton()
			{
				IsDefaultButton = isDefault,
				Text = text,
				Value = val
			};
			this.AddButton(messageBoxExButton);
			return messageBoxExButton;
		}

		public MessageBoxExButton AddButton(MessageBoxExButtons button, bool isDefault)
		{
			string localizedString = MessageBoxExManager.GetLocalizedString(button.ToString()) ?? button.ToString();
			string str = button.ToString();
			MessageBoxExButton messageBoxExButton = new MessageBoxExButton()
			{
				IsDefaultButton = isDefault,
				Text = localizedString,
				Value = str
			};
			if (button == MessageBoxExButtons.Cancel)
			{
				messageBoxExButton.IsCancelButton = true;
			}
			this.AddButton(messageBoxExButton);
			return messageBoxExButton;
		}

		public void AddButtons(MessageBoxButtons buttons)
		{
			switch (buttons)
			{
				case MessageBoxButtons.OK:
				{
					this.AddButton(MessageBoxExButtons.Ok, false);
					return;
				}
				case MessageBoxButtons.OKCancel:
				{
					this.AddButton(MessageBoxExButtons.Ok, false);
					this.AddButton(MessageBoxExButtons.Cancel, false);
					return;
				}
				case MessageBoxButtons.AbortRetryIgnore:
				{
					this.AddButton(MessageBoxExButtons.Abort, false);
					this.AddButton(MessageBoxExButtons.Retry, false);
					this.AddButton(MessageBoxExButtons.Ignore, false);
					return;
				}
				case MessageBoxButtons.YesNoCancel:
				{
					this.AddButton(MessageBoxExButtons.Yes, false);
					this.AddButton(MessageBoxExButtons.No, false);
					this.AddButton(MessageBoxExButtons.Cancel, false);
					return;
				}
				case MessageBoxButtons.YesNo:
				{
					this.AddButton(MessageBoxExButtons.Yes, false);
					this.AddButton(MessageBoxExButtons.No, false);
					return;
				}
				case MessageBoxButtons.RetryCancel:
				{
					this.AddButton(MessageBoxExButtons.Retry, false);
					this.AddButton(MessageBoxExButtons.Cancel, false);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this._msgBox != null)
				{
					this._msgBox.Dispose();
				}
			}
		}

		public string Show()
		{
			return this.Show(null);
		}

		public string Show(IWin32Window owner)
		{
			MessageBoxExForm.RECT rECT;
			if (this._useSavedResponse && this.Name != null)
			{
				string savedResponse = MessageBoxExManager.GetSavedResponse(this);
				if (savedResponse != null)
				{
					return savedResponse;
				}
			}
			if (this.StartPosition == FormStartPosition.Manual && this.MainWindowHandle != IntPtr.Zero)
			{
				MessageBoxExForm.GetWindowRect(this.MainWindowHandle, out rECT);
				this._msgBox.Location = new Point(rECT.Left + (rECT.Right - rECT.Left) / 2 - this._msgBox.Width / 2, rECT.Top + (rECT.Bottom - rECT.Top) / 2 - this._msgBox.Height / 2);
			}
			if (owner != null)
			{
				this._msgBox.ShowDialog(owner);
			}
			else
			{
				this._msgBox.ShowDialog();
			}
			if (this.Name == null)
			{
				this.Dispose();
			}
			else if (!this._msgBox.AllowSaveResponse || !this._msgBox.SaveResponse)
			{
				MessageBoxExManager.ResetSavedResponse(this.Name);
			}
			else
			{
				MessageBoxExManager.SetSavedResponse(this, this._msgBox.Result);
			}
			return this._msgBox.Result;
		}
	}
}