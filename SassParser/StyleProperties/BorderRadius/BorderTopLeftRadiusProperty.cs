
namespace SassParser
{
    internal sealed class BorderTopLeftRadiusProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderRadiusConverter.OrDefault(Length.Zero);

        internal BorderTopLeftRadiusProperty(Token token) : base(PropertyNames.BorderTopLeftRadius, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}