
namespace SassParser
{
    internal sealed class BackgroundImageProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.MultipleImageSourceConverter.OrDefault();

        internal BackgroundImageProperty(Token token) : base(PropertyNames.BackgroundImage, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}