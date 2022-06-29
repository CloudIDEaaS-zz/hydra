
namespace SassParser
{
    internal sealed class PositionProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.PositionModeConverter.OrDefault(PositionMode.Static);

        internal PositionProperty(Token token) : base(PropertyNames.Position, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}