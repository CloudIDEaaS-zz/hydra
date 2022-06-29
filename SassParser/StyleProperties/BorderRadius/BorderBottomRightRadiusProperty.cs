
namespace SassParser
{
    internal sealed class BorderBottomRightRadiusProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderRadiusConverter.OrDefault(Length.Zero);

        internal BorderBottomRightRadiusProperty(Token token) : base(PropertyNames.BorderBottomRightRadius, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}