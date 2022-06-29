
namespace SassParser
{
    internal sealed class FontStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.FontStyleConverter.OrDefault(FontStyle.Normal);

        internal FontStyleProperty(Token token) : base(PropertyNames.FontStyle, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}