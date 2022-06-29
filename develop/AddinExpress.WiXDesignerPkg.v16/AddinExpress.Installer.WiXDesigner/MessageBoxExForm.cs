using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class MessageBoxExForm : Form
	{
		private const int LEFT_PADDING = 24;

		private const int RIGHT_PADDING = 8;

		private const int TOP_PADDING = 24;

		private const int BOTTOM_PADDING = 12;

		private const int BUTTON_LEFT_PADDING = 4;

		private const int BUTTON_RIGHT_PADDING = 4;

		private const int BUTTON_TOP_PADDING = 4;

		private const int BUTTON_BOTTOM_PADDING = 4;

		private const int MIN_BUTTON_HEIGHT = 23;

		private const int MIN_BUTTON_WIDTH = 74;

		private const int ITEM_PADDING = 10;

		private const int ICON_MESSAGE_PADDING = 15;

		private const int BUTTON_PADDING = 5;

		private const int CHECKBOX_WIDTH = 20;

		private const int IMAGE_INDEX_EXCLAMATION = 0;

		private const int IMAGE_INDEX_QUESTION = 1;

		private const int IMAGE_INDEX_STOP = 2;

		private const int IMAGE_INDEX_INFORMATION = 3;

		private IContainer components;

		private CheckBox chbSaveResponse;

		private ImageList imageListIcons;

		private ToolTip buttonToolTip;

		private ArrayList _buttons = new ArrayList();

		private bool _allowSaveResponse;

		private bool _playAlert = true;

		private MessageBoxExButton _cancelButton;

		private Button _defaultButtonControl;

		private Rectangle _bodyArea;

		private int _maxLayoutWidth;

		private int _maxLayoutHeight;

		private int _maxWidth;

		private int _maxHeight;

		private bool _allowCancel = true;

		private string _result;

		private MessageBoxIcon _standardIcon;

		private System.Drawing.Icon _iconImage;

		private Timer timerTimeout;

		private int _timeout;

		private AddinExpress.Installer.WiXDesigner.TimeoutResult _timeoutResult;

		private Panel panelIcon;

		private RichTextBox rtbMessage;

		private Hashtable _buttonControlsTable = new Hashtable();

		private const int SPI_GETNONCLIENTMETRICS = 41;

		private const int LF_FACESIZE = 32;

		private const int SC_CLOSE = 61536;

		private const int MF_BYCOMMAND = 0;

		private const int MF_GRAYED = 1;

		private const int MF_ENABLED = 0;

		public bool AllowSaveResponse
		{
			get
			{
				return this._allowSaveResponse;
			}
			set
			{
				this._allowSaveResponse = value;
			}
		}

		public ArrayList Buttons
		{
			get
			{
				return this._buttons;
			}
		}

		public string Caption
		{
			set
			{
				this.Text = value;
			}
		}

		public MessageBoxExButton CustomCancelButton
		{
			set
			{
				this._cancelButton = value;
			}
		}

		public System.Drawing.Font CustomFont
		{
			set
			{
				this.Font = value;
			}
		}

		public System.Drawing.Icon CustomIcon
		{
			set
			{
				this._standardIcon = MessageBoxIcon.None;
				this._iconImage = value;
			}
		}

		public string Message
		{
			set
			{
				this.rtbMessage.Text = value;
			}
		}

		public bool PlayAlertSound
		{
			get
			{
				return this._playAlert;
			}
			set
			{
				this._playAlert = value;
			}
		}

		public string Result
		{
			get
			{
				return this._result;
			}
		}

		public bool SaveResponse
		{
			get
			{
				return this.chbSaveResponse.Checked;
			}
		}

		public string SaveResponseText
		{
			set
			{
				this.chbSaveResponse.Text = value;
			}
		}

		public MessageBoxIcon StandardIcon
		{
			set
			{
				this.SetStandardIcon(value);
			}
		}

		public int Timeout
		{
			get
			{
				return this._timeout;
			}
			set
			{
				this._timeout = value;
			}
		}

		public AddinExpress.Installer.WiXDesigner.TimeoutResult TimeoutResult
		{
			get
			{
				return this._timeoutResult;
			}
			set
			{
				this._timeoutResult = value;
			}
		}

		public MessageBoxExForm()
		{
			this.InitializeComponent();
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = false;
			Rectangle workingArea = SystemInformation.WorkingArea;
			this._maxWidth = (int)((double)workingArea.Width * 0.6);
			workingArea = SystemInformation.WorkingArea;
			this._maxHeight = (int)((double)workingArea.Height * 0.9);
		}

		private void AddOkButtonIfNoButtonsPresent()
		{
			if (this._buttons.Count == 0)
			{
				MessageBoxExButton messageBoxExButton = new MessageBoxExButton()
				{
					Text = MessageBoxExButtons.Ok.ToString(),
					Value = MessageBoxExButtons.Ok.ToString()
				};
				this._buttons.Add(messageBoxExButton);
			}
		}

		private void CalcBodyArea()
		{
			System.Drawing.Size buttonSize = this.GetButtonSize();
			this._bodyArea.X = 0;
			this._bodyArea.Y = 0;
			System.Drawing.Size clientSize = base.ClientSize;
			this._bodyArea.Width = clientSize.Width;
			clientSize = base.ClientSize;
			this._bodyArea.Height = clientSize.Height - (12 + buttonSize.Height + 12);
		}

		private void CenterForm()
		{
			Rectangle workingArea = SystemInformation.WorkingArea;
			int width = (workingArea.Width - base.Width) / 2;
			workingArea = SystemInformation.WorkingArea;
			int height = (workingArea.Height - base.Height) / 2;
			base.Location = new Point(width, height);
		}

		private Button CreateButton(MessageBoxExButton button, System.Drawing.Size size, Point location)
		{
			Button value = new Button()
			{
				Size = size,
				Text = button.Text,
				TextAlign = ContentAlignment.MiddleCenter,
				FlatStyle = FlatStyle.System
			};
			if (button.HelpText != null && button.HelpText.Trim().Length != 0)
			{
				this.buttonToolTip.SetToolTip(value, button.HelpText);
			}
			value.Location = location;
			value.Click += new EventHandler(this.OnButtonClicked);
			value.Tag = button.Value;
			return value;
		}

		private void DisableCloseButton(Form form)
		{
			try
			{
				MessageBoxExForm.EnableMenuItem(MessageBoxExForm.GetSystemMenu(form.Handle, false), 61536, 1);
			}
			catch (Exception exception)
			{
			}
		}

		private void DisableCloseIfMultipleButtonsAndNoCancelButton()
		{
			if (this._buttons.Count <= 1)
			{
				if (this._buttons.Count == 1)
				{
					this._cancelButton = this._buttons[0] as MessageBoxExButton;
					return;
				}
				this._allowCancel = false;
			}
			else
			{
				if (this._cancelButton != null)
				{
					return;
				}
				foreach (MessageBoxExButton _button in this._buttons)
				{
					if (!(_button.Text == MessageBoxExButtons.Cancel.ToString()) || !(_button.Value == MessageBoxExButtons.Cancel.ToString()))
					{
						continue;
					}
					this._cancelButton = _button;
					return;
				}
				this.DisableCloseButton(this);
				this._allowCancel = false;
				return;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

		private Button GetButton(MessageBoxExButton button, System.Drawing.Size size, Point location)
		{
			Button item = null;
			if (!this._buttonControlsTable.ContainsKey(button))
			{
				item = this.CreateButton(button, size, location);
				this._buttonControlsTable[button] = item;
				base.Controls.Add(item);
			}
			else
			{
				item = this._buttonControlsTable[button] as Button;
				item.Size = size;
				item.Location = location;
			}
			return item;
		}

		private Button GetButton(MessageBoxExButton button)
		{
			Button item = null;
			if (this._buttonControlsTable.ContainsKey(button))
			{
				item = this._buttonControlsTable[button] as Button;
			}
			return item;
		}

		private System.Drawing.Size GetButtonSize()
		{
			string longestButtonText = this.GetLongestButtonText();
			System.Drawing.Size size = this.MeasureString(longestButtonText, this._maxLayoutWidth);
			System.Drawing.Size size1 = new System.Drawing.Size(size.Width + 4 + 4, size.Height + 4 + 4);
			if (size1.Width < 74)
			{
				size1.Width = 74;
			}
			if (size1.Height < 23)
			{
				size1.Height = 23;
			}
			return size1;
		}

		private System.Drawing.Font GetCaptionFont()
		{
			System.Drawing.Font font;
			MessageBoxExForm.NONCLIENTMETRICS nONCLIENTMETRIC = new MessageBoxExForm.NONCLIENTMETRICS()
			{
				cbSize = Marshal.SizeOf(typeof(MessageBoxExForm.NONCLIENTMETRICS))
			};
			try
			{
				if (!MessageBoxExForm.SystemParametersInfo(41, nONCLIENTMETRIC.cbSize, ref nONCLIENTMETRIC, 0))
				{
					Marshal.GetLastWin32Error();
					font = null;
				}
				else
				{
					font = System.Drawing.Font.FromLogFont(nONCLIENTMETRIC.lfCaptionFont);
				}
			}
			catch (Exception exception)
			{
				return null;
			}
			return font;
		}

		private System.Drawing.Size GetCaptionSize()
		{
			System.Drawing.Font captionFont = this.GetCaptionFont() ?? new System.Drawing.Font("Tahoma", 11f);
			int num = this._maxWidth;
			System.Drawing.Size captionButtonSize = SystemInformation.CaptionButtonSize;
			int width = num - captionButtonSize.Width;
			captionButtonSize = SystemInformation.Border3DSize;
			int width1 = width - captionButtonSize.Width * 2;
			System.Drawing.Size size = this.MeasureString(this.Text, width1, captionFont);
			int num1 = size.Width;
			int width2 = SystemInformation.CaptionButtonSize.Width;
			captionButtonSize = SystemInformation.Border3DSize;
			size.Width = num1 + width2 + captionButtonSize.Width * 2;
			return size;
		}

		private string GetLongestButtonText()
		{
			int length = 0;
			string text = null;
			foreach (MessageBoxExButton _button in this._buttons)
			{
				if (_button.Text == null || _button.Text.Length <= length)
				{
					continue;
				}
				length = _button.Text.Length;
				text = _button.Text;
			}
			return text;
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		private int GetWidthOfAllButtons()
		{
			System.Drawing.Size buttonSize = this.GetButtonSize();
			return buttonSize.Width * this._buttons.Count + 5 * (this._buttons.Count - 1);
		}

		[DllImport("user32", CharSet=CharSet.None, ExactSpelling=false)]
		internal static extern int GetWindowRect(IntPtr hWnd, out MessageBoxExForm.RECT lpRect);

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MessageBoxExForm));
			this.panelIcon = new Panel();
			this.chbSaveResponse = new CheckBox();
			this.imageListIcons = new ImageList(this.components);
			this.buttonToolTip = new ToolTip(this.components);
			this.rtbMessage = new RichTextBox();
			base.SuspendLayout();
			this.panelIcon.BackColor = Color.Transparent;
			this.panelIcon.Location = new Point(10, 8);
			this.panelIcon.Name = "panelIcon";
			this.panelIcon.Size = new System.Drawing.Size(38, 32);
			this.panelIcon.TabIndex = 2;
			this.panelIcon.Visible = false;
			this.chbSaveResponse.FlatStyle = FlatStyle.System;
			this.chbSaveResponse.Location = new Point(67, 56);
			this.chbSaveResponse.Name = "chbSaveResponse";
			this.chbSaveResponse.Size = new System.Drawing.Size(125, 15);
			this.chbSaveResponse.TabIndex = 1;
			this.imageListIcons.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("imageListIcons.ImageStream");
			this.imageListIcons.TransparentColor = Color.Transparent;
			this.imageListIcons.Images.SetKeyName(0, "Warning.bmp");
			this.imageListIcons.Images.SetKeyName(1, "Question.bmp");
			this.imageListIcons.Images.SetKeyName(2, "Error.bmp");
			this.imageListIcons.Images.SetKeyName(3, "Info.bmp");
			this.rtbMessage.BackColor = SystemColors.Window;
			this.rtbMessage.BorderStyle = BorderStyle.None;
			this.rtbMessage.Location = new Point(170, 8);
			this.rtbMessage.Name = "rtbMessage";
			this.rtbMessage.ReadOnly = true;
			this.rtbMessage.Size = new System.Drawing.Size(120, 48);
			this.rtbMessage.TabIndex = 0;
			this.rtbMessage.TabStop = false;
			this.rtbMessage.Text = "";
			this.rtbMessage.Visible = false;
			this.rtbMessage.LinkClicked += new LinkClickedEventHandler(this.rtbMessage_LinkClicked);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			base.ClientSize = new System.Drawing.Size(322, 224);
			base.Controls.Add(this.rtbMessage);
			base.Controls.Add(this.chbSaveResponse);
			base.Controls.Add(this.panelIcon);
			this.Font = new System.Drawing.Font("Tahoma", 7.764706f, FontStyle.Regular, GraphicsUnit.Point, 204);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "MessageBoxExForm";
			base.ShowIcon = false;
			base.StartPosition = FormStartPosition.CenterParent;
			base.ResumeLayout(false);
		}

		private void LayoutControls()
		{
			this.panelIcon.Location = new Point(24, 24);
			this.rtbMessage.Location = new Point(24 + this.panelIcon.Width + 15 * (this.panelIcon.Width == 0 ? 0 : 1), 24);
			System.Drawing.Size buttonSize = this.GetButtonSize();
			CheckBox point = this.chbSaveResponse;
			int width = 24 + this.panelIcon.Width / 2;
			System.Drawing.Size clientSize = base.ClientSize;
			point.Location = new Point(width, clientSize.Height - 12 - buttonSize.Height);
			int widthOfAllButtons = this.GetWidthOfAllButtons();
			clientSize = base.ClientSize;
			int num = (clientSize.Width - widthOfAllButtons) / 2;
			if (Environment.OSVersion.Version.Major >= 6 || this.AllowSaveResponse)
			{
				clientSize = base.ClientSize;
				num = clientSize.Width - widthOfAllButtons - 8;
			}
			clientSize = base.ClientSize;
			int height = clientSize.Height - 12 - buttonSize.Height;
			Point x = new Point(num, height);
			bool flag = false;
			foreach (MessageBoxExButton _button in this._buttons)
			{
				Button button = this.GetButton(_button, buttonSize, x);
				if (!flag || _button.IsDefaultButton)
				{
					this._defaultButtonControl = button;
					flag = true;
				}
				x.X = x.X + buttonSize.Width + 5;
			}
		}

		private System.Drawing.Size MeasureString(string str, int maxWidth, System.Drawing.Font font)
		{
			System.Drawing.Size size;
			Graphics graphic = null;
			try
			{
				graphic = Graphics.FromHwnd(base.Handle);
				SizeF sizeF = graphic.MeasureString(str, font, maxWidth);
				size = new System.Drawing.Size((int)Math.Ceiling((double)sizeF.Width), (int)Math.Ceiling((double)sizeF.Height));
			}
			finally
			{
				graphic.Dispose();
			}
			return size;
		}

		private System.Drawing.Size MeasureString(string str, int maxWidth)
		{
			return this.MeasureString(str, maxWidth, this.Font);
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern bool MessageBeep(uint type);

		private void OnButtonClicked(object sender, EventArgs e)
		{
			Button button = sender as Button;
			if (button == null || button.Tag == null)
			{
				return;
			}
			this.SetResultAndClose(button.Tag as string);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (this._result == null)
			{
				if (!this._allowCancel)
				{
					e.Cancel = true;
					return;
				}
				this._result = this._cancelButton.Value;
			}
			if (this.timerTimeout != null)
			{
				this.timerTimeout.Stop();
			}
			base.OnClosing(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			this._result = null;
			if (Environment.OSVersion.Version.Major < 6)
			{
				this.rtbMessage.BackColor = this.BackColor;
			}
			base.Size = new System.Drawing.Size(this._maxWidth, this._maxHeight);
			System.Drawing.Size clientSize = base.ClientSize;
			this._maxLayoutWidth = clientSize.Width - 24 - 8;
			clientSize = base.ClientSize;
			this._maxLayoutHeight = clientSize.Height - 24 - 12;
			this.AddOkButtonIfNoButtonsPresent();
			this.DisableCloseIfMultipleButtonsAndNoCancelButton();
			this.SetIconSizeAndVisibility();
			this.SetMessageSizeAndVisibility();
			this.SetCheckboxSizeAndVisibility();
			this.SetOptimumSize();
			this.LayoutControls();
			if (base.StartPosition == FormStartPosition.CenterScreen || base.StartPosition == FormStartPosition.WindowsDefaultBounds || base.StartPosition == FormStartPosition.WindowsDefaultLocation)
			{
				this.CenterForm();
			}
			this.PlayAlert();
			this.SelectDefaultButton();
			this.StartTimerIfTimeoutGreaterThanZero();
			this.CalcBodyArea();
			base.OnLoad(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (this._iconImage != null)
			{
				e.Graphics.DrawIcon(this._iconImage, new Rectangle(this.panelIcon.Location, new System.Drawing.Size(32, 32)));
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			base.OnPaintBackground(e);
			if (Environment.OSVersion.Version.Major >= 6)
			{
				using (Brush solidBrush = new SolidBrush(SystemColors.Window))
				{
					e.Graphics.FillRectangle(solidBrush, this._bodyArea);
				}
			}
		}

		private void PlayAlert()
		{
			if (this._playAlert)
			{
				if (this._standardIcon != MessageBoxIcon.None)
				{
					MessageBoxExForm.MessageBeep((uint)this._standardIcon);
					return;
				}
				MessageBoxExForm.MessageBeep(0);
			}
		}

		protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
		{
			if (keyData == (Keys.LButton | Keys.RButton | Keys.Cancel | Keys.ShiftKey | Keys.ControlKey | Keys.Menu | Keys.Pause | Keys.Space | Keys.Prior | Keys.PageUp | Keys.Next | Keys.PageDown | Keys.End | Keys.D0 | Keys.D1 | Keys.D2 | Keys.D3 | Keys.A | Keys.B | Keys.C | Keys.P | Keys.Q | Keys.R | Keys.S | Keys.NumPad0 | Keys.NumPad1 | Keys.NumPad2 | Keys.NumPad3 | Keys.F1 | Keys.F2 | Keys.F3 | Keys.F4 | Keys.Alt) && !this._allowCancel)
			{
				return true;
			}
			if (keyData == Keys.Escape && this._allowCancel)
			{
				base.Close();
			}
			else if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
			{
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void rtbMessage_LinkClicked(object sender, LinkClickedEventArgs e)
		{
			try
			{
				Process.Start(e.LinkText);
			}
			catch (Exception exception)
			{
			}
			this.SelectDefaultButton();
		}

		private void SelectDefaultButton()
		{
			if (this._defaultButtonControl != null)
			{
				this._defaultButtonControl.Select();
			}
		}

		private void SetCheckboxSizeAndVisibility()
		{
			if (!this.AllowSaveResponse)
			{
				this.chbSaveResponse.Visible = false;
				this.chbSaveResponse.Size = System.Drawing.Size.Empty;
				return;
			}
			System.Drawing.Size width = this.MeasureString(this.chbSaveResponse.Text, this._maxLayoutWidth);
			width.Width = width.Width + 20;
			this.chbSaveResponse.Size = width;
			this.chbSaveResponse.Visible = true;
		}

		private void SetIconSizeAndVisibility()
		{
			if (this._iconImage == null)
			{
				this.panelIcon.Visible = false;
				this.panelIcon.Size = System.Drawing.Size.Empty;
				return;
			}
			this.panelIcon.Size = new System.Drawing.Size(32, 32);
			this.panelIcon.Visible = true;
		}

		private void SetMessageSizeAndVisibility()
		{
			if (this.rtbMessage.Text == null || this.rtbMessage.Text.Trim().Length == 0)
			{
				this.rtbMessage.Size = System.Drawing.Size.Empty;
				this.rtbMessage.Visible = false;
				return;
			}
			int width = this._maxLayoutWidth;
			if (this.panelIcon.Size.Width != 0)
			{
				System.Drawing.Size size = this.panelIcon.Size;
				width = width - (size.Width + 15);
			}
			width -= SystemInformation.VerticalScrollBarWidth;
			System.Drawing.Size width1 = this.MeasureString(this.rtbMessage.Text, width);
			width1.Width = width1.Width + SystemInformation.VerticalScrollBarWidth;
			width1.Height = Math.Max(this.panelIcon.Height, width1.Height) + SystemInformation.HorizontalScrollBarHeight;
			this.rtbMessage.Size = width1;
			this.rtbMessage.Visible = true;
		}

		private void SetOptimumSize()
		{
			int width = base.Width;
			System.Drawing.Size clientSize = base.ClientSize;
			int num = width - clientSize.Width;
			int height = base.Height;
			clientSize = base.ClientSize;
			int height1 = height - clientSize.Height;
			int width1 = this.rtbMessage.Width + 15 + this.panelIcon.Width;
			int num1 = this.chbSaveResponse.Width + this.panelIcon.Width / 2;
			int widthOfAllButtons = this.GetWidthOfAllButtons();
			int width2 = this.GetCaptionSize().Width;
			int num2 = Math.Max(num1, Math.Max(width1, widthOfAllButtons));
			int num3 = 24 + num2 + 8 + num;
			if (num3 < width2)
			{
				num3 = width2;
			}
			clientSize = this.GetButtonSize();
			int num4 = 24 + Math.Max(this.rtbMessage.Height, this.panelIcon.Height) + 10 + this.chbSaveResponse.Height + 10 + clientSize.Height + 12 + height1;
			if (num4 > this._maxHeight)
			{
				RichTextBox richTextBox = this.rtbMessage;
				richTextBox.Height = richTextBox.Height - (num4 - this._maxHeight);
			}
			int num5 = Math.Min(num4, this._maxHeight);
			int num6 = Math.Min(num3, this._maxWidth);
			base.Size = new System.Drawing.Size(num6, num5);
		}

		private void SetResultAndClose(string result)
		{
			this._result = result;
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
		}

		private void SetStandardIcon(MessageBoxIcon icon)
		{
			this._standardIcon = icon;
			if (icon > MessageBoxIcon.Hand)
			{
				if (icon == MessageBoxIcon.Question)
				{
					this._iconImage = SystemIcons.Question;
					return;
				}
				if (icon == MessageBoxIcon.Exclamation)
				{
					this._iconImage = SystemIcons.Exclamation;
					return;
				}
				if (icon == MessageBoxIcon.Asterisk)
				{
					this._iconImage = SystemIcons.Asterisk;
					return;
				}
			}
			else
			{
				if (icon != MessageBoxIcon.None)
				{
					if (icon != MessageBoxIcon.Hand)
					{
						return;
					}
					this._iconImage = SystemIcons.Error;
					return;
				}
				this._iconImage = null;
			}
		}

		private void StartTimerIfTimeoutGreaterThanZero()
		{
			if (this._timeout > 0)
			{
				if (this.timerTimeout == null)
				{
					this.timerTimeout = new Timer(this.components);
					this.timerTimeout.Tick += new EventHandler(this.timerTimeout_Tick);
				}
				if (!this.timerTimeout.Enabled)
				{
					this.timerTimeout.Interval = this._timeout;
					this.timerTimeout.Start();
				}
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern bool SystemParametersInfo(int uiAction, int uiParam, ref MessageBoxExForm.NONCLIENTMETRICS ncMetrics, int fWinIni);

		private void timerTimeout_Tick(object sender, EventArgs e)
		{
			this.timerTimeout.Stop();
			switch (this._timeoutResult)
			{
				case AddinExpress.Installer.WiXDesigner.TimeoutResult.Default:
				{
					this._defaultButtonControl.PerformClick();
					return;
				}
				case AddinExpress.Installer.WiXDesigner.TimeoutResult.Cancel:
				{
					if (this._cancelButton == null)
					{
						this._defaultButtonControl.PerformClick();
						return;
					}
					this.SetResultAndClose(this._cancelButton.Value);
					return;
				}
				case AddinExpress.Installer.WiXDesigner.TimeoutResult.Timeout:
				{
					this.SetResultAndClose("Timeout");
					return;
				}
				default:
				{
					return;
				}
			}
		}

		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (m.Msg != 33)
			{
				int msg = m.Msg;
			}
			else
			{
				Point mousePosition = Control.MousePosition;
				mousePosition = base.PointToClient(mousePosition);
				if (mousePosition.X >= this._bodyArea.Left && mousePosition.X <= this._bodyArea.Right && mousePosition.Y >= this._bodyArea.Top && mousePosition.Y <= this._bodyArea.Bottom)
				{
					bool value = false;
					try
					{
						PropertyInfo property = this.rtbMessage.GetType().GetProperty("LinkCursor", BindingFlags.Instance | BindingFlags.NonPublic);
						if (property != null)
						{
							value = (bool)property.GetValue(this.rtbMessage, null);
						}
					}
					catch (Exception exception)
					{
					}
					if (!value)
					{
						m.Result = (IntPtr)4;
						return;
					}
				}
			}
			base.WndProc(ref m);
		}

		private struct LOGFONT
		{
			public int lfHeight;

			public int lfWidth;

			public int lfEscapement;

			public int lfOrientation;

			public int lfWeight;

			public byte lfItalic;

			public byte lfUnderline;

			public byte lfStrikeOut;

			public byte lfCharSet;

			public byte lfOutPrecision;

			public byte lfClipPrecision;

			public byte lfQuality;

			public byte lfPitchAndFamily;

			public string lfFaceSize;
		}

		private struct NONCLIENTMETRICS
		{
			public int cbSize;

			public int iBorderWidth;

			public int iScrollWidth;

			public int iScrollHeight;

			public int iCaptionWidth;

			public int iCaptionHeight;

			public MessageBoxExForm.LOGFONT lfCaptionFont;

			public int iSmCaptionWidth;

			public int iSmCaptionHeight;

			public MessageBoxExForm.LOGFONT lfSmCaptionFont;

			public int iMenuWidth;

			public int iMenuHeight;

			public MessageBoxExForm.LOGFONT lfMenuFont;

			public MessageBoxExForm.LOGFONT lfStatusFont;

			public MessageBoxExForm.LOGFONT lfMessageFont;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct RECT
		{
			[FieldOffset(0)]
			public int Left;

			[FieldOffset(4)]
			public int Top;

			[FieldOffset(8)]
			public int Right;

			[FieldOffset(12)]
			public int Bottom;

			public RECT(int left, int top, int right, int bottom)
			{
				this.Left = left;
				this.Top = top;
				this.Right = right;
				this.Bottom = bottom;
			}

			public RECT(Rectangle rect)
			{
				this.Left = rect.Left;
				this.Top = rect.Top;
				this.Right = rect.Right;
				this.Bottom = rect.Bottom;
			}

			public Rectangle ToRectangle()
			{
				return new Rectangle(this.Left, this.Top, this.Right, this.Bottom - 1);
			}
		}
	}
}