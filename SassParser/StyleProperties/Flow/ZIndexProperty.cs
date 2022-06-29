
namespace SassParser
{
    internal sealed class ZIndexProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalIntegerConverter.OrDefault();

        internal ZIndexProperty(Token token) : base(PropertyNames.ZIndex, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}