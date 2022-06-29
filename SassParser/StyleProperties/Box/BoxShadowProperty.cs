
namespace SassParser
{
    internal sealed class BoxShadowProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.MultipleShadowConverter.OrDefault();

        internal BoxShadowProperty(Token token) : base(PropertyNames.BoxShadow, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}