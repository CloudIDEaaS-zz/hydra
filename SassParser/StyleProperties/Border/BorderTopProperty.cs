
namespace SassParser
{
    using static Converters;

    internal sealed class BorderTopProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            LineWidthConverter.Option().For(PropertyNames.BorderTopWidth),
            LineStyleConverter.Option().For(PropertyNames.BorderTopStyle),
            CurrentColorConverter.Option().For(PropertyNames.BorderTopColor)
        ).OrDefault();

        internal BorderTopProperty(Token token) : base(PropertyNames.BorderTop, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}