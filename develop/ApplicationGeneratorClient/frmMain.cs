using AbstraX;
using AbstraX.ClientFolderStructure;
using ApplicationGenerator.Agent;
using ApplicationGenerator.Client.NodeInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using File = AbstraX.ClientFolderStructure.File;

namespace ApplicationGenerator.Client
{
    public partial class frmMain : Form
    {
        private ApplicationGeneratorAgent agent;
        private Folder rootFolder;
        private bool forceClose;

        public frmMain()
        {
            InitializeComponent();

            treeViewFolderHierarchy.AssignToImageList();
            treeViewFolderHierarchy.SetTheme("explorer");
        }

        private void frmClient_Load(object sender, EventArgs e)
        {
            notifyIcon.RefreshTray();

            this.DelayInvoke(100, () =>
            {
                LaunchAgent();
            });
        }

        private void LaunchAgent()
        {
            dynamic connectObject;
            var emptyLineCount = 0;
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            var treeViewEvents = treeViewFolderHierarchy.GetEvents();

            Directory.SetCurrentDirectory(Path.Combine(hydraSolutionPath, @"ApplicationGenerator\TestOutput"));
            treeViewEvents.FolderSelected += TreeViewEvents_FolderSelected;
            treeViewEvents.FileSelected += TreeViewEvents_FileSelected;

            agent = ApplicationGeneratorAgent.GetInstance(true);

            agent.Exited += (sender, e) =>
            {
                statusLabel.Text = "Disconnected from agent";
            };

            connectObject = agent.SendSimpleCommand("connect");

            if (connectObject.Response == "Connected successfully")
            {
                var entitiesProjectPath = Path.Combine(hydraSolutionPath, @"Ripley.Entities\Ripley.Entities.csproj");
                var servicesProjectPath = Path.Combine(hydraSolutionPath, @"Ripley.Services\Ripley.Services.csproj");
                var responseCallback = new Func<string, bool>((c) =>
                {
                    if (c == string.Empty)
                    {
                        emptyLineCount++;

                        if (emptyLineCount == 2)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        emptyLineCount = 0;
                    }

                    textBoxSource.AppendText(c + Environment.NewLine);
                    Application.DoEvents();

                    return true;
                });

                statusLabel.Text = connectObject.Response;

                agent.Generate(hydraSolutionPath, entitiesProjectPath, servicesProjectPath, GeneratorPass.Files, responseCallback);

                rootFolder = agent.GetFolder(@"/");

                RecurseFolder(rootFolder);

                treeViewFolderHierarchy.Sort();

                btnStop.Enabled = true;
                stopToolStripMenuItem.Enabled = true;
            }

            notifyIcon.Visible = true;
        }

        private void TreeViewEvents_FileSelected(object sender, SelectFileEventArgs e)
        {
            var file = e.File;
            var sourceBytes = agent.GetFileContents(file.RelativeFullName);
            var source = ASCIIEncoding.UTF8.GetString(sourceBytes);

            textBoxSource.Text = source;

            oropertyGridSource.SelectedObject = file;
        }

        private void TreeViewEvents_FolderSelected(object sender, SelectFolderEventArgs e)
        {
            textBoxSource.Text = string.Empty;
            oropertyGridSource.SelectedObject = e.Folder;
        }

        private void RecurseFolder(Folder folder, TreeNode parentTreeNode = null)
        {
            TreeNode folderNode;

            if (parentTreeNode == null)
            {
                folderNode = AddFolderNode(FolderType.PhysicalFolder, folder, folder.Name);
            }
            else
            {
                folderNode = AddFolderNode(parentTreeNode, FolderType.PhysicalFolder, folder, folder.Name);
            }

            if (folder.HasFolders)
            {
                var folders = agent.GetFolders(folder.RelativeFullName);

                foreach (var subFolder in folders)
                {
                    RecurseFolder(subFolder, folderNode);
                }
            }

            if (folder.HasFiles)
            {
                var files = agent.GetFiles(folder.RelativeFullName);

                foreach (var file in files)
                {
                    AddFileNode(folderNode, file, file.Name);
                }
            }
        }

        private void frmClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (agent != null)
            {
                agent.Dispose();
            }

            notifyIcon.RefreshTray();
        }

        private TreeNode AddSpinsterNode(TreeNode treeNodeParent)
        {
            var imageIndex = treeViewFolderHierarchy.ImageList.GetImageIndex("Blank");
            var treeNode = new TreeNode("Loading...", imageIndex, imageIndex);

            if (treeNodeParent == null)
            {
                DebugUtils.Break();
            }

            treeNode.Tag = new SpinsterNode(treeNode);

            this.Invoke(() =>
            {
                treeNodeParent.Add(treeNode);
            });

            return treeNode;
        }

        private TreeNode AddFolderNode(FolderType folderType, Folder nodeParent, string text, bool addSpinster = false)
        {
            TreeNode treeNode;
            INodeInfo nodeInfo;

            treeNode = new TreeNode(text);

            nodeInfo = new FolderNode(treeNode, nodeParent, folderType);

            this.Invoke(() =>
            {
                treeNode.Assign(nodeInfo, NodeInfoType.Folder);
                treeViewFolderHierarchy.Add(treeNode);
            });

            if (addSpinster)
            {
                AddSpinsterNode(treeNode);
            }

            return treeNode;
        }

        private TreeNode AddFileNode(TreeNode treeNodeParent, File nodeParent, string text, bool expandParent = true)
        {
            TreeNode treeNode;
            FileNode nodeInfo;

            treeNode = new TreeNode(text);
            nodeInfo = new FileNode(treeNode, nodeParent);

            this.Invoke(() =>
            {
                treeNode.Assign(nodeInfo, () => agent.GetFileIcon(nodeParent.RelativeFullName));
                treeNodeParent.Add(treeNode);

                if (expandParent)
                {
                    treeNodeParent.Expand();
                }
            });

            return treeNode;
        }

        private TreeNode AddFolderNode(TreeNode treeNodeParent, FolderType folderType, Folder nodeParent, string text, bool addSpinster = false, bool expandParent = true)
        {
            TreeNode treeNode;
            INodeInfo nodeInfo;

            treeNode = new TreeNode(text);

            if (treeNodeParent == null)
            {
                DebugUtils.Break();
            }

            nodeInfo = new FolderNode(treeNode, nodeParent, folderType);

            this.Invoke(() =>
            {
                treeNode.Assign(nodeInfo, NodeInfoType.Folder);
                treeNodeParent.Add(treeNode);

                if (expandParent)
                {
                    treeNodeParent.Expand();
                }
            });

            if (addSpinster)
            {
                AddSpinsterNode(treeNode);
            }

            return treeNode;
        }

        private void Stop()
        {
            treeViewFolderHierarchy.Clear();
            textBoxSource.Clear();
            oropertyGridSource.SelectedObject = null;

            agent.Dispose();
            agent = null;

            btnStop.Enabled = false;
            stopToolStripMenuItem.Enabled = false;
        }

        private void Reload()
        {
            if (agent != null)
            {
                this.Stop();
                Thread.Sleep(500);
            }

            LaunchAgent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!forceClose)
            {
                e.Cancel = true;
                this.Visible = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            forceClose = true;
            this.Close();
        }

        private void showClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = !this.Visible;
            }
        }
    }
}
