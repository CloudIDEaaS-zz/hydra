
namespace SassParser
{
    internal sealed class BorderLeftWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal BorderLeftWidthProperty(Token token) : base(PropertyNames.BorderLeftWidth, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}