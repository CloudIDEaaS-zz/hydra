using System;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Metaspec;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
#if SILVERLIGHT
using Utils.DebugListenerServiceReference;
using System.Windows.Controls;
#endif

namespace Utils
{
    public static class DebugUtils
    {
#if SILVERLIGHT
        public static SetBreakOnThrownExceptionsOperation SetBreakOnThrownExceptions(bool on)
        {
            var service = new DebugListenerClient();
            var operation = new SetBreakOnThrownExceptionsOperation();

            service.SetBreakOnThrownExceptionsAsync(on);

            service.SetBreakOnThrownExceptionsCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    operation.SetException(e.Error);
                }
                else
                {
                    operation.SetComplete();
                }
            };

            return operation;
        }

        public static GetBreakOnThrownExceptionsOperation GetBreakOnThrownExceptions()
        {
            var service = new DebugListenerClient();
            var operation = new GetBreakOnThrownExceptionsOperation();

            service.GetBreakOnThrownExceptionsAsync();

            service.GetBreakOnThrownExceptionsCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    operation.SetException(e.Error);
                }
                else
                {
                    operation.SetComplete(e.Result);
                }
            };

            return operation;
        }
        public static string GetTypeAndName(this DependencyObject dependencyObject, string delimiter = null)
        {
            if (dependencyObject is FrameworkElement)
            {
                var element = (FrameworkElement)dependencyObject;

                if (string.IsNullOrEmpty(element.Name))
                {
                    return element.GetType().Name;
                }
                else if (delimiter != null)
                {
                    return element.GetType().Name + delimiter + element.Name;
                }
                else
                {
                    return element.GetType().Name + " (" + element.Name + ")";
                }
            }
            else
            {
                return dependencyObject.GetType().Name;
            }
        }

        private static IEnumerable<DependencyObject> GetChildren(this DependencyObject obj)
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);

            for (var x = 0; x < count; x++)
            {
                yield return VisualTreeHelper.GetChild(obj, x);
            }
        }

        private static IEnumerable<DependencyObject> GetDescendants(this DependencyObject obj)
        {
            var descendants = new List<DependencyObject>();

            obj.AddDescendants(descendants);

            return descendants;
        }

        private static void AddDescendants(this DependencyObject obj, List<DependencyObject> list)
        {
            foreach (var child in obj.GetChildren())
            {
                list.Add(child);

                child.AddDescendants(list);
            }
        }

        public static string GetTypeNameAndDescendantCount(this DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkElement)
            {
                var element = (FrameworkElement)dependencyObject;

                if (string.IsNullOrEmpty(element.Name))
                {
                    return element.GetType().Name + " [" + element.GetDescendants().Count() + "]";
                }
                else
                {
                    return element.GetType().Name + " (" + element.Name + ")" + " [" + element.GetDescendants().Count() + "]";
                }
            }
            else
            {
                return dependencyObject.GetType().Name;
            }
        }

        public static string GetTypeNameAndChildCount(this DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkElement)
            {
                var element = (FrameworkElement)dependencyObject;

                if (string.IsNullOrEmpty(element.Name))
                {
                    return element.GetType().Name + " [" + element.GetChildren().Count() + "]";
                }
                else
                {
                    return element.GetType().Name + " (" + element.Name + ")" + " [" + element.GetChildren().Count() + "]";
                }
            }
            else
            {
                return dependencyObject.GetType().Name;
            }
        }
#endif
        [Conditional("DEBUG")]
        [Obsolete("Please remove me before checkin.")]
        public static void NoOp()
        {
        }

        [DebuggerHidden()]
        public static void Break()
        {
#if DEBUG
            Debugger.Break();
#else
            throw new InvalidOperationException();
#endif
        }
         
        public static IEnumerable<StackFrame> GetStack(this object obj, int count)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Take(count + 1);

            return frames;
        }

        public static bool InPotentiallyPartialIterator(this object obj)
        {
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(2);
            var reflectedType = frame.GetMethod().ReflectedType;
            
            if (reflectedType.Namespace == "System.Linq")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<StackFrame> GetStack(this object obj)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();

            return frames;
        }

        public static IEnumerable<StackFrame> GetStackWhile(this object obj, Func<StackFrame, bool> whileFilter, int skip = 0)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Skip(skip).Where(f => whileFilter(f));

            return frames;
        }

        public static IEnumerable<StackFrame> GetStackUntil(this object obj, Func<StackFrame, bool> untilFilter, int skip = 0)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Skip(skip).Where(f => !untilFilter(f));

            return frames;
        }

        public static string GetDebuggerDisplay(this object obj)
        {
            var type = obj.GetType();

            if (type.HasCustomAttribute<DebuggerDisplayAttribute>())
            {
                var attr = type.GetCustomAttribute<DebuggerDisplayAttribute>();
                var attrValue = attr.Value;
                var builder = new StringBuilder();
                var regex = new Regex("{[^}]*}");
                var project = ICsProjectFactory.create(project_namespace.pn_project_namespace);

                if (regex.IsMatch(attrValue))
                {
                    var matches = regex.Matches(attrValue);
                    var x = 0;
                    var index = 0;
                    var matchStrings = new List<string>();
                    //var evaluator = new BracedAttributeArgumentEvaluator(obj.GetType(), @"C:\Projects\RazorViewsDesigner\TestEval\DebuggerDisplayEvaluator.dll");
                    var evaluator = new BracedAttributeArgumentEvaluator(obj.GetType());

                    foreach (Match match in matches)
                    {
                        var leftString = attrValue.Substring(index, match.Index - index);
                        var matchString = match.Value;

                        builder.Append(leftString);
                        builder.AppendFormat("{{{0}}}", x);

                        matchStrings.Add(matchString);

                        index = match.Index + match.Length;
                        x++;
                    }

                    evaluator.ProcessFormat(builder.ToString());

                    for (var y = 0; y < matchStrings.Count; y++)
                    {
                        var matchString = matchStrings[y];
                        var snippet = ICsSnippetFactory.create(matchString.ToCharArray(), null);
                        CsNode rootNode;

                        try
                        {
                            project.parseSnippet(snippet, CsExpectedSnippet.cses_statement, null, true);
                        }
                        catch
                        {
                            Debugger.Break();
                        }

                        rootNode = snippet.getNodes()[0];

                        evaluator.ProcessArg(rootNode, matchString);
                    }

                    evaluator.PostProcess(x);

                    try
                    {
                        return evaluator.Evaluate(obj);
                    }
                    catch (Exception ex)
                    {
                        Debugger.Break();
                    }
                }
            }

            return obj.ToString();
        }

        public static string GetFriendlyName(this StackFrame stackFrame)
        {
            var method = stackFrame.GetMethod();

            return method.DeclaringType.Name + "." + method.Name;
        }

        public static string ToString(this IEnumerable<StackFrame> stack)
        {
            return stack.Reverse().Select(f => f.GetFriendlyName()).ToDelimitedList(" -> ");
        }

        public static string GetDebugTime(bool includeBrackets = true)
        {
            if (includeBrackets)
            {
                return string.Format("[{0:hh:mm:ss.FFFFF}]", DateTime.Now);
            }
            else
            {
                return string.Format("{0:hh:mm:ss.FFFFF]", DateTime.Now);
            }
        }

        public static string ToDebugTimeText(this DateTime time, bool includeBrackets = true)
        {
            if (includeBrackets)
            {
                return string.Format("[{0:hh:mm:ss.FFFFF}]", time);
            }
            else
            {
                return string.Format("{0:hh:mm:ss.FFFFF}", time);
            }
        }

        public static string ToDebugDateText(this DateTime time, bool includeBrackets = true)
        {
            if (includeBrackets)
            {
                return string.Format("[{0:yyyy-MM-dd hh:mm:ss.FFFFF}]", time);
            }
            else
            {
                return string.Format("{0:yyyy-MM-dd hh:mm:ss.FFFFF}", time);
            }
        }

        public static string ToDebugDateTextShort(this DateTime time, bool includeBrackets = true)
        {
            if (includeBrackets)
            {
                return string.Format("[{0:yyyy-MM-dd hh:mm:ss}]", time);
            }
            else
            {
                return string.Format("{0:yyyy-MM-dd hh:mm:ss}", time);
            }
        }

        public static string GetStackFileName(this object obj, int frameIndex)
        {
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(frameIndex);

            return frame.GetFileName();
        }

        public static int GetStackLineNumber(this object obj, int frameIndex)
        {
            var stackTrace = new StackTrace(true);
            var frame = stackTrace.GetFrame(frameIndex);

            return frame.GetFileLineNumber();
        }

        public static string GetStackText(this object obj, int count, bool includeTime)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Take(count + 1);
            var callStack = frames.Reverse().Take(count - 1).Select(f => string.Format("{0}:{1}", f.GetMethod().Name, f.GetFileLineNumber())).ToDelimitedList(" -> ");

            return (includeTime ? GetDebugTime() + " " : string.Empty) + callStack;
        }

        public static string GetStackText(this object obj, int count)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Take(count + 1);
            var callStack = frames.Reverse().Take(count - 1).Select(f => string.Format("{0}:{1}", f.GetMethod().Name, f.GetFileLineNumber())).ToDelimitedList(" -> ");

            return callStack;
        }
         
        public static string GetStackText(this object obj, int count, int skip)
        {
            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames().Take(count + 1);
            var callStack = frames.Reverse().Take(count - skip - 1).Select(f => string.Format("{0}:{1}", f.GetMethod().Name, f.GetFileLineNumber())).ToDelimitedList(" -> ");

            return callStack;
        }

        [DebuggerStepThrough()]
        public static void DoIf(bool b, Action action)
        {
            if (b)
            {
                action();
            }
        }

        [DebuggerStepThrough()]
        public static void DoIfNot(bool b, Action action)
        {
            if (!b)
            {
                action();
            }
        }

        public static void ThrowIf(bool b, Func<Exception> func)
        {
            if (b)
            {
                throw func();
            }
        }

        public static void ThrowIfNot(bool b, Func<Exception> func)
        {
            if (!b)
            {
                throw func();
            }
        }

        public static void WriteLineIf(bool b, object value)
        {
            if (b)
            {
                Debug.WriteLine(value);
            }
        }

        public static void WriteLineIf(bool b, string message)
        {
            if (b)
            {
                Debug.WriteLine(message);
            }
        }

        public static void WriteLineIf(bool b, string format, params object[] args)
        {
            if (b)
            {
                Debug.WriteLine(format, args);
            }
        }

        public static void Fail(string message)
        {
            Debug.Assert(false, message);
        }

        public static void Fail(string p, string p_2)
        {
            throw new NotImplementedException();
        }
    }
}
