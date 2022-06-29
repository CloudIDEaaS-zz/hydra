
namespace SassParser
{
    internal sealed class BorderStyleProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.Periodic(
            PropertyNames.BorderTopStyle, PropertyNames.BorderRightStyle, PropertyNames.BorderBottomStyle,
            PropertyNames.BorderLeftStyle).OrDefault();

        internal BorderStyleProperty(Token token) : base(PropertyNames.BorderStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}