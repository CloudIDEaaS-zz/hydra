
namespace SassParser
{
    internal sealed class OutlineColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.InvertedColorConverter.OrDefault(Color.Transparent);

        internal OutlineColorProperty(Token token) : base(PropertyNames.OutlineColor, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}
