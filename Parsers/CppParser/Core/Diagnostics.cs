﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CppSharp
{
    /// <summary>
    /// Represents the kind of the diagnostic.
    /// </summary>
    public enum DiagnosticKind
    {
        Debug,
        Message,
        Warning,
        Error
    }

    /// <summary>
    /// Keeps information related to a single diagnostic.
    /// </summary>
    public struct DiagnosticInfo
    {
        public DiagnosticKind Kind;
        public string Message;
        public string File;
        public int Line;
        public int Column;
    }

    public interface IDiagnostics
    {
        DiagnosticKind Level { get; set; }
        void Emit(DiagnosticInfo info);
        void PushIndent(int level = 4);
        void PopIndent();
    }

    public static class DiagnosticExtensions
    {
        public static void Debug(this IDiagnostics consumer, string msg, params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Debug,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Message(this IDiagnostics consumer,  string msg, params object[] args)
        {
            var diagInfo = new DiagnosticInfo
                {
                    Kind = DiagnosticKind.Message,
                    Message = string.Format(msg, args)
                };

            consumer.Emit(diagInfo);
        }

        public static void Warning(this IDiagnostics consumer, string msg, params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Warning,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Error(this IDiagnostics consumer, string msg, params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Error,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Error(this IDiagnostics consumer, string msg)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Error,
                Message = msg
            };

            consumer.Emit(diagInfo);
        }
    }

    public class TextDiagnosticPrinter : IDiagnostics
    {
        public Stack<int> Indents;
        public DiagnosticKind Level { get; set; }

        public TextDiagnosticPrinter()
        {
            Indents = new Stack<int>();
            Level = DiagnosticKind.Message;
        }

        public void Emit(DiagnosticInfo info)
        {
            if (info.Kind < Level)
                return;

            var currentIndent = Indents.Sum();
            var message = new string(' ', currentIndent) + info.Message;

            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public void PushIndent(int level)
        {
            Indents.Push(level);
        }

        public void PopIndent()
        {
            Indents.Pop();
        }
    }
}
