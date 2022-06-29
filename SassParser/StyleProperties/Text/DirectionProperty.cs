
namespace SassParser
{
    internal sealed class DirectionProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.DirectionModeConverter.OrDefault(DirectionMode.Ltr);

        internal DirectionProperty(Token token) : base(PropertyNames.Direction, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}