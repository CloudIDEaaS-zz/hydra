
namespace SassParser
{
    internal sealed class BorderImageWidthProperty : Property
    {
        internal static readonly IValueConverter TheConverter = Converters.ImageBorderWidthConverter.Periodic();
        private static readonly IValueConverter StyleConverter = TheConverter.OrDefault(Length.Full);

        internal BorderImageWidthProperty(Token token) : base(PropertyNames.BorderImageWidth, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}