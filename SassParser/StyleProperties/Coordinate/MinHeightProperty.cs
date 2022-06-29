
namespace SassParser
{
    internal sealed class MinHeightProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthOrPercentConverter.OrDefault(Length.Zero);

        internal MinHeightProperty(Token token) : base(PropertyNames.MinHeight, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}