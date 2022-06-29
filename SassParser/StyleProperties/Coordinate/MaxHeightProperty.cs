
namespace SassParser
{
    internal sealed class MaxHeightProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalLengthOrPercentConverter.OrDefault();

        internal MaxHeightProperty(Token token) : base(PropertyNames.MaxHeight, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}