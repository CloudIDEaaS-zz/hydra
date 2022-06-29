
namespace SassParser
{
    internal sealed class MaxWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalLengthOrPercentConverter.OrDefault();

        internal MaxWidthProperty(Token token) : base(PropertyNames.MaxWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}