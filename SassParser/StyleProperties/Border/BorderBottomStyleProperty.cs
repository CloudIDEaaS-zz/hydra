
namespace SassParser
{
    internal sealed class BorderBottomStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal BorderBottomStyleProperty(Token token) : base(PropertyNames.BorderBottomStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}