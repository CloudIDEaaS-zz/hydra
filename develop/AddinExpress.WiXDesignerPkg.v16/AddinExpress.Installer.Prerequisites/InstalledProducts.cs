using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal class InstalledProducts : Form
	{
		private static List<WeakReference> __ENCList;

		private Button CancelBtn;

		private Button OKButton;

		private ColumnHeader ProductCode;

		private ListView ProductList;

		private ColumnHeader ProductNameColumn;

		private IContainer components = new System.ComponentModel.Container();

		private Panel panel1;

		public string SelectedProductCode;

		static InstalledProducts()
		{
			InstalledProducts.__ENCList = new List<WeakReference>();
		}

		public InstalledProducts()
		{
			base.Load += new EventHandler(this.InstalledProducts_Load);
			lock (InstalledProducts.__ENCList)
			{
				InstalledProducts.__ENCList.Add(new WeakReference(this));
			}
			this.SelectedProductCode = null;
			this.InitializeComponent();
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
			this.OKButton = new Button();
			this.CancelBtn = new Button();
			this.ProductList = new ListView();
			this.ProductNameColumn = new ColumnHeader();
			this.ProductCode = new ColumnHeader();
			this.panel1 = new Panel();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.OKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.OKButton.Location = new Point(505, 15);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(87, 25);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new EventHandler(this.OKButton_Click);
			this.CancelBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new Point(598, 15);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(87, 25);
			this.CancelBtn.TabIndex = 2;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.ProductList.Columns.AddRange(new ColumnHeader[] { this.ProductNameColumn, this.ProductCode });
			this.ProductList.Dock = DockStyle.Fill;
			this.ProductList.Location = new Point(0, 0);
			this.ProductList.MultiSelect = false;
			this.ProductList.Name = "ProductList";
			this.ProductList.Size = new System.Drawing.Size(697, 355);
			this.ProductList.TabIndex = 0;
			this.ProductList.UseCompatibleStateImageBehavior = false;
			this.ProductList.View = View.Details;
			this.ProductList.SelectedIndexChanged += new EventHandler(this.ProductList_SelectedIndexChanged);
			this.ProductList.DoubleClick += new EventHandler(this.ProductList_DoubleClick);
			this.ProductNameColumn.Text = "Product Name";
			this.ProductNameColumn.Width = 285;
			this.ProductCode.Text = "Product Code";
			this.ProductCode.Width = 285;
			this.panel1.Controls.Add(this.OKButton);
			this.panel1.Controls.Add(this.CancelBtn);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 355);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(697, 52);
			this.panel1.TabIndex = 3;
			base.AcceptButton = this.OKButton;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(697, 407);
			base.Controls.Add(this.ProductList);
			base.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "InstalledProducts";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Installed Products";
			base.Load += new EventHandler(this.InstalledProducts_Load);
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
		}

		private void InstalledProducts_Load(object sender, EventArgs e)
		{
			string[] name = new string[2];
			foreach (MsiUtils.InstalledProductInfo installedProductInfo in MsiUtils.ListInstalledProducts2())
			{
				name[0] = installedProductInfo.Name;
				name[1] = installedProductInfo.Code;
				ListViewItem listViewItem = new ListViewItem(name)
				{
					Tag = installedProductInfo.Code
				};
				this.ProductList.Items.Add(listViewItem);
			}
			this.OKButton.Enabled = false;
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			base.Close();
		}

		private void ProductList_DoubleClick(object sender, EventArgs e)
		{
			if (this.OKButton.Enabled)
			{
				base.DialogResult = System.Windows.Forms.DialogResult.OK;
				base.Close();
			}
		}

		private void ProductList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OKButton.Enabled = this.ProductList.SelectedItems.Count > 0;
			if (this.ProductList.SelectedItems.Count > 0)
			{
				this.SelectedProductCode = this.ProductList.SelectedItems[0].Tag.ToString();
			}
		}
	}
}