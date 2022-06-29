
namespace SassParser
{
    internal sealed class CaptionSideProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.CaptionSideConverter.OrDefault(true);

        internal CaptionSideProperty(Token token) : base(PropertyNames.CaptionSide, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}