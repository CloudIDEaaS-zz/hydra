
namespace SassParser
{
    using static Converters;

    internal sealed class BorderRightProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            LineWidthConverter.Option().For(PropertyNames.BorderRightWidth),
            LineStyleConverter.Option().For(PropertyNames.BorderRightStyle),
            CurrentColorConverter.Option().For(PropertyNames.BorderRightColor)
        ).OrDefault();

        internal BorderRightProperty(Token token) : base(PropertyNames.BorderRight, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}