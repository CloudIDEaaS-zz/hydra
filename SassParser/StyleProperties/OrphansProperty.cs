
namespace SassParser
{
    internal sealed class OrphansProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.NaturalIntegerConverter.OrDefault(2);

        internal OrphansProperty(Token token) : base(PropertyNames.Orphans, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}