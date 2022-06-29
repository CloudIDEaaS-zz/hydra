using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AddinExpress.Installer.WiXDesigner
{
	internal abstract class DropDownControlBase : Control
	{
		private const int CONTROL_HEIGHT = 7;

		private const int DROPDOWNBUTTON_WIDTH = 17;

		private bool _drawWithVisualStyles;

		private Rectangle _dropDownButtonBounds;

		private bool _droppedDown;

		private BufferedPainter<ComboBoxState> _bufferedPainter;

		[DefaultValue(typeof(Color), "Window")]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[Browsable(false)]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}

		[Browsable(false)]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("Determines whether to draw the control with visual styles.")]
		public bool DrawWithVisualStyles
		{
			get
			{
				return this._drawWithVisualStyles;
			}
			set
			{
				this._drawWithVisualStyles = value;
				base.Invalidate();
			}
		}

		[Browsable(false)]
		public virtual bool DroppedDown
		{
			get
			{
				return this._droppedDown;
			}
			set
			{
				this._droppedDown = value;
				this._bufferedPainter.State = this.GetComboBoxState();
			}
		}

		public DropDownControlBase()
		{
			this.DoubleBuffered = true;
			base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			base.SetStyle(ControlStyles.Opaque, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
			base.SetStyle(ControlStyles.Selectable, true);
			base.SetStyle(ControlStyles.StandardClick, true);
			base.SetStyle(ControlStyles.UserPaint, true);
			this._drawWithVisualStyles = Application.RenderWithVisualStyles;
			this.BackColor = SystemColors.Window;
			this._bufferedPainter = new BufferedPainter<ComboBoxState>(this);
			this._bufferedPainter.PaintVisualState += new EventHandler<BufferedPaintEventArgs<ComboBoxState>>(this.bufferedPainter_PaintVisualState);
			BufferedPainter<ComboBoxState> bufferedPainter = this._bufferedPainter;
			int num = 1;
			ComboBoxState comboBoxState = (ComboBoxState)num;
			this._bufferedPainter.DefaultState = (ComboBoxState)num;
			bufferedPainter.State = comboBoxState;
			this._bufferedPainter.AddTransition(ComboBoxState.Normal, ComboBoxState.Hot, 250);
			this._bufferedPainter.AddTransition(ComboBoxState.Hot, ComboBoxState.Normal, 350);
			this._bufferedPainter.AddTransition(ComboBoxState.Pressed, ComboBoxState.Normal, 350);
		}

		private void bufferedPainter_PaintVisualState(object sender, BufferedPaintEventArgs<ComboBoxState> e)
		{
			if (this._drawWithVisualStyles && this._bufferedPainter.BufferedPaintSupported && this._bufferedPainter.Enabled)
			{
				(new VisualStyleRenderer(VisualStyleElement.Button.PushButton.Normal)).DrawParentBackground(e.Graphics, base.ClientRectangle, this);
				Rectangle clientRectangle = base.ClientRectangle;
				clientRectangle.Inflate(1, 1);
				ButtonRenderer.DrawButton(e.Graphics, clientRectangle, this.GetPushButtonState(e.State));
				Rectangle rectangle = this._dropDownButtonBounds;
				rectangle.Inflate(-2, -2);
				e.Graphics.SetClip(rectangle);
				ComboBoxRenderer.DrawDropDownButton(e.Graphics, this._dropDownButtonBounds, e.State);
				e.Graphics.SetClip(base.ClientRectangle);
			}
			else if (!this._drawWithVisualStyles || !ComboBoxRenderer.IsSupported)
			{
				Rectangle height = base.ClientRectangle;
				height.Height = height.Height + 1;
				e.Graphics.FillRectangle(new SolidBrush(this.BackColor), base.ClientRectangle);
				ControlPaint.DrawBorder3D(e.Graphics, height);
				ControlPaint.DrawComboButton(e.Graphics, this._dropDownButtonBounds, this.GetPlainButtonState());
			}
			else
			{
				ComboBoxRenderer.DrawTextBox(e.Graphics, base.ClientRectangle, this.GetTextBoxState());
				ComboBoxRenderer.DrawDropDownButton(e.Graphics, this._dropDownButtonBounds, e.State);
			}
			this.OnPaintContent(new DropDownPaintEventArgs(e.Graphics, base.ClientRectangle, this.GetTextBoxBounds()));
		}

		private ComboBoxState GetComboBoxState()
		{
			if (!base.Enabled)
			{
				return ComboBoxState.Disabled;
			}
			if (!this._droppedDown && !base.ClientRectangle.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)))
			{
				return ComboBoxState.Normal;
			}
			if (!this._droppedDown && (Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left)
			{
				return ComboBoxState.Hot;
			}
			return ComboBoxState.Pressed;
		}

		private ButtonState GetPlainButtonState()
		{
			if (!base.Enabled)
			{
				return ButtonState.Inactive;
			}
			if (!this._droppedDown && (!this._dropDownButtonBounds.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)) || (Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left))
			{
				return ButtonState.Normal;
			}
			return ButtonState.Pushed;
		}

		private PushButtonState GetPushButtonState(ComboBoxState state)
		{
			switch (state)
			{
				case ComboBoxState.Hot:
				{
					return PushButtonState.Hot;
				}
				case ComboBoxState.Pressed:
				{
					return PushButtonState.Pressed;
				}
				case ComboBoxState.Disabled:
				{
					return PushButtonState.Disabled;
				}
			}
			return PushButtonState.Normal;
		}

		private Rectangle GetTextBoxBounds()
		{
			return new Rectangle(0, 0, this._dropDownButtonBounds.Left, base.ClientRectangle.Height);
		}

		private ComboBoxState GetTextBoxState()
		{
			if (!base.Enabled)
			{
				return ComboBoxState.Disabled;
			}
			if (!this.Focused && !base.ClientRectangle.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)))
			{
				return ComboBoxState.Normal;
			}
			return ComboBoxState.Hot;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Return || (int)keyData - (int)Keys.Prior <= (int)(Keys.LButton | Keys.RButton | Keys.Cancel | Keys.MButton | Keys.XButton1 | Keys.XButton2))
			{
				return true;
			}
			return base.IsInputKey(keyData);
		}

		protected virtual void OnDropDown(EventArgs e)
		{
			if (this.DropDown != null)
			{
				this.DropDown(this, e);
			}
		}

		protected virtual void OnDropDownButtonClick(EventArgs e)
		{
			if (this.DropDownButtonClick != null)
			{
				this.DropDownButtonClick(this, e);
			}
		}

		protected virtual void OnDropDownClosed(EventArgs e)
		{
			if (this.DropDownClosed != null)
			{
				this.DropDownClosed(this, e);
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.SetHeight();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if (this.ShowFocusCues)
			{
				base.Invalidate();
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (this.ShowFocusCues)
			{
				base.Invalidate();
			}
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (this._dropDownButtonBounds.Contains(e.Location))
			{
				this.OnDropDownButtonClick(e);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			base.Focus();
			this._bufferedPainter.State = this.GetComboBoxState();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			this._bufferedPainter.State = this.GetComboBoxState();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this._bufferedPainter.State = this.GetComboBoxState();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this._bufferedPainter.State = this.GetComboBoxState();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this._bufferedPainter.State = this.GetComboBoxState();
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if (!this._drawWithVisualStyles || !this._bufferedPainter.BufferedPaintSupported || !this._bufferedPainter.Enabled)
			{
				base.OnPaintBackground(pevent);
			}
		}

		protected virtual void OnPaintContent(DropDownPaintEventArgs e)
		{
			if (this.PaintContent != null)
			{
				this.PaintContent(this, e);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			System.Drawing.Size clientSize = base.ClientSize;
			int width = clientSize.Width - 17;
			clientSize = base.ClientSize;
			this._dropDownButtonBounds = new Rectangle(width, 0, 17, clientSize.Height);
		}

		private void SetHeight()
		{
			base.Height = 7 + this.Font.Height;
		}

		[Description("Occurs when the drop-down portion of the control is displayed.")]
		public event EventHandler DropDown;

		[Description("Occurs when the user clicks the dropdown button at the right edge of the control.")]
		protected event EventHandler DropDownButtonClick;

		[Description("Occurs when the drop-down portion of the control is closed.")]
		public event EventHandler DropDownClosed;

		[Description("Occurs when the content of the editable portion of the control is painted.")]
		public event EventHandler<DropDownPaintEventArgs> PaintContent;
	}
}