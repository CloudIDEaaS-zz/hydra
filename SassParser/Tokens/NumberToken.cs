﻿using System.Globalization;

namespace SassParser
{
    internal sealed class NumberToken : Token
    {
        private static readonly char[] FloatIndicators = {'.', 'e', 'E'};

        public NumberToken(string number, TextPosition position)
            : base(TokenType.Number, number, position)
        {
        }

        public bool IsInteger => Data.IndexOfAny(FloatIndicators) == -1;

        public int IntegerValue => int.Parse(Data, CultureInfo.InvariantCulture);

        public float Value => float.Parse(Data, CultureInfo.InvariantCulture);
    }
}