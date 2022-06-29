
namespace SassParser
{
    internal sealed class BorderBottomLeftRadiusProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderRadiusConverter.OrDefault(Length.Zero);

        internal BorderBottomLeftRadiusProperty(Token token) : base(PropertyNames.BorderBottomLeftRadius, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}