
namespace SassParser
{
    internal sealed class WhiteSpaceProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.WhitespaceConverter.OrDefault(Whitespace.Normal);

        internal WhiteSpaceProperty(Token token) : base(PropertyNames.WhiteSpace, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}