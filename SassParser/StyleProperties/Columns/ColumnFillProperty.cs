
namespace SassParser
{
    internal sealed class ColumnFillProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ColumnFillConverter.OrDefault(true);

        internal ColumnFillProperty(Token token) : base(PropertyNames.ColumnFill, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}