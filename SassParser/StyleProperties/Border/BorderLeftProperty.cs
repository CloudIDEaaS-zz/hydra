
namespace SassParser
{
    using static Converters;

    internal sealed class BorderLeftProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            LineWidthConverter.Option().For(PropertyNames.BorderLeftWidth),
            LineStyleConverter.Option().For(PropertyNames.BorderLeftStyle),
            CurrentColorConverter.Option().For(PropertyNames.BorderLeftColor)
        ).OrDefault();

        internal BorderLeftProperty(Token token) : base(PropertyNames.BorderLeft, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}