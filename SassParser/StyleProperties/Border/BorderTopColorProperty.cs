
namespace SassParser
{
    internal sealed class BorderTopColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.CurrentColorConverter.OrDefault(Color.Transparent);
       
        internal BorderTopColorProperty(Token token) : base(PropertyNames.BorderTopColor, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}