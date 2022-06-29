
namespace SassParser
{
    internal sealed class BackgroundClipProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.BoxModelConverter.FromList().OrDefault(BoxModel.BorderBox);

        internal BackgroundClipProperty(Token token) : base(PropertyNames.BackgroundClip, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}