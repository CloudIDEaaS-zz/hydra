
namespace SassParser
{
    internal sealed class BorderTopStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal BorderTopStyleProperty(Token token) : base(PropertyNames.BorderTopStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}