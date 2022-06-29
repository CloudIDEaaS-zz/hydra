
namespace SassParser
{
    using static Converters;

    internal sealed class BorderBottomProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            LineWidthConverter.Option().For(PropertyNames.BorderBottomWidth),
            LineStyleConverter.Option().For(PropertyNames.BorderBottomStyle),
            CurrentColorConverter.Option().For(PropertyNames.BorderBottomColor)
        ).OrDefault();

        internal BorderBottomProperty(Token token) : base(PropertyNames.BorderBottom, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}