
namespace SassParser
{
    internal sealed class ColumnCountProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalIntegerConverter.OrDefault();

        internal ColumnCountProperty(Token token) : base(PropertyNames.ColumnCount, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}