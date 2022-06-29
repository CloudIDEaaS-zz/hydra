
namespace SassParser
{
    internal sealed class FontSizeAdjustProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalNumberConverter.OrDefault();

        internal FontSizeAdjustProperty(Token token) : base(PropertyNames.FontSizeAdjust, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}