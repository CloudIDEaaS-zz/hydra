﻿
namespace SassParser
{
    using static Converters;

    internal sealed class FontProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithOrder(
            WithAny(
                FontStyleConverter.Option().For(PropertyNames.FontStyle),
                FontVariantConverter.Option().For(PropertyNames.FontVariant),
                FontWeightConverter.Or(WeightIntegerConverter).Option().For(PropertyNames.FontWeight),
                FontStretchConverter.Option().For(PropertyNames.FontStretch)),
            WithOrder(
                FontSizeConverter.Required().For(PropertyNames.FontSize),
                LineHeightConverter.StartsWithDelimiter().Option().For(PropertyNames.LineHeight),
                FontFamiliesConverter.Required().For(PropertyNames.FontFamily))).Or(
            SystemFontConverter);

        internal FontProperty(Token token) : base(PropertyNames.Font, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}