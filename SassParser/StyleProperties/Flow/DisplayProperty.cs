
namespace SassParser
{
    internal sealed class DisplayProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.DisplayModeConverter.OrDefault(DisplayMode.Inline);

        internal DisplayProperty(Token token) : base(PropertyNames.Display, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}