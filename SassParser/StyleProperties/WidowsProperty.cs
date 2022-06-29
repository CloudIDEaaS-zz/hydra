
namespace SassParser
{
    internal sealed class WidowsProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.IntegerConverter.OrDefault(2);

        internal WidowsProperty(Token token) : base(PropertyNames.Widows, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}