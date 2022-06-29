
namespace SassParser
{
    internal sealed class QuotesProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.EvenStringsConverter.OrNone().OrDefault(new[] {"«", "»"});

        internal QuotesProperty(Token token) : base(PropertyNames.Quotes, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}