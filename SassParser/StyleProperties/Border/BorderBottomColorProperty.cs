
namespace SassParser
{
    internal sealed class BorderBottomColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.CurrentColorConverter.OrDefault(Color.Transparent);

        internal BorderBottomColorProperty(Token token) : base(PropertyNames.BorderBottomColor, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}