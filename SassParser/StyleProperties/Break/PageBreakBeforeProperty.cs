
namespace SassParser
{
    internal sealed class PageBreakBeforeProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.PageBreakModeConverter.OrDefault(BreakMode.Auto);

        internal PageBreakBeforeProperty(Token token) : base(PropertyNames.PageBreakBefore, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}