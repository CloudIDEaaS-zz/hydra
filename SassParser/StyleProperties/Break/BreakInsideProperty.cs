
namespace SassParser
{
    internal sealed class BreakInsideProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.BreakInsideModeConverter.OrDefault(BreakMode.Auto);

        internal BreakInsideProperty(Token token) : base(PropertyNames.BreakInside, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}