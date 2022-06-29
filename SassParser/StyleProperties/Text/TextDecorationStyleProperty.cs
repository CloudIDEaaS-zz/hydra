
namespace SassParser
{
    internal sealed class TextDecorationStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.TextDecorationStyleConverter.OrDefault(TextDecorationStyle.Solid);

        internal TextDecorationStyleProperty(Token token) : base(PropertyNames.TextDecorationStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}