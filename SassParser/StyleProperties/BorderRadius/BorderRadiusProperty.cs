
namespace SassParser
{
    internal sealed class BorderRadiusProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderRadiusShorthandConverter.OrDefault();

        internal BorderRadiusProperty(Token token) : base(PropertyNames.BorderRadius, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}