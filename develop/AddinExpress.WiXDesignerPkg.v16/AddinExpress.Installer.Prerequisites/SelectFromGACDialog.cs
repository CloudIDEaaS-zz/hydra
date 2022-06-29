using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AddinExpress.Installer.Prerequisites
{
	internal class SelectFromGACDialog : Form
	{
		private static List<WeakReference> __ENCList;

		private ColumnHeader AssemblyName;

		private Button Button2;

		private Button Cancel;

		private Button CancelBtn;

		private Button CancelingButton;

		private ColumnHeader ColumnHeader1;

		private ColumnHeader ColumnHeader2;

		private ColumnHeader ColumnHeader3;

		private ColumnHeader ColumnHeader4;

		private ColumnHeader ColumnHeader5;

		private ColumnHeader Culture;

		private ToolStripMenuItem DetailsToolStripMenuItem;

		private ToolStripMenuItem LargeIconsToolStripMenuItem;

		private ToolStripMenuItem ListToolStripMenuItem;

		private System.Windows.Forms.ListView ListView;

		private ToolStripDropDownButton ListViewToolStripButton;

		private Button OKButton;

		private ColumnHeader ProcessorArchitecture;

		private ColumnHeader PublicKeyToken;

		private ToolStripMenuItem SmallIconsToolStripMenuItem;

		private ToolStripMenuItem TileToolStripMenuItem;

		private System.Windows.Forms.ToolStrip ToolStrip;

		private System.Windows.Forms.ToolTip ToolTip;

		private ImageList TreeNodeImageList;

		private ColumnHeader Version;

		private IContainer components;

		private List<System.Reflection.AssemblyName> m_selectedAssemblies;

		private ToolStripPanel BottomToolStripPanel;

		private ToolStripPanel TopToolStripPanel;

		private ToolStripPanel RightToolStripPanel;

		private ToolStripPanel LeftToolStripPanel;

		private ToolStripContentPanel ContentPanel;

		private Panel panel1;

		private System.Reflection.AssemblyName m_selectedAssembly;

		public bool MultiSelect
		{
			get
			{
				return this.ListView.MultiSelect;
			}
			set
			{
				this.ListView.MultiSelect = value;
			}
		}

		public List<System.Reflection.AssemblyName> SelectedAssemblies
		{
			get
			{
				return this.m_selectedAssemblies;
			}
		}

		public System.Reflection.AssemblyName SelectedAssembly
		{
			get
			{
				return this.m_selectedAssembly;
			}
		}

		static SelectFromGACDialog()
		{
			SelectFromGACDialog.__ENCList = new List<WeakReference>();
		}

		public SelectFromGACDialog()
		{
			base.Load += new EventHandler(this.SelectFromGACDialog_Load);
			lock (SelectFromGACDialog.__ENCList)
			{
				SelectFromGACDialog.__ENCList.Add(new WeakReference(this));
			}
			this.InitializeComponent();
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			if (this.m_selectedAssemblies != null)
			{
				this.m_selectedAssemblies.Clear();
			}
			this.m_selectedAssembly = null;
			base.Hide();
		}

		private void DetailsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetView(View.Details);
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
			this.components = new System.ComponentModel.Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(SelectFromGACDialog));
			this.TreeNodeImageList = new ImageList(this.components);
			this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.BottomToolStripPanel = new ToolStripPanel();
			this.TopToolStripPanel = new ToolStripPanel();
			this.ToolStrip = new System.Windows.Forms.ToolStrip();
			this.ListViewToolStripButton = new ToolStripDropDownButton();
			this.ListToolStripMenuItem = new ToolStripMenuItem();
			this.DetailsToolStripMenuItem = new ToolStripMenuItem();
			this.LargeIconsToolStripMenuItem = new ToolStripMenuItem();
			this.SmallIconsToolStripMenuItem = new ToolStripMenuItem();
			this.TileToolStripMenuItem = new ToolStripMenuItem();
			this.RightToolStripPanel = new ToolStripPanel();
			this.LeftToolStripPanel = new ToolStripPanel();
			this.ContentPanel = new ToolStripContentPanel();
			this.ListView = new System.Windows.Forms.ListView();
			this.AssemblyName = new ColumnHeader();
			this.Version = new ColumnHeader();
			this.Culture = new ColumnHeader();
			this.PublicKeyToken = new ColumnHeader();
			this.ProcessorArchitecture = new ColumnHeader();
			this.OKButton = new Button();
			this.CancelBtn = new Button();
			this.CancelingButton = new Button();
			this.Cancel = new Button();
			this.Button2 = new Button();
			this.ColumnHeader1 = new ColumnHeader();
			this.ColumnHeader2 = new ColumnHeader();
			this.ColumnHeader3 = new ColumnHeader();
			this.ColumnHeader4 = new ColumnHeader();
			this.ColumnHeader5 = new ColumnHeader();
			this.panel1 = new Panel();
			this.ToolStrip.SuspendLayout();
			this.panel1.SuspendLayout();
			base.SuspendLayout();
			this.TreeNodeImageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("TreeNodeImageList.ImageStream");
			this.TreeNodeImageList.TransparentColor = Color.Transparent;
			this.TreeNodeImageList.Images.SetKeyName(0, "ClosedFolder");
			this.TreeNodeImageList.Images.SetKeyName(1, "OpenFolder");
			this.TreeNodeImageList.Images.SetKeyName(2, "assembly.ico");
			this.BottomToolStripPanel.Location = new Point(0, 0);
			this.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.BottomToolStripPanel.Orientation = Orientation.Horizontal;
			this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0);
			this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
			this.TopToolStripPanel.Location = new Point(0, 0);
			this.TopToolStripPanel.Name = "TopToolStripPanel";
			this.TopToolStripPanel.Orientation = Orientation.Horizontal;
			this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0);
			this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
			this.ToolStrip.GripStyle = ToolStripGripStyle.Hidden;
			this.ToolStrip.Items.AddRange(new ToolStripItem[] { this.ListViewToolStripButton });
			this.ToolStrip.Location = new Point(0, 0);
			this.ToolStrip.Name = "ToolStrip";
			this.ToolStrip.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0);
			this.ToolStrip.RenderMode = ToolStripRenderMode.System;
			this.ToolStrip.Size = new System.Drawing.Size(594, 25);
			this.ToolStrip.TabIndex = 0;
			this.ToolStrip.Text = "ToolStrip1";
			this.ListViewToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
			this.ListViewToolStripButton.DropDownItems.AddRange(new ToolStripItem[] { this.ListToolStripMenuItem, this.DetailsToolStripMenuItem, this.LargeIconsToolStripMenuItem, this.SmallIconsToolStripMenuItem, this.TileToolStripMenuItem });
			this.ListViewToolStripButton.Image = (Image)componentResourceManager.GetObject("ListViewToolStripButton.Image");
			this.ListViewToolStripButton.ImageTransparentColor = Color.Black;
			this.ListViewToolStripButton.Name = "ListViewToolStripButton";
			this.ListViewToolStripButton.Size = new System.Drawing.Size(29, 22);
			this.ListViewToolStripButton.Text = "Views";
			this.ListToolStripMenuItem.Name = "ListToolStripMenuItem";
			this.ListToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.ListToolStripMenuItem.Text = "List";
			this.ListToolStripMenuItem.Click += new EventHandler(this.ListToolStripMenuItem_Click);
			this.DetailsToolStripMenuItem.Checked = true;
			this.DetailsToolStripMenuItem.CheckState = CheckState.Checked;
			this.DetailsToolStripMenuItem.Name = "DetailsToolStripMenuItem";
			this.DetailsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.DetailsToolStripMenuItem.Text = "Details";
			this.DetailsToolStripMenuItem.Click += new EventHandler(this.DetailsToolStripMenuItem_Click);
			this.LargeIconsToolStripMenuItem.Name = "LargeIconsToolStripMenuItem";
			this.LargeIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.LargeIconsToolStripMenuItem.Text = "Large Icons";
			this.LargeIconsToolStripMenuItem.Click += new EventHandler(this.LargeIconsToolStripMenuItem_Click);
			this.SmallIconsToolStripMenuItem.Name = "SmallIconsToolStripMenuItem";
			this.SmallIconsToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.SmallIconsToolStripMenuItem.Text = "Small Icons";
			this.SmallIconsToolStripMenuItem.Click += new EventHandler(this.SmallIconsToolStripMenuItem_Click);
			this.TileToolStripMenuItem.Name = "TileToolStripMenuItem";
			this.TileToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
			this.TileToolStripMenuItem.Text = "Tile";
			this.TileToolStripMenuItem.Click += new EventHandler(this.TileToolStripMenuItem_Click);
			this.RightToolStripPanel.Location = new Point(0, 0);
			this.RightToolStripPanel.Name = "RightToolStripPanel";
			this.RightToolStripPanel.Orientation = Orientation.Horizontal;
			this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0);
			this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
			this.LeftToolStripPanel.Location = new Point(0, 0);
			this.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.LeftToolStripPanel.Orientation = Orientation.Horizontal;
			this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(0);
			this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
			this.ContentPanel.Size = new System.Drawing.Size(200, 100);
			this.ListView.Columns.AddRange(new ColumnHeader[] { this.AssemblyName, this.Version, this.Culture, this.PublicKeyToken, this.ProcessorArchitecture });
			this.ListView.Dock = DockStyle.Fill;
			this.ListView.LargeImageList = this.TreeNodeImageList;
			this.ListView.Location = new Point(0, 25);
			this.ListView.Name = "ListView";
			this.ListView.Size = new System.Drawing.Size(594, 292);
			this.ListView.SmallImageList = this.TreeNodeImageList;
			this.ListView.StateImageList = this.TreeNodeImageList;
			this.ListView.TabIndex = 1;
			this.ListView.UseCompatibleStateImageBehavior = false;
			this.ListView.SelectedIndexChanged += new EventHandler(this.ListView_SelectedIndexChanged);
			this.ListView.DoubleClick += new EventHandler(this.ListView_DoubleClick);
			this.AssemblyName.Text = "Assembly Name";
			this.AssemblyName.Width = 280;
			this.Version.Text = "Version";
			this.Version.Width = 80;
			this.Culture.Text = "Culture";
			this.Culture.Width = 70;
			this.PublicKeyToken.Text = "Public Key Token";
			this.PublicKeyToken.Width = 128;
			this.ProcessorArchitecture.Text = "Processor Architecture";
			this.ProcessorArchitecture.Width = 130;
			this.OKButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Enabled = false;
			this.OKButton.Location = new Point(402, 17);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(87, 25);
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "&OK";
			this.OKButton.Click += new EventHandler(this.OKButton_Click);
			this.CancelBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new Point(495, 17);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(87, 25);
			this.CancelBtn.TabIndex = 1;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.Click += new EventHandler(this.Button2_Click);
			this.CancelingButton.Location = new Point(545, 428);
			this.CancelingButton.Name = "CancelingButton";
			this.CancelingButton.Size = new System.Drawing.Size(75, 23);
			this.CancelingButton.TabIndex = 2;
			this.CancelingButton.Text = "&Cancel";
			this.Cancel.Location = new Point(545, 428);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 2;
			this.Cancel.Text = "&Cancel";
			this.Button2.Location = new Point(545, 428);
			this.Button2.Name = "Button2";
			this.Button2.Size = new System.Drawing.Size(75, 23);
			this.Button2.TabIndex = 2;
			this.Button2.Text = "&Cancel";
			this.panel1.Controls.Add(this.OKButton);
			this.panel1.Controls.Add(this.CancelBtn);
			this.panel1.Dock = DockStyle.Bottom;
			this.panel1.Location = new Point(0, 317);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(594, 54);
			this.panel1.TabIndex = 2;
			base.AutoScaleDimensions = new SizeF(96f, 96f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			base.ClientSize = new System.Drawing.Size(594, 371);
			base.Controls.Add(this.ListView);
			base.Controls.Add(this.ToolStrip);
			base.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "SelectFromGACDialog";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = FormStartPosition.CenterParent;
			this.Text = "Global Assemlby Cache";
			this.ToolStrip.ResumeLayout(false);
			this.ToolStrip.PerformLayout();
			this.panel1.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private void LargeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetView(View.LargeIcon);
		}

		private void ListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetView(View.List);
		}

		private void ListView_DoubleClick(object sender, EventArgs e)
		{
			this.OKButton_Click(RuntimeHelpers.GetObjectValue(sender), e);
		}

		private void ListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OKButton.Enabled = this.ListView.SelectedItems.Count >= 1;
		}

		private void LoadListView()
		{
			string str;
			this.ListView.Items.Clear();
			this.Cursor = Cursors.WaitCursor;
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.System));
				directoryInfo = directoryInfo.Parent.GetDirectories("assembly")[0];
				if (directoryInfo.Exists)
				{
					DirectoryInfo[] directories = directoryInfo.GetDirectories("GAC*");
					for (int i = 0; i < (int)directories.Length; i++)
					{
						DirectoryInfo[] directoryInfoArray = directories[i].GetDirectories();
						for (int j = 0; j < (int)directoryInfoArray.Length; j++)
						{
							DirectoryInfo[] directories1 = directoryInfoArray[j].GetDirectories();
							for (int k = 0; k < (int)directories1.Length; k++)
							{
								FileInfo[] files = directories1[k].GetFiles("*.dll");
								for (int l = 0; l < (int)files.Length; l++)
								{
									FileInfo fileInfo = files[l];
									try
									{
										System.Reflection.AssemblyName name = Assembly.LoadFile(fileInfo.FullName).GetName();
										ListViewItem listViewItem = this.ListView.Items.Add(name.Name);
										listViewItem.ImageKey = "assembly.ico";
										listViewItem.Tag = name;
										string str1 = name.Version.ToString();
										str = (name.CultureInfo.Equals(CultureInfo.InvariantCulture) ? "" : name.CultureInfo.Name);
										string str2 = "";
										byte[] publicKeyToken = name.GetPublicKeyToken();
										for (int m = 0; m < (int)publicKeyToken.Length; m++)
										{
											string str3 = Convert.ToString(publicKeyToken[m], 16);
											if (str3.Length == 1)
											{
												str3 = string.Concat("0", str3);
											}
											str2 = string.Concat(str2, str3);
										}
										string lower = str2.ToLower();
										string str4 = name.ProcessorArchitecture.ToString();
										listViewItem.SubItems.AddRange(new string[] { str1, str, lower, str4 });
									}
									catch (Exception exception)
									{
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			base.DialogResult = System.Windows.Forms.DialogResult.OK;
			if (!this.MultiSelect)
			{
				this.m_selectedAssembly = (System.Reflection.AssemblyName)this.ListView.SelectedItems[0].Tag;
			}
			else
			{
				IEnumerator enumerator = null;
				this.m_selectedAssemblies.Clear();
				try
				{
					enumerator = this.ListView.SelectedItems.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ListViewItem current = (ListViewItem)enumerator.Current;
						this.m_selectedAssemblies.Add((System.Reflection.AssemblyName)current.Tag);
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
			}
			base.Hide();
		}

		private void SelectFromGACDialog_Load(object sender, EventArgs e)
		{
			this.SetUpListViewColumns();
			this.LoadListView();
		}

		private void SetUpListViewColumns()
		{
			this.SetView(View.Details);
		}

		private void SetView(System.Windows.Forms.View View)
		{
			ToolStripMenuItem largeIconsToolStripMenuItem = null;
			IEnumerator enumerator = null;
			switch (View)
			{
				case System.Windows.Forms.View.LargeIcon:
				{
					largeIconsToolStripMenuItem = this.LargeIconsToolStripMenuItem;
					break;
				}
				case System.Windows.Forms.View.Details:
				{
					largeIconsToolStripMenuItem = this.DetailsToolStripMenuItem;
					break;
				}
				case System.Windows.Forms.View.SmallIcon:
				{
					largeIconsToolStripMenuItem = this.SmallIconsToolStripMenuItem;
					break;
				}
				case System.Windows.Forms.View.List:
				{
					largeIconsToolStripMenuItem = this.ListToolStripMenuItem;
					break;
				}
				case System.Windows.Forms.View.Tile:
				{
					largeIconsToolStripMenuItem = this.TileToolStripMenuItem;
					break;
				}
				default:
				{
					View = System.Windows.Forms.View.Details;
					largeIconsToolStripMenuItem = this.DetailsToolStripMenuItem;
					break;
				}
			}
			try
			{
				enumerator = this.ListViewToolStripButton.DropDownItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ToolStripMenuItem current = (ToolStripMenuItem)enumerator.Current;
					if (current != largeIconsToolStripMenuItem)
					{
						current.Checked = false;
					}
					else
					{
						current.Checked = true;
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			this.ListView.View = View;
		}

		private void SmallIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetView(View.SmallIcon);
		}

		private void TileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.SetView(View.Tile);
		}
	}
}