
namespace SassParser
{
    internal sealed class FontVariantProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.FontVariantConverter.OrDefault(FontVariant.Normal);

        internal FontVariantProperty(Token token) : base(PropertyNames.FontVariant, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}