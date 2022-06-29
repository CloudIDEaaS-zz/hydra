
namespace SassParser
{
    internal sealed class BorderWidthProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.Periodic(
            PropertyNames.BorderTopWidth, PropertyNames.BorderRightWidth, PropertyNames.BorderBottomWidth,
            PropertyNames.BorderLeftWidth).OrDefault();

        internal BorderWidthProperty(Token token) : base(PropertyNames.BorderWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}