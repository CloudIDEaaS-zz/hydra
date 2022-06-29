
namespace SassParser
{
    internal sealed class BackgroundOriginProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.BoxModelConverter.FromList().OrDefault(BoxModel.PaddingBox);

        internal BackgroundOriginProperty(Token token) : base(PropertyNames.BackgroundOrigin, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}