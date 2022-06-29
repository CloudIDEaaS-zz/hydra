
namespace SassParser
{
    using static Converters;

    internal sealed class ColumnsProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            AutoLengthConverter.Option().For(PropertyNames.ColumnWidth),
            OptionalIntegerConverter.Option().For(PropertyNames.ColumnCount)).OrDefault();

        internal ColumnsProperty(Token token) : base(PropertyNames.Columns, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}