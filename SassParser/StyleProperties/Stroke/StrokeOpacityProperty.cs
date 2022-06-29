
namespace SassParser
{
    internal sealed class StrokeOpacityProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.NumberConverter.OrDefault(1f);

        internal StrokeOpacityProperty(Token token) : base(PropertyNames.StrokeOpacity, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}