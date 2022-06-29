
namespace SassParser
{
    internal sealed class BorderRightWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal BorderRightWidthProperty(Token token) : base(PropertyNames.BorderRightWidth, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}