using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Utils;
using System.Threading.Tasks;
using System.Threading;

namespace VisualStudioProvider
{
    [DebuggerDisplay(" { Name } ")]
    public class VSHierarchyItem
    {
        private uint parentID;
        private string debugInfo;
        public IVsHierarchy Hierarchy { get; private set; }
        public VSHierarchyItem NestedHierarchy { get; private set; }
        public uint ItemID { get; private set; }
        public bool IsNested { get; private set; }

        public VSHierarchyItem(IVsHierarchy hierarchy, uint itemID, uint parentID)
        {
            IntPtr pNestedHierarchy;
            IVsHierarchy nestedHierarchy;
            uint nestedItemId;

            this.Hierarchy = hierarchy;
            this.ItemID = itemID;
            this.parentID = parentID;

            if (hierarchy != null && hierarchy.GetNestedHierarchy(itemID, typeof(IVsHierarchy).GUID, out pNestedHierarchy, out nestedItemId) == VSConstants.S_OK)
            {
                if (pNestedHierarchy != IntPtr.Zero)
                {
                    var convertedId = (VSConstants.VSITEMID)nestedItemId;

                    nestedHierarchy = (IVsHierarchy)Marshal.GetObjectForIUnknown(pNestedHierarchy);

                    nestedItemId = (uint) convertedId;
                    this.IsNested = true;

                    this.NestedHierarchy = new VSHierarchyItem(nestedHierarchy, nestedItemId, itemID);
                }
            }
        }

        public static uint NILUint
        {
            get
            {
                return unchecked((uint)(int)__VSHPROPID.VSHPROPID_NIL);
            }
        }

        public static int NILInt
        {
            get
            {
                return (int)__VSHPROPID.VSHPROPID_NIL;
            }
        }

        public static uint RootUint
        {
            get
            {
                return unchecked((uint)(int)VSConstants.VSITEMID.Root);
            }
        }

        public static int RootInt
        {
            get
            {
                return unchecked((int)VSConstants.VSITEMID.Root);
            }
        }

        public string DebugInfo
        {
            get
            {
                if (debugInfo == null)
                {
                    try
                    {
                        string name;

                        try
                        {
                            name = this.Name;
                        }
                        catch (Exception ex)
                        {
                            name = ex.GetExtendedMessage();
                        }
                        
                        debugInfo = string.Format("ItemID={0}, Name='{1}'", this.ItemID, name);
                    }
                    catch (Exception ex)
                    {
                        debugInfo = ex.GetExtendedMessage();
                    }
                }

                return debugInfo;
            }
        }

        public Dictionary<VSHierarchyItem, IVsHierarchy> NestedHierarchies
        {
            get
            {
                var nestedHierarchies = new Dictionary<VSHierarchyItem, IVsHierarchy>();

                Action<VSHierarchyItem> addNestedHierarchies = null;

                addNestedHierarchies = (parent) =>
                {
                    foreach (var child in parent.Children)
                    {
                        string name = null;

                        try
                        {
                            name = child.Name;
                        }
                        catch
                        {
                        }

                        if (child.IsNested)
                        {
                            if (!nestedHierarchies.ContainsKey(child))
                            {
                                nestedHierarchies.Add(child, child.NestedHierarchy.Hierarchy);
                            }
                        }

                        addNestedHierarchies(child);
                    }
                };

                addNestedHierarchies(this);

                return nestedHierarchies;
            }
        }

        public IEnumerable<VSHierarchyItem> Children
        {
            get
            {
                VSHierarchyItem parentHierarchy;
                var children = new List<VSHierarchyItem>();

                debugInfo = null;

                if (this.IsNested)
                {
                    parentHierarchy = this.NestedHierarchy;
                }
                else
                {
                    parentHierarchy = this;
                }

                var firstChild = parentHierarchy.FirstChild;

                if (firstChild != null)
                {
                    var nextSibling = firstChild.NextSibling;
                    string name = null;

                    try
                    {
                        name = firstChild.Name;
                    }
                    catch
                    {
                    }

                    children.Add(firstChild);

                    while (nextSibling != null)
                    {
                        try
                        {
                            name = nextSibling.Name;
                        }
                        catch
                        {
                        }

                        children.Add(nextSibling);

                        nextSibling = nextSibling.NextSibling;
                    }

                    try
                    {
                        name = firstChild.Name;
                    }
                    catch
                    {
                    }

                    try
                    {
                        nextSibling = firstChild.NextVisibleSibling;

                        if (!children.Any(c => c.ItemID == firstChild.ItemID))
                        {
                            children.Add(firstChild);
                        }

                        while (nextSibling != null)
                        {
                            try
                            {
                                name = nextSibling.Name;
                            }
                            catch
                            {
                            }

                            if (!children.Any(c => c.ItemID == nextSibling.ItemID))
                            {
                                children.Add(nextSibling);
                            }

                            nextSibling = nextSibling.NextVisibleSibling;
                        }
                    }
                    catch 
                    { 
                    }
                }

                try
                {
                    foreach (var child in this.VisibleChildren.Where(c => !children.Any(c2 => c2.ItemID == c.ItemID)))
                    {
                        children.Add(child);
                    }
                }
                catch
                {
                }

                return children;
            }
        }

        public IEnumerable<VSHierarchyItem> VisibleChildren
        {
            get
            {
                VSHierarchyItem parentHierarchy;
                var children = new List<VSHierarchyItem>();

                debugInfo = null;

                if (this.IsNested)
                {
                    parentHierarchy = this.NestedHierarchy;
                }
                else
                {
                    parentHierarchy = this;
                }

                var firstChild = parentHierarchy.FirstVisibleChild;

                if (firstChild != null)
                {
                    string name = null;

                    try
                    {
                        name = firstChild.Name;
                    }
                    catch
                    {
                    }

                    try
                    {
                        var nextSibling = firstChild.NextVisibleSibling;

                        if (!children.Any(c => c.ItemID == firstChild.ItemID))
                        {
                            children.Add(firstChild);
                        }

                        while (nextSibling != null)
                        {
                            try
                            {
                                name = nextSibling.Name;
                            }
                            catch
                            {
                            }

                            if (!children.Any(c => c.ItemID == nextSibling.ItemID))
                            {
                                children.Add(nextSibling);
                            }

                            nextSibling = nextSibling.NextVisibleSibling;
                        }
                    }
                    catch
                    {
                    }
                }

                return children;
            }
        }

        public bool DefaultEnableDeployProjectCfg
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_DefaultEnableDeployProjectCfg, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool DefaultEnableBuildProjectCfg
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_DefaultEnableBuildProjectCfg, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool HasEnumerationSideEffects
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_HasEnumerationSideEffects, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSDESIGNER_FUNCTIONVISIBILITY DesignerFunctionVisibility
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_DesignerFunctionVisibility, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (VSDESIGNER_FUNCTIONVISIBILITY)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSDESIGNER_VARIABLENAMING DesignerVariableNaming
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_DesignerVariableNaming, out value);

                if (hr == VSConstants.S_OK)
                {
                    return ( VSDESIGNER_VARIABLENAMING)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid ProjectIDGuid
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool ShowOnlyItemCaption
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ShowOnlyItemCaption, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsNewUnsavedItem
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsNewUnsavedItem, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool AllowEditInRunMode
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_AllowEditInRunMode, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool ShowProjInSolutionPage
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ShowProjInSolutionPage, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid PreferredLanguageSID
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_PreferredLanguageSID, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool CanBuildFromMemory
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_CanBuildFromMemory, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsFindInFilesForegroundOnly
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsFindInFilesForegroundOnly, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsNonSearchable
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsNonSearchable, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string DefaultNamespace
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_DefaultNamespace, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSOVERLAYICON OverlayIconIndex
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_OverlayIconIndex, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (VSOVERLAYICON)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string ItemSubType
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ItemSubType, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string StorageType
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_StorageType, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsNonLocalStorage
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsNonLocalStorage, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsNonMemberItem
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsNonMemberItem, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsHiddenItem
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IsHiddenItem, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem NextVisibleSibling
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_NextVisibleSibling, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem((IVsHierarchy) this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint NextVisibleSiblingId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_NextVisibleSibling, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem FirstVisibleChild
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int) value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem(this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint FirstVisibleChildId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object StartupServices
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_StartupServices, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string OwnerKey
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_OwnerKey, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IVsHierarchy ImplantHierarchy
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ImplantHierarchy, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (IVsHierarchy)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object ConfigurationProvider
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ConfigurationProvider, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool Expanded
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Expanded, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IntPtr ItemDocCookie
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ItemDocCookie, out value);

                if (hr == VSConstants.S_OK)
                {
                    return new IntPtr((int)value);
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint ParentHierarchyItemId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ParentHierarchyItemid, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IVsHierarchy ParentHierarchy
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ParentHierarchy, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (IVsHierarchy)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object ReloadableProjectFile
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ReloadableProjectFile, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool HandlesOwnReload
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_HandlesOwnReload, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object ProjectType
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ProjectType, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string TypeName
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_TypeName, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VsStateIcon StateIconIndex
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_StateIconIndex, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (VsStateIcon)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object ExtSelectedItem
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ExtSelectedItem, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object ExtObject
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ExtObject, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string EditLabel
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_EditLabel, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IVsUserContext UserContext
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_UserContext, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (IVsUserContext)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public int SortPriority
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_SortPriority, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (int)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string ProjectDir
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ProjectDir, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint AltItemId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_AltItemid, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IVsHierarchy AltHierarchy
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_AltHierarchy, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (IVsHierarchy)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object BrowseObject
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_BrowseObject, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object SelContainer
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_SelContainer, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid CmdUIGuid
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_CmdUIGuid, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public int OpenFolderIconIndex
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_OpenFolderIconIndex, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (int)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IntPtr OpenFolderIconHandle
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_OpenFolderIconHandle, out value);

                if (hr == VSConstants.S_OK)
                {
                    return new IntPtr((int)value);
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IntPtr IconHandle
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IconHandle, out value);

                if (hr == VSConstants.S_OK)
                {
                    return new IntPtr((int)value);
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string ProjectName
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ProjectName, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string Name
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Name, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool ExpandByDefault
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_ExpandByDefault, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool Expandable
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Expandable, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public int IconIndex
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IconIndex, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (int)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public IntPtr hIconImgList
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_IconImgList, out value);

                if (hr == VSConstants.S_OK)
                {
                    return new IntPtr((int)value);
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string Caption
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Caption, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string SaveName
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_SaveName, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid TypeGuid
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_TypeGuid, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem Root
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Root, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem(this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint RootId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Root, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem NextSibling
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_NextSibling, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem(this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint NextSiblingId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_NextSibling, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem FirstChild
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_FirstChild, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem(this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint FirstChildId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_FirstChild, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public VSHierarchyItem Parent
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Parent, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return null;
                    }
                    else
                    {
                        return new VSHierarchyItem(this.Hierarchy, (uint)(int)value, this.ItemID);
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public uint ParentId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID.VSHPROPID_Parent, out value);

                if (hr == VSConstants.S_OK)
                {
                    if ((int)value == (int)__VSHPROPID.VSHPROPID_NIL)
                    {
                        return Convert.ToUInt32(__VSHPROPID.VSHPROPID_NIL);
                    }
                    else
                    {
                        return (uint)(int)value;
                    }
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        // __VSHPROPID2

        public string SupportedMyApplicationTypes
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_SupportedMyApplicationTypes, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool ExcludeFromExportItemTemplate
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_ExcludeFromExportItemTemplate, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool NoDefaultNestedHierSorting
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_NoDefaultNestedHierSorting, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string PriorityPropertyPagesCLSIDList
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_PriorityPropertyPagesCLSIDList, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid ProjectDesignerEditor
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_ProjectDesignerEditor, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool DisableApplicationSettings
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_DisableApplicationSettings, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid CategoryGuid
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_CategoryGuid, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string DebuggerSourcePaths
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_DebuggerSourcePaths, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string AppTitleBarTopHierarchyName
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_AppTitleBarTopHierarchyName, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool EnableDataSourceWindow
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_EnableDataSourceWindow, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool UseInnerHierarchyIconList
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_UseInnerHierarchyIconList, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool Container
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_Container, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool SuppressOutOfDateMessageOnBuild
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_SuppressOutOfDateMessageOnBuild, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public __VSDESIGNER_HIDDENCODEGENERATION DesignerHiddenCodeGeneration
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_DesignerHiddenCodeGeneration, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (__VSDESIGNER_HIDDENCODEGENERATION)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsUpgradeRequired
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_IsUpgradeRequired, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object IntellisenseUnknown
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_IntellisenseUnknown, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool SupportsProjectDesigner
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_SupportsProjectDesigner, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool KeepAliveDocument
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_KeepAliveDocument, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool IsLinkFile
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_IsLinkFile, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public int DebuggeeProcessId
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_DebuggeeProcessId, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (int)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string StatusBarClientText
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_StatusBarClientText, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public bool ChildrenEnumerated
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_ChildrenEnumerated, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (bool)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid AddItemTemplatesGuid
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_AddItemTemplatesGuid, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public object CfgBrowseObjectCATID
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_CfgBrowseObjectCATID, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid BrowseObjectCATID
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_BrowseObjectCATID, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public Guid ExtObjectCATID
        {
            get
            {
                int hr;
                Guid value;

                hr = Hierarchy.GetGuidProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_ExtObjectCATID, out value);

                if (hr == VSConstants.S_OK)
                {
                    return value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string CfgPropertyPagesCLSIDList
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }

        public string PropertyPagesCLSIDList
        {
            get
            {
                int hr;
                object value;

                hr = Hierarchy.GetProperty((uint)this.ItemID, (int)__VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList, out value);

                if (hr == VSConstants.S_OK)
                {
                    return (string)value;
                }
                else
                {
                    var exception = Marshal.GetExceptionForHR(hr);

                    throw exception;
                }
            }
        }
    }
}
