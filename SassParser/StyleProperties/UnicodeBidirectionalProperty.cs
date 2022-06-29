
namespace SassParser
{
    internal sealed class UnicodeBidirectionalProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.UnicodeModeConverter.OrDefault(UnicodeMode.Normal);

        internal UnicodeBidirectionalProperty(Token token) : base(PropertyNames.UnicodeBidirectional, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}