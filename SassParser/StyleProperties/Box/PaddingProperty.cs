﻿
namespace SassParser
{
    internal sealed class PaddingProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.LengthOrPercentConverter.Periodic(
                PropertyNames.PaddingTop, PropertyNames.PaddingRight, PropertyNames.PaddingBottom, PropertyNames.PaddingLeft)
            .OrDefault(Length.Zero);

        internal PaddingProperty(Token token) : base(PropertyNames.Padding, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}