
namespace SassParser
{
    internal sealed class ListStylePositionProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.ListPositionConverter.OrDefault(ListPosition.Outside);

        internal ListStylePositionProperty(Token token) : base(PropertyNames.ListStylePosition, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}