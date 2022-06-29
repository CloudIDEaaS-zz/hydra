
namespace SassParser
{
    internal sealed class BorderLeftColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.CurrentColorConverter.OrDefault(Color.Transparent);

        internal BorderLeftColorProperty(Token token) : base(PropertyNames.BorderLeftColor, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}