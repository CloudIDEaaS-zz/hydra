
namespace SassParser
{
    internal sealed class TextTransformProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.TextTransformConverter.OrDefault(TextTransform.None);

        internal TextTransformProperty(Token token) : base(PropertyNames.TextTransform, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}