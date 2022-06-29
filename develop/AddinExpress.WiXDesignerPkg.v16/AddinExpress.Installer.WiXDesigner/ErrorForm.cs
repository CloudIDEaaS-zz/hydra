using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ErrorForm : Form
	{
		internal Button btnMore;

		internal TextBox txtMore;

		internal Button btn3;

		internal Button btn2;

		internal Button btn1;

		internal RichTextBox ErrorBox;

		internal PictureBox PictureBox1;

		private System.ComponentModel.Container components;

		private const int _intSpacing = 10;

		public ErrorForm()
		{
			this.InitializeComponent();
		}

		private void ADXErrorForm_Load(object sender, EventArgs e)
		{
			base.TopMost = true;
			base.TopMost = false;
			this.txtMore.Anchor = AnchorStyles.None;
			this.txtMore.Visible = false;
			this.SizeBox(this.ErrorBox);
			this.btnMore.Top = this.ErrorBox.Top + this.ErrorBox.Height + 20;
			Button button = this.btn1;
			Button button1 = this.btn2;
			Button button2 = this.btn3;
			int top = this.btnMore.Top;
			int num = top;
			button2.Top = top;
			int num1 = num;
			int num2 = num1;
			button1.Top = num1;
			button.Top = num2;
			this.txtMore.Width = this.btn3.Right - this.btnMore.Left;
			System.Drawing.Size clientSize = base.ClientSize;
			base.ClientSize = new System.Drawing.Size(clientSize.Width, this.btnMore.Bottom + 10);
			base.CenterToScreen();
		}

		private void btn1_Click(object sender, EventArgs e)
		{
			base.Close();
			base.DialogResult = this.DetermineDialogResult(this.btn1.Text);
		}

		private void btn2_Click(object sender, EventArgs e)
		{
			base.Close();
			base.DialogResult = this.DetermineDialogResult(this.btn2.Text);
		}

		private void btn3_Click(object sender, EventArgs e)
		{
			base.Close();
			base.DialogResult = this.DetermineDialogResult(this.btn3.Text);
		}

		private void btnMore_Click(object sender, EventArgs e)
		{
			System.Drawing.Size clientSize;
			if (this.btnMore.Text != "Details >>")
			{
				base.SuspendLayout();
				this.btnMore.Text = "Details >>";
				clientSize = base.ClientSize;
				base.ClientSize = new System.Drawing.Size(clientSize.Width, this.btnMore.Bottom + 10);
				this.txtMore.Visible = false;
				this.txtMore.Anchor = AnchorStyles.None;
				base.ResumeLayout();
				return;
			}
			base.Height = base.Height + 300;
			this.txtMore.Location = new Point(this.btnMore.Left, this.btnMore.Top + this.btnMore.Height + 10);
			TextBox height = this.txtMore;
			clientSize = base.ClientSize;
			height.Height = clientSize.Height - this.txtMore.Top - this.btnMore.Left;
			this.txtMore.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			this.txtMore.Visible = true;
			this.btn3.Focus();
			this.btnMore.Text = "<< Details";
		}

		private System.Windows.Forms.DialogResult DetermineDialogResult(string strButtonText)
		{
			strButtonText = strButtonText.Replace("&", "");
			string lower = strButtonText.ToLower();
			if (lower != null)
			{
				if (lower == "abort")
				{
					return System.Windows.Forms.DialogResult.Abort;
				}
				if (lower == "cancel")
				{
					return System.Windows.Forms.DialogResult.Cancel;
				}
				if (lower == "ignore")
				{
					return System.Windows.Forms.DialogResult.Ignore;
				}
				if (lower == "no")
				{
					return System.Windows.Forms.DialogResult.No;
				}
				if (lower == "none")
				{
					return System.Windows.Forms.DialogResult.None;
				}
				if (lower == "ok")
				{
					return System.Windows.Forms.DialogResult.OK;
				}
				if (lower == "retry")
				{
					return System.Windows.Forms.DialogResult.Retry;
				}
				if (lower == "yes")
				{
					return System.Windows.Forms.DialogResult.Yes;
				}
			}
			return System.Windows.Forms.DialogResult.None;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.btnMore = new Button();
			this.txtMore = new TextBox();
			this.btn3 = new Button();
			this.btn2 = new Button();
			this.btn1 = new Button();
			this.ErrorBox = new RichTextBox();
			this.PictureBox1 = new PictureBox();
			((ISupportInitialize)this.PictureBox1).BeginInit();
			base.SuspendLayout();
			this.btnMore.Location = new Point(12, 110);
			this.btnMore.Name = "btnMore";
			this.btnMore.Size = new System.Drawing.Size(75, 24);
			this.btnMore.TabIndex = 20;
			this.btnMore.Text = "Details >>";
			this.btnMore.Click += new EventHandler(this.btnMore_Click);
			this.txtMore.CausesValidation = false;
			this.txtMore.Font = new System.Drawing.Font("Lucida Console", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
			this.txtMore.Location = new Point(7, 144);
			this.txtMore.Multiline = true;
			this.txtMore.Name = "txtMore";
			this.txtMore.ReadOnly = true;
			this.txtMore.ScrollBars = ScrollBars.Both;
			this.txtMore.Size = new System.Drawing.Size(460, 192);
			this.txtMore.TabIndex = 21;
			this.txtMore.Text = "(detailed information, such as exception details)";
			this.txtMore.WordWrap = false;
			this.btn3.Location = new Point(387, 110);
			this.btn3.Name = "btn3";
			this.btn3.Size = new System.Drawing.Size(75, 23);
			this.btn3.TabIndex = 24;
			this.btn3.Text = "Button3";
			this.btn3.Click += new EventHandler(this.btn3_Click);
			this.btn2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn2.Location = new Point(306, 110);
			this.btn2.Name = "btn2";
			this.btn2.Size = new System.Drawing.Size(75, 23);
			this.btn2.TabIndex = 23;
			this.btn2.Text = "Button2";
			this.btn2.Click += new EventHandler(this.btn2_Click);
			this.btn1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn1.Location = new Point(225, 110);
			this.btn1.Name = "btn1";
			this.btn1.Size = new System.Drawing.Size(75, 23);
			this.btn1.TabIndex = 22;
			this.btn1.Text = "Button1";
			this.btn1.Click += new EventHandler(this.btn1_Click);
			this.ErrorBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			this.ErrorBox.BackColor = SystemColors.Control;
			this.ErrorBox.BorderStyle = BorderStyle.None;
			this.ErrorBox.CausesValidation = false;
			this.ErrorBox.Location = new Point(51, 15);
			this.ErrorBox.Name = "ErrorBox";
			this.ErrorBox.ReadOnly = true;
			this.ErrorBox.ScrollBars = RichTextBoxScrollBars.Vertical;
			this.ErrorBox.Size = new System.Drawing.Size(411, 78);
			this.ErrorBox.TabIndex = 14;
			this.ErrorBox.Text = "(error message)";
			this.PictureBox1.Location = new Point(7, 10);
			this.PictureBox1.Name = "PictureBox1";
			this.PictureBox1.Size = new System.Drawing.Size(32, 32);
			this.PictureBox1.TabIndex = 13;
			this.PictureBox1.TabStop = false;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(474, 345);
			base.Controls.Add(this.btnMore);
			base.Controls.Add(this.txtMore);
			base.Controls.Add(this.btn3);
			base.Controls.Add(this.btn2);
			base.Controls.Add(this.btn1);
			base.Controls.Add(this.ErrorBox);
			base.Controls.Add(this.PictureBox1);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ADXErrorForm";
			base.ShowIcon = false;
			base.StartPosition = FormStartPosition.CenterScreen;
			this.Text = "Error";
			base.TopMost = true;
			base.Load += new EventHandler(this.ADXErrorForm_Load);
			((ISupportInitialize)this.PictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void SizeBox(RichTextBox ctl)
		{
			using (Graphics graphic = null)
			{
				try
				{
					graphic = Graphics.FromHwnd(ctl.Handle);
					SizeF sizeF = graphic.MeasureString(ctl.Text, ctl.Font, new SizeF((float)ctl.Width, (float)ctl.Height));
					graphic.Dispose();
					ctl.Height = Convert.ToInt32(sizeF.Height) + 5;
				}
				catch
				{
				}
			}
		}
	}
}