using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Utils;
using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using EnvDTE;
using EnvDTE80;
using CodeInterfaces;
using Microsoft.VisualStudio.OLE.Interop;
using System.Reflection;
using Microsoft.VisualStudio.PlatformUI;
using WPFControlLibrary;
using System.Windows;
using System.Windows.Controls;

namespace VisualStudioProvider
{
    public static class VSShellExtensions
    {
        public static Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

        public static string InspectVsComObject(this object obj)
        {
            ////return VsComObjectUtility.InspectVsComObject(obj);
            return null;
        }

#if USE_VS         
        public static string GetCaption(this VsMenuItem vsMenuItem)
        {
            string caption = string.Empty;

            vsMenuItem.GetDescendants(d =>
            {
                var element = (FrameworkElement)d;

                if (element is TextBlock textBlock)
                {
                    if (!textBlock.Text.IsNullOrEmpty())
                    {
                        caption = textBlock.Text;
                    }
                }
                else if (element is TextBox textBox)
                {
                    if (!textBox.Text.IsNullOrEmpty())
                    {
                        caption = textBox.Text;
                    }
                }
                else if (element is AccessText accessText)
                {
                    if (!accessText.Text.IsNullOrEmpty())
                    {
                        caption = accessText.Text;
                    }
                }

                return true;
            });

            return caption;
        }
#endif
        public static IEnumerable<object> GetObjects(this ISelectionContainer selectionContainer)
        {
            uint count;
            object[] objects;

            selectionContainer.CountObjects(0, out count);
            objects = new object[count];

            selectionContainer.GetObjects(0, count, objects);

            return objects;
        }

        public static Dictionary<Guid, uint> Proffer(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, params string[] serviceGuids)
        {
            var cookies = new Dictionary<Guid, uint>();
            var profferService = serviceProvider.QueryService<IProfferService>();

            foreach (var serviceGuid in serviceGuids)
            {
                uint cookie;
                var alternateGuid = Guid.Parse(serviceGuid);
                var hr = profferService.ProfferService(ref alternateGuid, serviceProvider, out cookie);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            return cookies;
        }

        public static T QueryService<T>(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider)
        {
            var type = typeof(T);
            var guidAttribute = type.GetCustomAttribute<GuidAttribute>();
            var guid = Guid.Parse(guidAttribute.Value);
            IntPtr ppvObject;
            int hr;

            hr = serviceProvider.QueryService(ref guid, ref IID_IUnknown, out ppvObject);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return (T)Marshal.GetObjectForIUnknown(ppvObject);
        }

        public static object QueryService(this Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider, Type type)
        {
            var guidAttribute = type.GetCustomAttribute<GuidAttribute>();
            var guid = Guid.Parse(guidAttribute.Value);
            IntPtr ppvObject;
            int hr;

            hr = serviceProvider.QueryService(ref guid, ref IID_IUnknown, out ppvObject);

            if (hr != VSConstants.S_OK)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            return Marshal.GetObjectForIUnknown(ppvObject);
        }

        public static T GetPropertyValue<T>(this IVsHierarchy hierarchy, uint itemid, __VSHPROPID property)
        {
            object obj;

            if (ErrorHandler.Succeeded(hierarchy.GetProperty(itemid, (int)property, out obj)))
            {
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }

        public static T GetPropertyValue<T>(this IVsWindowFrame frame, __VSFPROPID property)
        {
            object obj;

            if (ErrorHandler.Succeeded(frame.GetProperty((int)property, out obj)))
            {
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }

        public static T GetPropertyValue<T>(this IVsWindowView view, __VSFPROPID property)
        {
            object obj;

            if (ErrorHandler.Succeeded(view.GetProperty((int)property, out obj)))
            {
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }


        public static IOleDocumentView GetView(this IOleDocument document)
        {
            IEnumOleDocumentViews enumViews;
            IOleDocumentView view;

            document.EnumViews(out enumViews, out view);

            return view;
        }

        public static IEnumerable<string> GetVerbs(this IVsUIDataSource dataSource)
        {
            IVsUIEnumDataSourceVerbs enumVerbs;
            var verbs = new string[1];
            uint fetched;

            dataSource.EnumVerbs(out enumVerbs);

            while (enumVerbs.Next(1, verbs, out fetched) == VSConstants.S_OK)
            {
                yield return verbs[0];
            }
        }

        public static object GetValue(this IVsUIDataSource dataSource, string name)
        {
            IVsUIObject vsUIObject;
            object value;

            dataSource.GetValue(name, out vsUIObject);

            if (vsUIObject != null)
            {
                vsUIObject.get_Data(out value);
                return value;
            }

            return null;
        }

        public static IEnumerable<VsUIPropertyDescriptor> GetProperties(this IVsUIDataSource dataSource)
        {
            IVsUIEnumDataSourceProperties enumProperties;
            var properties = new VsUIPropertyDescriptor[1];
            uint fetched;

            dataSource.EnumProperties(out enumProperties);

            while (enumProperties.Next(1, properties, out fetched) == VSConstants.S_OK)
            {
                yield return properties[0];
            }
        }

        public static IEnumerable<IOleDocumentView> GetViews(this IOleDocument document)
        {
            IEnumOleDocumentViews enumViews;
            IOleDocumentView view;
            var views = new IOleDocumentView[0];
            uint fetched;

            document.EnumViews(out enumViews, out view);

            while (enumViews.Next(1, views, out fetched) == VSConstants.S_OK)
            {
                yield return views[0];
            }
        }

        public static string GetAttributeValue(this IVsUserContextItem item, string attribute)
        {
            string[] names;
            string[] values;
            int count;

            item.CountAttributes(attribute, out count);

            if (count > 0)
            {
                names = new string[count];
                values = new string[count];

                item.GetAttribute(attribute, 0, names, values);

                return values.First();
            }
            else
            {
                ErrorHandler.ThrowOnFailure(VSConstants.E_NOINTERFACE);
                return null;
            }
        }

        public static bool HasAttribute(this IVsUserContext userContext, string attribute)
        {
            string name;
            string value;
            int hr;

            if ((hr = userContext.GetAttribute(-1, attribute, 0, out name, out value)) == VSConstants.S_OK)
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        public static string GetAttributeValue(this IVsUserContext userContext, string attribute)
        {
            string name;
            string value;
            int hr;

            if ((hr = userContext.GetAttribute(-1, attribute, 0, out name, out value)) == VSConstants.S_OK)
            {
                return value;
            }
            else
            {
                ErrorHandler.ThrowOnFailure(hr);
                return null;
            }
        }

        public static IEnumerable<IVsUserContext> GetSubContexts(this IVsUserContext userContext)
        {
            int count;

            ErrorHandler.ThrowOnFailure(userContext.CountSubcontexts(out count));

            for (var x = 0; x < count; x++)
            {
                IVsUserContext subContext;

                ErrorHandler.ThrowOnFailure(userContext.GetSubcontext(x, out subContext));

                yield return subContext;
            }
        }

        public static bool HasSubContext(this IVsUserContext parentContext, IVsUserContext childContext)
        {
            int count;

            ErrorHandler.ThrowOnFailure(parentContext.CountSubcontexts(out count));

            for (var x = 0; x < count; x++)
            { 
                IVsUserContext subContext;

                ErrorHandler.ThrowOnFailure(parentContext.GetSubcontext(x, out subContext));

                if (subContext == childContext)
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<IVsUserContext> GetSubContexts(this IVsUserContext userContext, Func<IVsUserContext, bool> filter)
        {
            int count;

            ErrorHandler.ThrowOnFailure(userContext.CountSubcontexts(out count));

            for (var x = 0; x < count; x++)
            {
                IVsUserContext subContext;

                ErrorHandler.ThrowOnFailure(userContext.GetSubcontext(x, out subContext));

                if (filter(subContext))
                {
                    yield return subContext;
                }
            }
        }

        public static _Assembly LoadOutputAssembly(this IVSProject project)
        {
            if (File.Exists(project.OutputFile))
            {
                var outputFile = project.OutputFile;

                return AssemblyLoader.LoadClone(outputFile);
            }
            else
            {
                return null;
            }
        }

        public static int GetWidth(this Microsoft.VisualStudio.OLE.Interop.RECT rect)
        {
            return rect.right - rect.left;
        }

        public static int GetHeight(this  Microsoft.VisualStudio.OLE.Interop.RECT rect)
        {
            return rect.bottom - rect.top;
        }

        public static Project GetSelectedProject()
        {
            IntPtr hierarchyPointer;
            IntPtr selectionContainerPointer;
            Object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint projectItemId;

            var monitorSelection = (IVsMonitorSelection) Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            monitorSelection.GetCurrentSelection(out hierarchyPointer, out projectItemId, out multiItemSelect, out selectionContainerPointer);

            if (multiItemSelect != null)
            {
                uint count;
                int singleHierarchy;
                VSITEMSELECTION[] items;

                multiItemSelect.GetSelectionInfo(out count, out singleHierarchy);

                items = new VSITEMSELECTION[count];

                multiItemSelect.GetSelectedItems(0, count, items);

                var selectedHierarchy = items.First().pHier as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    projectItemId = items.First().itemid;

                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
                }
            }
            else if (hierarchyPointer != IntPtr.Zero)
            {
                var selectedHierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    string name;

                    selectedHierarchy.GetCanonicalName(projectItemId, out name);

                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
                }
            }

            if (selectedObject is Project)
            {
                var selectedProject = selectedObject as Project;

                return selectedProject;
            }
            else if (selectedObject is ProjectItem)
            {
                var selectedItem = selectedObject as ProjectItem;

                return selectedItem.ContainingProject;
            }

            return null;
        }

        public static UIHierarchyItem GetItem(this EnvDTE.Window window, string path)
        {
            UIHierarchyItem item = null;
            string ancestorPath;
            var hierarchy = (UIHierarchy)window.Object;
            var pathParts = path.Split('\\');

            try
            {
                item = (UIHierarchyItem)hierarchy.GetItem(path);
            }
            catch (Exception ex)
            {
            }

            if (item == null)
            {
                while (pathParts.Count() > 1)
                {
                    UIHierarchyItem ancestorItem;
                    pathParts = pathParts.Take(pathParts.Count() - 1).ToArray();
                    ancestorPath = pathParts.ToDelimitedList("\\");

                    try
                    {
                        ancestorItem = (UIHierarchyItem)hierarchy.GetItem(ancestorPath);
                        ancestorItem.UIHierarchyItems.Expanded = true;

                        try
                        {
                            item = (UIHierarchyItem)hierarchy.GetItem(path);
                            break;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return item;
        }

        public static IEnumerable<ProjectItem> GetAllProjectItems(this Project project)
        {
            Action<IEnumerable<EnvDTE.ProjectItem>> getItems = null;
            var projectItems = new List<ProjectItem>();

            try
            {
                getItems = (items) =>
                {
                    foreach (var item in items)
                    {
                        projectItems.Add(item);

                        if (item.ProjectItems != null)
                        {
                            getItems(item.ProjectItems.Cast<EnvDTE.ProjectItem>());
                        }
                    }
                };

                foreach (var item in project.ProjectItems.Cast<ProjectItem>())
                {
                    projectItems.Add(item);

                    if (project.ProjectItems != null)
                    {
                        getItems(item.ProjectItems.Cast<EnvDTE.ProjectItem>());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }

            return projectItems;
        }

        public static IEnumerable<string> GetFileNames(this ProjectItem projectItem)
        {
            for (var x = 1; x <= projectItem.FileCount; x++)
            {
                yield return projectItem.get_FileNames((short) x);
            }
        }

        public static IEnumerable<Project> GetAllProjects(this Solution solution)
        {
            Action<IEnumerable<EnvDTE.ProjectItem>> getProjects = null;
            var projects = new List<Project>();

            try
            {
                getProjects = (items) =>
                {
                    foreach (var item in items)
                    {
                        if (item.SubProject != null)
                        {
                            projects.Add(item.SubProject);

                            if (item.SubProject.ProjectItems != null)
                            {
                                getProjects(item.SubProject.ProjectItems.Cast<EnvDTE.ProjectItem>());
                            }
                        }

                        if (item.ProjectItems != null)
                        {
                            getProjects(item.ProjectItems.Cast<EnvDTE.ProjectItem>());
                        }
                    }
                };

                foreach (var project in solution.Projects.Cast<Project>())
                {
                    projects.Add(project);

                    if (project.ProjectItems != null)
                    {
                        getProjects(project.ProjectItems.Cast<EnvDTE.ProjectItem>());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
            }

            return projects;
        }

        public static Project FindProject(this Solution solution, string projectName)
        {
            Project projectFind = null;
            Func<IEnumerable<EnvDTE.ProjectItem>, Project> findProject = null;

            findProject = (items) =>
            {
                foreach (var item in items)
                {
                    if (item.SubProject != null)
                    {
                        if (item.SubProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                        {
                            var projectFind2 = findProject(item.SubProject.ProjectItems.Cast<EnvDTE.ProjectItem>());

                            if (projectFind2 != null)
                            {
                                return projectFind2;
                            }
                        }
                        else if (item.SubProject.Name == projectName)
                        {
                            return item.SubProject;
                        }
                    }

                    if (item.ProjectItems != null)
                    {
                        var projectFind2 = findProject(item.ProjectItems.Cast<EnvDTE.ProjectItem>());

                        if (projectFind2 != null)
                        {
                            return projectFind2;
                        }
                    }
                }

                return null;
            };

            foreach (Project project in solution.Projects.Cast<Project>())
            {
                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    projectFind = findProject(project.ProjectItems.Cast<EnvDTE.ProjectItem>());

                    if (projectFind != null)
                    {
                        break;
                    }
                }
                else if (project.Name == projectName)
                {
                    return project;
                }
            }

            return projectFind;
        }

        public static T GetService<T>(this IServiceProvider provider)
        {
            var serviceProvider = new ServiceProvider(provider);

            return (T) serviceProvider.GetService(typeof(T));
        }

        public static object GetService(this IServiceProvider provider, Type type)
        {
            var serviceProvider = new ServiceProvider(provider);

            return serviceProvider.GetService(type);
        }

        public static string GetFileName(this IVsProject project)
        {
            string name;
            var hr = project.GetMkDocument((uint) VSConstants.VSITEMID.Root, out name);

            if (ErrorHandler.Failed(hr))
            {
                return null;
            }

            return name;
        }

        public static string GetFileName(this IVsProject project, uint itemid)
        {
            string name;
            var hr = project.GetMkDocument(itemid, out name);

            if (ErrorHandler.Failed(hr))
            {
                return null;
            }

            return name;
        }

        public static void ExpandItem(this IVsUIHierarchyWindow2 window, IVsUIHierarchy hierarchy, VSHierarchyItem item)
        {
            int hr;
            __VSHIERARCHYITEMSTATE state;
            uint stateAsInt;

            hr = window.GetItemState(hierarchy, (uint)item.ItemID, (uint)__VSHIERARCHYITEMSTATE.HIS_Expanded, out stateAsInt);

            if (ErrorHandler.Failed(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            state = (__VSHIERARCHYITEMSTATE)stateAsInt;

            if (state != __VSHIERARCHYITEMSTATE.HIS_Expanded)
            {
                hr = window.ExpandItem(hierarchy, (uint)item.ItemID, EXPANDFLAGS.EXPF_ExpandFolder);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        public static void SelectItem(this IVsUIHierarchyWindow2 window, IVsUIHierarchy hierarchy, VSHierarchyItem item)
        {
            int hr;
            __VSHIERARCHYITEMSTATE state;
            uint stateAsInt;

            hr = window.GetItemState(hierarchy, (uint)item.ItemID, (uint)__VSHIERARCHYITEMSTATE.HIS_Selected, out stateAsInt);

            if (ErrorHandler.Failed(hr))    
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            state = (__VSHIERARCHYITEMSTATE)stateAsInt;

            if (state != __VSHIERARCHYITEMSTATE.HIS_Selected)
            {
                hr = window.ExpandItem(hierarchy, (uint)item.ItemID, EXPANDFLAGS.EXPF_SelectItem);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        public static void SelectItem(this IVsUIHierarchyWindow2 window, IVsUIHierarchy hierarchy, uint itemId)
        {
            int hr;
            __VSHIERARCHYITEMSTATE state;
            uint stateAsInt;

            hr = window.GetItemState(hierarchy, (uint)itemId, (uint)__VSHIERARCHYITEMSTATE.HIS_Selected, out stateAsInt);

            if (ErrorHandler.Failed(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            state = (__VSHIERARCHYITEMSTATE)stateAsInt;

            if (state != __VSHIERARCHYITEMSTATE.HIS_Selected)
            {
                hr = window.ExpandItem(hierarchy, (uint)itemId, EXPANDFLAGS.EXPF_SelectItem);

                if (ErrorHandler.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        public static Solution GetSolution(this IVsUIHierarchy hierarchy)
        {
            var extObject = hierarchy.GetRootItem().ExtObject;

            return (Solution)extObject;
        }

        public static string DebugGetHierarchy(this IVsHierarchy hierarchy)
        {
            var builder = new StringBuilder();
            var root = hierarchy.GetRootItem();
            Action<VSHierarchyItem, int> recurse = null;

            Console.WriteLine("\r\n**************** Hierarchy for {0} ****************\r\n", hierarchy.ToString());

            recurse = (parent, indent) =>
            {
                builder.AppendLineFormatTabIndent(indent, parent.Name);
                Console.WriteLine(string.Format("{0}{1}", new string('\t', indent), parent.Name));

                foreach (var child in parent.Children)
                {
                    recurse(child, indent + 1);
                }
            };

            recurse(root, 0);

            Console.WriteLine("\r\n**************** End Hierarchy ****************\r\n", hierarchy.ToString());

            return builder.ToString();
        } 

        public static string GetDebugVisibleHierarchy(this IVsHierarchy hierarchy)
        {
            var builder = new StringBuilder();
            var root = hierarchy.GetRootItem();
            Action<VSHierarchyItem, int> recurse = null;

            Console.WriteLine("\r\n**************** Visible Hierarchy for {0} ****************\r\n", hierarchy.ToString());

            recurse = (parent, indent) =>
            {
                builder.AppendLineFormatTabIndent(indent, parent.Name);
                Console.WriteLine(string.Format("{0}{1}", new string('\t', indent), parent.Name));

                foreach (var child in parent.VisibleChildren)
                {
                    recurse(child, indent + 1);
                }
            };

            recurse(root, 0);

            Console.WriteLine("\r\n**************** End Hierarchy ****************\r\n", hierarchy.ToString());

            return builder.ToString();
        }

        public static IEnumerable<IVsProject> GetProjects(this IVsSolution solution)
        {
            IEnumHierarchies projects;
            Guid refGuid = Guid.Empty;
            var hr = solution.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_ALLINSOLUTION, ref refGuid, out projects);
            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched = 0;

            if (ErrorHandler.Failed(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            while (projects.Next(1, hierarchy, out fetched) == VSConstants.S_OK)
            {
                if (hierarchy[0] is IVsProject)
                {
                    var project = (IVsProject)hierarchy[0];

                    yield return project;
                }
                else
                {
                    var item = hierarchy[0];
                    var debug = false;

                    if (debug)
                    {
                        item.InspectIDispatch();
                    }
                }
            }
        }

        public static IVsProject GetProject(this IVsSolution solution, Func<IVsProject, bool> predicate)
        {
            IEnumHierarchies projects;
            Guid refGuid = Guid.Empty;
            var hr = solution.GetProjectEnum((uint) __VSENUMPROJFLAGS.EPF_ALLINSOLUTION, ref refGuid, out projects);
            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched = 0;
            IVsProject projectFound = null;

            if (ErrorHandler.Failed(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            while (projects.Next(1, hierarchy, out fetched) == VSConstants.S_OK)
            {
                if (hierarchy[0] is IVsProject)
                {
                    var project = (IVsProject)hierarchy[0];

                    if (predicate(project))
                    {
                        if (projectFound != null)
                        {
                            throw new InvalidOperationException("More than one element satisfies the condition in predicate.");
                        }

                        projectFound = project;
                    }
                }
                else
                {
                    var item = hierarchy[0];
                    var debug = false;

                    if (debug)
                    {
                        item.InspectIDispatch();
                    }
                }
            }

            return projectFound;
        }

        public static VSHierarchyItem GetLowestLevelItem(this IVsUIHierarchy hierarchy, string path)
        {
            var parts = path.Split('\\');
            var root = hierarchy.GetRootItem();
            var builder = new StringBuilder();
            VSHierarchyItem item = null;
            VSHierarchyItem lowest = null;

            foreach (var part in parts)
            {
                if (item == null)
                {
                    if (root.Name != part)
                    {
                        return null;
                    }
                    else
                    {
                        item = root;
                        lowest = item;
                    }
                }
                else
                {
                    lowest = item;
                    item = item.Children.SingleOrDefault(i => i.Name.ToLower() == part.ToLower());

                    if (item == null)
                    {
                        return lowest;
                    }
                    else
                    {
                        lowest = item;
                    }
                }
            }

            return lowest;
        }

        public static VSHierarchyItem GetLowestLevelVisibleItem(this IVsUIHierarchy hierarchy, string path)
        {
            var parts = path.Split('\\');
            var root = hierarchy.GetRootItem();
            var builder = new StringBuilder();
            VSHierarchyItem item = null;
            VSHierarchyItem lowest = null;

            foreach (var part in parts)
            {
                if (item == null)
                {
                    if (root.Name != part)
                    {
                        return null;
                    }
                    else
                    {
                        item = root;
                        lowest = item;
                    }
                }
                else
                {
                    lowest = item;
                    item = item.VisibleChildren.SingleOrDefault(i => i.Name.ToLower() == part.ToLower());

                    if (item == null)
                    {
                        return lowest;
                    }
                    else
                    {
                        lowest = item;
                    }
                }
            }

            return lowest;
        }

        public static VSHierarchyItem GetItem(this IVsHierarchy hierarchy, IVSProject project)
        {
            var projectRelativePath = Path.Combine(project.ParentSolution.Name, project.Hierarchy);
            var projectItem = hierarchy.GetItem(projectRelativePath);

            return projectItem;
        }

        public static VSHierarchyItem GetItem(this IVsHierarchy hierarchy, string path)
        {
            var parts = path.Split('\\');
            var root = hierarchy.GetRootItem();
            var builder = new StringBuilder();
            VSHierarchyItem item = null;

            foreach (var part in parts)
            {
                if (item == null)
                {
                    if (root.Name != part)
                    {
                        return null;
                    }
                    else
                    {
                        item = root;
                    }
                }
                else
                {
                    item = item.Children.SingleOrDefault(i => i.Name == part);

                    if (item == null)
                    {
                        break;
                    }
                }
            }

            return item;
        } 

        public static VSHierarchyItem GetItem(this IVsHierarchy hierarchy, uint itemId)
        {
            var root = hierarchy.GetRootItem();
            var name = root.Name;
            var builder = new StringBuilder();
            VSHierarchyItem item = null;
            Action<VSHierarchyItem> recurseFind = null;

            recurseFind = (parent) =>
            {
                name = parent.Name;

                if (parent.ItemID == itemId)
                {
                    item = parent;
                    return;
                }

                foreach (var childItem in parent.Children)
                {
                    name = childItem.Name;

                    if (item != null)
                    {
                        break;
                    }
                    else
                    {
                        recurseFind(childItem);
                    }
                }
            };

            recurseFind(root);

            return item;
        }

        public static string GetHierarchyPath(this IVsProject project)
        {
            var vsHierarchy = project as IVsHierarchy;
            string path = null;

            if (vsHierarchy != null)
            {
                var hierarchyItem = vsHierarchy.GetRootItem();
                IVsHierarchy parentHierarchy = null;

                path = hierarchyItem.Name;

                try
                {
                    parentHierarchy = hierarchyItem.ParentHierarchy;
                }
                catch
                {
                }

                while (parentHierarchy != null)
                {
                    var vsParentHierarchy = (IVsHierarchy)hierarchyItem.ParentHierarchy;

                    hierarchyItem = vsParentHierarchy.GetRootItem();

                    path = path.Prepend(hierarchyItem.Name.Append(@"\"));

                    try
                    {
                        parentHierarchy = hierarchyItem.ParentHierarchy;
                    }
                    catch
                    {
                        parentHierarchy = null;
                    }
                }
            }

            return path;
        }

        public static VSHierarchyItem GetRootItem(this IVsHierarchy hierarchy)
        {
            return new VSHierarchyItem(hierarchy, (uint)VSConstants.VSITEMID.Root, 0);
        }

        public static IVsUIHierarchy GetSolutionRootHierarchy(this IVsUIHierarchyWindow2 window)
        {
            var hierarchy = (IVsUIHierarchy) Package.GetGlobalService(typeof(SVsSolution));

            return hierarchy;
        }

        public static string GetCaption(this IVsWindowFrame frame)
        {
            object caption;

            frame.GetProperty((int) __VSFPROPID.VSFPROPID_Caption, out caption);

            return (string)caption;
        }

        public static string GetEditorCaption(this IVsWindowFrame frame)
        {
            object caption;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_EditorCaption, out caption);

            return (string)caption;
        }

        public static Guid GetEditorType(this IVsWindowFrame frame)
        {
            Guid type;

            frame.GetGuidProperty((int)__VSFPROPID.VSFPROPID_guidEditorType, out type);

            return type;
        }

        public static object GetDocData(this IVsWindowFrame frame)
        {
            object docData;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocData, out docData);

            return docData;
        }

        public static string GetDocName(this IVsWindowFrame frame)
        {
            object name;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out name);

            return (string) name;
        }

        public static IVsUserContext GetUserContext(this IVsWindowFrame frame)
        {
            object userContext;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_UserContext, out userContext);

            return (IVsUserContext)userContext;
        }

        public static T GetView<T>(this IVsWindowFrame frame)
        {
            object frameView;

            frame.GetProperty((int)__VSFPROPID.VSFPROPID_DocView, out frameView);

            return (T) frameView;
        }

        public static IEnumerable<IVsWindowFrame> GetToolWindows(this IVsUIShell shell)
        {
            IEnumWindowFrames enumWindowFrames;
            var frames = new IVsWindowFrame[1];
            uint fetched;
            int hr = shell.GetToolWindowEnum(out enumWindowFrames);

            if (ErrorHandler.Failed(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            while (enumWindowFrames.Next(1, frames, out fetched) == VSConstants.S_OK)
            {
                yield return frames[0];
            }
        }

        public static IEnumerable<IVsPackage> GetPackages(this IVsShell shell)
        {
            IEnumPackages enumPackages;
            var packages = new IVsPackage[1];
            uint fetched;
            int hr = shell.GetPackageEnum(out enumPackages);

            if (ErrorHandler.Failed(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            while (enumPackages.Next(1, packages, out fetched) == VSConstants.S_OK)
            {
                yield return packages[0];
            }
        }
        public static IEnumerable<IVsWindowFrame> GetDocumentWindows(this IVsUIShell shell)
        {
            IEnumWindowFrames enumWindowFrames;
            var frames = new IVsWindowFrame[1];
            uint fetched;
            int hr = shell.GetDocumentWindowEnum(out enumWindowFrames);

            if (ErrorHandler.Failed(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            while (enumWindowFrames.Next(1, frames, out fetched) == VSConstants.S_OK)
            {
                yield return frames[0];
            }
        }

        public static void InspectIDispatch(this object obj)
        {
        }

        public static void Inspect(this VSHierarchyItem item)
        {
            try
            {
                var defaultEnableDeployProjectCfg = item.DefaultEnableDeployProjectCfg;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var defaultEnableBuildProjectCfg = item.DefaultEnableBuildProjectCfg;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var hasEnumerationSideEffects = item.HasEnumerationSideEffects;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var designerFunctionVisibility = item.DesignerFunctionVisibility;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var designerVariableNaming = item.DesignerVariableNaming;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var projectIDGuid = item.ProjectIDGuid;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var showOnlyItemCaption = item.ShowOnlyItemCaption;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isNewUnsavedItem = item.IsNewUnsavedItem;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var allowEditInRunMode = item.AllowEditInRunMode;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var showProjInSolutionPage = item.ShowProjInSolutionPage;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var preferredLanguageSID = item.PreferredLanguageSID;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var canBuildFromMemory = item.CanBuildFromMemory;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isFindInFilesForegroundOnly = item.IsFindInFilesForegroundOnly;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isNonSearchable = item.IsNonSearchable;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var defaultNamespace = item.DefaultNamespace;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var overlayIconIndex = item.OverlayIconIndex;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var itemSubType = item.ItemSubType;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var storageType = item.StorageType;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isNonLocalStorage = item.IsNonLocalStorage;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isNonMemberItem = item.IsNonMemberItem;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isHiddenItem = item.IsHiddenItem;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var nextVisibleSibling = item.NextVisibleSibling;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var nextVisibleSiblingId = item.NextVisibleSiblingId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var firstVisibleChild = item.FirstVisibleChild;
                var firstVisibleChildName = firstVisibleChild.Name;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var firstVisibleChildId = item.FirstVisibleChildId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var startupServices = item.StartupServices;

                if (startupServices != null)
                {
                    startupServices.InspectIDispatch();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var ownerKey = item.OwnerKey;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var implantHierarchy = item.ImplantHierarchy;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var configurationProvider = item.ConfigurationProvider;

                if (configurationProvider != null)
                {
                    configurationProvider.InspectIDispatch();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var expanded = item.Expanded;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var itemDocCookie = item.ItemDocCookie;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var parentHierarchyItemId = item.ParentHierarchyItemId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var parentHierarchy = item.ParentHierarchy;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var reloadableProjectFile = item.ReloadableProjectFile;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var handlesOwnReload = item.HandlesOwnReload;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var projectType = item.ProjectType;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var typeName = item.TypeName;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var stateIconIndex = item.StateIconIndex;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var extSelectedItem = item.ExtSelectedItem;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var extObject = item.ExtObject;  // almost always VSLangProject

                if (extObject != null)
                {
                    extObject.InspectIDispatch();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var editLabel = item.EditLabel;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var userContext = item.UserContext;

                if (userContext != null)
                {
                    userContext.InspectIDispatch();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var sortPriority = item.SortPriority;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var projectDir = item.ProjectDir;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var altItemId = item.AltItemId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var altHierarchy = item.AltHierarchy;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var browseObject = item.BrowseObject;

                if (browseObject != null)
                {
                    browseObject.InspectIDispatch();
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var selContainer = item.SelContainer;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var cmdUIGuid = item.CmdUIGuid;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var openFolderIconIndex = item.OpenFolderIconIndex;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var openFolderIconHandle = item.OpenFolderIconHandle;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var iconHandle = item.IconHandle;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var projectName = item.ProjectName;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var name = item.Name;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var expandByDefault = item.ExpandByDefault;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var expandable = item.Expandable;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var iconIndex = item.IconIndex;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var hIconImgList = item.hIconImgList;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var caption = item.Caption;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var saveName = item.SaveName;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var typeGuid = item.TypeGuid;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var root = item.Root;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var rootId = item.RootId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var nextSibling = item.NextSibling;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var nextSiblingId = item.NextSiblingId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var firstChild = item.FirstChild;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var firstChildId = item.FirstChildId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var parent = item.Parent;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var parentId = item.ParentId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var supportedMyApplicationTypes = item.SupportedMyApplicationTypes;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var excludeFromExportItemTemplate = item.ExcludeFromExportItemTemplate;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var noDefaultNestedHierSorting = item.NoDefaultNestedHierSorting;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var priorityPropertyPagesCLSIDList = item.PriorityPropertyPagesCLSIDList;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var projectDesignerEditor = item.ProjectDesignerEditor;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var disableApplicationSettings = item.DisableApplicationSettings;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var categoryGuid = item.CategoryGuid;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var debuggerSourcePaths = item.DebuggerSourcePaths;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var appTitleBarTopHierarchyName = item.AppTitleBarTopHierarchyName;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var enableDataSourceWindow = item.EnableDataSourceWindow;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var useInnerHierarchyIconList = item.UseInnerHierarchyIconList;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var container = item.Container;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var suppressOutOfDateMessageOnBuild = item.SuppressOutOfDateMessageOnBuild;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var designerHiddenCodeGeneration = item.DesignerHiddenCodeGeneration;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isUpgradeRequired = item.IsUpgradeRequired;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var intellisenseUnknown = item.IntellisenseUnknown;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var supportsProjectDesigner = item.SupportsProjectDesigner;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var keepAliveDocument = item.KeepAliveDocument;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var isLinkFile = item.IsLinkFile;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var debuggeeProcessId = item.DebuggeeProcessId;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var statusBarClientText = item.StatusBarClientText;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var childrenEnumerated = item.ChildrenEnumerated;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var addItemTemplatesGuid = item.AddItemTemplatesGuid;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var cfgBrowseObjectCATID = item.CfgBrowseObjectCATID;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var browseObjectCATID = item.BrowseObjectCATID;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var extObjectCATID = item.ExtObjectCATID;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var cfgPropertyPagesCLSIDList = item.CfgPropertyPagesCLSIDList;
            }
            catch (Exception ex)
            {
            }

            try
            {
                var propertyPagesCLSIDList = item.PropertyPagesCLSIDList;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
