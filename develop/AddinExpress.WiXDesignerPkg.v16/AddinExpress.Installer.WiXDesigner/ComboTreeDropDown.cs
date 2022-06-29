using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AddinExpress.Installer.WiXDesigner
{
	[DesignerCategory("")]
	[ToolboxItem(false)]
	internal class ComboTreeDropDown : ToolStripDropDown
	{
		private const int GLYPH_SIZE = 16;

		private const int INDENT_WIDTH = 4;

		private const int TEXT_INDENT_WIDTH = 8;

		private const int MIN_ITEM_HEIGHT = 18;

		private const int MIN_THUMB_HEIGHT = 20;

		private const int SCROLLBAR_WIDTH = 17;

		private readonly System.Drawing.Size SCROLLBUTTON_SIZE = new System.Drawing.Size(17, 17);

		private const TextFormatFlags TEXT_FORMAT_FLAGS = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

		private static Bitmap _collapsed;

		private static Bitmap _expanded;

		private Dictionary<ComboTreeDropDown.BitmapInfo, Image> _bitmaps;

		private int _dropDownHeight;

		private int _highlightedItemIndex;

		private Rectangle _interior;

		private int _itemHeight;

		private int _numItemsDisplayed;

		private bool _scrollBarVisible;

		private ComboTreeDropDown.ScrollBarInfo _scrollBar;

		private bool _scrollDragging;

		private int _scrollOffset;

		private Timer _scrollRepeater;

		private ComboTreeBox _sourceControl;

		private List<ComboTreeDropDown.NodeInfo> _visibleItems;

		private Image Collapsed
		{
			get
			{
				if (ComboTreeDropDown._collapsed == null)
				{
					ComboTreeDropDown._collapsed = new Bitmap(16, 16);
					Graphics graphic = Graphics.FromImage(ComboTreeDropDown._collapsed);
					Rectangle rectangle = new Rectangle(4, 4, 8, 8);
					graphic.FillRectangle(Brushes.White, rectangle);
					graphic.DrawRectangle(Pens.Gray, rectangle);
					graphic.DrawLine(Pens.Black, Point.Add(rectangle.Location, new System.Drawing.Size(2, 4)), Point.Add(rectangle.Location, new System.Drawing.Size(6, 4)));
					graphic.DrawLine(Pens.Black, Point.Add(rectangle.Location, new System.Drawing.Size(4, 2)), Point.Add(rectangle.Location, new System.Drawing.Size(4, 6)));
				}
				return ComboTreeDropDown._collapsed;
			}
		}

		protected override System.Windows.Forms.Padding DefaultPadding
		{
			get
			{
				return new System.Windows.Forms.Padding(0, 1, 0, 1);
			}
		}

		public int DropDownHeight
		{
			get
			{
				return this._dropDownHeight;
			}
			set
			{
				this._dropDownHeight = value;
				this.UpdateVisibleItems();
			}
		}

		private Image Expanded
		{
			get
			{
				if (ComboTreeDropDown._expanded == null)
				{
					ComboTreeDropDown._expanded = new Bitmap(16, 16);
					Graphics graphic = Graphics.FromImage(ComboTreeDropDown._expanded);
					Rectangle rectangle = new Rectangle(4, 4, 8, 8);
					graphic.FillRectangle(Brushes.White, rectangle);
					graphic.DrawRectangle(Pens.Gray, rectangle);
					graphic.DrawLine(Pens.Black, Point.Add(rectangle.Location, new System.Drawing.Size(2, 4)), Point.Add(rectangle.Location, new System.Drawing.Size(6, 4)));
				}
				return ComboTreeDropDown._expanded;
			}
		}

		public ComboTreeNode TopNode
		{
			get
			{
				return this._visibleItems[this._scrollOffset].Node;
			}
			set
			{
				int num = 0;
				while (num < this._visibleItems.Count)
				{
					if (this._visibleItems[num].Node != value)
					{
						num++;
					}
					else
					{
						if (num >= this._scrollOffset && num < this._scrollOffset + this._numItemsDisplayed)
						{
							break;
						}
						this._scrollOffset = Math.Min(Math.Max(0, num - this._numItemsDisplayed + 1), this._visibleItems.Count - this._numItemsDisplayed);
						this.UpdateScrolling();
						return;
					}
				}
			}
		}

		public int VisibleCount
		{
			get
			{
				return this._numItemsDisplayed;
			}
		}

		public ComboTreeDropDown(ComboTreeBox sourceControl)
		{
			this._visibleItems = new List<ComboTreeDropDown.NodeInfo>();
			this._bitmaps = new Dictionary<ComboTreeDropDown.BitmapInfo, Image>();
			this._scrollBar = new ComboTreeDropDown.ScrollBarInfo();
			this.AutoSize = false;
			this._sourceControl = sourceControl;
			base.RenderMode = ToolStripRenderMode.System;
			base.BackColor = Color.White;
			this._dropDownHeight = 150;
			this._itemHeight = 18;
			this.Items.Add("");
			this._scrollRepeater = new Timer();
			this._scrollRepeater.Tick += new EventHandler(this.scrollRepeater_Tick);
		}

		private Image GenerateBitmap(ComboTreeDropDown.BitmapInfo bitmapInfo, Image nodeImage)
		{
			int nodeDepth = 8 * bitmapInfo.NodeDepth;
			int num = this._itemHeight / 2;
			Bitmap bitmap = new Bitmap(4 + nodeDepth + (nodeImage != null ? nodeImage.Width : 0), this._itemHeight);
			Graphics graphic = Graphics.FromImage(bitmap);
			if (nodeImage != null)
			{
				graphic.DrawImage(nodeImage, new Rectangle(4 + nodeDepth, bitmap.Height / 2 - nodeImage.Height / 2, nodeImage.Width, nodeImage.Height));
			}
			return bitmap;
		}

		private ButtonState GetButtonState(Rectangle bounds)
		{
			ButtonState buttonState = ButtonState.Normal;
			if (bounds.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)) && !this._scrollDragging && (Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left)
			{
				buttonState = ButtonState.Pushed;
			}
			return buttonState;
		}

		private ComboTreeNodeCollection GetCollectionContainingNode(ComboTreeNode node)
		{
			if (node.Parent == null)
			{
				return this._sourceControl.Nodes;
			}
			return node.Parent.Nodes;
		}

		private Image GetItemBitmap(ComboTreeNode node)
		{
			ComboTreeDropDown.BitmapInfo count = new ComboTreeDropDown.BitmapInfo();
			ComboTreeNodeCollection collectionContainingNode = this.GetCollectionContainingNode(node);
			count.HasChildren = node.Nodes.Count > 0;
			count.IsLastPeer = collectionContainingNode.IndexOf(node) == collectionContainingNode.Count - 1;
			count.IsFirst = node == this._sourceControl.Nodes[0];
			count.NodeDepth = node.Depth;
			count.NodeExpanded = (!node.Expanded ? false : count.HasChildren);
			count.ImageIndex = (count.NodeExpanded ? node.ExpandedImageIndex : node.ImageIndex);
			count.ImageKey = (count.NodeExpanded ? node.ExpandedImageKey : node.ImageKey);
			count.VerticalLines = new bool[count.NodeDepth];
			ComboTreeNode comboTreeNode = node;
			int num = 0;
			while (true)
			{
				ComboTreeNode parent = comboTreeNode.Parent;
				comboTreeNode = parent;
				if (parent == null)
				{
					break;
				}
				ComboTreeNodeCollection comboTreeNodeCollections = this.GetCollectionContainingNode(comboTreeNode);
				count.VerticalLines[num] = (!comboTreeNode.Expanded ? false : comboTreeNodeCollections.IndexOf(comboTreeNode) != comboTreeNodeCollections.Count - 1);
				num++;
			}
			if (this._bitmaps.ContainsKey(count))
			{
				return this._bitmaps[count];
			}
			Dictionary<ComboTreeDropDown.BitmapInfo, Image> bitmapInfos = this._bitmaps;
			Image image = this.GenerateBitmap(count, this._sourceControl.GetNodeImage(node));
			Image image1 = image;
			bitmapInfos[count] = image;
			return image1;
		}

		private ScrollBarState GetScrollBarState(Rectangle bounds)
		{
			ScrollBarState scrollBarState = ScrollBarState.Normal;
			Point client = base.PointToClient(System.Windows.Forms.Cursor.Position);
			if (bounds.Contains(client) && !this._scrollDragging && !this._scrollBar.DownArrow.Contains(client) && !this._scrollBar.UpArrow.Contains(client) && !this._scrollBar.Thumb.Contains(client))
			{
				scrollBarState = ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left ? ScrollBarState.Hot : ScrollBarState.Pressed);
			}
			return scrollBarState;
		}

		private ScrollBarArrowButtonState GetScrollBarStateDown()
		{
			ScrollBarArrowButtonState scrollBarArrowButtonState = ScrollBarArrowButtonState.DownNormal;
			if (this._scrollBar.DownArrow.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)) && !this._scrollDragging)
			{
				scrollBarArrowButtonState = ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left ? ScrollBarArrowButtonState.DownHot : ScrollBarArrowButtonState.DownPressed);
			}
			return scrollBarArrowButtonState;
		}

		private ScrollBarArrowButtonState GetScrollBarStateUp()
		{
			ScrollBarArrowButtonState scrollBarArrowButtonState = ScrollBarArrowButtonState.UpNormal;
			if (this._scrollBar.UpArrow.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)) && !this._scrollDragging)
			{
				scrollBarArrowButtonState = ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left ? ScrollBarArrowButtonState.UpHot : ScrollBarArrowButtonState.UpPressed);
			}
			return scrollBarArrowButtonState;
		}

		private ScrollBarState GetScrollBarThumbState()
		{
			ScrollBarState scrollBarState = ScrollBarState.Normal;
			if (this._scrollBar.Thumb.Contains(base.PointToClient(System.Windows.Forms.Cursor.Position)))
			{
				scrollBarState = ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left ? ScrollBarState.Hot : ScrollBarState.Pressed);
			}
			return scrollBarState;
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Return || (int)keyData - (int)Keys.Prior <= (int)(Keys.LButton | Keys.RButton | Keys.Cancel | Keys.MButton | Keys.XButton1 | Keys.XButton2))
			{
				return true;
			}
			return base.IsInputKey(keyData);
		}

		protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
		{
			base.OnClosed(e);
			this._sourceControl.SetDroppedDown(false, false);
		}

		protected override void OnClosing(ToolStripDropDownClosingEventArgs e)
		{
			if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
			{
				e.Cancel = true;
			}
			if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked && this._sourceControl.ClientRectangle.Contains(this._sourceControl.PointToClient(System.Windows.Forms.Cursor.Position)))
			{
				e.Cancel = true;
			}
			base.OnClosing(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this._itemHeight = Math.Max(18, this.Font.Height);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			int num;
			int num1 = 1;
			bool flag = (bool)num1;
			e.SuppressKeyPress = (bool)num1;
			e.Handled = flag;
			if (e.KeyCode == Keys.Return || e.Alt && e.KeyCode == Keys.Up)
			{
				this._sourceControl.SelectedNode = this._visibleItems[this._highlightedItemIndex].Node;
				base.Close();
			}
			else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
			{
				this._highlightedItemIndex = Math.Max(0, this._highlightedItemIndex - 1);
				this._sourceControl.SelectedNode = this._visibleItems[this._highlightedItemIndex].Node;
				this.ScrollToHighlighted(true);
				this.Refresh();
			}
			else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
			{
				this._highlightedItemIndex = Math.Min(this._highlightedItemIndex + 1, this._visibleItems.Count - 1);
				this._sourceControl.SelectedNode = this._visibleItems[this._highlightedItemIndex].Node;
				this.ScrollToHighlighted(false);
				this.Refresh();
			}
			else if (e.KeyCode == Keys.Home)
			{
				int num2 = 0;
				num = num2;
				this._scrollOffset = num2;
				this._highlightedItemIndex = num;
				this.UpdateScrolling();
				base.Invalidate();
			}
			else if (e.KeyCode == Keys.End)
			{
				this._scrollOffset = this._visibleItems.Count - this._numItemsDisplayed;
				this._highlightedItemIndex = this._visibleItems.Count - 1;
				this.UpdateScrolling();
				base.Invalidate();
			}
			else if (e.KeyCode == Keys.Next)
			{
				this._scrollOffset = Math.Min(this._scrollOffset + this._numItemsDisplayed, this._visibleItems.Count - this._numItemsDisplayed);
				this._highlightedItemIndex = Math.Min(this._scrollOffset + this._numItemsDisplayed - 1, this._visibleItems.Count - 1);
				this.UpdateScrolling();
				this.Refresh();
			}
			else if (e.KeyCode != Keys.Prior)
			{
				int num3 = 0;
				flag = (bool)num3;
				e.SuppressKeyPress = (bool)num3;
				e.Handled = flag;
			}
			else
			{
				int num4 = Math.Max(this._scrollOffset - this._numItemsDisplayed, 0);
				num = num4;
				this._scrollOffset = num4;
				this._highlightedItemIndex = num;
				this.UpdateScrolling();
				this.Refresh();
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			e.Handled = true;
			if (e.KeyChar == '+')
			{
				ComboTreeDropDown.NodeInfo item = this._visibleItems[this._highlightedItemIndex];
				if (item.Node.Nodes.Count > 0)
				{
					item.Node.Expanded = true;
					this.UpdateVisibleItems();
				}
			}
			else if (e.KeyChar == '-')
			{
				ComboTreeDropDown.NodeInfo nodeInfo = this._visibleItems[this._highlightedItemIndex];
				if (nodeInfo.Node.Nodes.Count > 0)
				{
					nodeInfo.Node.Expanded = false;
					this.UpdateVisibleItems();
				}
			}
			if (e.KeyChar == '*')
			{
				this._sourceControl.ExpandAll();
				this.UpdateVisibleItems();
			}
			else if (e.KeyChar != '/')
			{
				e.Handled = false;
			}
			else
			{
				this._sourceControl.CollapseAll();
				this.UpdateVisibleItems();
			}
			base.OnKeyPress(e);
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (this._scrollDragging)
			{
				return;
			}
			if (e.Button == System.Windows.Forms.MouseButtons.Left)
			{
				for (int i = this._scrollOffset; i < this._scrollOffset + this._numItemsDisplayed; i++)
				{
					ComboTreeDropDown.NodeInfo item = this._visibleItems[i];
					if (item.DisplayRectangle.Contains(e.Location))
					{
						if (item.GlyphRectangle.Contains(e.Location))
						{
							item.Node.Expanded = !item.Node.Expanded;
							this.UpdateVisibleItems();
							return;
						}
						this._sourceControl.SelectedNode = this._visibleItems[i].Node;
						base.Close();
						return;
					}
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != System.Windows.Forms.MouseButtons.Left)
			{
				return;
			}
			if (this._scrollBarVisible && this._scrollBar.DisplayRectangle.Contains(e.Location))
			{
				if (e.Y > this._scrollBar.Thumb.Bottom)
				{
					this.ScrollDropDown((this._scrollBar.DownArrow.Contains(e.Location) ? 1 : this._numItemsDisplayed));
					if (!this._scrollRepeater.Enabled)
					{
						this._scrollRepeater.Interval = 250;
						this._scrollRepeater.Start();
					}
					return;
				}
				if (e.Y < this._scrollBar.Thumb.Top)
				{
					this.ScrollDropDown(-(this._scrollBar.UpArrow.Contains(e.Location) ? 1 : this._numItemsDisplayed));
					if (!this._scrollRepeater.Enabled)
					{
						this._scrollRepeater.Interval = 250;
						this._scrollRepeater.Start();
					}
					return;
				}
				if (this._scrollBar.Thumb.Contains(e.Location))
				{
					this._scrollDragging = true;
				}
				base.Invalidate();
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left)
			{
				this._scrollDragging = false;
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if ((Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) != System.Windows.Forms.MouseButtons.Left)
			{
				this._scrollDragging = false;
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (!this._scrollDragging)
			{
				if (this._scrollBarVisible && this._scrollBar.DisplayRectangle.Contains(e.Location))
				{
					base.Invalidate();
					return;
				}
				this._scrollRepeater.Stop();
				for (int i = this._scrollOffset; i < this._scrollOffset + this._numItemsDisplayed; i++)
				{
					if (this._visibleItems[i].DisplayRectangle.Contains(e.Location))
					{
						this._highlightedItemIndex = i;
						base.Invalidate();
						return;
					}
				}
				return;
			}
			int height = this._scrollBar.DisplayRectangle.Height - 2 * this.SCROLLBUTTON_SIZE.Height;
			Rectangle thumb = this._scrollBar.Thumb;
			double num = (double)(height - thumb.Height);
			int y = e.Location.Y;
			thumb = this._scrollBar.DisplayRectangle;
			int top = y - thumb.Top - this.SCROLLBUTTON_SIZE.Height;
			thumb = this._scrollBar.Thumb;
			double num1 = Math.Min((double)(top - thumb.Height / 2), num);
			this._scrollOffset = Math.Max(0, Math.Min((int)(num1 / num * (double)(this._visibleItems.Count - this._numItemsDisplayed)), this._visibleItems.Count - this._numItemsDisplayed));
			this.UpdateScrolling();
			this.Refresh();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			this._scrollRepeater.Stop();
			this._scrollDragging = false;
			if (!this._scrollBarVisible || !this._scrollBar.DisplayRectangle.Contains(e.Location))
			{
				return;
			}
			base.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle displayRectangle;
			base.OnPaint(e);
			if (this._scrollBarVisible)
			{
				int left = this._scrollBar.DisplayRectangle.Left;
				int top = this._scrollBar.DisplayRectangle.Top;
				int width = this._scrollBar.DisplayRectangle.Width;
				int num = this._scrollBar.Thumb.Top;
				displayRectangle = this._scrollBar.DisplayRectangle;
				Rectangle rectangle = new Rectangle(left, top, width, num - displayRectangle.Top);
				int left1 = this._scrollBar.DisplayRectangle.Left;
				int bottom = this._scrollBar.Thumb.Bottom;
				int width1 = this._scrollBar.DisplayRectangle.Width;
				int bottom1 = this._scrollBar.DisplayRectangle.Bottom;
				displayRectangle = this._scrollBar.Thumb;
				Rectangle rectangle1 = new Rectangle(left1, bottom, width1, bottom1 - displayRectangle.Bottom);
				if (!this._sourceControl.DrawWithVisualStyles || !ScrollBarRenderer.IsSupported)
				{
					Rectangle displayRectangle1 = this._scrollBar.DisplayRectangle;
					displayRectangle1.Offset(1, 0);
					Rectangle upArrow = this._scrollBar.UpArrow;
					upArrow.Offset(1, 0);
					Rectangle downArrow = this._scrollBar.DownArrow;
					downArrow.Offset(1, 0);
					Rectangle thumb = this._scrollBar.Thumb;
					thumb.Offset(1, 0);
					HatchBrush hatchBrush = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlLightLight, SystemColors.Control);
					e.Graphics.FillRectangle(hatchBrush, displayRectangle1);
					ControlPaint.DrawScrollButton(e.Graphics, upArrow, ScrollButton.Up, this.GetButtonState(this._scrollBar.UpArrow));
					ControlPaint.DrawScrollButton(e.Graphics, downArrow, ScrollButton.Down, this.GetButtonState(this._scrollBar.DownArrow));
					ControlPaint.DrawButton(e.Graphics, thumb, ButtonState.Normal);
				}
				else
				{
					ScrollBarRenderer.DrawUpperVerticalTrack(e.Graphics, rectangle, this.GetScrollBarState(rectangle));
					ScrollBarRenderer.DrawLowerVerticalTrack(e.Graphics, rectangle1, this.GetScrollBarState(rectangle1));
					ScrollBarRenderer.DrawArrowButton(e.Graphics, this._scrollBar.UpArrow, this.GetScrollBarStateUp());
					ScrollBarRenderer.DrawArrowButton(e.Graphics, this._scrollBar.DownArrow, this.GetScrollBarStateDown());
					ScrollBarRenderer.DrawVerticalThumb(e.Graphics, this._scrollBar.Thumb, this.GetScrollBarThumbState());
					ScrollBarRenderer.DrawVerticalThumbGrip(e.Graphics, this._scrollBar.Thumb, this.GetScrollBarThumbState());
				}
			}
			for (int i = this._scrollOffset; i < this._scrollOffset + this._numItemsDisplayed; i++)
			{
				bool flag = this._highlightedItemIndex == i;
				ComboTreeDropDown.NodeInfo item = this._visibleItems[i];
				if (flag)
				{
					e.Graphics.FillRectangle(SystemBrushes.Highlight, item.DisplayRectangle);
				}
				if (item.Image != null)
				{
					Graphics graphics = e.Graphics;
					Image image = item.Image;
					displayRectangle = item.DisplayRectangle;
					graphics.DrawImage(image, new Rectangle(displayRectangle.Location, item.Image.Size));
				}
				System.Drawing.Font font = new System.Drawing.Font(this.Font, this._visibleItems[i].Node.FontStyle);
				displayRectangle = item.DisplayRectangle;
				int x = displayRectangle.X + item.Image.Width + 2;
				int y = item.DisplayRectangle.Y;
				displayRectangle = item.DisplayRectangle;
				Rectangle rectangle2 = new Rectangle(x, y, displayRectangle.Width - item.Image.Width - 4, this._itemHeight);
				TextRenderer.DrawText(e.Graphics, item.Node.Text, font, rectangle2, (flag ? SystemColors.HighlightText : base.ForeColor), TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
				if (flag && this._sourceControl.Focused && this._sourceControl.ShowsFocusCues)
				{
					ControlPaint.DrawFocusRectangle(e.Graphics, item.DisplayRectangle);
				}
			}
		}

		public void Open()
		{
			if (this._sourceControl.SelectedNode != null)
			{
				ComboTreeNode selectedNode = this._sourceControl.SelectedNode;
				while (true)
				{
					ComboTreeNode parent = selectedNode.Parent;
					selectedNode = parent;
					if (parent == null)
					{
						break;
					}
					selectedNode.Expanded = true;
				}
			}
			this.UpdateVisibleItems();
			if (this._sourceControl.SelectedNode != null)
			{
				int num = 0;
				while (num < this._visibleItems.Count)
				{
					if (this._visibleItems[num].Node != this._sourceControl.SelectedNode)
					{
						num++;
					}
					else
					{
						this._highlightedItemIndex = num;
						if (this._highlightedItemIndex >= this._scrollOffset && this._highlightedItemIndex < this._scrollOffset + this._numItemsDisplayed)
						{
							break;
						}
						this._scrollOffset = Math.Min(Math.Max(0, this._highlightedItemIndex - this._numItemsDisplayed + 1), this._visibleItems.Count - this._numItemsDisplayed);
						this.UpdateScrolling();
						break;
					}
				}
			}
			ComboTreeBox comboTreeBox = this._sourceControl;
			Rectangle clientRectangle = this._sourceControl.ClientRectangle;
			base.Show(comboTreeBox, new Point(0, clientRectangle.Height));
		}

		public void ScrollDropDown(int offset)
		{
			if (offset < 0)
			{
				this._scrollOffset = Math.Max(this._scrollOffset + offset, 0);
				this.UpdateScrolling();
				base.Invalidate();
				return;
			}
			if (offset > 0)
			{
				this._scrollOffset = Math.Min(this._scrollOffset + offset, this._visibleItems.Count - this._numItemsDisplayed);
				this.UpdateScrolling();
				base.Invalidate();
			}
		}

		private void scrollRepeater_Tick(object sender, EventArgs e)
		{
			this._scrollRepeater.Interval = 50;
			Point client = base.PointToClient(System.Windows.Forms.Cursor.Position);
			this.OnMouseDown(new MouseEventArgs(Control.MouseButtons, 1, client.X, client.Y, 0));
		}

		private void ScrollToHighlighted(bool highlightedAtTop)
		{
			if (this._highlightedItemIndex < this._scrollOffset || this._highlightedItemIndex >= this._scrollOffset + this._numItemsDisplayed)
			{
				if (!highlightedAtTop)
				{
					this._scrollOffset = Math.Min(Math.Max(0, this._highlightedItemIndex - this._numItemsDisplayed + 1), this._visibleItems.Count - this._numItemsDisplayed);
				}
				else
				{
					this._scrollOffset = Math.Min(this._highlightedItemIndex, this._visibleItems.Count - this._numItemsDisplayed);
				}
				this.UpdateScrolling();
			}
		}

		private void UpdateScrolling()
		{
			if (this._scrollBarVisible)
			{
				int height = this._scrollBar.DisplayRectangle.Height;
				System.Drawing.Size sCROLLBUTTONSIZE = this.SCROLLBUTTON_SIZE;
				int num = height - 2 * sCROLLBUTTONSIZE.Height;
				double count = (double)this._numItemsDisplayed / (double)this._visibleItems.Count;
				int num1 = Math.Max((int)(count * (double)num), 20);
				int num2 = Math.Max(0, 20 - (int)(count * (double)num));
				int num3 = Math.Min((int)Math.Ceiling((double)this._scrollOffset / (double)this._visibleItems.Count * (double)(num - num2)), num - 20);
				ComboTreeDropDown.ScrollBarInfo rectangle = this._scrollBar;
				int x = this._scrollBar.DisplayRectangle.X;
				int top = this._scrollBar.DisplayRectangle.Top;
				sCROLLBUTTONSIZE = this.SCROLLBUTTON_SIZE;
				rectangle.Thumb = new Rectangle(new Point(x, top + sCROLLBUTTONSIZE.Height + num3), new System.Drawing.Size(17, num1));
			}
			for (int i = this._scrollOffset; i < this._scrollOffset + this._numItemsDisplayed; i++)
			{
				ComboTreeDropDown.NodeInfo item = this._visibleItems[i];
				if (item.Image == null)
				{
					item.Image = this.GetItemBitmap(item.Node);
				}
				item.DisplayRectangle = new Rectangle(this._interior.X, this._interior.Y + this._itemHeight * (i - this._scrollOffset), this._interior.Width, this._itemHeight);
				int depth = item.Node.Depth * 4;
				Rectangle displayRectangle = item.DisplayRectangle;
				item.GlyphRectangle = new Rectangle(depth, displayRectangle.Top, item.Image.Width - depth, item.Image.Height);
			}
		}

		internal void UpdateVisibleItems()
		{
			base.SuspendLayout();
			this._bitmaps.Clear();
			this._visibleItems.Clear();
			foreach (ComboTreeNode allNode in this._sourceControl.AllNodes)
			{
				if (!this._sourceControl.IsNodeVisible(allNode))
				{
					continue;
				}
				this._visibleItems.Add(new ComboTreeDropDown.NodeInfo(allNode));
			}
			this._highlightedItemIndex = Math.Max(0, Math.Min(this._highlightedItemIndex, this._visibleItems.Count - 1));
			this._numItemsDisplayed = Math.Min(this._dropDownHeight / this._itemHeight + 1, this._visibleItems.Count);
			int num = ((this._dropDownHeight - 2) / this._itemHeight + 1) * this._itemHeight + 2;
			Rectangle clientRectangle = this._sourceControl.ClientRectangle;
			base.Size = new System.Drawing.Size(clientRectangle.Width, Math.Min(num, this._visibleItems.Count * this._itemHeight + 2));
			this._interior = base.ClientRectangle;
			this._interior.Inflate(-1, -1);
			this._scrollBarVisible = this._numItemsDisplayed < this._visibleItems.Count;
			this._scrollOffset = Math.Max(0, Math.Min(this._scrollOffset, this._visibleItems.Count - this._numItemsDisplayed));
			if (this._scrollBarVisible)
			{
				ref Rectangle width = ref this._interior;
				width.Width = width.Width - 17;
				this._scrollBar.DisplayRectangle = new Rectangle(this._interior.Right, this._interior.Top, 17, this._interior.Height);
				ComboTreeDropDown.ScrollBarInfo rectangle = this._scrollBar;
				clientRectangle = this._scrollBar.DisplayRectangle;
				rectangle.UpArrow = new Rectangle(clientRectangle.Location, this.SCROLLBUTTON_SIZE);
				ComboTreeDropDown.ScrollBarInfo scrollBarInfo = this._scrollBar;
				int x = this._scrollBar.DisplayRectangle.X;
				clientRectangle = this._scrollBar.DisplayRectangle;
				scrollBarInfo.DownArrow = new Rectangle(new Point(x, clientRectangle.Bottom - 17), this.SCROLLBUTTON_SIZE);
			}
			this.UpdateScrolling();
			base.ResumeLayout();
			base.Invalidate();
		}

		private struct BitmapInfo : IEquatable<ComboTreeDropDown.BitmapInfo>
		{
			public bool HasChildren
			{
				get;
				set;
			}

			public int ImageIndex
			{
				get;
				set;
			}

			public string ImageKey
			{
				get;
				set;
			}

			public bool IsFirst
			{
				get;
				set;
			}

			public bool IsLastPeer
			{
				get;
				set;
			}

			public int NodeDepth
			{
				get;
				set;
			}

			public bool NodeExpanded
			{
				get;
				set;
			}

			public bool[] VerticalLines
			{
				get;
				set;
			}

			public bool Equals(ComboTreeDropDown.BitmapInfo that)
			{
				if (this.HasChildren != that.HasChildren)
				{
					return false;
				}
				if (this.IsLastPeer != that.IsLastPeer)
				{
					return false;
				}
				if (this.IsFirst != that.IsFirst)
				{
					return false;
				}
				if (this.NodeDepth != that.NodeDepth)
				{
					return false;
				}
				if (this.NodeExpanded != that.NodeExpanded)
				{
					return false;
				}
				if ((int)this.VerticalLines.Length != (int)that.VerticalLines.Length)
				{
					return false;
				}
				if (this.ImageIndex != that.ImageIndex)
				{
					return false;
				}
				if (this.ImageKey != that.ImageKey)
				{
					return false;
				}
				for (int i = 0; i < (int)this.VerticalLines.Length; i++)
				{
					if (this.VerticalLines[i] != that.VerticalLines[i])
					{
						return false;
					}
				}
				return true;
			}
		}

		private class NodeInfo
		{
			public Rectangle DisplayRectangle
			{
				get;
				set;
			}

			public Rectangle GlyphRectangle
			{
				get;
				set;
			}

			public Image Image
			{
				get;
				set;
			}

			public ComboTreeNode Node
			{
				get;
				private set;
			}

			public NodeInfo(ComboTreeNode node)
			{
				this.Node = node;
			}
		}

		private class ScrollBarInfo
		{
			public Rectangle DisplayRectangle
			{
				get;
				set;
			}

			public Rectangle DownArrow
			{
				get;
				set;
			}

			public Rectangle Thumb
			{
				get;
				set;
			}

			public Rectangle UpArrow
			{
				get;
				set;
			}

			public ScrollBarInfo()
			{
			}
		}
	}
}