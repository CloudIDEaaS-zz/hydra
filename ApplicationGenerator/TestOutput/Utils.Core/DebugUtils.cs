using System;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Utils.Logging;
using Microsoft.Extensions.DependencyInjection;
#if SILVERLIGHT
using Utils.DebugListenerServiceReference;
using System.Windows.Controls;
#endif

namespace Utils
{
    public static class DebugUtils
    {
        public static FixedDictionary<string, LineTracker> Trackers { get; private set; }

        [Conditional("DEBUG")]
        [Obsolete("Please remove me before checkin.")]
        public static void NoOp()
        {
        }

        public static void OpenLogFile(string logFile)
        {
            var process = new Process();

            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = logFile
            };

            process.Start();
        }

        public static ILoggingBuilder AddLineLogging(this ILoggingBuilder builder, string path)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<ILineLoggerProvider, LineLoggerProvider>(services => new LineLoggerProvider(path));
            builder.AddFilter<LineLoggerProvider>(null, LogLevel.Trace);

            return builder;
        }

        public static IDisposable StartLineTracking(IServiceProvider serviceProvider, string scope = null, [CallerFilePath] string filePath = null, int lineNumber = 0, [CallerMemberName] string caller = null)
        {
#if ENABLE_LINE_TRACKING
            LineTracker lineTracker;
            var key = scope ?? filePath;

            if (Trackers == null)
            {
                Trackers = new FixedDictionary<string, LineTracker>(100);
            }

            if (!Trackers.ContainsKey(key))
            {
                lineTracker = new LineTracker(scope, filePath, serviceProvider);
                Trackers.Add(key, lineTracker);
            }
            else
            {
                lineTracker = Trackers[key];
            }

            TrackLine(filePath, lineNumber, caller);

            return lineTracker.AsDisposable(() =>
            {
                if (lineTracker.PrimaryFilePath == filePath)
                {
                    Trackers.Remove(filePath);
                }
            });
#else
            return typeof(DebugUtils).AsDisposable(() => { });
#endif
        }

        public static void TrackLine([CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
#if ENABLE_LINE_TRACKING
            LineTracker lineTracker;
            Line line;

            if (Trackers != null)
            {
                if (Trackers.ContainsKey(filePath))
                {
                    lineTracker = Trackers[filePath];

                    line = new Line(filePath, lineNumber, caller);
                    lineTracker.Add(line);
                }
                else
                {
                    var trackers = Trackers.Values.Where(t => t.PrimaryFilePath.AsCaseless() == filePath || t.AdditionalFilePaths.Any(p => p.AsCaseless() == filePath));

                    line = new Line(filePath, lineNumber, caller);
                    trackers.ForEach(t => t.Add(line));
                }
            }
#endif
        }

        [DebuggerHidden()]
        public static T BreakReturn<T>(T value)
        {
#if DEBUG
            Debugger.Break();
            return value;
#else
            throw new InvalidOperationException();
#endif
        }

        [DebuggerHidden()]
        public static dynamic BreakReturnNull()
        {
#if DEBUG
            Debugger.Break();
            return null;
#else
            throw new InvalidOperationException();
#endif
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

        [DebuggerHidden()]
        public static void Break(string error)
        {
#if DEBUG
            Debugger.Break();
#else
            throw new InvalidOperationException(error);
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

        //public static string GetDebuggerDisplay(this object obj)
        //{
        //    var type = obj.GetType();

        //    if (type.HasCustomAttribute<DebuggerDisplayAttribute>())
        //    {
        //        var attr = type.GetCustomAttribute<DebuggerDisplayAttribute>();
        //        var attrValue = attr.Value;
        //        var builder = new StringBuilder();
        //        var regex = new Regex("{[^}]*}");
        //        var project = ICsProjectFactory.create(project_namespace.pn_project_namespace);

        //        if (regex.IsMatch(attrValue))
        //        {
        //            var matches = regex.Matches(attrValue);
        //            var x = 0;
        //            var index = 0;
        //            var matchStrings = new List<string>();
        //            //var evaluator = new BracedAttributeArgumentEvaluator(obj.GetType(), @"C:\Projects\RazorViewsDesigner\TestEval\DebuggerDisplayEvaluator.dll");
        //            var evaluator = new BracedAttributeArgumentEvaluator(obj.GetType());

        //            foreach (Match match in matches)
        //            {
        //                var leftString = attrValue.Substring(index, match.Index - index);
        //                var matchString = match.Value;

        //                builder.Append(leftString);
        //                builder.AppendFormat("{{{0}}}", x);

        //                matchStrings.Add(matchString);

        //                index = match.Index + match.Length;
        //                x++;
        //            }

        //            evaluator.ProcessFormat(builder.ToString());

        //            for (var y = 0; y < matchStrings.Count; y++)
        //            {
        //                var matchString = matchStrings[y];
        //                var snippet = ICsSnippetFactory.create(matchString.ToCharArray(), null);
        //                CsNode rootNode;

        //                try
        //                {
        //                    project.parseSnippet(snippet, CsExpectedSnippet.cses_statement, null, true);
        //                }
        //                catch
        //                {
        //                    Debugger.Break();
        //                }

        //                rootNode = snippet.getNodes()[0];

        //                evaluator.ProcessArg(rootNode, matchString);
        //            }

        //            evaluator.PostProcess(x);

        //            try
        //            {
        //                return evaluator.Evaluate(obj);
        //            }
        //            catch (Exception ex)
        //            {
        //                Debugger.Break();
        //            }
        //        }
        //    }

        //    return obj.ToString();
        //}

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

        public static string GetDebugUctTime(bool includeBrackets = true)
        {
            if (includeBrackets)
            {
                return string.Format("[{0:hh:mm:ss.FFFFF}]", DateTime.UtcNow);
            }
            else
            {
                return string.Format("{0:hh:mm:ss.FFFFF]", DateTime.UtcNow);
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

        public static void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(string.Format(format, args));
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
