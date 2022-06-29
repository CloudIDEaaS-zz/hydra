
namespace SassParser
{
    internal sealed class BorderTopWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal BorderTopWidthProperty(Token token) : base(PropertyNames.BorderTopWidth, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}