
namespace SassParser
{
    internal sealed class BreakAfterProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BreakModeConverter.OrDefault(BreakMode.Auto);

        internal BreakAfterProperty(Token token) : base(PropertyNames.BreakAfter, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}