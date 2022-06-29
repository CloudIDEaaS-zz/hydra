﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SassParser
{
    internal abstract class LexerBase : IDisposable
    {
        private readonly Stack<ushort> _columns;

        protected LexerBase(TextSource source)
        {
            StringBuffer = Pool.NewStringBuilder();
            _columns = new Stack<ushort>();
            Source = source;
            Current = Symbols.Null;
            Column = 0;
            Line = 1;
        }

        public string FlushBuffer()
        {
            var content = StringBuffer.ToString();
            StringBuffer.Clear();
            return content;
        }

        public void Dispose()
        {
            var isDisposed = StringBuffer == null;
            if (!isDisposed)
            {
                var disposable = Source as IDisposable;
                disposable?.Dispose();
                StringBuffer.Clear().ToPool();
                StringBuffer = null;
            }
        }

        public TextPosition GetCurrentPosition()
        {
            return new TextPosition(Line, Column, Position);
        }

        //protected bool ContinuesWithInsensitive(string val)
        //{
        //    var content = PeekString(val.Length);
        //    return (content.Length == val.Length) && content.Isi(val);
        //}

        //protected bool ContinuesWithSensitive(string val)
        //{
        //    var content = PeekString(val.Length);
        //    return (content.Length == val.Length) && content.Isi(val);
        //}

        //protected string PeekString(int length)
        //{
        //    var mark = Source.Index;
        //    Source.Index--;
        //    var content = Source.ReadCharacters(length);
        //    Source.Index = mark;
        //    return content;
        //}

        protected char SkipSpaces()
        {
            var c = GetNext();
            while (c.IsSpaceCharacter())
                c = GetNext();
            return c;
        }

        protected char GetNext()
        {
            Advance();
            return Current;
        }

        protected char GetPrevious()
        {
            Back();
            return Current;
        }

        protected void Advance()
        {
            if (Current != Symbols.EndOfFile)
            {
                AdvanceNative();
            }
        }

        protected void Advance(int distance)
        {
            while ((distance-- > 0) && (Current != Symbols.EndOfFile))
            {
                AdvanceNative();
            }
        }

        protected void Back()
        {
            if (InsertionPoint > 0)
            {
                BackNative();
            }
        }

        protected void Back(int distance)
        {
            while ((distance-- > 0) && (InsertionPoint > 0))
            {
                BackNative();
            }
        }

        private void AdvanceNative()
        {
            if (Current == Symbols.LineFeed)
            {
                _columns.Push(Column);
                Column = 1;
                Line++;
            }
            else
            {
                Column++;
            }
            Current = NormalizeForward(Source.ReadCharacter());
        }

        private void BackNative()
        {
            Source.Index -= 1;
            if (Source.Index == 0)
            {
                Column = 0;
                Current = Symbols.Null;
                return;
            }
            var c = NormalizeBackward(Source[Source.Index - 1]);
            if (c == Symbols.LineFeed)
            {
                Column = _columns.Count != 0 ? _columns.Pop() : (ushort) 1;
                Line--;
                Current = c;
            }
            else if (c != Symbols.Null)
            {
                Current = c;
                Column--;
            }
        }

        private char NormalizeForward(char symbol)
        {
            if (symbol != Symbols.CarriageReturn)
            {
                return symbol;
            }
            if (Source.ReadCharacter() != Symbols.LineFeed)
            {
                Source.Index--;
            }
            return Symbols.LineFeed;
        }

        private char NormalizeBackward(char symbol)
        {
            if (symbol != Symbols.CarriageReturn)
            {
                return symbol;
            }
            if ((Source.Index < Source.Length) && (Source[Source.Index] == Symbols.LineFeed))
            {
                BackNative();
                return Symbols.Null;
            }
            return Symbols.LineFeed;
        }

        protected StringBuilder StringBuffer { get; private set; }

        public TextSource Source { get; }

        public ushort Line { get; private set; }
        public ushort Column { get; private set; }
        public int Position => Source.Index;
        protected char Current { get; private set; }
        public int InsertionPoint
        {
            get => Source.Index;
            protected set
            {
                var delta = Source.Index - value;
                while (delta > 0)
                {
                    BackNative();
                    delta--;
                }
                while (delta < 0)
                {
                    AdvanceNative();
                    delta++;
                }
            }
        }
    }
}