
namespace SassParser
{
    internal sealed class FontStretchProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.FontStretchConverter.OrDefault(FontStretch.Normal);

        internal FontStretchProperty(Token token) : base(PropertyNames.FontStretch, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}