
namespace SassParser
{
    internal sealed class BreakBeforeProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BreakModeConverter.OrDefault(BreakMode.Auto);

        internal BreakBeforeProperty(Token token) : base(PropertyNames.BreakBefore, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}