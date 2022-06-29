

namespace SassParser
{
    internal sealed class TextShadowProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.MultipleShadowConverter.OrDefault();

        internal TextShadowProperty(Token token) : base(PropertyNames.TextShadow, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}