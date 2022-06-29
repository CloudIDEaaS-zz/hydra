
namespace SassParser
{
    internal sealed class ColumnSpanProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ColumnSpanConverter.OrDefault(false);

        internal ColumnSpanProperty(Token token) : base(PropertyNames.ColumnSpan, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}