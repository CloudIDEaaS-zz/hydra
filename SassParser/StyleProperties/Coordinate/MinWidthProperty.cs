
namespace SassParser
{
    internal sealed class MinWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthOrPercentConverter.OrDefault(Length.Zero);

        internal MinWidthProperty(Token token) : base(PropertyNames.MinWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}