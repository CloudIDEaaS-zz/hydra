
namespace SassParser
{
    internal sealed class OpacityProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.NumberConverter.OrDefault(1f);

        internal OpacityProperty(Token token) : base(PropertyNames.Opacity, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}