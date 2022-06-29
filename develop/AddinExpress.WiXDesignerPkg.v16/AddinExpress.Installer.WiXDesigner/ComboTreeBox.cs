using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	[DesignerCategory("")]
	[ToolboxItem(true)]
	internal class ComboTreeBox : DropDownControlBase
	{
		private const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.PathEllipsis | TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter;

		private ComboTreeDropDown _dropDown;

		private int _expandedImageIndex;

		private string _expandedImageKey;

		private int _imageIndex;

		private string _imageKey;

		private ImageList _images;

		private bool _isUpdating;

		private ComboTreeNodeCollection _nodes;

		private string _nullValue;

		private string _pathSeparator;

		private ComboTreeNode _selectedNode;

		private bool _showPath;

		private bool _useNodeNamesForPath;

		[Browsable(false)]
		public IEnumerable<ComboTreeNode> AllNodes
		{
			get
			{
				ComboTreeBox comboTreeBox = null;
				IEnumerator<ComboTreeNode> nodesRecursive = comboTreeBox.GetNodesRecursive(comboTreeBox._nodes, false);
				while (nodesRecursive.MoveNext())
				{
					yield return nodesRecursive.Current;
				}
			}
		}

		[Category("Behavior")]
		[DefaultValue(150)]
		[Description("The height of the dropdown portion of the control.")]
		public int DropDownHeight
		{
			get
			{
				return this._dropDown.DropDownHeight;
			}
			set
			{
				this._dropDown.DropDownHeight = value;
			}
		}

		[Browsable(false)]
		public override bool DroppedDown
		{
			get
			{
				return base.DroppedDown;
			}
			set
			{
				this.SetDroppedDown(value, true);
			}
		}

		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The index of the default image to use for nodes when expanded.")]
		public int ExpandedImageIndex
		{
			get
			{
				return this._expandedImageIndex;
			}
			set
			{
				this._expandedImageIndex = value;
				this._dropDown.UpdateVisibleItems();
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The name of the default image to use for nodes when expanded.")]
		public string ExpandedImageKey
		{
			get
			{
				return this._expandedImageKey;
			}
			set
			{
				this._expandedImageKey = value;
				this._dropDown.UpdateVisibleItems();
			}
		}

		[Category("Appearance")]
		[DefaultValue(0)]
		[Description("The index of the default image to use for nodes.")]
		public int ImageIndex
		{
			get
			{
				return this._imageIndex;
			}
			set
			{
				this._imageIndex = value;
				this._dropDown.UpdateVisibleItems();
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The name of the default image to use for nodes.")]
		public string ImageKey
		{
			get
			{
				return this._imageKey;
			}
			set
			{
				this._imageKey = value;
				this._dropDown.UpdateVisibleItems();
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("An ImageList component which provides the images displayed beside nodes in the control.")]
		public ImageList Images
		{
			get
			{
				return this._images;
			}
			set
			{
				this._images = value;
				this._dropDown.UpdateVisibleItems();
			}
		}

		[Category("Data")]
		[Description("The collection of top-level nodes contained by the control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ComboTreeNodeCollection Nodes
		{
			get
			{
				return this._nodes;
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The text displayed in the editable portion of the control if the SelectedNode property is null.")]
		public string NullValue
		{
			get
			{
				return this._nullValue;
			}
			set
			{
				this._nullValue = value;
				base.Invalidate();
			}
		}

		[Category("Behavior")]
		[DefaultValue("")]
		[Description("The path to the selected node.")]
		public string Path
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this._selectedNode != null)
				{
					stringBuilder.Append((this._useNodeNamesForPath ? this._selectedNode.Name : this._selectedNode.Text));
					ComboTreeNode comboTreeNode = this._selectedNode;
					while (true)
					{
						ComboTreeNode parent = comboTreeNode.Parent;
						comboTreeNode = parent;
						if (parent == null)
						{
							break;
						}
						stringBuilder.Insert(0, this._pathSeparator);
						stringBuilder.Insert(0, (this._useNodeNamesForPath ? comboTreeNode.Name : comboTreeNode.Text));
					}
				}
				return stringBuilder.ToString();
			}
			set
			{
				ComboTreeNode item = null;
				string[] strArrays = value.Split(new string[] { this._pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					ComboTreeNodeCollection comboTreeNodeCollections = (item == null ? this._nodes : item.Nodes);
					if (!this._useNodeNamesForPath)
					{
						bool flag = false;
						foreach (ComboTreeNode comboTreeNode in comboTreeNodeCollections)
						{
							if (!comboTreeNode.Text.Equals(strArrays[i], StringComparison.InvariantCultureIgnoreCase))
							{
								continue;
							}
							item = comboTreeNode;
							flag = true;
							goto Label0;
						}
					Label0:
						if (!flag)
						{
							throw new ArgumentException("Invalid path string.", "value");
						}
					}
					else
					{
						try
						{
							item = comboTreeNodeCollections[strArrays[i]];
						}
						catch (KeyNotFoundException keyNotFoundException)
						{
							throw new ArgumentException("Invalid path string.", "value", keyNotFoundException);
						}
					}
				}
				this.SelectedNode = item;
			}
		}

		[Category("Behavior")]
		[DefaultValue("\\")]
		[Description("The string used to separate nodes in the path string.")]
		public string PathSeparator
		{
			get
			{
				return this._pathSeparator;
			}
			set
			{
				this._pathSeparator = value;
				if (this._showPath)
				{
					base.Invalidate();
				}
			}
		}

		[Browsable(false)]
		public ComboTreeNode SelectedNode
		{
			get
			{
				return this._selectedNode;
			}
			set
			{
				if (!this.OwnsNode(value))
				{
					throw new ArgumentException("Node does not belong to this control.", "value");
				}
				this.SetSelectedNode(value);
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("Determines whether the path to the selected node is displayed in the editable portion of the control.")]
		public bool ShowPath
		{
			get
			{
				return this._showPath;
			}
			set
			{
				this._showPath = value;
				base.Invalidate();
			}
		}

		internal bool ShowsFocusCues
		{
			get
			{
				return base.ShowFocusCues;
			}
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				return string.Empty;
			}
			set
			{
				base.Text = string.Empty;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ComboTreeNode TopNode
		{
			get
			{
				return this._dropDown.TopNode;
			}
			set
			{
				this._dropDown.TopNode = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Determines whether the Name property of the nodes is used to construct the path string. The default behaviour is to use the Text property.")]
		public bool UseNodeNamesForPath
		{
			get
			{
				return this._useNodeNamesForPath;
			}
			set
			{
				this._useNodeNamesForPath = value;
				if (this._showPath)
				{
					base.Invalidate();
				}
			}
		}

		[Browsable(false)]
		public int VisibleCount
		{
			get
			{
				return this._dropDown.VisibleCount;
			}
		}

		public ComboTreeBox()
		{
			this._nullValue = string.Empty;
			this._pathSeparator = "\\";
			int num = 0;
			int num1 = num;
			this._imageIndex = num;
			this._expandedImageIndex = num1;
			string empty = string.Empty;
			string str = empty;
			this._imageKey = empty;
			this._expandedImageKey = str;
			this._nodes = new ComboTreeNodeCollection(null);
			this._nodes.CollectionChanged += new NotifyCollectionChangedEventHandler(this.nodes_CollectionChanged);
			this._dropDown = new ComboTreeDropDown(this);
			this._dropDown.Opened += new EventHandler(this.dropDown_Opened);
			this._dropDown.Closed += new ToolStripDropDownClosedEventHandler(this.dropDown_Closed);
			this._dropDown.UpdateVisibleItems();
		}

		public void BeginUpdate()
		{
			this._isUpdating = true;
		}

		public void CollapseAll()
		{
			foreach (ComboTreeNode allNode in this.AllNodes)
			{
				allNode.Expanded = false;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._dropDown.Dispose();
			}
			base.Dispose(disposing);
		}

		private void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			this.OnDropDownClosed(EventArgs.Empty);
		}

		private void dropDown_Opened(object sender, EventArgs e)
		{
			this.OnDropDown(EventArgs.Empty);
		}

		public void EndUpdate()
		{
			this._isUpdating = false;
			if (!this.OwnsNode(this._selectedNode))
			{
				this.SetSelectedNode(null);
			}
			this._dropDown.UpdateVisibleItems();
		}

		public void ExpandAll()
		{
			foreach (ComboTreeNode allNode in this.AllNodes)
			{
				if (allNode.Nodes.Count <= 0)
				{
					continue;
				}
				allNode.Expanded = true;
			}
		}

		private ComboTreeNode GetNextDisplayedNode()
		{
			bool flag = false;
			IEnumerator<ComboTreeNode> nodesRecursive = this.GetNodesRecursive(this._nodes, false);
			while (nodesRecursive.MoveNext())
			{
				if (flag || this._selectedNode == null)
				{
					if (!this.IsNodeVisible(nodesRecursive.Current))
					{
						continue;
					}
					return nodesRecursive.Current;
				}
				else
				{
					if (nodesRecursive.Current != this._selectedNode)
					{
						continue;
					}
					flag = true;
				}
			}
			return null;
		}

		internal Image GetNodeImage(ComboTreeNode node)
		{
			if (this._images != null && node != null)
			{
				if (!node.Expanded)
				{
					if (this._images.Images.ContainsKey(node.ImageKey))
					{
						return this._images.Images[node.ImageKey];
					}
					if (node.ImageIndex >= 0)
					{
						return this._images.Images[node.ImageIndex];
					}
					if (this._images.Images.ContainsKey(this._imageKey))
					{
						return this._images.Images[this._imageKey];
					}
					if (this._imageIndex >= 0)
					{
						return this._images.Images[this._imageIndex];
					}
				}
				else
				{
					if (this._images.Images.ContainsKey(node.ExpandedImageKey))
					{
						return this._images.Images[node.ExpandedImageKey];
					}
					if (node.ExpandedImageIndex >= 0)
					{
						return this._images.Images[node.ExpandedImageIndex];
					}
					if (this._images.Images.ContainsKey(this._expandedImageKey))
					{
						return this._images.Images[this._expandedImageKey];
					}
					if (this._expandedImageIndex >= 0)
					{
						return this._images.Images[this._expandedImageIndex];
					}
				}
			}
			return null;
		}

		private IEnumerator<ComboTreeNode> GetNodesRecursive(ComboTreeNodeCollection collection, bool reverse)
		{
			ComboTreeBox comboTreeBox = null;
			int i;
			IEnumerator<ComboTreeNode> nodesRecursive;
			if (reverse)
			{
				for (i = collection.Count - 1; i >= 0; i--)
				{
					nodesRecursive = comboTreeBox.GetNodesRecursive(collection[i].Nodes, reverse);
					while (nodesRecursive.MoveNext())
					{
						yield return nodesRecursive.Current;
					}
					yield return collection[i];
					nodesRecursive = null;
				}
			}
			else
			{
				for (i = 0; i < collection.Count; i++)
				{
					yield return collection[i];
					nodesRecursive = comboTreeBox.GetNodesRecursive(collection[i].Nodes, reverse);
					while (nodesRecursive.MoveNext())
					{
						yield return nodesRecursive.Current;
					}
					nodesRecursive = null;
				}
			}
		}

		private ComboTreeNode GetPrevDisplayedNode()
		{
			bool flag = false;
			IEnumerator<ComboTreeNode> nodesRecursive = this.GetNodesRecursive(this._nodes, true);
			while (nodesRecursive.MoveNext())
			{
				if (flag || this._selectedNode == null)
				{
					if (!this.IsNodeVisible(nodesRecursive.Current))
					{
						continue;
					}
					return nodesRecursive.Current;
				}
				else
				{
					if (nodesRecursive.Current != this._selectedNode)
					{
						continue;
					}
					flag = true;
				}
			}
			return null;
		}

		internal bool IsNodeVisible(ComboTreeNode node)
		{
			bool flag = true;
			ComboTreeNode comboTreeNode = node;
			while (true)
			{
				ComboTreeNode parent = comboTreeNode.Parent;
				comboTreeNode = parent;
				if (parent == null)
				{
					break;
				}
				if (!comboTreeNode.Expanded)
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		private void nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!this._isUpdating)
			{
				if (!this.OwnsNode(this._selectedNode))
				{
					this.SetSelectedNode(null);
				}
				this._dropDown.UpdateVisibleItems();
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this._dropDown.Font = this.Font;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			int num = 1;
			bool flag = (bool)num;
			e.SuppressKeyPress = (bool)num;
			e.Handled = flag;
			if (e.Alt && e.KeyCode == Keys.Down)
			{
				this.DroppedDown = true;
			}
			else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
			{
				ComboTreeNode prevDisplayedNode = this.GetPrevDisplayedNode();
				if (prevDisplayedNode != null)
				{
					this.SetSelectedNode(prevDisplayedNode);
				}
			}
			else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
			{
				ComboTreeNode nextDisplayedNode = this.GetNextDisplayedNode();
				if (nextDisplayedNode != null)
				{
					this.SetSelectedNode(nextDisplayedNode);
				}
			}
			else
			{
				int num1 = 0;
				flag = (bool)num1;
				e.SuppressKeyPress = (bool)num1;
				e.Handled = flag;
			}
			base.OnKeyDown(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (!this._dropDown.Focused)
			{
				this._dropDown.Close();
			}
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				this.DroppedDown = !this.DroppedDown;
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			((HandledMouseEventArgs)e).Handled = true;
			base.OnMouseWheel(e);
			if (this.DroppedDown)
			{
				this._dropDown.ScrollDropDown(-(e.Delta / 120) * SystemInformation.MouseWheelScrollLines);
				return;
			}
			if (e.Delta > 0)
			{
				ComboTreeNode prevDisplayedNode = this.GetPrevDisplayedNode();
				if (prevDisplayedNode != null)
				{
					this.SetSelectedNode(prevDisplayedNode);
					return;
				}
			}
			else if (e.Delta < 0)
			{
				ComboTreeNode nextDisplayedNode = this.GetNextDisplayedNode();
				if (nextDisplayedNode != null)
				{
					this.SetSelectedNode(nextDisplayedNode);
				}
			}
		}

		protected override void OnPaintContent(DropDownPaintEventArgs e)
		{
			Rectangle bounds;
			string str;
			Rectangle rectangle;
			base.OnPaintContent(e);
			Image nodeImage = this.GetNodeImage(this._selectedNode);
			if (this._selectedNode != null)
			{
				str = (this._showPath ? this.Path : this._selectedNode.Text);
			}
			else
			{
				str = this._nullValue;
			}
			string str1 = str;
			if (nodeImage == null)
			{
				rectangle = new Rectangle(1, 0, 0, 0);
			}
			else
			{
				bounds = e.Bounds;
				rectangle = new Rectangle(4, bounds.Height / 2 - nodeImage.Height / 2, nodeImage.Width, nodeImage.Height);
			}
			Rectangle rectangle1 = rectangle;
			int right = rectangle1.Right;
			bounds = e.Bounds;
			int width = bounds.Width - rectangle1.Right - 3;
			bounds = e.Bounds;
			Rectangle rectangle2 = new Rectangle(right, 0, width, bounds.Height);
			if (nodeImage != null)
			{
				e.Graphics.DrawImage(nodeImage, rectangle1);
			}
			TextRenderer.DrawText(e.Graphics, str1, this.Font, rectangle2, this.ForeColor, TextFormatFlags.PathEllipsis | TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter);
			if (this.Focused && this.ShowFocusCues && !this.DroppedDown)
			{
				e.DrawFocusRectangle();
			}
		}

		protected virtual void OnSelectedNodeChanged(EventArgs e)
		{
			if (this.SelectedNodeChanged != null)
			{
				this.SelectedNodeChanged(this, e);
			}
		}

		private bool OwnsNode(ComboTreeNode node)
		{
			if (node == null)
			{
				return true;
			}
			ComboTreeNode parent = node;
			while (parent.Parent != null)
			{
				parent = parent.Parent;
			}
			return this._nodes.Contains(parent);
		}

		internal void SetDroppedDown(bool droppedDown, bool raiseEvents)
		{
			base.DroppedDown = droppedDown;
			if (raiseEvents)
			{
				if (droppedDown)
				{
					this._dropDown.Open();
					return;
				}
				this._dropDown.Close();
			}
		}

		private void SetSelectedNode(ComboTreeNode node)
		{
			if (this._selectedNode != node)
			{
				this._selectedNode = node;
				base.Invalidate();
				this.OnSelectedNodeChanged(EventArgs.Empty);
			}
		}

		public void Sort()
		{
			this.Sort(null);
		}

		public void Sort(IComparer<ComboTreeNode> comparer)
		{
			bool flag = this._isUpdating;
			this._isUpdating = true;
			this._nodes.Sort(comparer);
			if (!flag)
			{
				this.EndUpdate();
			}
		}

		[Description("Occurs when the SelectedNode property changes.")]
		public event EventHandler SelectedNodeChanged;
	}
}