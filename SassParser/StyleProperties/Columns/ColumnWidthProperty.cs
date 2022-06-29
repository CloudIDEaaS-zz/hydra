
namespace SassParser
{
    internal sealed class ColumnWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.AutoLengthConverter.OrDefault(Keywords.Auto);

        internal ColumnWidthProperty(Token token) : base(PropertyNames.ColumnWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}