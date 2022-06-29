using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using Utils;
using System.Linq.Expressions;

namespace SLControlLibrary
{
    internal static class UIRelationCacheEngine
    {
        private static List<IUIRelationCacheSet> cacheSets;
        private static Dictionary<UIAncestorDescendentKey, UIAncestorDescendantInfo> isChildRelations;
        private static Dictionary<TaggedStackFrameKey, UIRelationDiagnostic> diagnostics;
        private static Analytics analytics;
        private static Dictionary<DependencyObject, UIRelationChangeHandlerEntry> changeHandlers;

        static UIRelationCacheEngine()
        {
            changeHandlers = new Dictionary<DependencyObject, UIRelationChangeHandlerEntry>();
            cacheSets = new List<IUIRelationCacheSet>();
            isChildRelations = new Dictionary<UIAncestorDescendentKey, UIAncestorDescendantInfo>();
            diagnostics = new Dictionary<TaggedStackFrameKey, UIRelationDiagnostic>();
            analytics = new Analytics();

            Globals.Idle += new EventHandler(OnIdle);
        }

        private static void OnIdle(object sender, EventArgs e)
        {
            var debugViewDiagnostics = false;

            if (debugViewDiagnostics)
            {
                foreach (var diagnostic in diagnostics.Values)
                {
                    try
                    {
                        var debugInfo = diagnostic.DebugInfo;

                        var comparison = diagnostic.AverageComparison;
                        var byReset = diagnostic.AveragesByReset;
                    }
                    catch
                    {
                        Debugger.Break();
                    }
                }
            }

            if (analytics.AverageProcessLoad > 0)
            {
                return;
            }

            var oneSecond = TimeSpan.FromSeconds(1);
            var now = DateTime.Now;
            var deleteIsChildKeys = new List<UIAncestorDescendentKey>();

            foreach (var cacheSet in cacheSets)
            {
                foreach (var cacheEntry in cacheSet.AllEntries)
                {
                    if (cacheEntry.Entries != null)
                    {
                        if (cacheEntry.LastUpdated == DateTime.MinValue)
                        {
                            Debugger.Break();
                        }
                        else if (now - cacheEntry.LastUpdated > oneSecond || cacheEntry.IsDirty)
                        {
                            cacheSet.NotifyReset(cacheEntry, UIRelationCacheEntryResetReason.TimeOut);
                            cacheEntry.Reset(UIRelationCacheEntryResetReason.TimeOut);
                        }
                    }
                }

                cacheSet.FinalizeReset();
            }

            if (analytics.AverageProcessLoad > 0)
            {
                return;
            }

            foreach (var isChildRelationPair in isChildRelations)
            {
                var ancestorDescendantInfo = isChildRelationPair.Value;

                if (ancestorDescendantInfo.LastUpdated == DateTime.MinValue)
                {
                    Debugger.Break();
                }
                else if (now - ancestorDescendantInfo.LastUpdated > oneSecond || ancestorDescendantInfo.IsDirty)
                {
                    deleteIsChildKeys.Add(isChildRelationPair.Key);
                }
            }

            if (analytics.AverageProcessLoad > 0)
            {
                return;
            }

            foreach (var deleteIsChildKey in deleteIsChildKeys)
            {
                isChildRelations.Remove(deleteIsChildKey);
            }
        }

        private static void ChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dependencyObject = (DependencyObject)sender;
            var handlerEntry = changeHandlers[dependencyObject];

            if (e.OldValue == null)
            {
                if (e.NewValue != handlerEntry.InitialValue)
                {
                    handlerEntry.Handlers.ForEach(a => a(handlerEntry.DependencyPropertyOwner, e));
                }
            }
            else
            {
                handlerEntry.Handlers.ForEach(a => a(handlerEntry.DependencyPropertyOwner, e));
            }
        }

        internal static void UnRegisterForChange<T>(this DependencyObject dependencyObject, Expression<Func<T>> dependencyPropertyFunc, Action<object, DependencyPropertyChangedEventArgs> senderChangedCallback)
        {
            var changeHandlerEntry = changeHandlers[dependencyObject];
            
            dependencyObject.UnRegisterDependencyPropertyChanged(dependencyPropertyFunc);

            changeHandlerEntry.Handlers.Remove(senderChangedCallback);
        }

        internal static void RegisterForChange<T>(this DependencyObject dependencyObject, Expression<Func<T>> dependencyPropertyFunc, object initialValue, Action<object, DependencyPropertyChangedEventArgs> senderChangedCallback, DependencyObject dependencyOwner)
        {
            UIRelationChangeHandlerEntry changeHandlerEntry;

            if (changeHandlers.ContainsKey(dependencyObject))
            {
                changeHandlerEntry = changeHandlers[dependencyObject];
            }
            else
            {
                changeHandlerEntry = new UIRelationChangeHandlerEntry(dependencyOwner, dependencyObject, initialValue);

                changeHandlers.Add(dependencyObject, changeHandlerEntry);

                Action<DependencyPropertyChangedEventArgs> changedCallback = (e) =>
                {
                    ChangedHandler(dependencyObject, e);
                };

                dependencyObject.RegisterDependencyPropertyChanged(dependencyPropertyFunc, changedCallback);
            }

            changeHandlerEntry.Handlers.Add(senderChangedCallback);
        }

        internal static UIRelationTimingAverage GetAverage(this IEnumerable<UIRelationTimingEntry> entries)
        {
            var average = new UIRelationTimingAverage();

            if (entries.Count() > 0)
            {
                average.Milliseconds = (int)entries.Average(e => e.Milliseconds);
                average.ProcessLoad = entries.Average(e => e.ProcessLoad);
                average.TimeRange = entries.Last().Timestamp - entries.First().Timestamp;
            }
            else
            {
                average.NoEntries = true;
            }

            return average;
        }

        internal static UIRelationDiagnostic GetDiagnostic(this StackFrame frame, int hash)
        {
            UIRelationDiagnostic diagnostic = null;
            var method = frame.GetMethod();
            var key = new TaggedStackFrameKey { MethodBase = frame.GetMethod(), Hash = hash };

            diagnostic = diagnostics[key];

            return diagnostic;
        }

        internal static UIRelationDiagnostic GetCreateDiagnostic(this StackFrame frame, int hash)
        {
            UIRelationDiagnostic diagnostic = null;
            var method = frame.GetMethod();
            var key = new TaggedStackFrameKey { MethodBase = frame.GetMethod(), Hash = hash };

            if (diagnostics.ContainsKey(key))
            {
                diagnostic = diagnostics[key];
            }
            else
            {
                diagnostic = new UIRelationDiagnostic(key);

                diagnostics.Add(key, diagnostic);
            }

            return diagnostic;
        }

        internal static float GetLoad(this UIRelationTimingEntry entry)
        {
            return analytics.AverageProcessLoad;
        }

        internal static string GetObjectMethod(this IUIObjectMethod objectMethod)
        {
            var methodSignature = objectMethod.Method.GetSignature();
            var regex = new Regex(@".*?(?<replace>\(\w+(,)*)");
            string result;

            if (regex.IsMatch(methodSignature))
            {
                var match = regex.Match(methodSignature);
                var replaceValue = match.Groups["replace"].Value;

                methodSignature = methodSignature.Replace(replaceValue, "(");
            }

            if (objectMethod.UIObject is DependencyObject)
            {
                result = ((DependencyObject) objectMethod.UIObject).GetTypeAndName("_") + "." + methodSignature;
            }
            else if (objectMethod.UIObject is IControl)
            {
                result = ((IControl)objectMethod.UIObject).GetTypeAndName("_") + "." + methodSignature;
            }
            else if (objectMethod.UIObject != null)
            {
                result = objectMethod.UIObject.GetType().Name + "." + methodSignature;
            }
            else
            {
                result = objectMethod.Hash + "." + methodSignature;
            }

            return result;
        }

        internal static bool GetAncestors(this DependencyObject obj, ref List<DependencyObject> ancestors)
        {
            if (obj is FrameworkElement)
            {
                var element = (FrameworkElement)obj;

                if (element.Tag is IUIRelationCacheSet)
                {
                    var cacheSet = (IUIRelationCacheSet)element.Tag;

                    if (cacheSet.Ancestors.Entries != null)
                    {
                        var stackTrace = new StackTrace();
                        var stackFrame = stackTrace.GetFrame(1);

                        ancestors = cacheSet.Ancestors.Entries.OfType<DependencyObject>().ToList();

                        var diagnostics = stackFrame.GetCreateDiagnostic(obj.GetHashCode());

                        diagnostics.CurrentStopWatch.IsFromCache = true;

                        return true;
                    }
                }
            }

            return false;
        }

        internal static void SetAncestors(this DependencyObject obj, List<DependencyObject> ancestors)
        {
            if (obj is FrameworkElement)
            {
                IUIRelationCacheSet cacheSet = null;
                var element = (FrameworkElement)obj;

                if (element.Tag is IUIRelationCacheSet)
                {
                    cacheSet = (IUIRelationCacheSet)element.Tag;
                }
                else if (element.Tag == null)
                {
                    cacheSet = new UIRelationCacheSet<DependencyObject>(obj);

                    element.Tag = cacheSet;

                    cacheSets.Add(cacheSet);
                }

                if (cacheSet != null)
                {
                    var stackTrace = new StackTrace();
                    var stackFrame = stackTrace.GetFrame(1);
                    var hash = obj.GetHashCode();
                    var diagnostic = stackFrame.GetDiagnostic(hash);

                    cacheSet.Ancestors.Diagnostic = diagnostic;
                    cacheSet.Ancestors.Entries = ancestors.OfType<object>().ToList();
                    cacheSet.Ancestors.DependencyObject = obj;
                }
            }
        }

        internal static bool GetDescendants(DependencyObject obj, ref List<DependencyObject> descendants)
        {
            if (obj is FrameworkElement)
            {
                var element = (FrameworkElement)obj;

                if (element.Tag is IUIRelationCacheSet)
                {
                    var cacheSet = (IUIRelationCacheSet)element.Tag;

                    if (cacheSet.Descendants.Entries != null)
                    {
                        var stackTrace = new StackTrace();
                        var stackFrame = stackTrace.GetFrame(1);

                        descendants = cacheSet.Descendants.Entries.OfType<DependencyObject>().ToList();

                        var diagnostics = stackFrame.GetCreateDiagnostic(obj.GetHashCode());

                        diagnostics.CurrentStopWatch.IsFromCache = true;

                        return true;
                    }
                }
            }

            return false;
        }

        internal static void SetDescendants(DependencyObject obj, List<DependencyObject> descendants)
        {
            if (obj is FrameworkElement)
            {
                IUIRelationCacheSet cacheSet = null;
                var element = (FrameworkElement)obj;

                if (element.Tag is IUIRelationCacheSet)
                {
                    cacheSet = (IUIRelationCacheSet)element.Tag;
                }
                else if (element.Tag == null)
                {
                    cacheSet = new UIRelationCacheSet<DependencyObject>(obj);

                    element.Tag = cacheSet;

                    cacheSets.Add(cacheSet);
                }

                if (cacheSet != null)
                {
                    var stackTrace = new StackTrace();
                    var stackFrame = stackTrace.GetFrame(1);
                    var hash = obj.GetHashCode();
                    var diagnostic = stackFrame.GetDiagnostic(hash);

                    cacheSet.Descendants.Diagnostic = diagnostic;
                    cacheSet.Descendants.Entries = descendants.OfType<object>().ToList();
                    cacheSet.Descendants.DependencyObject = obj;
                }
            }
        }

        internal static bool GetDescendants(IControl control, ref List<IControl> descendants)
        {
            var tag = control.GetTag();

            if (tag is IUIRelationCacheSet)
            {
                var cacheSet = (IUIRelationCacheSet)tag;

                if (cacheSet.Descendants.Entries != null)
                {
                    var stackTrace = new StackTrace();
                    var stackFrame = stackTrace.GetFrame(1);

                    descendants = cacheSet.Descendants.Entries.OfType<IControl>().ToList();

                    var diagnostics = stackFrame.GetCreateDiagnostic(control.GetHashCode());

                    diagnostics.CurrentStopWatch.IsFromCache = true;

                    return true;
                }
            }

            return false;
        }

        internal static void SetDescendants(IControl control, List<IControl> descendants)
        {
            IUIRelationCacheSet cacheSet = null;
            var tag = control.GetTag();

            if (tag is IUIRelationCacheSet)
            {
                cacheSet = (IUIRelationCacheSet)tag;
            }
            else if (tag == null)
            {
                cacheSet = new UIRelationCacheSet<IControl>(control);

                control.SetTag(cacheSet);

                cacheSets.Add(cacheSet);
            }

            if (cacheSet != null)
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(1);
                var hash = control.GetHashCode();
                var diagnostic = stackFrame.GetDiagnostic(hash);

                cacheSet.Descendants.Diagnostic = diagnostic;
                cacheSet.Descendants.Entries = descendants.OfType<object>().ToList();
                cacheSet.Descendants.IControl = control;
            }
        }

        internal static bool GetIsChild(IControl control, object child, ref bool isChild)
        {
            return GetIsChild((object) control, child, ref isChild);
        }

        internal static void SetIsChild(IControl control, object child, bool isChild)
        {
            SetIsChild((object)control, child, isChild);
        }

        internal static bool GetIsChild(FrameworkElement control, object child, ref bool isChild)
        {
            return GetIsChild((object)control, child, ref isChild);
        }

        internal static void SetIsChild(FrameworkElement control, object child, bool isChild)
        {
            SetIsChild((object)control, child, isChild);
        }

        private static bool GetIsChild(object control, object child, ref bool isChild)
        {
            var key = new UIAncestorDescendentKey(control, child);

            if (isChildRelations.ContainsKey(key))
            {
                var info = isChildRelations[key];

                isChild = info.IsChild;

                return true;
            }

            return false;
        }

        private static void SetIsChild(object control, object child, bool isChild)
        {
            var key = new UIAncestorDescendentKey(control, child);

            Debug.Assert(!isChildRelations.ContainsKey(key));

            var info = new UIAncestorDescendantInfo(control, child, isChild);

            isChildRelations.Add(key, info);
        }

        internal static bool GetElementsInHostCoordinates(FrameworkElement subtree, Rect rect, ref List<UIElement> elements)
        {
            var tag = subtree.Tag;

            if (tag is IUIRelationCacheSet)
            {
                var cacheSet = (IUIRelationCacheSet)tag;

                if (cacheSet.ElementsInHostCoordinates != null && cacheSet.ElementsInHostCoordinates.ContainsKey(rect))
                {
                    var stackTrace = new StackTrace();
                    var stackFrame = stackTrace.GetFrame(1);

                    elements = cacheSet.ElementsInHostCoordinates[rect].Entries.OfType<UIElement>().ToList();

                    var diagnostics = stackFrame.GetCreateDiagnostic(subtree.GetHashCode());

                    diagnostics.CurrentStopWatch.IsFromCache = true;

                    return true;
                }
            }

            return false;
        }

        internal static void SetElementsInHostCoordinates(FrameworkElement subtree, Rect rect, List<UIElement> elements)
        {
            IUIRelationCacheSet cacheSet = null;
            var tag = subtree.Tag;

            if (tag is IUIRelationCacheSet)
            {
                cacheSet = (IUIRelationCacheSet)tag;
            }
            else if (tag == null)
            {
                cacheSet = new UIRelationCacheSet<UIElement>(subtree);

                subtree.Tag = cacheSet;

                cacheSets.Add(cacheSet);
            }

            if (cacheSet != null)
            {
                if (!cacheSet.ElementsInHostCoordinates.ContainsKey(rect))
                {
                    var stackTrace = new StackTrace();
                    var stackFrame = stackTrace.GetFrame(1);
                    var hash = subtree.GetHashCode();
                    var diagnostic = stackFrame.GetDiagnostic(hash);
                    var cacheEntry = new UIRelationCacheEntry(UIRelationCacheType.ElementsInHostCoordinates);

                    cacheSet.ElementsInHostCoordinates.Add(rect, cacheEntry);

                    cacheEntry.Diagnostic = diagnostic;
                    cacheEntry.DependencyObject = subtree;
                }

                cacheSet.ElementsInHostCoordinates[rect].Entries = elements.OfType<object>().ToList();
            }
        }
    }
}
