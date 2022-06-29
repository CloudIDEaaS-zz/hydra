
namespace SassParser
{
    internal sealed class BorderBottomWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal BorderBottomWidthProperty(Token token) : base(PropertyNames.BorderBottomWidth, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}