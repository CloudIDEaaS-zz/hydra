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
using Decompiler.Gui;
using Decompiler.Gui.Controls;
using System.ComponentModel;
using System.ComponentModel.Design;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ContextMenu = System.Windows.Forms.ContextMenu;
using DataObject = System.Windows.Forms.DataObject;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using DragEventHandler = System.Windows.Forms.DragEventHandler;
using DragDropEffects = System.Windows.Forms.DragDropEffects;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Decompiler.UnitTests.Gui.Windows
{
    [TestFixture]
    public class ProjectBrowserServiceTests
    {
        private readonly string cr = Environment.NewLine == "\r\n"
                ? "&#xD;&#xA;"
                : "&#xA;";
        private MockRepository mr;
        private ServiceContainer sc;
        private FakeTreeView fakeTree;
        private ITreeView mockTree;
        private ITreeNodeCollection mockNodes;
        private IDecompilerService decompilerSvc;
        private IDecompiler decompiler;
        private Program program;
        private Project project;
        private IDecompilerShellUiService uiSvc;

        [SetUp]
        public void Setup()
        {
            mr = new MockRepository();
            sc = new ServiceContainer();
            mockTree = mr.StrictMock<ITreeView>();
            mockNodes = mr.StrictMock<ITreeNodeCollection>();
            decompilerSvc = mr.StrictMock<IDecompilerService>();
            uiSvc = mr.StrictMock<IDecompilerShellUiService>();
            decompiler = mr.StrictMock<IDecompiler>();
            mockTree.Stub(t => t.Nodes).Return(mockNodes);
            uiSvc.Stub(u => u.GetContextMenu(0)).IgnoreArguments().Return(new ContextMenu());
            sc.AddService<IDecompilerShellUiService>(uiSvc);
            fakeTree = new FakeTreeView();
        }

        private void Expect(string sExp)
        {
            var x = new XElement("foo");
            Func<ITreeNode, XNode> render = null;
            render = new Func<ITreeNode, XNode>(n =>
            {
                var e = new XElement(
                    "node",
                    new XAttribute[] {
                        n.Text != null ? new XAttribute("text", n.Text): null,
                        n.ToolTipText != null ? new XAttribute("tip", n.ToolTipText) : null,
                        n.Tag != null ? new XAttribute("tag", n.Tag.GetType().Name) : null
                    }.Where(a => a != null),
                    n.Nodes.Select(c => render(c)));
                    
                return e;
            });
            var sb = new StringWriter();
            var xdoc = new XDocument(
                new XElement("root",
                    fakeTree.Nodes.Select(n => render(n))));
            xdoc.RemoveAnnotations<XProcessingInstruction>();
            xdoc.WriteTo(new XmlTextWriter(sb));
            Console.WriteLine(sb.ToString());
            Assert.AreEqual(sExp, sb.ToString());
        }

        #region Fake TreeView (Move to UnitTests.Fakes?)


        private class FakeTreeView : ITreeView
        {
            public event EventHandler AfterSelect;
            public event DragEventHandler DragEnter;
            public event DragEventHandler DragOver;
            public event DragEventHandler DragDrop;
            public event EventHandler DragLeave;
            public event MouseEventHandler MouseWheel;

            public FakeTreeView()
            {
                this.Nodes = new FakeTreeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }

            public ITreeNode SelectedNode { get { return selectedItem; } set { selectedItem = value; AfterSelect.Fire(this); } }
            private ITreeNode selectedItem;

            public bool Focused { get; set; }
            public ContextMenu ContextMenu { get; set; }
            public bool ShowRootLines { get; set; }
            public bool ShowNodeToolTips { get; set; }

            public ITreeNode CreateNode()
            {
                return new FakeTreeNode();
            }

            public ITreeNode CreateNode(string text)
            {
                return new FakeTreeNode { Text = text };
            }

            public void PerformDragEnter(DragEventArgs e)
            {
                DragEnter(this, e);
            }

            public void PerformDragOver(DragEventArgs e)
            {
                DragOver(this, e);
            }

            public void PerformDragDrop(DragEventArgs e)
            {
                DragDrop(this, e);
            }

            public void PerformDragLeave(EventArgs e)
            {
                DragLeave(this, e);
            }

            public void PerformMouseWheel(MouseEventArgs e)
            {
                MouseWheel(this, e);
            }
        }

        public class FakeNodeCollection : ITreeNodeCollection
        {
            private List<ITreeNode> nodes = new List<ITreeNode>();

            public ITreeNode Add(string text)
            {
                var node = new FakeNode { Text = text };
                nodes.Add(node);
                return node;
            }

            public void AddRange(IEnumerable<ITreeNode> nodes)
            {
                this.nodes.AddRange(nodes);
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
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(ITreeNode item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class FakeNode : ITreeNode
        {
            public FakeNode()
            {
                Nodes = new FakeNodeCollection();
            }

            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public object Tag { get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Expand()
            {
            }
        }

        [Designer(typeof(TestDesigner))]
        public class TestComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class ParentComponent
        {
        }

        [Designer(typeof(TestDesigner))]
        public class GrandParentComponent
        {
        }


        private class FakeTreeNodeCollection : List<ITreeNode>,  ITreeNodeCollection
        {
            public ITreeNode Add(string text)
            {
                var node = new FakeTreeNode { Text = text };
                base.Add(node);
                return node;
            }
        }

        private class FakeTreeNode : ITreeNode
        {
            public FakeTreeNode()
            {
                Nodes = new FakeTreeNodeCollection();
            }

            public object Tag { get; set; }
            public ITreeNodeCollection Nodes { get; private set; }
            public string ImageName { get; set; }
            public string Text { get; set; }
            public string ToolTipText { get; set; }

            public void Expand() { }
        }

        #endregion

        [Test]
        public void PBS_NoProject()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            pbs.Load(null);

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?><root><node text=\"(No project loaded)\" /></root>");
            Assert.IsFalse(fakeTree.ShowRootLines);
            Assert.IsFalse(fakeTree.ShowNodeToolTips);
        }

        private void Given_ProgramWithOneSegment()
        {
            var image = new LoadedImage(Address.Ptr32(0x12340000), new byte[0x1000]);
            var imageMap = image.CreateImageMap();
            imageMap.AddSegment(Address.Ptr32(0x12340000), ".text", AccessMode.Execute);
            var arch = mr.StrictMock<IProcessorArchitecture>();
            var platform = new DefaultPlatform(sc, arch);
            this.program = new Program(image, imageMap, arch, platform);
            this.program.Name = "foo.exe";
            this.program.Filename = @"c:\test\foo.exe";
            project.Programs.Add(program);
        }

        [Test]
        public void PBS_SingleBinary()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();

            pbs.Load(project);

            Assert.IsTrue(fakeTree.ShowNodeToolTips);
            
            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12340000\" " +
                    "tag=\"Program\">" +
                    "<node " + 
                        "text=\"Image base\" " +
                        "tip=\"Image base" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "rw-" + "\" " +
                        "tag=\"ImageMapSegment\" />" +
                "</node>" +
                "</root>");
        }

        [Test]
        public void PBS_AddBinary()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            mr.ReplayAll();

            pbs.Load(project);

            project.Programs.Add(new Program
            {
                Filename = "bar.exe",
                Image = new LoadedImage(Address.Ptr32(0x1231300), new byte[128])
            });

            Expect("<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root><node text=\"foo.exe\" tip=\"c:\\test\\foo.exe&#xD;&#xA;12340000\" tag=\"Program\">" +
                    "<node text=\"Image base\" tip=\"Image base&#xD;&#xA;Address: 12340000&#xD;&#xA;Size: 1000&#xD;&#xA;rw-\" tag=\"ImageMapSegment\" />" +
                 "</node>" +
                 "</root>");
            mr.VerifyAll();
        }

        private void Given_Project()
        {
            this.project = new Project
            {
            };
        }


        private void Given_UserProcedure(uint addr, string name)
        {
            program.UserProcedures.Add(
                Address.Ptr32(addr), new Decompiler.Core.Serialization.Procedure_v1
                {
                    Address = addr.ToString(),
                    Name = name
                });
        }

        [Test]
        public void PBS_UserProcedures()
        {
            var pbs = new ProjectBrowserService(sc, fakeTree);
            Given_Project();
            Given_ProgramWithOneSegment();
            Given_UserProcedure(0x12340500, "MyFoo");
            mr.ReplayAll();

            pbs.Load(project);

            Expect(
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<root>" +
                "<node " +
                    "text=\"foo.exe\" " +
                    "tip=\"c:\\test\\foo.exe" + cr + "12340000\" " +
                    "tag=\"Program\">" +
                    "<node " +
                        "text=\"Image base\" " +
                        "tip=\"Image base" + cr + "Address: 12340000" + cr + "Size: 1000" + cr + "rw-" + "\" " +
                        "tag=\"ImageMapSegment\">" +
                        "<node " +
                            "text=\"MyFoo\" " +
                            "tip=\"12340500\" " +
                            "tag=\"ProcedureDesigner\" />" +
                    "</node>" +
                "</node>" +
                "</root>");
        }

         
        [Test]
        public void PBS_AfterSelect_Calls_DoDefaultAction()
        {
            var des = mr.StrictMock<TreeNodeDesigner>();
            var node = mr.Stub<ITreeNode>();
            des.Expect(d => d.DoDefaultAction());
            des.Stub(d => d.Initialize(null)).IgnoreArguments();
            mockTree = new FakeTreeView();
            mr.ReplayAll();
            
            var pbs = new ProjectBrowserService(sc, mockTree);
            pbs.AddComponents(new object[] { des });
            var desdes = pbs.GetDesigner(des);
            Assert.IsNotNull(desdes);

            mockTree.SelectedNode = des.TreeNode;

            mr.VerifyAll();
        }

        
        public class TestDesigner : TreeNodeDesigner
        {
        }

        [Test]
        public void PBS_FindGrandParent()
        {
            mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var gp = new GrandParentComponent();
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { gp });
            pbs.AddComponents(gp, new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.AreSame(gp, o);
        }

        [Test]
        public void PBS_NoGrandParent()
        {
            mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var p = new ParentComponent();
            var c = new TestComponent();

            pbs.AddComponents(new[] { p });
            pbs.AddComponents(p, new[] { c });

            var o = pbs.GetAncestorOfType<GrandParentComponent>(c);
            Assert.IsNull(o);
        }

        [Test]
        public void PBS_AddTypeLib()
        {
            mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var project = new Project();
            pbs.Load(project);

            project.MetadataFiles.Add(new MetadataFile
            {
                ModuleName = "..\\foo.tlb"
            });

            Assert.AreEqual(1, mockTree.Nodes.Count);
            Assert.AreEqual("foo.tlb", mockTree.Nodes[0].Text);
        }

        [Test]
        public void PBS_AcceptFiles()
        {
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var e = Given_DraggedFile();
            mr.ReplayAll();

            var project = new Project();
            pbs.Load(project);
            mockTree.PerformDragEnter(e);
            Assert.AreEqual(DragDropEffects.Copy, e.Effect);
        }

        [Test]
        public void PBS_RejectTextDrop()
        {
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            var e = Given_DraggedText();
            mr.ReplayAll();

            var project = new Project();
            pbs.Load(project);
            mockTree.PerformDragEnter(e);
            Assert.AreEqual(DragDropEffects.None, e.Effect);
        }

        [Test]
        public void PBS_AcceptDrop()
        {
            string filename = null;
            var mockTree = new FakeTreeView();
            var pbs = new ProjectBrowserService(sc, mockTree);
            pbs.FileDropped += (sender, ee) => { filename = ee.Filename; };
            var e = Given_DraggedFile();
            mr.ReplayAll();

            var project = new Project();
            pbs.Load(project);
            mockTree.PerformDragDrop(e);
            Assert.AreEqual("/home/bob/foo.exe", filename);
        }

        private DragEventArgs Given_DraggedFile()
        {
            var dObject = new DataObject(
                DataFormats.FileDrop,
                "/home/bob/foo.exe");

            return new DragEventArgs(
                    dObject, 0, 40, 40,
                    DragDropEffects.All,
                    DragDropEffects.All);
        }

        private DragEventArgs Given_DraggedText()
        {
            var dObject = new DataObject(
                DataFormats.UnicodeText,
                "hello world");

            return new DragEventArgs(
                    dObject, 0, 40, 40,
                    DragDropEffects.All,
                    DragDropEffects.All);
        }
    }
}
