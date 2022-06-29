using System;
using System.ComponentModel;
using System.Drawing;

namespace AddinExpress.Installer.WiXDesigner
{
	[DefaultProperty("Text")]
	internal class ComboTreeNode : IComparable<ComboTreeNode>
	{
		private string _name;

		private ComboTreeNodeCollection _nodes;

		private string _text;

		private System.Drawing.FontStyle _fontStyle;

		private int _imageIndex;

		private string _imageKey;

		private bool _expanded;

		private int _expandedImageIndex;

		private string _expandedImageKey;

		private ComboTreeNode _parent;

		private object _tag;

		[Browsable(false)]
		public int Depth
		{
			get
			{
				int num = 0;
				ComboTreeNode comboTreeNode = this;
				while (true)
				{
					ComboTreeNode comboTreeNode1 = comboTreeNode._parent;
					comboTreeNode = comboTreeNode1;
					if (comboTreeNode1 == null)
					{
						break;
					}
					num++;
				}
				return num;
			}
		}

		[Browsable(false)]
		public bool Expanded
		{
			get
			{
				return this._expanded;
			}
			set
			{
				this._expanded = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(-1)]
		[Description("The index of the image to use for this node when expanded.")]
		public int ExpandedImageIndex
		{
			get
			{
				return this._expandedImageIndex;
			}
			set
			{
				this._expandedImageIndex = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The name of the image to use for this node when expanded.")]
		public string ExpandedImageKey
		{
			get
			{
				return this._expandedImageKey;
			}
			set
			{
				this._expandedImageKey = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(System.Drawing.FontStyle.Regular)]
		[Description("The font style to use when painting the node.")]
		public System.Drawing.FontStyle FontStyle
		{
			get
			{
				return this._fontStyle;
			}
			set
			{
				this._fontStyle = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(-1)]
		[Description("The index of the image (in the ImageList on the ComboTreeBox control) to use for this node.")]
		public int ImageIndex
		{
			get
			{
				return this._imageIndex;
			}
			set
			{
				this._imageIndex = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue("")]
		[Description("The name of the image to use for this node.")]
		public string ImageKey
		{
			get
			{
				return this._imageKey;
			}
			set
			{
				this._imageKey = value;
			}
		}

		[Category("Design")]
		[DefaultValue("")]
		[Description("The name of the node.")]
		public string Name
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

		[Category("Data")]
		[Description("The collection of the child nodes for this node.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ComboTreeNodeCollection Nodes
		{
			get
			{
				return this._nodes;
			}
		}

		[Browsable(false)]
		public ComboTreeNode Parent
		{
			get
			{
				return this._parent;
			}
			internal set
			{
				this._parent = value;
			}
		}

		[Category("Data")]
		[DefaultValue(null)]
		[Description("User-defined object associated with this ComboTreeNode.")]
		public object Tag
		{
			get
			{
				return this._tag;
			}
			set
			{
				this._tag = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue("ComboTreeNode")]
		[Description("The text displayed on the node.")]
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		public ComboTreeNode()
		{
			this._nodes = new ComboTreeNodeCollection(this);
			string empty = string.Empty;
			string str = empty;
			this._text = empty;
			this._name = str;
			this._fontStyle = System.Drawing.FontStyle.Regular;
			int num = -1;
			int num1 = num;
			this._imageIndex = num;
			this._expandedImageIndex = num1;
			string empty1 = string.Empty;
			str = empty1;
			this._imageKey = empty1;
			this._expandedImageKey = str;
			this._expanded = false;
		}

		public ComboTreeNode(string text) : this()
		{
			this._text = text;
		}

		public ComboTreeNode(string name, string text) : this()
		{
			this._text = text;
			this._name = name;
		}

		public int CompareTo(ComboTreeNode other)
		{
			return StringComparer.InvariantCultureIgnoreCase.Compare(this._text, other._text);
		}

		public override string ToString()
		{
			return string.Format("Name=\"{0}\", Text=\"{1}\"", this._name, this._text);
		}
	}
}