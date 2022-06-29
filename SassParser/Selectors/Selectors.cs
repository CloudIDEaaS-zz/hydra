﻿using System.Collections;
using System.Collections.Generic;

namespace SassParser
{
    internal abstract class Selectors : StylesheetNode, IEnumerable<ISelector>
    {
        protected readonly List<ISelector> _selectors;

        protected Selectors(Token token) : base(token)
        {
            _selectors = new List<ISelector>();
        }

        public Priority Specifity
        {
            get
            {
                var sum = new Priority();

                foreach (var t in _selectors)
                {
                    sum += t.Specifity;
                }

                return sum;
            }
        }

        public string Text => this.ToCss();
        public int Length => _selectors.Count;
        public ISelector this[int index]
        {
            get => _selectors[index];
            set => _selectors[index] = value;
        }

        public void Add(ISelector selector)
        {
            _selectors.Add(selector);
        }

        public void Remove(ISelector selector)
        {
            _selectors.Remove(selector);
        }

        public IEnumerator<ISelector> GetEnumerator()
        {
            return _selectors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}