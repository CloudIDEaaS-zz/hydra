
namespace SassParser
{
    using static Converters;

    internal sealed class BorderProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            LineWidthConverter.Option()
                .For(PropertyNames.BorderTopWidth, PropertyNames.BorderRightWidth, PropertyNames.BorderBottomWidth,
                    PropertyNames.BorderLeftWidth),
            LineStyleConverter.Option()
                .For(PropertyNames.BorderTopStyle, PropertyNames.BorderRightStyle, PropertyNames.BorderBottomStyle,
                    PropertyNames.BorderLeftStyle),
            CurrentColorConverter.Option()
                .For(PropertyNames.BorderTopColor, PropertyNames.BorderRightColor, PropertyNames.BorderBottomColor,
                    PropertyNames.BorderLeftColor)
        ).OrDefault();

        internal BorderProperty(Token token) : base(PropertyNames.Border, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}