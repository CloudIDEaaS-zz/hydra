
namespace SassParser
{
    internal sealed class TextDecorationColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ColorConverter.OrDefault(Color.Black);

        internal TextDecorationColorProperty(Token token) : base(PropertyNames.TextDecorationColor, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}