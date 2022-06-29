
namespace SassParser
{
    internal sealed class FontSizeProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.FontSizeConverter.OrDefault(FontSize.Medium.ToLength());

        internal FontSizeProperty(Token token) : base(PropertyNames.FontSize, token, PropertyFlags.Inherited | PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}