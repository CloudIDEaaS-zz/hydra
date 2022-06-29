
namespace SassParser
{
    internal sealed class BorderRightColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.CurrentColorConverter.OrDefault(Color.Transparent);

        internal BorderRightColorProperty(Token token) : base(PropertyNames.BorderRightColor, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}