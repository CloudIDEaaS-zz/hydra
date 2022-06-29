
namespace SassParser
{
    internal sealed class BackgroundAttachmentProperty : Property
    {
        private static readonly IValueConverter AttachmentConverter =
            Converters.BackgroundAttachmentConverter.FromList().OrDefault(BackgroundAttachment.Scroll);

        internal BackgroundAttachmentProperty(Token token) : base(PropertyNames.BackgroundAttachment, token)
        {
        }

        internal override IValueConverter Converter => AttachmentConverter;
    }
}