using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;

namespace VisualStudioProvider
{
    [DebuggerDisplay(" { DebugInfo } ")]
    public class DocumentInfo
    {
        private DocumentText documentText;
        public string DocumentName { get; private set; }
        public uint ItemId { get; private set; }
        public uint DocCookie { get; private set; }
        public VSHierarchyItem HierarchyItem { get; private set; }
        public IVsWindowFrame Frame { get; set; }
        public bool Saved { get; set; }
        public static RunningDocumentTable DocumentTable { get; set; }
        public RunningDocumentInfo InternalInfo { get; private set; }

        public DocumentInfo(RunningDocumentInfo info)
        {
            this.ItemId = info.ItemId;
            this.DocCookie = info.DocCookie;
            this.HierarchyItem = new VSHierarchyItem(info.Hierarchy, info.ItemId, 0);
            this.DocumentName = info.Moniker;
            this.InternalInfo = info;
        }

        public DocumentText DocumentText
        {
            get
            {
                if (documentText == null)
                {
                    documentText = new DocumentText(this, DocumentTable.GetText(this));
                }

                return documentText;
            }
        }

        public IVsProject ParentProject
        {
            get
            {
                return this.HierarchyItem.Hierarchy as IVsProject;
            }
        }

        public string ParentProjectName
        {
            get
            {
                var project = this.ParentProject;

                if (project != null)
                {
                    var projectName = project.GetFileName();

                    return projectName;
                }

                return null;
            }
        }

        public string HierarchyItemName
        {
            get
            {
                return HierarchyItem.Name;
            }
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("DocumentInfo: Name='{0}', HierarchItem='{1}' DocCookie={2}, ItemID={3}, Saved={4}", this.DocumentName, this.HierarchyItem.Name, this.DocCookie, this.ItemId, this.Saved);
            }
        }
    }
}
