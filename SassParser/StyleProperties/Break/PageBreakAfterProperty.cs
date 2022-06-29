
namespace SassParser
{
    internal sealed class PageBreakAfterProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.PageBreakModeConverter.OrDefault(BreakMode.Auto);

        internal PageBreakAfterProperty(Token token) : base(PropertyNames.PageBreakAfter, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}