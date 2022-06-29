
namespace SassParser
{
    internal sealed class TextDecorationLineProperty : Property
    {
        private static readonly IValueConverter ListConverter = Converters.TextDecorationLinesConverter.OrDefault();

        internal TextDecorationLineProperty(Token token) : base(PropertyNames.TextDecorationLine, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}