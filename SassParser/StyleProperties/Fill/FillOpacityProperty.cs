
namespace SassParser
{
    internal sealed class FillOpacityProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.NumberConverter.OrDefault(1f);

        internal FillOpacityProperty(Token token) : base(PropertyNames.FillOpacity, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}