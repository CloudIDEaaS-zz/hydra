using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VisualStudioProvider
{
    public class RunningDocumentTable : BaseDictionary<string, DocumentInfo>, IVsRunningDocTableEvents4, IVsRunningDocTableEvents3, IDisposable
    {
        private Microsoft.VisualStudio.Shell.RunningDocumentTable runningDocTable;
        public event EventHandler<EventArgs<DocumentInfo>> OnDocumentSaved;
        public event EventHandler<EventArgs<DocumentInfo>> OnDocumentOpen;
        private uint runningDocEventsCookie;
        private IServiceProvider site;

        public RunningDocumentTable(System.IServiceProvider site)
        {
            this.runningDocTable = new Microsoft.VisualStudio.Shell.RunningDocumentTable(site);
            this.site = site;

            runningDocEventsCookie = runningDocTable.Advise(this);
        }

        public void Dispose()
        {
            runningDocTable.Unadvise(runningDocEventsCookie);
        }

        public override int Count
        {
            get 
            {
                return this.Count();
            }
        }

        public override void Clear()
        {
            throw new NotImplementedException();
        }

        public override void Add(string key, DocumentInfo value)
        {
            throw new NotImplementedException();
        }

        public override bool ContainsKey(string key)
        {
            foreach (var pair in this)
            {
                if (pair.Key == key)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetValue(string key, out DocumentInfo value)
        {
            value = null;

            foreach (var pair in this)
            {
                if (pair.Key == key)
                {
                    value = pair.Value;
                    return true;
                }
            }

            return false;
        }

        public override IEnumerator<KeyValuePair<string, DocumentInfo>> GetEnumerator()
        {
            foreach (var document in runningDocTable)
            {
                yield return new KeyValuePair<string, DocumentInfo>(document.Moniker, new DocumentInfo(document));
            }
        }

        protected override void SetValue(string key, DocumentInfo value)
        {
            throw new NotImplementedException();
        }

        public IVsTextLines GetText(DocumentInfo info)
        {
            uint flags;
            uint editLocks;
            uint readLocks;
            string moniker;
            IVsHierarchy docHierarchy;
            uint itemId;
            IntPtr docData = IntPtr.Zero;
            IVsTextLines lines = null;

            var rdt = site.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;

            if (rdt.GetDocumentInfo(info.DocCookie, out flags, out readLocks, out editLocks, out moniker, out docHierarchy, out itemId, out docData) == VSConstants.S_OK)
            {
                var obj = Marshal.GetObjectForIUnknown(docData);
                lines = (IVsTextLines)obj;
            }

            return lines;
        }

        public DocumentInfo GetDocument(Func<DocumentInfo, bool> filter)
        {
            foreach (var document in this.Values)
            {
                if (filter(document))
                {
                    return document;
                }
            }

            return null;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeSave(uint docCookie)
        {
            try
            {
                var document = this.Values.SingleOrDefault(d => d.DocCookie == docCookie);

                if (document != null)
                {
                }
                else
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
            }

            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            try
            {
                var document = this.Values.LastOrDefault(d => d.DocCookie == docCookie);

                if (document != null)
                {
                    document.Saved = true;

                    if (OnDocumentSaved != null)
                    {
                        OnDocumentSaved(this, new EventArgs<DocumentInfo>(document));
                    }
                }
                else
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            try
            {
                var document = this.Values.SingleOrDefault(d => d.DocCookie == docCookie);

                if (document != null)
                {
                    document.Frame = pFrame;

                    if (OnDocumentOpen != null)
                    {
                        OnDocumentOpen(this, new EventArgs<DocumentInfo>(document));
                    }
                }
                else
                {
                    Debugger.Break();
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }

            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLastDocumentUnlock(IVsHierarchy pHier, uint itemid, string pszMkDocument, int fClosedWithoutSaving)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSaveAll()
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeFirstDocumentLock(IVsHierarchy pHier, uint itemid, string pszMkDocument)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChangeEx(uint docCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew)
        {
            return VSConstants.S_OK;
        }
    }
}
