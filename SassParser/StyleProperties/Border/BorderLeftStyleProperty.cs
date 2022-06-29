
namespace SassParser
{
    internal sealed class BorderLeftStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal BorderLeftStyleProperty(Token token) : base(PropertyNames.BorderLeftStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}