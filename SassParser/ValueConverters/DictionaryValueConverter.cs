﻿using System.Collections.Generic;

namespace SassParser
{
    internal sealed class DictionaryValueConverter<T> : IValueConverter
    {
        private readonly Dictionary<string, T> _values;

        public DictionaryValueConverter(Dictionary<string, T> values)
        {
            _values = values;
        }

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var identifier = value.ToIdentifier();

            return (identifier != null) && _values.TryGetValue(identifier, out _)
                ? new EnumeratedValue(identifier, value)
                : null;
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<EnumeratedValue>();
        }

        private sealed class EnumeratedValue : IPropertyValue
        {
            public EnumeratedValue(string identifier, IEnumerable<Token> tokens)
            {
                CssText = identifier;
                Original = new TokenValue(tokens);
            }

            public string CssText { get; }

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name)
            {
                return Original;
            }
        }
    }
}