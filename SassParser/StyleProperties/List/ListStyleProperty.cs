
namespace SassParser
{
    using static Converters;

    internal sealed class ListStyleProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            ListStyleConverter.Option().For(PropertyNames.ListStyleType),
            ListPositionConverter.Option().For(PropertyNames.ListStylePosition),
            OptionalImageSourceConverter.Option().For(PropertyNames.ListStyleImage)).OrDefault();

        internal ListStyleProperty(Token token) : base(PropertyNames.ListStyle, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}