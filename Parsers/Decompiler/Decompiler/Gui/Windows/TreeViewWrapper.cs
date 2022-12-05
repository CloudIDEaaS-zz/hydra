﻿#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Decompiler.Core;
using Decompiler.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows
{
    /// <summary>
    /// Wraps a Windows forms TreeView in the platform independent ITreeView interface.
    /// </summary>
    public class TreeViewWrapper : ITreeView
    {
        public event EventHandler AfterSelect;
        public event DragEventHandler DragEnter;
        public event DragEventHandler DragOver;
        public event DragEventHandler DragDrop;
        public event MouseEventHandler MouseWheel;
        public event EventHandler DragLeave;

        private TreeView treeView;

        public TreeViewWrapper(TreeView treeView)
        {
            this.treeView = treeView;
            this.Nodes = new WrappedNodeList(treeView.Nodes);
            this.treeView.AfterSelect += treeView_AfterSelect;
            this.treeView.DragEnter += treeView_DragEnter;
            this.treeView.DragLeave += treeView_DragLeave;
            this.treeView.DragOver += treeView_DragOver;
            this.treeView.DragDrop += treeView_DragDrop;
            this.treeView.MouseWheel += treeView_MouseWheel;
        }

        public ContextMenu ContextMenu { get { return treeView.ContextMenu; } set { treeView.ContextMenu = value; } }
        public bool Focused { get { return treeView.Focused; } } 
        public ITreeNodeCollection Nodes { get; private set; }
        public bool ShowLines { get { return treeView.ShowLines; } set { treeView.ShowLines = value; } }
        public bool ShowNodeToolTips { get { return treeView.ShowNodeToolTips; } set { treeView.ShowNodeToolTips = value; } }
        public bool ShowRootLines { get { return treeView.ShowRootLines; } set { treeView.ShowRootLines = value; } }

        public ITreeNode SelectedNode
        {
            get
            {
                return (ITreeNode) treeView.SelectedNode;
            }
            set
            {
                treeView.SelectedNode = (WrappedNode) value;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterSelect.Fire(this);
        }

        void treeView_DragDrop(object sender, DragEventArgs e)
        {
            var eh = DragDrop;
            if (eh != null)
                eh(this, e);
        }

        void treeView_DragOver(object sender, DragEventArgs e)
        {
 	        var eh = DragOver;
            if (eh != null)
                eh(this, e);
        }

        void treeView_DragLeave(object sender, EventArgs e)
        {
 	        var eh = DragLeave;
            if (eh != null)
                eh(this, e);
        }

        void treeView_DragEnter(object sender, DragEventArgs e)
        {
            var eh = DragEnter;
            if (eh != null)
                eh(this, e);
        }

        void treeView_MouseWheel(object sender, MouseEventArgs e)
        {
            var eh = MouseWheel;
            if (eh != null)
                eh(this, e);
        }

        private TreeNode NodeOf(TreeNodeCollection nodes, object value)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != value)
                    return node;
                var n = NodeOf(node.Nodes, value);
                if (n != null)
                    return n;
            }
            return null;
        }

        public ITreeNode CreateNode()
        {
            return new WrappedNode();
        }

        public ITreeNode CreateNode(string text)
        {
            return new WrappedNode { Text = text };
        }

        public class WrappedNode : TreeNode, ITreeNode
        {
            private Lazy<WrappedNodeList> nodes;

            public WrappedNode()
            {
                nodes = new Lazy<WrappedNodeList>(() => new WrappedNodeList(base.Nodes), true);
            }

            public new ITreeNodeCollection Nodes { get { return nodes.Value; } }

            public string ImageName
            {
                get { return ImageKey; }
                set
                {
                    ImageKey = value;
                    SelectedImageKey = value;
                }
            }
        }

        public class WrappedNodeList : ITreeNodeCollection
        {
            private TreeNodeCollection nodes;

            public WrappedNodeList(TreeNodeCollection nodes)
            {
                this.nodes = nodes;
            }

            public int IndexOf(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public ITreeNode this[int index]
            {
                get { return (ITreeNode) nodes[index]; }
                set { nodes[index] = (WrappedNode) value; }
            }

            public void Add(ITreeNode item)
            {
                nodes.Add((WrappedNode) item);
            }

            public ITreeNode Add(string text)
            {
                var node = new WrappedNode { Text = text };
                nodes.Add(node);
                return node;
            }

            public void AddRange(IEnumerable<ITreeNode> items)
            {
                nodes.AddRange(items.Cast<TreeNode>().ToArray());
            }

            public void Clear()
            {
                nodes.Clear();
            }

            public bool Contains(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(ITreeNode[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<ITreeNode> GetEnumerator()
            {
                return nodes.Cast<WrappedNode>().GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}