
namespace SassParser
{
    internal sealed class BorderImageSourceProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalImageSourceConverter.OrDefault();

        internal BorderImageSourceProperty(Token token) : base(PropertyNames.BorderImageSource, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}