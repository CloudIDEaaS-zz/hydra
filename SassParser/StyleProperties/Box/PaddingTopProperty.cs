﻿
namespace SassParser
{
    internal sealed class PaddingTopProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthOrPercentConverter.OrDefault(Length.Zero);

        internal PaddingTopProperty(Token token) : base(PropertyNames.PaddingTop, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}