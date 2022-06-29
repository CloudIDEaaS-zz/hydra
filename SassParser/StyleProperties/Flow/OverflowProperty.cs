
namespace SassParser
{
    internal sealed class OverflowProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.OverflowModeConverter.OrDefault(Overflow.Visible);

        internal OverflowProperty(Token token) : base(PropertyNames.Overflow, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}