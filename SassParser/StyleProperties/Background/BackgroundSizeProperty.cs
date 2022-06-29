
namespace SassParser
{
    internal sealed class BackgroundSizeProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.BackgroundSizeConverter.FromList().OrDefault();

        internal BackgroundSizeProperty(Token token) : base(PropertyNames.BackgroundSize, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}