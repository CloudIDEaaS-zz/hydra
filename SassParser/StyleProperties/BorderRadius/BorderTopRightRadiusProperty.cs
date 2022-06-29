
namespace SassParser
{
    internal sealed class BorderTopRightRadiusProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderRadiusConverter.OrDefault(Length.Zero);

        internal BorderTopRightRadiusProperty(Token token) : base(PropertyNames.BorderTopRightRadius, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}