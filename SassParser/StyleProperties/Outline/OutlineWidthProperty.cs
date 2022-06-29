
namespace SassParser
{
    internal sealed class OutlineWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal OutlineWidthProperty(Token token) : base(PropertyNames.OutlineWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}