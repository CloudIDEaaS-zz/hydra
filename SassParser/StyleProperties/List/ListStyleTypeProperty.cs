
namespace SassParser
{
    internal sealed class ListStyleTypeProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ListStyleConverter.OrDefault(ListStyle.Disc);

        internal ListStyleTypeProperty(Token token) : base(PropertyNames.ListStyleType, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}