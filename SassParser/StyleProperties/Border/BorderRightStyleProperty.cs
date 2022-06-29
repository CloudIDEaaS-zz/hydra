
namespace SassParser
{
    internal sealed class BorderRightStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal BorderRightStyleProperty(Token token) : base(PropertyNames.BorderRightStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}